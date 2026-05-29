using Fut7Manager.Admin.Models;
using static System.Net.WebRequestMethods;

namespace Fut7Manager.Admin.Services {

    public class Fut7MatchService : BaseService {

        public async Task<List<Fut7MatchDto>> GetFut7MatchsAsync(
            int leagueId) {

            return await GetAsync<List<Fut7MatchDto>>(
                $"/api/Fut7Matches?LeagueId={leagueId}&pageSize=0")
                ?? new List<Fut7MatchDto>();
        }

        public async Task<bool> UpdateFut7MatchAsync(
            Fut7MatchDto fut7match) {

            var body = new {

                homeGoals = fut7match.HomeGoals,
                awayGoals = fut7match.AwayGoals,
                homePenaltyGoals = fut7match.HomePenaltyGoals,
                awayPenaltyGoals = fut7match.AwayPenaltyGoals,
                matchDate = fut7match.MatchDate,
                location = fut7match.Location
            };

            return await PutAsync(
                $"/api/Fut7Matches/{fut7match.Id}",
                body);
        }

        public async Task<List<MatchdayDto>> GetMatchdaysAsync(int leagueId) {
            return await GetAsync<List<MatchdayDto>>(
                $"api/Fut7Matches/matchdays?leagueId={leagueId}")
                ?? new List<MatchdayDto>();
        }
    }
}