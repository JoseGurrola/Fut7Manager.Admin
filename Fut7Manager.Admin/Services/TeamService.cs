using Fut7Manager.Admin.Helpers;
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
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202");
        }

        public async Task<List<TeamDto>> GetTeamsAsync(int leagueId) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/teams?LeagueId={leagueId}");

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

        public async Task<TeamDto?> CreateTeamAsync(TeamDto team) {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Teams");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                name = team.Name,
                logoUrl = team.LogoUrl,
                groupId = team.GroupId,
                leagueId = team.LeagueId,
                teamManagerName = team.TeamManagerName,
                teamManagerPhone = team.TeamManagerPhone
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[CreateTeamAsync] [{response.StatusCode}] Error");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[CreateTeamAsync] STATUS: {response.StatusCode} JSON: {json}");

            return JsonConvert.DeserializeObject<TeamDto>(json);
        }

        public async Task<bool> EditTeamAsync(TeamDto team) {
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Teams/{team.Id}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                name = team.Name,
                logoUrl = team.LogoUrl,
                groupId = team.GroupId,
                leagueId = team.LeagueId,
                teamManagerName = team.TeamManagerName,
                teamManagerPhone = team.TeamManagerPhone
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[EditTeamAsync] [{response.StatusCode}] Error");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[EditTeamAsync] STATUS: {response.StatusCode}");
            return true;
        }

        public async Task<bool> DeleteTeamAsync(int id) {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Teams/{id}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[DeleteTeamAsync] [{response.StatusCode}] Error");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[DeleteTeamAsync] STATUS: {response.StatusCode}");

            // La API solo devuelve 200 sin body
            return true;
        }
    }
}