using Fut7Manager.Admin.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class GroupService : BaseService {
        public async Task<List<GroupDto>> GetGroupsAsync(int leagueId) {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/groups?LeagueId={leagueId}");

            var result =
                await SendAsync<List<GroupDto>>(request);

            return result ?? new List<GroupDto>();
        }

        public async Task<GroupDto?> CreateGroupAsync(GroupDto group) {
            var body = new {
                name = group.Name,
                leagueId = group.LeagueId
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/Groups") {
                Content = JsonContent.Create(body)
            };

            return await SendAsync<GroupDto>(request);
        }

        //public async Task<bool> EditGroupAsync(GroupDto group)
        //{
        //    var body = new
        //    {
        //        name = group.Name,
        //        logoUrl = group.LogoUrl
        //    };

        //    var request = new HttpRequestMessage(
        //        HttpMethod.Put,
        //        $"/api/Groups/{group.Id}")
        //    {
        //        Content = JsonContent.Create(body)
        //    };

        //    var result =
        //        await SendAsync<object>(request);

        //    return result != null;
        //}

        public async Task<bool> DeleteGroupAsync(int id) {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/Groups/{id}");

            var result =
                await SendAsync<object>(request);

            return result != null;
        }
    }
}