using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using NAudio.Midi;

namespace SquadMod
{
    public class MidiOutConverter : IValueConverter
    {
        private int lastValue;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return lastValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try { lastValue = (int)value; }
            catch { return new MidiOut(0); }
            return new MidiOut((int)value);
        }
    }
}
