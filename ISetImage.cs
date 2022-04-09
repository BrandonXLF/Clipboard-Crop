using System.Windows.Media.Imaging;

namespace ClipboardCrop {
    public interface ISetImage {
        public abstract void SetImage(BitmapSource? image);
    }
}
