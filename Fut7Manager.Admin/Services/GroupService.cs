using Fut7Manager.Admin.Models;

namespace Fut7Manager.Admin.Services {

    public class GroupService : BaseService {

        public async Task<List<GroupDto>> GetGroupsAsync(
            int leagueId) {

            return await GetAsync<List<GroupDto>>(
                $"/api/groups?LeagueId={leagueId}")
                ?? new List<GroupDto>();
        }

        public async Task<GroupDto?> CreateGroupAsync(
            GroupDto group) {

            System.Diagnostics.Debug.WriteLine(
                "[CreateGroupAsync] Creando grupo...");

            var body = new {

                name = group.Name,
                leagueId = group.LeagueId
            };

            return await PostAsync<GroupDto>(
                "/api/groups",
                body);
        }

        public async Task<bool> DeleteGroupAsync(
            int id) {

            System.Diagnostics.Debug.WriteLine(
                "[DeleteGroupAsync] Borrando grupo...");

            return await DeleteAsync(
                $"/api/groups/{id}");
        }
    }
}