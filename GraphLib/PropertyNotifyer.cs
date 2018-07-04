//Fredric Lagedal AH2318, 2017-09-19, Assignment 1

using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace GraphLib
{
    /// <summary>
    /// Klasser som ärver av denna klass kan anropa OnPropertyChanged när dess properties sätts. På så sätts uppmärksammas ett Control objekt som är bundet till ett property
    /// att det har uppdaterats, och Control objektet kan då uppdatera sitt värde.
    /// </summary>
    public class PropertyNotifyer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Metoden som meddelar Control objekten att property't de är bundna till har uppdaterats.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
