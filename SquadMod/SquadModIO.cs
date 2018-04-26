using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Serialization;

namespace SquadMod
{
    /// <summary>
    /// Defines methods for importing and exporting ModulationRule configurations
    /// </summary>
    public static class SquadModIO
    {
        /// <summary>
        /// Exports the current ModulationRule configuration to an XML file
        /// </summary>
        /// <param name="ruleDataRowCollection">the collection of ModulationRuleDataRow objects</param>
        public static void ExportRules(UIElementCollection ruleDataRowCollection)
        {
            XmlDocument configDocument = new XmlDocument();
            XmlElement rootNode = configDocument.CreateElement("MODULATION_RULES");
            configDocument.AppendChild(rootNode);

            var ruleList = new List<ModulationRule>();

            foreach(ModulationRuleDataRow dataRow in ruleDataRowCollection)
            {
                ruleList.Add(dataRow.BoundRule);
            }

            string filename;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML-File | *.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filename = saveFileDialog.FileName;
            }
            else { return; }

            XmlSerializer serializer = new XmlSerializer(typeof(List<ModulationRule>));
            TextWriter writer = new StreamWriter(filename);

            serializer.Serialize(writer, ruleList);

            writer.Close();
        }

        /// <summary>
        /// Imports a ModulationRule configuration from an XML file
        /// </summary>
        /// <param name="ruleDataRowCollection">the collection of ModulationRuleDataRow objects</param>
        public static void ImportRules(UIElementCollection ruleDataRowCollection)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files|*.xml";
            openFileDialog.RestoreDirectory = true;

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var ioStream = openFileDialog.OpenFile();
                XmlDocument ruleConfigDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(typeof(List<ModulationRule>));
                
                var ruleList = new List<ModulationRule>();
                try
                {
                    ruleList = (List<ModulationRule>)serializer.Deserialize(ioStream);
                }
                catch(Exception e)
                {
                    MessageBox.Show("Failed to deserialize the XML file: " + e.Message, "SquadMod");
                }

                if (ruleList.Count == 0)
                {
                    MessageBox.Show("The selected XML file contains no Modulation Rules.", "SquadMod");
                    return;
                }

                ruleDataRowCollection.Clear();
                foreach(ModulationRule rule in ruleList)
                {
                    ruleDataRowCollection.Add(new ModulationRuleDataRow(rule));
                }
            }
        }
    }
}