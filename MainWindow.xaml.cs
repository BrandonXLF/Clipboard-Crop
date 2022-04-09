using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Linq;

namespace ClipboardCrop {
    public partial class MainWindow : Window, ISetImage {
        public MainWindow() {
            InitializeComponent();

            BitmapSource? initialImage = SaveLoad.LoadClipboard();

            if (initialImage == null) {
                main.Content = new SelectImageView();
            } else {
                main.Content = new CropImageView(initialImage);
            }
        }

        public void SetImage(BitmapSource? image) {
           ((ISetImage)main.Content).SetImage(image);
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e) {
            BitmapSource? pastedImage = SaveLoad.LoadClipboard();

            if (pastedImage == null) {
                string? file = Clipboard.GetFileDropList()?.OfType<string>().FirstOrDefault();
                if (file != null) pastedImage = SaveLoad.LoadFile(file);
            }

            SetImage(pastedImage);
        }

        private void Window_DragOver(object sender, DragEventArgs e) {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e) {
            string? filename = ((string[])e.Data.GetData(DataFormats.FileDrop))?.FirstOrDefault();

            if (filename == null) return;

            SetImage(SaveLoad.LoadFile(filename));
        }
    }
}
