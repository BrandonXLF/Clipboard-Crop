using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ClipboardCrop
{
    internal class SaveLoad {
        private static readonly Dictionary<string, Type> encoders = new() {
            { ".png", typeof(PngBitmapEncoder) },
            { ".jpg", typeof(JpegBitmapEncoder) },
            { ".jpeg", typeof(JpegBitmapEncoder) },
            { ".jpe", typeof(JpegBitmapEncoder) },
            { ".jfif", typeof(JpegBitmapEncoder) },
            { ".bmp", typeof(BmpBitmapEncoder) },
            { ".gif", typeof(GifBitmapEncoder) },
            { ".tiff", typeof(TiffBitmapEncoder) },
            { ".tif", typeof(TiffBitmapEncoder) },
        };

        public static void SaveFile(BitmapSource image) {
            SaveFileDialog dialog = new() {
                Filter = "PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|TIFF (*.tiff;*.tif)|*.tiff;*.tif",
                FileName = "cropped",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() != true) return;

            SaveFile(image, dialog.FileName);
        }

        public static void SaveFile(BitmapSource image, string path) {
            using FileStream stream = File.Create(path);
            string extension = Path.GetExtension(path);

            if (!encoders.ContainsKey(extension))
                MessageBox.Show("Unable to save file with unsupported file extension: " + extension, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            BitmapEncoder encoder = (BitmapEncoder)Activator.CreateInstance(encoders[extension]);

            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
        }

        public static void SaveClipboard(BitmapSource image) {
            PngBitmapEncoder encoder = new();
            using MemoryStream stream = new();

            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);

            DataObject data = new();
            data.SetData(DataFormats.Bitmap, image);
            data.SetData("PNG", stream);
            Clipboard.SetDataObject(data, true);
        }

        public static BitmapSource? LoadFile() {
            OpenFileDialog dialog = new() {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.bmp;*.gif;*.tiff;*.tif)|*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.bmp;*.gif;*.tiff;*.tif",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() != true) throw new OperationCanceledException();

            return LoadFile(dialog.FileName);
        }

        public static BitmapSource? LoadFile(string path) {
            try {
                BitmapImage bitmapImage = new();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(path);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            } catch {
                MessageBox.Show("Unable to load image from selected file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static BitmapSource? LoadClipboard() {
            BitmapSource? pastedImage = null;

            try {
                pastedImage = Clipboard.GetImage();
            } catch { }

            if (pastedImage != null)
                return pastedImage;

            string? file = Clipboard.GetFileDropList()?.OfType<string>().FirstOrDefault();
            
            if (file != null)
                return LoadFile(file);

            return null;
        }
    }
}
