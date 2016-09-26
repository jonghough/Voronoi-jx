using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Voronoi_jx;

namespace Voronoi_jx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Brush _brush = System.Windows.Media.Brushes.LightSteelBlue;
        private Color _pointColor = Color.FromArgb(255, 0, 0, 0);
        List<VectorNode> _pointList = new List<VectorNode>();
        private VoronoiGenerator _voronoi;

        private enum ColorSelect { Bg, Line, Point };
        private ColorSelect _colorSelect;

        private bool _showVoronoi = true;
        private bool _showDelaunay = false;

        public MainWindow()
        {
            InitializeComponent();
            ClrPcker_Background.Visibility = Visibility.Hidden;

            _voronoi = new VoronoiGenerator(_pointList);
        }


        private void SaveCanvasAsBitmap()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Image";
            saveFileDialog.DefaultExt = ".png";
            saveFileDialog.Filter = "Image files (.png)|*.png";

            Nullable<bool> result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveFileDialog.FileName;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas1.RenderSize.Width,
                    (int)this.canvas1.RenderSize.Height, 100d, 100d, System.Windows.Media.PixelFormats.Default);
                rtb.Render(canvas1);

                var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)canvas1.Width, (int)canvas1.Height));

                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                using (var fs = System.IO.File.OpenWrite(filename))
                {
                    pngEncoder.Save(fs);
                }
            }
        }

        private void DrawPoint(Point p)
        {
            var circle = new Ellipse();
            circle.Width = 4;
            circle.Height = 4;
            circle.Margin = new Thickness(p.X - 2, p.Y - 2, 0, 0);
            SolidColorBrush solidColorBrush = new SolidColorBrush();

            solidColorBrush.Color = _pointColor;
            circle.Fill = solidColorBrush;
            this.canvas1.Children.Add(circle);
        }



        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            switch (_colorSelect)
            {
                case ColorSelect.Bg:
                    canvas1.Background = new SolidColorBrush(ClrPcker_Background.SelectedColor.Value);
                    break;
                case ColorSelect.Line:
                    _brush = new SolidColorBrush(ClrPcker_Background.SelectedColor.Value);
                    break;
                case ColorSelect.Point:
                    _pointColor = ClrPcker_Background.SelectedColor.Value;
                    break;
            }
            ClrPcker_Background.Visibility = Visibility.Hidden;
            DrawWithoutRebuild();
        }


        private void DrawWithoutRebuild()
        {
            this.canvas1.Children.Clear();
            foreach (var point in _pointList)
            {
                Point p = new Point(point.x, point.y);
                DrawPoint(p);
            }

            //only draw voronoi if ok
            if (_showVoronoi)
            {
                var circles = _voronoi.GetNodes();
                foreach (var ce in circles)
                {
                    HalfEdge edge = ce.halfEdge;


                    HalfEdge f = edge;
                    while (edge != null)
                    {
                        var myLine = new Line();
                        myLine.Stroke = _brush;
                        myLine.X1 = edge.Twin().GetTarget().X();
                        myLine.X2 = edge.GetTarget().X();
                        myLine.Y1 = edge.Twin().GetTarget().Y();
                        myLine.Y2 = edge.GetTarget().Y();
                        myLine.HorizontalAlignment = HorizontalAlignment.Left;
                        myLine.VerticalAlignment = VerticalAlignment.Center;
                        myLine.StrokeThickness = 1;
                        this.canvas1.Children.Add(myLine);
                        edge = edge.Next();
                        if (edge == null || f == edge)
                            break;
                    }
                }
            }
            if (_showDelaunay)
            {
                ShowDelaunayEdges();
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point px = Mouse.GetPosition(this.canvas1);
            _pointList.Add(new VectorNode((int)px.X, (int)px.Y));
          

            this.canvas1.Children.Clear();
            foreach (var point in _pointList)
            {
                Point p = new Point(point.x, point.y);
                DrawPoint(p);
            }

            _voronoi.Rebuild();

            //only draw voronoi if ok
            if (_showVoronoi)
            {
                var circles = _voronoi.GetNodes();
                foreach (var ce in circles)
                {
                    HalfEdge edge = ce.halfEdge;
                    HalfEdge f = edge;
                    while (edge != null)
                    {
                        var myLine = new Line();
                        myLine.Stroke = _brush;
                        myLine.X1 = edge.Twin().GetTarget().X();
                        myLine.X2 = edge.GetTarget().X();
                        myLine.Y1 = edge.Twin().GetTarget().Y();
                        myLine.Y2 = edge.GetTarget().Y();
                        myLine.HorizontalAlignment = HorizontalAlignment.Left;
                        myLine.VerticalAlignment = VerticalAlignment.Center;
                        myLine.StrokeThickness = 1;
                        this.canvas1.Children.Add(myLine);
                        edge = edge.Next();
                        if (edge == null || f == edge)
                            break;
                    }
                }
            }

            if (_showDelaunay)
            {
                ShowDelaunayEdges();
            }
        }


        private void ShowDelaunayEdges()
        {
            var delaunayEdges = _voronoi.generateDelaunayGraph();
            foreach (var d in delaunayEdges)
            {

                var myLine = new Line();
                myLine.Stroke = _brush;
                myLine.X1 = d.from.x;
                myLine.X2 = d.to.x;
                myLine.Y1 = d.from.y;
                myLine.Y2 = d.to.y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 1;
                this.canvas1.Children.Add(myLine);

            }
        }
        


        private void _saveClick(object sender, RoutedEventArgs e)
        {
            SaveCanvasAsBitmap();
        }

        private void _newClick(object sender, RoutedEventArgs e)
        {
            _pointList = new List<VectorNode>();
            _voronoi = new VoronoiGenerator(_pointList);
            this.canvas1.Children.Clear();
        }

        private void Line_Color_Click(object sender, RoutedEventArgs e)
        {
            _colorSelect = ColorSelect.Line;
            ClrPcker_Background.Visibility = Visibility.Hidden;
            this.ClrPcker_Background.IsOpen = true;
        }

        private void PointColorSelect(object sender, RoutedEventArgs e)
        {
            _colorSelect = ColorSelect.Point;
            ClrPcker_Background.Visibility = Visibility.Hidden;
            this.ClrPcker_Background.IsOpen = true;
        }


        private void BG_Click(object sender, RoutedEventArgs e)
        {
            _colorSelect = ColorSelect.Bg;
            ClrPcker_Background.Visibility = Visibility.Hidden;
            this.ClrPcker_Background.IsOpen = true;
        }

        private void Delaunay_Click(object sender, RoutedEventArgs e)
        {
            if (!_showDelaunay)
            {
                if (_voronoi != null && _voronoi.GetAllCircleEvents().Count > 1)
                {
                    
                    _showDelaunay = true;
                    DrawWithoutRebuild();
                    this.ShowDelaunayMenuItem.Header = "hide delaunay graph";
                }
                
            }
            else
            {
                _showDelaunay = false;
                DrawWithoutRebuild();
                this.ShowDelaunayMenuItem.Header = "show delaunay graph";
            }

            
        }

        private void Voronoi_Click(object sender, RoutedEventArgs e)
        {
            if (_showVoronoi)
            {
                _showVoronoi = false;
                this.ShowVoronoiMenuItem.Header = "show voronoi edges";
                DrawWithoutRebuild();
            }
            else
            {

                _showVoronoi = true;
                this.ShowVoronoiMenuItem.Header = "hide voronoi edges";
                DrawWithoutRebuild();
            }
        }
    }
}
