using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Fut7Manager.Admin.Converters {

    public class SafeImageConverter : IValueConverter {

        private static readonly Dictionary<string, BitmapImage> _cache = new();

        private static readonly HttpClient _httpClient = new();

        public object? Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture) {

            var url = value as string;

            if (string.IsNullOrWhiteSpace(url))
                return null;

            // cache
            if (_cache.TryGetValue(url, out var cached))
                return cached;

            try {

                var bytes = _httpClient
                    .GetByteArrayAsync(url)
                    .GetAwaiter()
                    .GetResult();

                var bitmap = new BitmapImage();

                using (var ms = new MemoryStream(bytes)) {

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                _cache[url] = bitmap;

                return bitmap;
            }
            catch {

                return null;
            }
        }

        public object? ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture) {

            throw new NotImplementedException();
        }
    }
}