using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml;

namespace SquadMod
{
    public static class SquadModIO
    {
        public static void ExportRules(UIElementCollection ruleDataRowCollection)
        {
            XmlDocument configDocument = new XmlDocument();
            XmlElement rootNode = configDocument.CreateElement("MODULATION_RULES");
            configDocument.AppendChild(rootNode);

            foreach(ModulationRuleDataRow dataRow in ruleDataRowCollection)
            {
                rootNode.AppendChild(FillRuleAttributes(configDocument.CreateElement("MODULATION_RULE"), dataRow.BoundRule));
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML-File | *.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                configDocument.Save(saveFileDialog.FileName);
            }
        }

        public static void ImportRules(UIElementCollection ruleDataRowCollection)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files|*.xml";
            openFileDialog.RestoreDirectory = true;

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var ioStream = openFileDialog.OpenFile();
                XmlDocument ruleConfigDocument = new XmlDocument();

                try
                {
                    ruleConfigDocument.Load(ioStream);
                }
                catch(XmlException e)
                {
                    MessageBox.Show("Could not parse the XML file. " + e, "SquadMod");
                    return;
                }

                XmlNodeList ruleNodes = ruleConfigDocument.SelectNodes("//MODULATION_RULE");

                if (ruleNodes.Count > 0)
                {
                    ruleDataRowCollection.Clear();

                    foreach (XmlElement ruleElement in ruleNodes)
                    {
                        ruleDataRowCollection.Add(CreateDataRow(ruleElement));
                    }
                }
                else
                {
                    MessageBox.Show("The selected file does not contain any modulation rules.", "SquadMod");
                }
            }
        }

        private static XmlElement FillRuleAttributes(XmlElement ruleElement, ModulationRule rule)
        {
            ruleElement.SetAttribute("NAME", rule.Name);
            ruleElement.SetAttribute("MIDI_CC", rule.MidiCC.ToString());
            ruleElement.SetAttribute("DIVISIONS", rule.Divisions.ToString());
            ruleElement.SetAttribute("CHANNEL", rule.Channel.ToString());
            ruleElement.SetAttribute("ENABLED", rule.RuleEnabled.ToString());
            ruleElement.SetAttribute("PROCESS_Z", rule.ProcessZ.ToString());
            ruleElement.SetAttribute("START_X", rule.StartVector.X.ToString());
            ruleElement.SetAttribute("START_Y", rule.StartVector.Y.ToString());
            ruleElement.SetAttribute("START_Z", rule.StartVector.Z.ToString());
            ruleElement.SetAttribute("END_X", rule.EndVector.X.ToString());
            ruleElement.SetAttribute("END_Y", rule.EndVector.Y.ToString());
            ruleElement.SetAttribute("END_Z", rule.EndVector.Z.ToString());

            return ruleElement;
        }

        private static ModulationRuleDataRow CreateDataRow(XmlElement ruleElement)
        {
            var dataRow = new ModulationRuleDataRow();

            dataRow.BoundRule.Name = ruleElement.GetAttribute("NAME");
            dataRow.BoundRule.MidiCC = int.Parse(ruleElement.GetAttribute("MIDI_CC"));
            dataRow.BoundRule.Divisions = int.Parse(ruleElement.GetAttribute("DIVISIONS"));
            dataRow.BoundRule.Channel = int.Parse(ruleElement.GetAttribute("CHANNEL"));
            dataRow.BoundRule.RuleEnabled = bool.Parse(ruleElement.GetAttribute("ENABLED"));
            dataRow.BoundRule.ProcessZ = bool.Parse(ruleElement.GetAttribute("PROCESS_Z"));

            double x, y, z;
            x = double.Parse(ruleElement.GetAttribute("START_X"));
            y = double.Parse(ruleElement.GetAttribute("START_Y"));
            z = double.Parse(ruleElement.GetAttribute("START_Z"));
            dataRow.BoundRule.StartVector = new Vector3D(x, y, z);

            x = double.Parse(ruleElement.GetAttribute("END_X"));
            y = double.Parse(ruleElement.GetAttribute("END_Y"));
            z = double.Parse(ruleElement.GetAttribute("END_Z"));
            dataRow.BoundRule.EndVector = new Vector3D(x, y, z);

            return dataRow;
        }
    }
}
