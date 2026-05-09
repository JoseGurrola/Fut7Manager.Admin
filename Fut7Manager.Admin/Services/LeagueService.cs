using Fut7Manager.Admin.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class LeagueService : BaseService {
        public async Task<List<LeagueDto>> GetLeaguesAsync() {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/leagues");

            var result =
                await SendAsync<List<LeagueDto>>(request);

            return result ?? new List<LeagueDto>();
        }

        public async Task<LeagueDto> GetLeagueByIdAsync(int leagueId) {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/leagues/{leagueId}");

            var result =
                await SendAsync<LeagueDto>(request);

            return result ?? new LeagueDto();
        }

        public async Task<LeagueDto?> CreateLeagueAsync(LeagueDto league) {
            var body = new {
                name = league.Name,
                registrationFee = league.RegistrationFee,
                status = league.Status,
                logoUrl = league.LogoUrl
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/leagues") {
                Content = JsonContent.Create(body)
            };

            return await SendAsync<LeagueDto>(request);
        }

        public async Task<bool> EditLeagueAsync(LeagueDto league) {

            var body = new {
                name = league.Name,
                registrationFee = league.RegistrationFee,
                status = league.Status,
                logoUrl = league.LogoUrl
            };

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/leagues/{league.Id}") {
                Content = JsonContent.Create(body)
            };

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<MatchdayDto>?> GenerateSchedule(
            int leagueId,
            bool interGroupMatches) {
            var body = new {
                interGroupMatches
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/leagues/{leagueId}/schedule") {
                Content = JsonContent.Create(body)
            };

            return await SendAsync<List<MatchdayDto>>(request);
        }

        public async Task<bool> DeleteLeagueAsync(int id) {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/leagues/{id}");

            var result =
                await SendAsync<object>(request);

            return result != null;
        }

        public async Task<LeagueDashboardDto?> GetDashboardAsync(int leagueId) {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/leagues/{leagueId}/dashboard");

            return await SendAsync<LeagueDashboardDto>(request);
        }

        public async Task<StandingsResponseDto?> GetStandingsAsync(int leagueId) {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/leagues/{leagueId}/standings");

            return await SendAsync<StandingsResponseDto>(request);
        }
    }
}