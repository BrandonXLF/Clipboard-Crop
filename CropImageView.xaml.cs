using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClipboardCrop {
    public partial class CropImageView : UserControl, ISetImage, INotifyPropertyChanged {
        private BitmapSource _baseImage;
        private BitmapSource _previewImage;

        private int rotation = 0;
        private int hFlip = 1;
        private int vFlip = 1;
        private float _contrast = 1;
        private float _brightness = 0;
        private float _saturation = 1;

        public float Contrast {
            get => _contrast;
            set {
                _contrast = value;

                ApplyTransformations();
                NotifyPropertyChanged("Contrast");
            }
        }

        public float Brightness {
            get => _brightness;
            set {
                _brightness = value;

                ApplyTransformations();
                NotifyPropertyChanged("Brightness");
            }
        }

        public float Saturation {
            get => _saturation;
            set {
                _saturation = value;

                ApplyTransformations();
                NotifyPropertyChanged("Saturation");
            }
        }

        public BitmapSource? Image {
            get => _baseImage;

            set {
                if (value == null) {
                    ((ContentControl)Parent).Content = new SelectImageView();
                    return;
                }

                _baseImage = value;
                _previewImage = value;

                ResetTransformations();
                NotifyPropertyChanged("PreviewImage");
            }
        }

        public BitmapSource PreviewImage { 
            get => _previewImage;

            set {
                _previewImage = value;
                NotifyPropertyChanged("PreviewImage");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CropImageView(BitmapSource image) {
            InitializeComponent();
            Image = image;
        }

        private void NotifyPropertyChanged(string prop) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
            Image = SaveLoad.LoadClipboard();
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e) {
            try {
                Image = SaveLoad.LoadFile();
            } catch (OperationCanceledException) {}
        }

        private void SaveClipboard_Click(object sender, RoutedEventArgs e) {
            SaveLoad.SaveClipboard(new CroppedBitmap(Image, GetCropSize()));
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e) {
            SaveLoad.SaveFile(new CroppedBitmap(Image, GetCropSize()));
        }

        private void RotateLeft_Click(object sender, RoutedEventArgs e) {
            rotation -= 90;
            rotation %= 360;
            
            ApplyTransformations();
        }

        private void RotateRight_Click(object sender, RoutedEventArgs e) {
            rotation += 90;
            rotation %= 360;

            ApplyTransformations();
        }

        private void FlipHorizontally_Click(object sender, RoutedEventArgs e) {
            hFlip *= -1;

            ApplyTransformations();
        }

        private void FlipVertically_Click(object sender, RoutedEventArgs e) {
            vFlip *= -1;

            ApplyTransformations();
        }

        private void ApplyTransformations() {
            PreviewImage = Image;

            if (Contrast != 1 || Brightness != 0 || Saturation != 1) {
                FormatConvertedBitmap converted = new();
                converted.BeginInit();
                converted.Source = PreviewImage;
                converted.DestinationFormat = PixelFormats.Bgra32;
                converted.EndInit();

                byte[] bytes = new byte[converted.PixelHeight * converted.PixelWidth * 4];
                converted.CopyPixels(bytes, converted.PixelWidth * 4, 0);

                for (int i = 0; i < bytes.Length; i += 4) {
                    float grey = (bytes[i] + bytes[i + 1] + bytes[i + 2]) / 3;

                    for (int j = 0; j < 3; j++) {
                        float value = ((bytes[i + j] - 128) * Contrast + 128 + Brightness) * Saturation + grey * (1 - Saturation);

                        if (value < 0) {
                            value = 0;
                        } else if (value > 255) {
                            value = 255;
                        }

                        bytes[i + j] = (byte)value;
                    }
                }

                PreviewImage = BitmapSource.Create(
                    converted.PixelWidth,
                    converted.PixelHeight,
                    converted.DpiX,
                    converted.DpiY,
                    converted.Format,
                    null,
                    bytes,
                    converted.PixelWidth * 4
                );
            }

            if (rotation != 0)
                PreviewImage = new TransformedBitmap(PreviewImage, new RotateTransform(rotation));

            if (hFlip == -1 || vFlip == -1)
                PreviewImage = new TransformedBitmap(PreviewImage, new ScaleTransform(hFlip, vFlip));
        }

        private void ResetTransformations() {
            rotation = 0;
            hFlip = 1;
            vFlip = 1;
            _contrast = 1;
            _brightness = 0;
        }
    }
}
