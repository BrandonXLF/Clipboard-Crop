using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClipboardCrop {
    public partial class SelectImageView : UserControl, ISetImage  {
        public SelectImageView() {
            InitializeComponent();
        }

        public void SetImage(BitmapSource? image) {
            if (image != null) {
                ((ContentControl)Parent).Content = new CropImageView(image);
            }
        }

        private void LoadClipboard_Click(object sender, RoutedEventArgs e) {
            SetImage(SaveLoad.LoadClipboard());
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e) {
            try {
                SetImage(SaveLoad.LoadFile());
            } catch (OperationCanceledException) { }
        }
    }
}
