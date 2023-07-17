using System.Windows.Media.Imaging;

namespace ClipboardCrop {
    public interface ISetImage {
        public BitmapSource? Image { set; }
    }
}
