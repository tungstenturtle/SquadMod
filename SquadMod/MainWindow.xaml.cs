﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using NAudio.Midi;
using System.Windows.Media.Media3D;
using System.Timers;

namespace SquadMod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MidiOut midiOut;
        private Timer timer;

        public MidiOut MidiOut
        {
            get { return midiOut; }
            set
            {
                if (midiOut != null)
                    midiOut.Dispose();
                midiOut = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                midiPortCombo.Items.Add(MidiOut.DeviceInfo(device).ProductName);
            }

            midiPortCombo.SelectedIndex = 0;
            midiPortCombo.DataContext = this;

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            intervalTextBox.DataContext = timer;

            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyHandleMouseButton), true);

            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText), true);
        }

        private void SelectivelyHandleMouseButton(object sender, MouseButtonEventArgs e)
        {
            var textbox = (sender as TextBox);

            if(textbox != null && !textbox.IsKeyboardFocusWithin)
            {
                if(e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    e.Handled = true;
                    textbox.Focus();
                }
            }
        }

        private void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textbox = e.OriginalSource as TextBox;

            if(textbox != null) textbox.SelectAll(); 
        }

        private void addRuleButton_Click(object sender, RoutedEventArgs e)
        {
            ruleStack.Children.Add(new ModulationRuleDataRow());
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ruleStack.Dispatcher.Invoke(CallRules);
        }

        private void CallRules()
        {
            foreach (ModulationRuleDataRow dataRow in ruleStack.Children)
            {
                dataRow.BoundRule.Evaluate(new Vector3D(63, 63, 0), midiOut);                    
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (midiOut != null) midiOut.Dispose();
        }
    }
}
