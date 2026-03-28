using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class GroupService {
        private readonly HttpClient _httpClient;

        public GroupService() {
            //_httpClient = new HttpClient();
            _httpClient = new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202");
        }

        public async Task<List<GroupDto>> GetGroupsAsync(int leagueId) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/groups?LeagueId={leagueId}");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[GetGroupsAsync] [{response.StatusCode}]IsSuccessStatusCode: " + response.IsSuccessStatusCode);
                return new List<GroupDto>();
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetGroupsAsync] STATUS: {response.StatusCode} JSON: {json}");


            return JsonConvert.DeserializeObject<List<GroupDto>>(json) ?? new List<GroupDto>();  
        }

        public async Task<GroupDto?> CreateGroupAsync(GroupDto group) {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Groups");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var body = new {
                name = group.Name,
                LeagueId = group.LeagueId
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[CreateGroupAsync] [{response.StatusCode}] Error");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[CreateGroupAsync] STATUS: {response.StatusCode} JSON: {json}");

            return JsonConvert.DeserializeObject<GroupDto>(json);
        }

        //public async Task<bool> EditGroupAsync(GroupDto group) {
        //    var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Groups/{group.Id}");

        //    request.Headers.Authorization =
        //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

        //    var body = new {
        //        name = group.Name,
        //        logoUrl = group.LogoUrl
        //    };

        //    request.Content = JsonContent.Create(body);

        //    var response = await _httpClient.SendAsync(request);

        //    if (!response.IsSuccessStatusCode) {
        //        System.Diagnostics.Debug.WriteLine($"[EditGroupAsync] [{response.StatusCode}] Error");
        //        return false;
        //    }

        //    System.Diagnostics.Debug.WriteLine($"[EditGroupAsync] STATUS: {response.StatusCode}");
        //    return true;
        //}

        public async Task<bool> DeleteGroupAsync(int id) {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Groups/{id}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine($"[DeleteGroupAsync] [{response.StatusCode}] Error");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[DeleteGroupAsync] STATUS: {response.StatusCode}");

            // La API solo devuelve 200 sin body
            return true;
        }
    }
}