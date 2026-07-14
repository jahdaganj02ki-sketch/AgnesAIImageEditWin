using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AgnesAIImageEdit.Views
{
    public partial class FullScreenWindow : Window
    {
        public BitmapImage ImageSource { get; }
        private Point _lastMousePos;
        private bool _isPanning;

        public FullScreenWindow(BitmapImage imageSource)
        {
            InitializeComponent();
            ImageSource = imageSource;
            DataContext = this;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            else if (e.Key == Key.Add || e.Key == Key.OemPlus)
                Zoom(1.1);
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                Zoom(1 / 1.1);
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var mousePos = e.GetPosition(Image);
            double factor = e.Delta > 0 ? 1.1 : 1 / 1.1;
            Zoom(factor, mousePos);
        }

        private void Zoom(double factor, Point? mousePos = null)
        {
            double newScale = ScaleTransform.ScaleX * factor;
            if (newScale < 0.1) newScale = 0.1;
            if (newScale > 5.0) newScale = 5.0;
            if (Math.Abs(newScale - ScaleTransform.ScaleX) < 0.001) return;

            Point center = mousePos ?? new Point(Image.ActualWidth / 2, Image.ActualHeight / 2);
            double scaleFactor = newScale / ScaleTransform.ScaleX;

            TranslateTransform.X = center.X - (center.X - TranslateTransform.X) * scaleFactor;
            TranslateTransform.Y = center.Y - (center.Y - TranslateTransform.Y) * scaleFactor;

            ScaleTransform.ScaleX = newScale;
            ScaleTransform.ScaleY = newScale;

            ClampTranslation();
        }

        private void ClampTranslation()
        {
            if (Image.Source is not System.Windows.Media.Imaging.BitmapSource bitmap) return;

            double scaledWidth = bitmap.PixelWidth * ScaleTransform.ScaleX;
            double scaledHeight = bitmap.PixelHeight * ScaleTransform.ScaleY;
            double viewportWidth = Image.ActualWidth;
            double viewportHeight = Image.ActualHeight;

            if (scaledWidth <= viewportWidth)
            {
                TranslateTransform.X = (viewportWidth - scaledWidth) / 2;
            }
            else
            {
                double maxX = (scaledWidth - viewportWidth) / 2;
                TranslateTransform.X = Math.Max(-maxX, Math.Min(maxX, TranslateTransform.X));
            }

            if (scaledHeight <= viewportHeight)
            {
                TranslateTransform.Y = (viewportHeight - scaledHeight) / 2;
            }
            else
            {
                double maxY = (scaledHeight - viewportHeight) / 2;
                TranslateTransform.Y = Math.Max(-maxY, Math.Min(maxY, TranslateTransform.Y));
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ResetZoom();
                return;
            }

            if (ScaleTransform.ScaleX > 1.0)
            {
                _isPanning = true;
                _lastMousePos = e.GetPosition(Image);
                Image.CaptureMouse();
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && ScaleTransform.ScaleX > 1.0)
            {
                Point currentPos = e.GetPosition(Image);
                TranslateTransform.X += currentPos.X - _lastMousePos.X;
                TranslateTransform.Y += currentPos.Y - _lastMousePos.Y;
                _lastMousePos = currentPos;
                ClampTranslation();
            }

            ShowToolbar();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                Image.ReleaseMouseCapture();
            }
        }

        private void Image_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ResetZoom();
        }

        private void ResetZoom()
        {
            ScaleTransform.ScaleX = 1.0;
            ScaleTransform.ScaleY = 1.0;
            TranslateTransform.X = 0;
            TranslateTransform.Y = 0;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e) => Zoom(1.1);
        private void ZoomOut_Click(object sender, RoutedEventArgs e) => Zoom(1 / 1.1);
        private void ResetZoom_Click(object sender, RoutedEventArgs e) => ResetZoom();
        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void ShowToolbar()
        {
            Toolbar.Visibility = Visibility.Visible;
            Toolbar.Opacity = 1.0;

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                HideToolbar();
            };
            timer.Start();
        }

        private void HideToolbar()
        {
            if (Toolbar.Visibility == Visibility.Collapsed) return;

            var anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300));
            anim.Completed += (s, args) =>
            {
                Toolbar.Visibility = Visibility.Collapsed;
            };
            Toolbar.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}