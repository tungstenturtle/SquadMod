using System;
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
        private UDPListener networkConnection = UDPListener.Instance;
        private MidiOut midiOut;
        private Timer timer;

        public MidiOut MidiOut
        {
            get { return midiOut; }
            set
            {
                if (midiOut != null) midiOut.Close();
                midiOut = value;
            }
        }

        public bool Enabled { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                midiPortCombo.Items.Add(MidiOut.DeviceInfo(device).ProductName);
            }

            midiPortCombo.SelectedIndex = 0;
            midiPortCombo.DataContext = this;

            enableButton.DataContext = this;

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            intervalTextBox.DataContext = timer;

            this.Closing += MainWindow_Closing;

            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyHandleMouseButton), true);

            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText), true);

            networkConnection.Listen();
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
            if(Enabled) ruleStack.Dispatcher.Invoke(CallRules);
        }

        private void CallRules()
        {
            foreach (ModulationRuleDataRow dataRow in ruleStack.Children)
            {
                dataRow.BoundRule.Evaluate(networkConnection.NextPoint(), midiOut);                    
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (midiOut != null) midiOut.Close();

            networkConnection.Stop();
            networkConnection.Close();
        }
    }
}
