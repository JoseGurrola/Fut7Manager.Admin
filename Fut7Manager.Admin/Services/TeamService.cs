using Fut7Manager.Admin.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class TeamService : BaseService {

        public async Task<List<TeamDto>> GetTeamsAsync(int leagueId) {

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/teams?LeagueId={leagueId}");

            var result =
                await SendAsync<List<TeamDto>>(request);

            return result ?? new List<TeamDto>();
        }

        public async Task<TeamDto?> CreateTeamAsync(TeamDto team) {

            var body = new {
                name = team.Name,
                logoUrl = team.LogoUrl,
                groupId = team.GroupId,
                leagueId = team.LeagueId,
                teamManagerName = team.TeamManagerName,
                teamManagerPhone = team.TeamManagerPhone,
                teamPrimaryColor = team.TeamPrimaryColor,
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/Teams") {
                Content = JsonContent.Create(body)
            };

            return await SendAsync<TeamDto>(request);
        }

        public async Task<bool> EditTeamAsync(TeamDto team) {

            var body = new {
                name = team.Name,
                logoUrl = team.LogoUrl,
                groupId = team.GroupId,
                leagueId = team.LeagueId,
                teamManagerName = team.TeamManagerName,
                teamManagerPhone = team.TeamManagerPhone,
                teamPrimaryColor = team.TeamPrimaryColor
            };

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/Teams/{team.Id}") {
                Content = JsonContent.Create(body)
            };

            var result =
                await SendAsync<object>(request);

            return result != null;
        }

        public async Task<bool> DeleteTeamAsync(int id) {

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/Teams/{id}");

            var result =
                await SendAsync<object>(request);

            return result != null;
        }
    }
}