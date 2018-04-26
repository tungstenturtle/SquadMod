using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SquadMod
{
    public class ModulationRule : INotifyPropertyChanged
    {
        private Vector3D startVector;
        private Vector3D endVector;
        private bool ruleEnabled;
        private bool processZ;
        private int midiCC;
        private int divisions;
        private int channel;
        private string name;
        private int outputValue;

        public Vector3D StartVector
        {
            get { return startVector; }
            set
            {
                startVector = value;
                OnPropertyChanged();
            }
        }

        public Vector3D EndVector
        {
            get { return endVector; }
            set
            {
                endVector = value;
                OnPropertyChanged();
            }
        }

        public bool RuleEnabled
        {
            get { return ruleEnabled; }
            set
            {
                ruleEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessZ
        {
            get { return processZ; }
            set
            {
                processZ = value;
                OnPropertyChanged();
            }
        }

        public int MidiCC
        {
            get { return midiCC; }
            set
            {
                if (value < 0)
                    midiCC = 0;
                else if (value > 127)
                    midiCC = 127;
                else
                    midiCC = value;

                OnPropertyChanged();
            }
        }

        public int Divisions
        {
            get { return divisions; }
            set
            {
                if (value < 1)
                    divisions = 1;
                else if (value > 128)
                    divisions = 128;
                else
                    divisions = value;

                OnPropertyChanged();
            }
        }

        public int Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public int OutputValue
        {
            get { return outputValue; }
            set
            {
                if (value < 1)
                    outputValue = 1;
                else if (value > 127)
                    outputValue = 127;
                else
                    outputValue = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Creates a new ModulationRule with default values
        /// </summary>
        public ModulationRule()
        {
            this.StartVector = new Vector3D();
            this.EndVector = new Vector3D();
            this.RuleEnabled = false;
            this.MidiCC = 0;
            this.Divisions = 1;
            this.Channel = 1;
            this.Name = "New Rule";
        }

        /// <summary>
        /// Computes the output value for a MIDI event based on a gradient from the start vector
        /// to the end vector
        /// </summary>
        /// <param name="vector">the position to compute the value for</param>
        /// <returns>The calculated value if the rule is enabled, otherwise -1</returns>
        public virtual int Evaluate(Vector3D vector)
        {
            if (!ruleEnabled) return -1;

            Vector3D localVector = Vector3D.Subtract(vector, startVector);
            Vector3D resultant = Vector3D.Subtract(endVector, startVector);

            if(!processZ)
            {
                resultant.Z = 0;
                localVector.Z = 0;
            }

            Vector3D projection = Vector3D.DotProduct(localVector, resultant) / resultant.LengthSquared * resultant;
            OutputValue = (int)(Math.Round((projection.Length / resultant.Length) * divisions) / divisions * 127);

            return OutputValue;
        }

        /// <summary>
        /// Sends a midi control event
        /// </summary>
        /// <param name="midiOut">the MidiOut port to send the event to</param>
        /// <param name="outputValue">the value to send</param>
        public void SendEvent(MidiOut midiOut, int outputValue)
        {
            ControlChangeEvent controlEvent = new ControlChangeEvent(0, channel, (MidiController)midiCC, outputValue);
            midiOut.Send(controlEvent.GetAsShortMessage());
        }

        /// <summary>
        /// PropertyChanged event to properly handle data binding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// raises the PropertyChanged event
        /// </summary>
        /// <param name="caller">the caller of this method</param>
        protected void OnPropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}