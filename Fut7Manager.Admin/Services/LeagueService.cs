using Fut7Manager.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fut7Manager.Admin.Services {
    public class LeagueService : BaseService {
        public async Task<List<LeagueDto>> GetLeaguesAsync() {
            return await GetAsync<List<LeagueDto>>("/api/leagues")
                   ?? new List<LeagueDto>();
        }

        public async Task<LeagueDto> GetLeagueByIdAsync(int leagueId) {
            return await GetAsync<LeagueDto>($"/api/leagues/{leagueId}")
                   ?? new LeagueDto();
        }

        public async Task<LeagueDto> CreateLeagueAsync(LeagueDto league) {
            var body = new {
                name = league.Name,
                registrationFee = league.RegistrationFee,
                status = league.Status,
                logoUrl = league.LogoUrl
            };

            return await PostAsync<LeagueDto>("/api/leagues", body) ?? new LeagueDto();
        }

        public async Task<bool> EditLeagueAsync(LeagueDto league) {
            var body = new {
                name = league.Name,
                registrationFee = league.RegistrationFee,
                status = league.Status,
                logoUrl = league.LogoUrl
            };

            return await PutAsync($"/api/leagues/{league.Id}", body);
        }

        // ============================================
        // PREVIEW SCHEDULE
        // ============================================

        public async Task<List<MatchdayDto>> PreviewScheduleAsync(int leagueId, bool interGroupMatches, List<TeamGroupAssignmentDto> teams) {
            var body = new {
                interGroupMatches,
                teams
            };

            return await PostAsync<List<MatchdayDto>>($"/api/leagues/{leagueId}/schedule/preview", body) ?? new List<MatchdayDto>();
        }

        // ============================================
        // FINALIZE SETUP
        // ============================================

        public async Task<bool> FinalizeSetupAsync(int leagueId,bool interGroupMatches,List<TeamGroupAssignmentDto> teams) {
            var body = new {
                interGroupMatches,
                teams
            };

            await PostAsync<object>(
                $"/api/leagues/{leagueId}/finalize-setup",
                body);

            return true;
        }

        public async Task<bool> DeleteLeagueAsync(int id) {
            return await DeleteAsync($"/api/leagues/{id}");
        }

        public async Task<LeagueDashboardDto> GetDashboardAsync(int leagueId) {
            return await GetAsync<LeagueDashboardDto>(
                $"/api/leagues/{leagueId}/dashboard") ?? new LeagueDashboardDto();
        }

        public async Task<StandingsResponseDto> GetStandingsAsync(int leagueId) {
            return await GetAsync<StandingsResponseDto>(
                $"/api/leagues/{leagueId}/standings") ?? new StandingsResponseDto();
        }
    }
}