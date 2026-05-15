using Fut7Manager.Admin.Models;

namespace Fut7Manager.Admin.Services {

    public class TeamService : BaseService {

        public async Task<List<TeamDto>> GetTeamsAsync(
            int leagueId) {

            return await GetAsync<List<TeamDto>>(
                $"/api/teams?LeagueId={leagueId}")
                ?? new List<TeamDto>();
        }

        public async Task<TeamDto?> CreateTeamAsync(
            TeamDto team) {

            var body = new {

                name = team.Name,
                logoUrl = team.LogoUrl,
                leagueId = team.LeagueId,
                teamManagerName = team.TeamManagerName,
                teamManagerPhone = team.TeamManagerPhone,
                teamPrimaryColor = team.TeamPrimaryColor,
            };

            return await PostAsync<TeamDto>(
                "/api/Teams",
                body);
        }

        public async Task<bool> EditTeamAsync(
            TeamDto team) {

            var body = new {

                name = team.Name,
                logoUrl = team.LogoUrl,
                groupId = team.GroupId,
                leagueId = team.LeagueId,
                teamManagerName = team.TeamManagerName,
                teamManagerPhone = team.TeamManagerPhone,
                teamPrimaryColor = team.TeamPrimaryColor
            };

            return await PutAsync(
                $"/api/Teams/{team.Id}",
                body);
        }

        public async Task<bool> DeleteTeamAsync(
            int id) {

            return await DeleteAsync(
                $"/api/Teams/{id}");
        }
    }
}