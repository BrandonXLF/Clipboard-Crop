using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClipboardCrop {
    public partial class CropImageView : UserControl, ISetImage, INotifyPropertyChanged {
        private BitmapSource _image;
        public BitmapSource Image { get => _image; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public CropImageView(BitmapSource image) {
            InitializeComponent();
            SetImage(image);
        }

        private void NotifyPropertyChanged(string prop) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void SetImage(BitmapSource? image) {
            if (image == null) {
                ((ContentControl)Parent).Content = new SelectImageView();
                return;
            }

            _image = image;
            NotifyPropertyChanged("Image");
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e) {
            double deltaVertical, deltaHorizontal;

            if (sender is not Control thumb) return;

            switch (thumb.VerticalAlignment) {
                case VerticalAlignment.Top:
                    deltaVertical = Math.Clamp(e.VerticalChange, -topMarign.Height.Value, parent.ActualHeight - parent.MinHeight);
                    topMarign.Height = new GridLength(topMarign.Height.Value + deltaVertical);
                    parent.Height -= deltaVertical;

                    break;
                case VerticalAlignment.Bottom:
                    deltaVertical = Math.Clamp(-e.VerticalChange, -container.ActualHeight + parent.ActualHeight + topMarign.Height.Value, parent.ActualHeight - parent.MinHeight);
                    parent.Height -= deltaVertical;

                    break;
            }

            switch (thumb.HorizontalAlignment) {
                case HorizontalAlignment.Left:
                    deltaHorizontal = Math.Clamp(e.HorizontalChange, -leftMargin.Width.Value, parent.ActualWidth - parent.MinWidth);
                    leftMargin.Width = new GridLength(leftMargin.Width.Value + deltaHorizontal);
                    parent.Width -= deltaHorizontal;

                    break;
                case HorizontalAlignment.Right:
                    deltaHorizontal = Math.Clamp(-e.HorizontalChange, -container.ActualWidth + parent.ActualWidth + leftMargin.Width.Value, parent.ActualWidth - parent.MinWidth);
                    parent.Width -= deltaHorizontal;

                    break;
            }

            e.Handled = true;
        }

        private void CenterThumb_DragDelta(object sender, DragDeltaEventArgs e) {
            if (e.VerticalChange > 0) {
                topMarign.Height = new GridLength(Math.Min(container.ActualHeight - parent.ActualHeight, topMarign.Height.Value + e.VerticalChange));
            } else if (e.VerticalChange < 0) {
                topMarign.Height = new GridLength(Math.Max(0, topMarign.Height.Value + e.VerticalChange));
            }

            if (e.HorizontalChange > 0) {
                leftMargin.Width = new GridLength(Math.Min(container.ActualWidth - parent.ActualWidth, leftMargin.Width.Value + e.HorizontalChange));
            } else if (e.HorizontalChange < 0) {
                leftMargin.Width = new GridLength(Math.Max(0, leftMargin.Width.Value + e.HorizontalChange));
            }

            e.Handled = true;
        }

        private Int32Rect GetCropSize() {
            double widthRatio = Image.PixelWidth / container.ActualWidth;
            double heightRatio = Image.PixelHeight / container.ActualHeight;

            return new Int32Rect(
                (int)(leftMargin.Width.Value * widthRatio),
                (int)(topMarign.Height.Value * heightRatio),
                (int)(parent.ActualWidth * widthRatio),
                (int)(parent.ActualHeight * heightRatio)
            );
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e) {
            if (e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0) {
                parent.Width = imageElement.ActualWidth;
                parent.Height = imageElement.ActualHeight;
                leftMargin.Width = new GridLength(0);
                topMarign.Height = new GridLength(0);

                return;
            }

            double widthRatio = e.NewSize.Width / e.PreviousSize.Width;
            double heightRatio = e.NewSize.Height / e.PreviousSize.Height;

            parent.Width *= widthRatio;
            parent.Height *= heightRatio;

            leftMargin.Width = new GridLength(leftMargin.Width.Value * widthRatio);
            topMarign.Height = new GridLength(topMarign.Height.Value * heightRatio);
        }

        private void LoadClipboard_Click(object sender, RoutedEventArgs e) {
            SetImage(SaveLoad.LoadClipboard());
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e) {
            try {
                SetImage(SaveLoad.LoadFile());
            } catch (OperationCanceledException) {}
        }

        private void SaveClipboard_Click(object sender, RoutedEventArgs e) {
            SaveLoad.SaveClipboard(new CroppedBitmap(Image, GetCropSize()));
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e) {
            SaveLoad.SaveFile(new CroppedBitmap(Image, GetCropSize()));
        }

        private void RotateLeft_Click(object sender, RoutedEventArgs e) {
            SetImage(new TransformedBitmap(Image, new RotateTransform(-90)));
        }

        private void RotateRight_Click(object sender, RoutedEventArgs e) {
            SetImage(new TransformedBitmap(Image, new RotateTransform(90)));
        }

        private void FlipHorizontally_Click(object sender, RoutedEventArgs e) {
            SetImage(new TransformedBitmap(Image, new ScaleTransform(-1, 1)));
        }

        private void FlipVertically_Click(object sender, RoutedEventArgs e) {
            SetImage(new TransformedBitmap(Image, new ScaleTransform(1, -1)));
        }
    }
}
