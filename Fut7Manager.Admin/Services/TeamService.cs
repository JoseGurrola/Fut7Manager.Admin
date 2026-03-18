using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class TeamService {
        private readonly HttpClient _httpClient;

        public TeamService() {
            //_httpClient = new HttpClient();
            _httpClient = new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202"); // tu API
        }

        public async Task<List<TeamDto>> GetTeamsAsync() {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/teams");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[GetTeamsAsync] [{response.StatusCode}]IsSuccessStatusCode: " + response.IsSuccessStatusCode);
                return new List<TeamDto>();
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetTeamsAsync] STATUS: {response.StatusCode} JSON: {json}");


            return JsonConvert.DeserializeObject<List<TeamDto>>(json) ?? new List<TeamDto>();  
        }
    }
}