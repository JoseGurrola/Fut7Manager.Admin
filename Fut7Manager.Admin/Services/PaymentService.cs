using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class PaymentService {
        private readonly HttpClient _httpClient;

        public PaymentService() {
            //_httpClient = new HttpClient();
            _httpClient = new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202");
        }

        public async Task<List<PaymentDto>> GetPaymentsAsync(int teamId) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Payments?teamId={teamId}");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[GetPaymentsAsync] [{response.StatusCode}]IsSuccessStatusCode: " + response.IsSuccessStatusCode);
                return new List<PaymentDto>();
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetPaymentsAsync] STATUS: {response.StatusCode} JSON: {json}");


            return JsonConvert.DeserializeObject<List<PaymentDto>>(json) ?? new List<PaymentDto>();  
        }

        public async Task<PaymentDto?> CreatePaymentAsync(PaymentDto team) {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Payments");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                teamId = team.TeamId,
                amount = team.Amount,
 
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[CreatePaymentAsync] [{response.StatusCode}] Error");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[CreatePaymentAsync] STATUS: {response.StatusCode} JSON: {json}");

            return JsonConvert.DeserializeObject<PaymentDto>(json);
        }


        public async Task<bool> DeletePaymentAsync(int id) {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Payments/{id}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[DeletePaymentAsync] [{response.StatusCode}] Error");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[DeletePaymentAsync] STATUS: {response.StatusCode}");

            // La API solo devuelve 200 sin body
            return true;
        }
    }
}