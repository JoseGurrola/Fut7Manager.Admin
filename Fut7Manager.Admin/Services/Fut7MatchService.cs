using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class Fut7MatchService {
        private readonly HttpClient _httpClient;

        public Fut7MatchService() {
            //_httpClient = new HttpClient();
            _httpClient = new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202");
        }

        public async Task<List<Fut7MatchDto>> GetFut7MatchsAsync(int leagueId) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Fut7Matches?LeagueId={leagueId}");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[GetFut7MatchsAsync] [{response.StatusCode}]IsSuccessStatusCode: " + response.IsSuccessStatusCode);
                return new List<Fut7MatchDto>();
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetFut7MatchsAsync] STATUS: {response.StatusCode} JSON: {json}");


            return JsonConvert.DeserializeObject<List<Fut7MatchDto>>(json) ?? new List<Fut7MatchDto>();
        }

        //public async Task<Fut7MatchDto?> CreateFut7MatchAsync(Fut7MatchDto fut7match) {
        //    var request = new HttpRequestMessage(HttpMethod.Post, "/api/Fut7Matches");

        //    request.Headers.Authorization =
        //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

        //    var body = new {
        //        name = team.Name,
        //        logoUrl = team.LogoUrl,
        //        groupId = team.GroupId,
        //        leagueId = team.LeagueId,
        //        teamManagerName = team.Fut7MatchManagerName,
        //        teamManagerPhone = team.Fut7MatchManagerPhone
        //    };

        //    request.Content = JsonContent.Create(body);

        //    var response = await _httpClient.SendAsync(request);

        //    if (!response.IsSuccessStatusCode) {
        //        System.Diagnostics.Debug.WriteLine($"[CreateFut7MatchAsync] [{response.StatusCode}] Error");
        //        return null;
        //    }

        //    var json = await response.Content.ReadAsStringAsync();

        //    System.Diagnostics.Debug.WriteLine($"[CreateFut7MatchAsync] STATUS: {response.StatusCode} JSON: {json}");

        //    return JsonConvert.DeserializeObject<Fut7MatchDto>(json);
        //}

        public async Task<bool> UpdateFut7MatchAsync(Fut7MatchDto fut7match) {
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Fut7Matches/{fut7match.Id}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                //id = fut7match.Id,
                //homeTeamId = fut7match.HomeTeamId,
                //awayTeamId = fut7match.AwayTeamId,
                homeGoals = fut7match.HomeGoals,
                awayGoals = fut7match.AwayGoals,
                matchDate = fut7match.MatchDate,
                location = fut7match.Location,
                //leagueId = fut7match.LeagueId
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[EditFut7MatchAsync] [{response.StatusCode}] Error");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[EditFut7MatchAsync] STATUS: {response.StatusCode}");
            return true;
        }

        //public async Task<bool> DeleteFut7MatchAsync(int id) {
        //    var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Fut7Matches/{id}");

        //    request.Headers.Authorization =
        //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

        //    var response = await _httpClient.SendAsync(request);

        //    if (!response.IsSuccessStatusCode) {
        //        System.Diagnostics.Debug.WriteLine($"[DeleteFut7MatchAsync] [{response.StatusCode}] Error");
        //        return false;
        //    }

        //    System.Diagnostics.Debug.WriteLine($"[DeleteFut7MatchAsync] STATUS: {response.StatusCode}");

        //    // La API solo devuelve 200 sin body
        //    return true;
        //}
    }
}