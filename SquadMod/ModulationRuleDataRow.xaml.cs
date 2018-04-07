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
using System.Windows.Media.Media3D;

namespace SquadMod
{
    /// <summary>
    /// Interaction logic for ModulationRuleDataRow.xaml
    /// </summary>
    public partial class ModulationRuleDataRow : UserControl
    {
        private ModulationRule boundRule = new ModulationRule();
        public ModulationRule BoundRule { get { return boundRule; } }

        public ModulationRuleDataRow()
        {
            InitializeComponent();

            for (int i = 1; i < 17; i++)
            {
                channelCombo.Items.Add(i.ToString());
            }

            DataContext = boundRule;
            channelCombo.SelectedItem = boundRule.Channel.ToString();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((StackPanel)this.Parent).Children.Remove(this);
        }

        private void setStartButton_Click(object sender, RoutedEventArgs e)
        {
            boundRule.StartVector = new Vector3D(63, 63, 0);
        }

        private void setEndButton_Click(object sender, RoutedEventArgs e)
        {
            boundRule.EndVector = new Vector3D(190, 190, 0);
        }
    }
}
