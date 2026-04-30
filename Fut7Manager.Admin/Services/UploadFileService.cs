using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Fut7Manager.Admin.Services {
    public class UploadFileService {
        private readonly HttpClient _httpClient;

        public UploadFileService() {
            _httpClient = new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });

            _httpClient.BaseAddress = new Uri("https://localhost:7202");
        }

        public async Task<string> UploadLogoAsync(string filePath, string type = "team") {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return string.Empty;

            try {
                using var form = new MultipartFormDataContent();

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileContent = new StreamContent(fileStream);

                // 🔹 Detectar tipo MIME
                var ext = Path.GetExtension(filePath).ToLower();
                var contentType = ext switch {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                form.Add(fileContent, "file", Path.GetFileName(filePath));

                // 🔹 Enviar tipo como query param
                var response = await _httpClient.PostAsync($"/api/uploads/image?type={type}", form);

                if (!response.IsSuccessStatusCode) {
                    System.Diagnostics.Debug.WriteLine($"[UploadLogoAsync] Error: {response.StatusCode}");
                    return string.Empty;
                }

                var json = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(json);

                if (result != null && result.url != null) {
                    string url = result.url;
                    System.Diagnostics.Debug.WriteLine($"[UploadLogoAsync] URL: {url}");
                    return url;
                }

                return string.Empty;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[UploadLogoAsync] Exception: {ex.Message}");
                return string.Empty;
            }
        }
    }
}