//Fredric Lagedal AH2318, 2017-09-19, Assignment 1

namespace GraphLib
{
    /// <summary>
    /// Klass som innehåller diverse allmänna metoder som kan vara användbara
    /// </summary>
    public class HelperMethods
    {

        /// <summary>
        /// Kollar om ett värde går att omvandla till en integer
        /// </summary>
        public static bool IsInteger(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            int result = 0;
            return int.TryParse(input, out result);
        }


        public static bool IsDouble(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            double result = 0;
            return double.TryParse(input, out result);
        }

    }
    
}
