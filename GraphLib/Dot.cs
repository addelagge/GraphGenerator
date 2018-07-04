using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphLib
{
    public class Dot
    {
        public Ellipse Ellipse { get; private set; }
        public Point Point { get; set; }
        public int YScaleFactor { get; set; }
        public int XScaleFactor { get; set; }

        public Dot()
        {
            Ellipse = new Ellipse() { Height = 6, Width = 6, Fill = Brushes.Red };
            YScaleFactor = 1;
            XScaleFactor = 1;
        }
    }
}
