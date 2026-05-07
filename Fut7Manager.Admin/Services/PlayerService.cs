using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class PlayerService {
        private readonly HttpClient _httpClient;

        public PlayerService() {
            //_httpClient = new HttpClient();
            _httpClient = new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202");
        }

        public async Task<List<PlayerDto>> GetPlayersAsync(int leagueId) {

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/players?LeagueId={leagueId}");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[GetPlayersAsync] [{response.StatusCode}]IsSuccessStatusCode: " + response.IsSuccessStatusCode);
                return new List<PlayerDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[GetMatchesAsync] STATUS: {response.StatusCode} JSON: {json}");
            return JsonConvert.DeserializeObject<List<PlayerDto>>(json) ?? new List<PlayerDto>();
        }

        public async Task<PlayerDto?> CreatePlayerAsync(PlayerDto player) {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/players");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                name = player.Name,
                jerseyNumber = player.JerseyNumber,
                phone = player.Phone,
                position = player.Position,
                dateOfBirth = player.DateOfBirth,
                active = player.Active,
                teamId = player.TeamId
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[CreatePlayerAsync] [{response.StatusCode}] Error");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[CreatePlayerAsync] STATUS: {response.StatusCode} JSON: {json}");

            return JsonConvert.DeserializeObject<PlayerDto>(json);
        }

        public async Task<PlayerDto?> EditPlayerAsync(int playerId, PlayerDto player) {

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/players/{playerId}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    TokenStorage.Token);

            request.Content = JsonContent.Create(player);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return player;
        }

        public async Task<bool> DeletePlayerAsync(int playerId) {

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/players/{playerId}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}