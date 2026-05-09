using Fut7Manager.Admin.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;

namespace Fut7Manager.Admin.Services {
    public abstract class BaseService {
        protected readonly HttpClient _httpClient;

        protected BaseService() {
            _httpClient = ApiClient.Instance;
        }

        protected async Task<T?> SendAsync<T>(HttpRequestMessage request) {
            try {
                // 🔐 Bearer token automático
                if (!string.IsNullOrWhiteSpace(TokenStorage.Token)) {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            TokenStorage.Token);
                }

                var response = await _httpClient.SendAsync(request);

                // ❌ Error HTTP
                if (!response.IsSuccessStatusCode) {
                    var error = await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        $"Error del servidor:\n{response.StatusCode}\n\n{error}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return default;
                }

                // 📦 Sin contenido
                if (response.Content == null)
                    return default;

                var json = await response.Content.ReadAsStringAsync();

                // 📭 JSON vacío
                if (string.IsNullOrWhiteSpace(json))
                    return default;

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (HttpRequestException) {
                MessageBox.Show(
                    "No se pudo conectar con el servidor.",
                    "Sin conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return default;
            }
            catch (TaskCanceledException) {
                MessageBox.Show(
                    "El servidor tardó demasiado en responder.",
                    "Timeout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return default;
            }
            catch (Exception ex) {
                MessageBox.Show(
                    ex.Message,
                    "Error inesperado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return default;
            }
        }

        protected async Task<T?> GetAsync<T>(string url) {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                url);

            return await SendAsync<T>(request);
        }

        protected async Task<T?> PostAsync<T>(string url, object body) {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                url);

            request.Content = JsonContent.Create(body);

            return await SendAsync<T>(request);
        }

        protected async Task<bool> PutAsync(string url, object body) {
            try {
                // 🔐 Bearer token automático
                if (!string.IsNullOrWhiteSpace(TokenStorage.Token)) {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            TokenStorage.Token);
                }

                var response = await _httpClient.PutAsJsonAsync(url, body);

                if (!response.IsSuccessStatusCode) {
                    var error = await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        $"Error del servidor:\n{response.StatusCode}\n\n{error}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }

                return true;
            }
            catch (HttpRequestException) {
                MessageBox.Show(
                    "No se pudo conectar con el servidor.",
                    "Sin conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
            catch (TaskCanceledException) {
                MessageBox.Show(
                    "El servidor tardó demasiado en responder.",
                    "Timeout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }
            catch (Exception ex) {
                MessageBox.Show(
                    ex.Message,
                    "Error inesperado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }

        protected async Task<bool> DeleteAsync(string url) {
            try {
                // 🔐 Bearer token automático
                if (!string.IsNullOrWhiteSpace(TokenStorage.Token)) {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            TokenStorage.Token);
                }

                var response = await _httpClient.DeleteAsync(url);

                if (!response.IsSuccessStatusCode) {
                    var error = await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        $"Error del servidor:\n{response.StatusCode}\n\n{error}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }

                return true;
            }
            catch (HttpRequestException) {
                MessageBox.Show(
                    "No se pudo conectar con el servidor.",
                    "Sin conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
            catch (TaskCanceledException) {
                MessageBox.Show(
                    "El servidor tardó demasiado en responder.",
                    "Timeout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }
            catch (Exception ex) {
                MessageBox.Show(
                    ex.Message,
                    "Error inesperado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }
    }
}