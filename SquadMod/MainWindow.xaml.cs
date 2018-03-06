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
using NAudio.Midi;

namespace SquadMod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                midiPortCombo.Items.Add(MidiOut.DeviceInfo(device).ProductName);
            }
        }

        private void addRule_Click(object sender, RoutedEventArgs e)
        {
            ruleStack.Children.Add(new SquadMod.ModulationRuleDataRow());
        }
    }
}
