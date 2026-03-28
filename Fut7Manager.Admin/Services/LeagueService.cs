using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class LeagueService {
        private readonly HttpClient _httpClient;

        public LeagueService() {
            //_httpClient = new HttpClient();
            _httpClient = new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202"); 
        }

        public async Task<List<LeagueDto>> GetLeaguesAsync() {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/leagues");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[GetLeaguesAsync] [{response.StatusCode}]IsSuccessStatusCode: " + response.IsSuccessStatusCode);
                return new List<LeagueDto>();
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetLeaguesAsync] STATUS: {response.StatusCode} JSON: {json}");

            return JsonConvert.DeserializeObject<List<LeagueDto>>(json) ?? new List<LeagueDto>();
        }

        public async Task<LeagueDto?> CreateLeagueAsync(LeagueDto league) {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/leagues");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                name = league.Name,
                registrationFee = league.RegistrationFee
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[CreateLeagueAsync] [{response.StatusCode}] Error");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[CreateLeagueAsync] STATUS: {response.StatusCode} JSON: {json}");

            return JsonConvert.DeserializeObject<LeagueDto>(json);
        }

        public async Task<bool> EditLeagueAsync(LeagueDto league) {
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/leagues/{league.Id}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                name = league.Name,
                registrationFee = league.RegistrationFee
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[EditLeagueAsync] [{response.StatusCode}] Error");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[EditLeagueAsync] STATUS: {response.StatusCode}");
            return true;
        }

        public async Task<bool> DeleteLeagueAsync(int id) {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/leagues/{id}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[DeleteLeagueAsync] [{response.StatusCode}] Error");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[DeleteLeagueAsync] STATUS: {response.StatusCode}");

            // La API solo devuelve 200 sin body
            return true;
        }
    }
}