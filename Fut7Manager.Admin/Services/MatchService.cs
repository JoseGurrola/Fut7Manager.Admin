using Fut7Manager.Admin.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fut7Manager.Admin.Services {
    public class MatchService : BaseService {

        public async Task<List<Fut7MatchDto>> GetMatchesAsync(int leagueId) {

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/fut7matches?LeagueId={leagueId}");

            var result = await SendAsync<List<Fut7MatchDto>>(request);

            return result ?? new List<Fut7MatchDto>();
        }
    }
}