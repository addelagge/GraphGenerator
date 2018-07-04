using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Graph
{
    public class GraphGenerator
    {
        private int GraphXOffset { get; set; }
        private int GraphYOffset { get; set; }

        /// <summary>
        /// Antalet linjer, punkter och annat som själva stommen av diagrammet består av
        /// </summary>
        private int NumGraphElements { get; set; }
        private List<Point> points;
        private Canvas canvas;

        public GraphGenerator(Canvas c)
        {
            canvas = c;
            points = new List<Point>();
        }


        /// <summary>
        /// Ritar linjer mellan alla tillagda Points
        /// </summary>
        public void DrawLinesBetweenPoints(Color color)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Line line = new Line();
                Point start = points[i];
                Point end = points[i + 1];
                line.Stroke = new SolidColorBrush(color);
                line.StrokeThickness = 1;
                line.X1 = start.X + GraphXOffset;
                line.Y1 = start.Y + GraphYOffset;
                line.X2 = end.X + GraphXOffset;
                line.Y2 = end.Y + GraphYOffset;
                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Lägger till en Point
        /// </summary>
        public void AddPoint(Point point)
        {
            if (point.X < 0 || point.Y < 0)
                throw new ArgumentOutOfRangeException("point", "X or Y value can not be < 0");

            points.Add(TranslatePoint(point));
        }

        /// <summary>
        /// Inverterar y-axeln
        /// </summary>
        private Point TranslatePoint(Point p)
        {
            p.Y = p.Y - canvas.ActualHeight;
            if (p.Y < 0)
                p.Y *= -1;

            return p;
        }

        /// <summary>
        /// Initierar diagrammet genom att rita ut x. och y-axlarna samt markeringarna på dessa
        /// </summary>
        /// <param name="axisCoordinates"></param>
        public void Init(List<Point> axisCoordinates)
        {
            foreach(Point point in axisCoordinates)
            {
                AddPoint(point);
                NumGraphElements++;
            }

            SortXValues();
            DrawLinesBetweenPoints(Colors.Black);
            points.Clear();

            //Insert x-axis marks
            for (int i = 1; i <= 8; i++)
            {
                int num = 30 * i;
                TextBlock t = new TextBlock() { Text = "|\n" + num, Foreground = Brushes.Red, Background = Brushes.Transparent };
                Canvas.SetLeft(t, GraphXOffset + num);
                Canvas.SetBottom(t, -GraphYOffset / 5);
                canvas.Children.Add(t);
                NumGraphElements++;
            }

            //Insert y-axis marks
            for (int i = 1; i <= 8; i++)
            {
                int num = 30 * i;
                TextBlock t = new TextBlock() { Text = num + "-", Foreground = Brushes.Red, Background = Brushes.Transparent };
                Canvas.SetLeft(t, GraphXOffset / 5);
                Canvas.SetBottom(t, -GraphYOffset + num);
                canvas.Children.Add(t);
                NumGraphElements++;
            }
        }


        /// <summary>
        /// Anger vad som ska räknas som 0 i y-led
        /// </summary>
        public void SetYOffset(int value)
        {
            GraphYOffset = value;
        }

        /// <summary>
        /// Anger vad som ska räknas som 0 i x-led
        /// </summary>
        public void SetXOffset(int value)
        {
            GraphXOffset = value;
        }


        public void SortXValues()
        {
            SortPoints();
        }


        /// <summary>
        /// Sorterar alla punkter i stigande storleksordning i x-led
        /// </summary>
        private void SortPoints()
        {
            bool notDone = true;
            while (notDone)
            {
                notDone = false;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    if (points[i].X > points[i + 1].X)
                    {
                        Point temp = new Point(points[i].X, points[i].Y);
                        points[i] = points[i + 1];
                        points[i + 1] = temp;
                        notDone = true;
                    }
                }
            }
        }


        /// <summary>
        /// Tar bort allt från ritytan förutom x/y-axlarna och dess markeringar
        /// </summary>
        public void ClearGraph()
        {
            canvas.Children.RemoveRange(NumGraphElements-1, canvas.Children.Count - 1);
        }

        /// <summary>
        /// Rensar diagrammet och ritar in det på nytt med de nuya värdena
        /// </summary>
        public void Render()
        {
            ClearGraph();

            foreach (Point point in points)
                DrawDotAt(point.X, point.Y);

            if (points.Count > 1)
            {
                SortXValues();
                DrawLinesBetweenPoints(Colors.Red);
            }
        }

        /// <summary>
        /// Tar bort alla värden som lagts till diagrammet.
        /// </summary>
        public void ResetDiagram()
        {
            points.Clear();
        }

        private void DrawDotAt(double x, double y)
        {
            Ellipse ellipse = new Ellipse() { Height = 6, Width = 6, Fill = Brushes.Red };
            Canvas.SetLeft(ellipse, x - ellipse.Width / 2 + GraphXOffset);
            Canvas.SetTop(ellipse, y - ellipse.Height / 2 + GraphYOffset);

            ellipse.MouseEnter += Ellipse_MouseEnter;
            ellipse.MouseLeave += Ellipse_MouseLeave;

            canvas.Children.Add(ellipse);
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse el = (Ellipse)sender;
            el.Fill = Brushes.Red;
            canvas.Children.RemoveAt(canvas.Children.Count - 1);
        }

        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse el = (Ellipse)sender;
            TextBlock t = new TextBlock() { Background = Brushes.Transparent, Foreground = Brushes.Blue };
            double ellipseTop = Canvas.GetTop(el) - canvas.ActualHeight;
            if (ellipseTop < 0)
                ellipseTop *= -1;

            t.Text = (Canvas.GetLeft(el) + el.Width / 2 - GraphXOffset).ToString() + ", " + (ellipseTop - el.Height / 2 + GraphYOffset);
            Canvas.SetBottom(t, 0);
            el.Fill = Brushes.Blue;
            canvas.Children.Add(t);
        }


    }
}
