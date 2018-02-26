using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;
using System.Windows.Media.Media3D;

namespace SquadMod
{
    class ModulationRule
    {
        public Vector3D StartVector { get; set; }
        public Vector3D EndVector { get; set; }
        public bool RuleEnabled { get; set; }
        public int MidiCC { get; set; }
        public int Divisions { get; set; }
        public int Channel { get; set; }
        public string Name { get; set; }        

        public ModulationRule()
        {
            this.StartVector = new Vector3D();
            this.EndVector = new Vector3D();
            this.RuleEnabled = false;
            this.MidiCC = 0;
            this.Divisions = 0;
            this.Channel = 1;
            this.Name = "New Rule";
        }

        public ModulationRule(Vector3D startVector, Vector3D endVector, bool ruleEnabled, int midiCC, int divisions, int channel, string name)
        {
            this.StartVector = startVector;
            this.EndVector = endVector;
            this.RuleEnabled = ruleEnabled;
            this.MidiCC = midiCC;
            this.Divisions = divisions;
            this.Channel = channel;
            this.Name = name;
        }

        public void Evaluate(Vector3D vector, MidiOut midiOut)
        {
            if (!RuleEnabled) return;

            Vector3D resultant = Vector3D.Subtract(EndVector, StartVector);
            Vector3D projection = Vector3D.DotProduct(vector, resultant) / resultant.LengthSquared * resultant;

            int outputValue = (int)((projection.Length / resultant.Length) * Divisions);
            if (outputValue < 0) outputValue = 0;
            if (outputValue > 127) outputValue = 127;

            ControlChangeEvent controlEvent = new ControlChangeEvent(0, Channel, (MidiController)MidiCC, outputValue);

            midiOut.Send(controlEvent.GetAsShortMessage());
        }
    }
}
