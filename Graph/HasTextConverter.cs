//Fredric Lagedal AH2318, 2017-09-19, Assignment 1

using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPlayerLib
{
    /// <summary>
    /// Returnerar false ifall en string är tom, true annars. 
    /// </summary>
    public class HasTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value.ToString();
            if (string.IsNullOrEmpty(input))
                return false;

            return true;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
