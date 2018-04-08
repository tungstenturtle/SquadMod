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
        //private MidiEventCollection eventCollection = new MidiEventCollection(0, 60);

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

        public virtual void Evaluate(Vector3D vector, MidiOut midiOut)
        {
            if (!ruleEnabled || midiOut == null) return;

            Vector3D resultant = Vector3D.Subtract(endVector, startVector);

            if(!processZ)
            {
                resultant.Z = 0;
                vector.Z = 0;
            }

            Vector3D projection = Vector3D.DotProduct(vector, resultant) / resultant.LengthSquared * resultant;

            int outputValue = (int)((projection.Length / resultant.Length) * (divisions - 1));

            if (outputValue < 0) outputValue = 0;
            else if (outputValue > 127) outputValue = 127;

            SendEvent(midiOut, outputValue);
        }

        protected void SendEvent(MidiOut midiOut, int outputValue)
        {
            ControlChangeEvent controlEvent = new ControlChangeEvent(0, channel, (MidiController)midiCC, outputValue);
            //eventCollection.AddEvent(controlEvent, 0);
            midiOut.Send(controlEvent.GetAsShortMessage());
        }

        //public void ExportEvents()
        //{

        //    eventCollection.AddEvent(new NoteEvent(0, 1, MidiCommandCode.NoteOn, 10, 100), 0);
        //    eventCollection.AddEvent(new NoteEvent(200, 1, MidiCommandCode.NoteOn, 10, 0), 0);
        //    eventCollection.PrepareForExport();
        //    MidiFile.Export("D:\\Events_" + this.name + ".mid", eventCollection);
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
