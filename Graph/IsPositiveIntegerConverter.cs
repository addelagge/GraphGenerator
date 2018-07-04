//Fredric Lagedal AH2318, 2017-09-19, Assignment 1

using System;
using System.Globalization;
using System.Windows.Data;
using UtilitiesLib;

namespace MediaPlayerLib
{

    /// <summary>
    /// Returnerar true om ett värde är en integer > 0, False annars.
    /// </summary>
    public class IsPositiveIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((HelperMethods.IsInteger(value.ToString())))
            {
                if (int.Parse(value.ToString()) > 0)
                    return true;
            }

            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
