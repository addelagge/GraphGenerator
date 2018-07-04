//Fredric Lagedal AH2318, 2017-11-02, Assignment 5

using System;
using System.Collections.Generic;
using System.Windows;
using GraphLib;

namespace Graph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GraphGenerator graph;

        public MainWindow()
        {
            InitializeComponent();
            graph = new GraphGenerator(canvasLines);
            DataContext = graph;
        }

        private Point GetPoint(string x, string y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                throw new ArgumentNullException("Coordinates can not be null");

            Point p = new Point(double.Parse(x), double.Parse(y));
            return p;
        }

        private void InitGraph()
        {
            List<Point> axisCoordinates = new List<Point>();
            axisCoordinates.Add(new Point(0, canvasLines.ActualHeight));
            axisCoordinates.Add(new Point(canvasLines.ActualWidth, 0));
            axisCoordinates.Add(new Point(0, 0));
            int yDivs = int.Parse(txtYDivs.Text);
            int xDivs = int.Parse(txtXDivs.Text);
            double maxXValue = double.Parse(txtEndX.Text);
            double maxYValue = double.Parse(txtEndY.Text);
            double minxValue = double.Parse(txtStartX.Text);
            double minYValue = double.Parse(txtStartY.Text);
            graph.Init(axisCoordinates, xDivs, yDivs, maxXValue, maxYValue, minxValue, minYValue);
        }

        private void btnAddPoints_Click(object sender, RoutedEventArgs e)
        {
            Point newPoint = GetPoint(txtX.Text, txtY.Text);
            graph.AddPoint(newPoint);
            graph.ClearGraph();
            graph.Render();
            txtX.Text = "";
            txtY.Text = "";

        }

        private void mainWindow_ContentRendered(object sender, EventArgs e)
        {
            graph.SetXOffset(30);
            graph.SetYOffset(-30);
        }

        private void btnClearGraph_Click(object sender, RoutedEventArgs e)
        {
            graph.ResetDiagram();
            graph.ClearGraph();
            graph.Render();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(double.Parse(txtEndX.Text) <= double.Parse(txtStartX.Text) || double.Parse(txtEndY.Text) <= double.Parse(txtStartY.Text))
            {
                MessageBox.Show("Values at the bottom of the scale can not be higher than the values at the top");
                return;
            }

            graph.ReadyForUse = true;
            graph.InSetupMode = false;
            diagramName.Text = txtInputDiagramName.Text;
            txtInputDiagramName.Text = "";
            InitGraph();
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (graph.SortByXValues)
                graph.SetSortByY();
            else
                graph.SetSortByX();

            graph.ClearGraph();
            graph.Render();
        }
    }
}
