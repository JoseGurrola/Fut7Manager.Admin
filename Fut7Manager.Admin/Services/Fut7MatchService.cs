using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class Fut7MatchService : BaseService {
        public async Task<List<Fut7MatchDto>> GetFut7MatchsAsync(int leagueId) {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/Fut7Matches?LeagueId={leagueId}&pageSize=0");

            var result =
                await SendAsync<List<Fut7MatchDto>>(request);

            return result ?? new List<Fut7MatchDto>();
        }

        //public async Task<Fut7MatchDto?> CreateFut7MatchAsync(Fut7MatchDto fut7match)
        //{
        //    var body = new
        //    {
        //    };

        //    var request = new HttpRequestMessage(
        //        HttpMethod.Post,
        //        "/api/Fut7Matches")
        //    {
        //        Content = JsonContent.Create(body)
        //    };

        //    return await SendAsync<Fut7MatchDto>(request);
        //}

        public async Task<bool> UpdateFut7MatchAsync(Fut7MatchDto fut7match) {
            var body = new {
                homeGoals = fut7match.HomeGoals,
                awayGoals = fut7match.AwayGoals,
                matchDate = fut7match.MatchDate,
                location = fut7match.Location
            };

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/Fut7Matches/{fut7match.Id}") {
                Content = JsonContent.Create(body)
            };

            var result =
                await SendAsync<object>(request);

            return result != null;
        }

        //public async Task<bool> DeleteFut7MatchAsync(int id)
        //{
        //    var request = new HttpRequestMessage(
        //        HttpMethod.Delete,
        //        $"/api/Fut7Matches/{id}");

        //    var result =
        //        await SendAsync<object>(request);

        //    return result != null;
        //}
    }
}