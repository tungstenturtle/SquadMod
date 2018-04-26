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

        /// <summary>
        /// Converts the last MidiOut value assigned to an integer - should never be called
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>the index of the last selected MidiOut object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return lastValue;
        }

        /// <summary>
        /// Converts the value stored in the combobox into a MidiOut stream
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>a new MidiOut object corresponding to the combobox selection</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try { lastValue = (int)value; }
            catch { return new MidiOut(0); }
            return new MidiOut((int)value);
        }
    }
}