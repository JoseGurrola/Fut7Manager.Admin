using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Fut7Manager.Admin.Converters {

    public class SafeImageConverter : IValueConverter {
        private static readonly Dictionary<string, BitmapImage> _cache = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var url = value as string;

            if (string.IsNullOrWhiteSpace(url))
                return null;

            // 🔥 Si ya está en cache, regresa directo
            if (_cache.ContainsKey(url))
                return _cache[url];

            try {
                using (var wc = new WebClient()) {
                    var bytes = wc.DownloadData(url);

                    var bitmap = new BitmapImage();
                    using (var ms = new MemoryStream(bytes)) {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }

                    // 🔥 Guardar en cache
                    _cache[url] = bitmap;

                    return bitmap;
                }
            }
            catch {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}