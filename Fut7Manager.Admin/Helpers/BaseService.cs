using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.ViewModels;
using Newtonsoft.Json;
using System.Net;
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

        // =========================================================
        // REQUESTS CON RESPONSE BODY
        // =========================================================

        protected async Task<T?> SendAsync<T>(
            HttpRequestMessage request) {

            try {

                if (!string.IsNullOrWhiteSpace(TokenStorage.Token)) {

                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            TokenStorage.Token);
                }

                var response =
                    await _httpClient.SendAsync(request);

                // TOKEN EXPIRADO
                if (response.StatusCode ==
                    HttpStatusCode.Unauthorized) {

                    HandleUnauthorized();

                    return default;
                }

                // ERROR HTTP
                if (!response.IsSuccessStatusCode) {

                    var error =
                        await response.Content.ReadAsStringAsync();

                    MessageService.Show(
                        $"Error del servidor:\n{response.StatusCode}\n\n{error}",
                        "Error");

                    return default;
                }

                // SIN CONTENIDO
                if (response.Content == null)
                    return default;

                var json =
                    await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                    return default;

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex) {

                MessageService.Show(
                    ex.Message,
                    "Error");

                return default;
            }
        }

        // =========================================================
        // REQUESTS SIN RESPONSE BODY
        // =========================================================

        protected async Task<bool> SendAsync(
            HttpRequestMessage request) {

            try {

                if (!string.IsNullOrWhiteSpace(TokenStorage.Token)) {

                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            TokenStorage.Token);
                }

                var response =
                    await _httpClient.SendAsync(request);

                // TOKEN EXPIRADO
                if (response.StatusCode ==
                    HttpStatusCode.Unauthorized) {

                    HandleUnauthorized();

                    return false;
                }

                // ERROR HTTP
                if (!response.IsSuccessStatusCode) {

                    var error =
                        await response.Content.ReadAsStringAsync();

                    MessageService.Show(
                        $"Error del servidor:\n{response.StatusCode}\n\n{error}",
                        "Error");

                    return false;
                }

                return true;
            }
            catch (Exception ex) {

                MessageService.Show(ex.Message,"Error");

                return false;
            }
        }

        protected async Task<HttpResponseMessage?> SendResponseAsync(
    HttpRequestMessage request) {

            try {

                if (!string.IsNullOrWhiteSpace(TokenStorage.Token)) {

                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            TokenStorage.Token);
                }

                var response =
                    await _httpClient.SendAsync(request);

                if (response.StatusCode ==
                    HttpStatusCode.Unauthorized) {

                    HandleUnauthorized();

                    return null;
                }

                return response;
            }
            catch (Exception ex) {

                MessageService.Show(
                    ex.Message,
                    "Error");

                return null;
            }
        }

        // =========================================================
        // GET
        // =========================================================

        protected async Task<T?> GetAsync<T>(string url) {

            if (string.IsNullOrWhiteSpace(url))
                return default;

            var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url);

            return await SendAsync<T>(request);
        }

        // =========================================================
        // POST
        // =========================================================

        protected async Task<T?> PostAsync<T>(string url, object body) {

            if (string.IsNullOrWhiteSpace(url))
                return default;

            var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    url);

            request.Content =
                JsonContent.Create(body);

            return await SendAsync<T>(request);
        }

        protected async Task<bool> PostAsync(string url, object body) {

            if (string.IsNullOrWhiteSpace(url))
                return false;

            var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    url);

            request.Content =
                JsonContent.Create(body);

            return await SendAsync(request);
        }

        // =========================================================
        // PUT
        // =========================================================

        protected async Task<bool> PutAsync(string url, object body) {

            var request =
                new HttpRequestMessage(
                    HttpMethod.Put,
                    url);

            request.Content =
                JsonContent.Create(body);

            return await SendAsync(request);
        }

        protected async Task<HttpResponseMessage?> PutResponseAsync(string url,object body) {

            var request =new HttpRequestMessage(HttpMethod.Put,url);

            request.Content =JsonContent.Create(body);

            return await SendResponseAsync(request);
        }

        // =========================================================
        // DELETE
        // =========================================================

        protected async Task<bool> DeleteAsync(
            string url) {

            var request =
                new HttpRequestMessage(
                    HttpMethod.Delete,
                    url);

            return await SendAsync(request);
        }

        // =========================================================
        // TOKEN EXPIRADO
        // =========================================================

        private void HandleUnauthorized() {

            TokenStorage.Token = null;

            MessageService.Show(
                "Tu sesión expiró. Inicia sesión nuevamente.",
                "Sesión expirada");

            Application.Current.Dispatcher.Invoke(() => {

                var mainWindow =
                    (MainWindow)Application.Current.MainWindow;

                if (mainWindow.DataContext is MainViewModel vm)
                    vm.Logout();
            });
        }
    }
}