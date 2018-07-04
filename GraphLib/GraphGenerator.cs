//Fredric Lagedal AH2318, 2017-11-06, Assignment 5

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphLib
{
    public class GraphGenerator : PropertyNotifyer
    {
        /// <summary>
        /// Ett offset värde som anger hur långt från vänstra kanten av ritytan grafens axlar ska ritas
        /// </summary>
        private int graphXOffset;

        /// <summary>
        /// Ett offset värde som anger hur långt från botten (eg toppen) av ritytan grafens axlar ska ritas
        /// </summary>
        private int graphYOffset;

        /// <summary>
        /// Text som visas på en sorteringsknapp
        /// </summary>
        private string sortByMessage;

        public bool SortByXValues { get; set; }

        /// <summary>
        /// Övre gränsen av intervallet på X-axeln
        /// </summary>
        private double maxXValue;

        /// <summary>
        /// Övre gränsen av intervallet på Y-axeln
        /// </summary>
        private double maxYValue;

        /// <summary>
        /// Undre gränsen av intervallet på X-axeln
        /// </summary>
        private double minXValue;

        /// <summary>
        /// Undre gränsen av intervallet på Y-axeln
        /// </summary>
        private double minYValue;

        /// <summary>
        /// Antalet divisioner y-axlen uppdelas i
        /// </summary>
        private int numYDivs;

        /// <summary>
        /// Antalet divisioner x-axlen uppdelas i
        /// </summary>
        private int numXDivs;

        /// <summary>
        /// Anger om graph-objektet är redo att börja ta emot värden
        /// </summary>
        private bool readyForUse;
        private bool inSetupMode;

        /// <summary>
        /// Anger om det några värden lagts till graph objektet
        /// </summary>
        private bool hasPoints;

        /// <summary>
        /// Antalet linjer, punkter och annat som själva stommen av diagrammet består av
        /// </summary>
        private int NumGraphElements { get; set; }

        /// <summary>
        /// Värdena som lagts till graph objektet
        /// </summary>
        private List<Point> pointsCanvasCoordinates;

        /// <summary>
        /// Kopia av listan points
        /// </summary>
        private ObservableCollection<Point> pointsActualValues;

        /// <summary>
        /// Ritytan där grafen ritas
        /// </summary>
        private Canvas canvas;

        public GraphGenerator(Canvas c)
        {
            canvas = c;
            pointsCanvasCoordinates = new List<Point>();
            pointsActualValues = new ObservableCollection<Point>();
            ReadyForUse = false;
            inSetupMode = true;
            SetSortByX();
        }


        /// <summary>
        /// Anger att punkterna ska sorteras efter deras X värde
        /// </summary>
        public void SetSortByX()
        {
            SortByXValues = true;
            SortByMessage = "Sort by Y";
        }


        /// <summary>
        /// Anger att punkterna ska sorteras efter deras Y värde
        /// </summary>
        public void SetSortByY()
        {
            SortByXValues = false;
            SortByMessage = "Sort by X";
        }


        /// <summary>
        /// Ritar linjer mellan alla tillagda Points
        /// </summary>
        public void DrawLinesBetweenPoints(Color color)
        {
            for (int i = 0; i < pointsCanvasCoordinates.Count - 1; i++)
            {
                Line line = new Line();
                Point start = pointsCanvasCoordinates[i];
                Point end = pointsCanvasCoordinates[i + 1];
                line.Stroke = new SolidColorBrush(color);
                line.StrokeThickness = 1;
                line.X1 = start.X + graphXOffset;
                line.Y1 = (start.Y + graphYOffset);
                line.X2 = end.X + graphXOffset;
                line.Y2 = end.Y + graphYOffset;
                canvas.Children.Add(line);
            }
        }


        /// <summary>
        /// Lägger till en Point. Om X- eller Y värdet är större än diagrammets övre gräns så skalas diagrammet om.
        /// </summary>
        public void AddPoint(Point point)
        {
            if (point.X < 0 || point.Y < 0)
                throw new ArgumentOutOfRangeException("point", "X or Y value can not be < 0");

            if (point.X < minXValue || point.Y < minYValue)
            {
                MessageBox.Show("Please enter values higher than the bottom values of each scale");
                return;
            }

            if (point.X > maxXValue || point.Y > maxYValue)
            {
                maxXValue *= (int)point.X / (int)maxXValue + 1;;
                maxYValue *= (int)point.Y / (int)maxYValue + 1;;
                TranslatePreviousPoints();
                RedrawGraph();
            }

            pointsActualValues.Add(point);
            pointsCanvasCoordinates.Add(TransformToCanvasCoordinates(point));
            HasPoints = pointsCanvasCoordinates.Count > 0;
        }


        /// <summary>
        /// Räknar ut nya "canvas koordinater" för tidigare tillagda points ifall diagrammet skalas om
        /// </summary>
        private Point TransformToCanvasCoordinates(Point point)
        {
            Point transformedPoint = new Point();
            transformedPoint.X = ((point.X - minXValue) / (maxXValue - minXValue)) * canvas.ActualWidth; 
            transformedPoint.Y = ((point.Y - minYValue) / (maxYValue - minYValue)) * canvas.ActualHeight; 
            return InvertYCoordinate(transformedPoint);
        }


        /// <summary>
        /// Omvandlar canvas koordinaterna till motsvarande diagramvärden
        /// </summary>
        private Point TransformToGraphCoordinates(Point point)
        {
            Point withNormalYAxis = InvertYCoordinate(point);
            double x = ((withNormalYAxis.X / canvas.ActualWidth) * (maxXValue - minXValue)) + minXValue;
            double y = ((withNormalYAxis.Y / canvas.ActualHeight) * (maxYValue - minYValue)) + minYValue;
            return new Point(x, y);
        }


        /// <summary>
        /// Omvandlar alla tidigare tillagda punkters x-värden till den nya scale factorn
        /// </summary>
        private void TranslatePreviousPoints()
        {
            pointsCanvasCoordinates.Clear();
            for (int i = 0; i < pointsActualValues.Count; i++)
            {
                Point p = TransformToCanvasCoordinates(pointsActualValues[i]);
                pointsCanvasCoordinates.Add(p);
            }
        }


        /// <summary>
        /// Ritar om markeringarna på axlarna med nya värden
        /// </summary>
        private void RedrawGraph()
        {
            NumGraphElements = 3;
            ClearGraph();
            InsertXYDivs();
        }


        /// <summary>
        /// Inverterar y-axeln
        /// </summary>
        private Point InvertYCoordinate(Point p)
        {
            p.Y = p.Y - canvas.ActualHeight;
            if (p.Y < 0)
                p.Y *= -1;

            return p;
        }


        /// <summary>
        /// Ritar x- och Y axlarna
        /// </summary>
        private void DrawXYAxis(List<Point> axisCoordinates)
        {
            foreach (Point point in axisCoordinates)
            {
                pointsCanvasCoordinates.Add(InvertYCoordinate(point));
                NumGraphElements++;
            }

            SortXValues();
            DrawLinesBetweenPoints(Colors.Black);
            ResetDiagram();
        }


        /// <summary>
        /// Ritar markeringarna på x- och y axlarna
        /// </summary>
        private void InsertXYDivs()
        {
            int pixelsBetweenYMarks = (int)canvas.ActualHeight / numYDivs;
            int pixelsBetweenXMarks = (int)canvas.ActualWidth / numXDivs;
            double xMark = (maxXValue - minXValue) / numXDivs;
            double yMark = (maxYValue - minYValue) / numYDivs;

            //Insert x-axis marks
            for (int i = 1; i <= numXDivs; i++)
            {
                int pos = pixelsBetweenXMarks * i;
                TextBlock t = new TextBlock() { Text = "|\n" + (xMark*i + minXValue), Foreground = Brushes.Red, Background = Brushes.Transparent };
                Canvas.SetLeft(t, graphXOffset + pos);
                Canvas.SetBottom(t, -graphYOffset / 5);
                canvas.Children.Add(t);
                NumGraphElements++;
            }

            //Insert y-axis marks
            for (int i = 1; i <= numYDivs; i++)
            {
                int pos = (int)(pixelsBetweenYMarks * i);
                TextBlock t = new TextBlock() { Text = (yMark*i + minYValue) + "-", Foreground = Brushes.Red, Background = Brushes.Transparent, Height = 15 };
                Canvas.SetLeft(t, graphXOffset / 5);
                Canvas.SetBottom(t, -graphYOffset + pos- t.Height / 2);
                canvas.Children.Add(t);
                NumGraphElements++;
            }
        }


        /// <summary>
        /// Initierar diagrammet genom att rita ut x. och y-axlarna samt markeringarna på dessa
        /// </summary>
        /// <param name="axisCoordinates"></param>
        public void Init(List<Point> axisCoordinates, int xDivs, int yDivs, double maxX, double maxY, double minX, double minY)
        {
            numXDivs = xDivs;
            numYDivs = yDivs;
            maxXValue = maxX;
            maxYValue = maxY;
            minXValue = minX;
            minYValue = minY;
            DrawXYAxis(axisCoordinates);
        }


        /// <summary>
        /// Tar bort allt från ritytan förutom x/y-axlarna och dess markeringar
        /// </summary>
        public void ClearGraph()
        {
            canvas.Children.RemoveRange(NumGraphElements - 1, canvas.Children.Count - 1);
        }


        /// <summary>
        /// Rensar diagrammet och ritar in det på nytt med de nuya värdena
        /// </summary>
        public void Render()
        {
            foreach (Point point in pointsCanvasCoordinates)
                DrawDotAt(point.X, point.Y);

            if (pointsCanvasCoordinates.Count > 1)
            {
                if (SortByXValues == true)
                    SortXValues();
                else
                    SortYValues();

                DrawLinesBetweenPoints(Colors.Red);
            }
        }


        /// <summary>
        /// Tar bort alla värden som lagts till diagrammet.
        /// </summary>
        public void ResetDiagram()
        {
            pointsCanvasCoordinates.Clear();
            pointsActualValues.Clear();
            HasPoints = pointsCanvasCoordinates.Count > 0;
            RedrawGraph();
        }


        /// <summary>
        /// Ritar en cirkel vid en punkts koordinater och lägger till en eventhandler som visar dessa värden när muspekaren är över cirkeln.
        /// </summary>
        private void DrawDotAt(double x, double y)
        {
            Ellipse ellipse = new Ellipse() { Height = 6, Width = 6, Fill = Brushes.Red };
            Canvas.SetTop(ellipse, y - ellipse.Height / 2 + graphYOffset);
            Canvas.SetLeft(ellipse, x - ellipse.Width / 2 + graphXOffset);

            ellipse.MouseEnter += Ellipse_MouseEnter;
            ellipse.MouseLeave += Ellipse_MouseLeave;

            canvas.Children.Add(ellipse);
        }      


        /// <summary>
        /// Anger vad som ska räknas som 0 i y-led
        /// </summary>
        public void SetYOffset(int value)
        {
            graphYOffset = value;
        }


        /// <summary>
        /// Anger vad som ska räknas som 0 i x-led
        /// </summary>
        public void SetXOffset(int value)
        {
            graphXOffset = value;
        }


        /// <summary>
        /// Sorterar alla punkter i stigande storleksordning i x-led
        /// </summary>
        private void SortXValues()
        {
            bool notDone = true;
            while (notDone)
            {
                notDone = false;
                for (int i = 0; i < pointsCanvasCoordinates.Count - 1; i++)
                {
                    if (pointsCanvasCoordinates[i].X > pointsCanvasCoordinates[i + 1].X)
                    {
                        Point temp = new Point(pointsCanvasCoordinates[i].X, pointsCanvasCoordinates[i].Y);
                        pointsCanvasCoordinates[i] = pointsCanvasCoordinates[i + 1];
                        pointsCanvasCoordinates[i + 1] = temp;
                        notDone = true;
                    }
                }
            }
        }


        /// <summary>
        /// Sorterar alla punkter i stigande storleksordning i y-led
        /// </summary>
        private void SortYValues()
        {
            bool notDone = true;
            while (notDone)
            {
                notDone = false;
                for (int i = 0; i < pointsCanvasCoordinates.Count - 1; i++)
                {
                    if (pointsCanvasCoordinates[i].Y > pointsCanvasCoordinates[i + 1].Y)
                    {
                        Point temp = new Point(pointsCanvasCoordinates[i].X, pointsCanvasCoordinates[i].Y);
                        pointsCanvasCoordinates[i] = pointsCanvasCoordinates[i + 1];
                        pointsCanvasCoordinates[i + 1] = temp;
                        notDone = true;
                    }
                }
            }
        }


        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse el = (Ellipse)sender;
            el.Fill = Brushes.Red;
            canvas.Children.RemoveAt(canvas.Children.Count - 1);
        }

        //När muspekaren är över en ellipse så ritas dess värden ut
        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse el = (Ellipse)sender;
            TextBlock t = new TextBlock() { Background = Brushes.Transparent, Foreground = Brushes.Blue };     
            double x = (Canvas.GetLeft(el) + el.Width / 2.0 - graphXOffset);
            double y = Canvas.GetTop(el) + el.Height / 2 - graphYOffset;
            Point point = TransformToGraphCoordinates(new Point(x, y));
            t.Text = $"X:{point.X}  Y:{point.Y}";
            Canvas.SetBottom(t, 0);
            el.Fill = Brushes.Blue;
            canvas.Children.Add(t);
        }

        public bool ReadyForUse
        {
            get { return readyForUse; }
            set
            {
                readyForUse = value;
                OnPropertyChanged();
            }
        }

        public bool HasPoints
        {
            get { return hasPoints; }
            set
            {
                hasPoints = value;
                OnPropertyChanged();
            }

        }

        public bool InSetupMode
        {
            get { return inSetupMode; }
            set
            {
                inSetupMode = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Point> Points
        {
            get { return pointsActualValues; }
        }

        public string SortByMessage
        {
            get { return sortByMessage; }
            set
            {
                sortByMessage = value;
                OnPropertyChanged();
            }
        }

    }
}
