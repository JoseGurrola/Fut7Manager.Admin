using Fut7Manager.Admin.Helpers;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Fut7Manager.Admin.Services {

    public class UploadFileService {

        private readonly HttpClient _httpClient;

        public UploadFileService() {

            _httpClient = ApiClient.Instance;
        }

        public async Task<string> UploadLogoAsync(
            string filePath,
            string type = "team") {

            if (string.IsNullOrWhiteSpace(filePath) ||
                !File.Exists(filePath)) {

                return string.Empty;
            }

            try {

                using var form =
                    new MultipartFormDataContent();

                using var fileStream =
                    new FileStream(
                        filePath,
                        FileMode.Open,
                        FileAccess.Read);

                var fileContent =
                    new StreamContent(fileStream);

                var ext =
                    Path.GetExtension(filePath).ToLower();

                var contentType = ext switch {

                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(contentType);

                form.Add(
                    fileContent,
                    "file",
                    Path.GetFileName(filePath));

                var request =
                    new HttpRequestMessage(
                        HttpMethod.Post,
                        $"/api/uploads/image?type={type}");

                request.Content = form;

                if (!string.IsNullOrWhiteSpace(TokenStorage.Token)) {

                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            TokenStorage.Token);
                }

                var response =
                    await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode) {

                    System.Diagnostics.Debug.WriteLine(
                        $"[UploadLogoAsync] Error: {response.StatusCode}");

                    return string.Empty;
                }

                var json =
                    await response.Content.ReadAsStringAsync();

                dynamic? result =
                    JsonConvert.DeserializeObject(json);

                if (result?.url != null) {

                    string url = result.url;

                    System.Diagnostics.Debug.WriteLine(
                        $"[UploadLogoAsync] URL: {url}");

                    return url;
                }

                return string.Empty;
            }
            catch (Exception ex) {

                System.Diagnostics.Debug.WriteLine(
                    $"[UploadLogoAsync] Exception: {ex.Message}");

                return string.Empty;
            }
        }
    }
}