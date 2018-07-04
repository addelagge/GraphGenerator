//Fredric Lagedal AH2318, 2017-09-19, Assignment 1

using System;
using System.Globalization;
using System.Windows.Data;

namespace GraphLib
{
    /// <summary>
    /// Returnerar false ifall något av värdena i object arrayen har värdet false. True i annat fall.
    /// </summary>
    public class MyMultipleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach(object value in values)
            {
                if ((value is bool) && (bool)value == false)
                    return false;
            }

            return true;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
