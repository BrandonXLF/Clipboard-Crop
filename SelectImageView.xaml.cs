using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClipboardCrop {
    public partial class SelectImageView : UserControl, ISetImage {
        public BitmapSource? Image {
            set {
                if (value == null)
                    return;

                ((ContentControl)Parent).Content = new CropImageView(value);
            }
        }

        public SelectImageView() {
            InitializeComponent();
        }

        private void LoadClipboard_Click(object sender, RoutedEventArgs e) {
            Image = SaveLoad.LoadClipboard();
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e) {
            try {
                Image = SaveLoad.LoadFile();
            } catch (OperationCanceledException) { }
        }
    }
}
