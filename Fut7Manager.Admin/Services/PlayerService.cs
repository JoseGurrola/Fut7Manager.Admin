using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Models.SecondaryModels;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class PlayerService : BaseService {

        public async Task<List<PlayerDto>> GetPlayersAsync(int leagueId) {

            var result = await GetAsync<List<PlayerDto>>(
                $"/api/players?LeagueId={leagueId}");

            return result ?? new List<PlayerDto>();
        }

        public async Task<List<PlayerDto>> GetPlayersByTeamAsync(int teamId) {
            var result = await GetAsync<List<PlayerDto>>(
                $"/api/players?teamId={teamId}");

            return result ?? new List<PlayerDto>();
        }

        public async Task<PlayerDto?> GetPlayerAsync(int playerId) {

            return await GetAsync<PlayerDto>(
                $"/api/players/{playerId}");
        }

        public async Task<PlayerDto?> CreatePlayerAsync(PlayerDto player) {

            var body = new {
                name = player.Name,
                jerseyNumber = player.JerseyNumber,
                phone = player.Phone,
                email = player.Email,
                position = player.Position,
                dateOfBirth = player.DateOfBirth,
                active = player.Active,
                teamId = player.TeamId
            };

            return await PostAsync<PlayerDto>(
                "/api/players",
                body);
        }

        public async Task<bool> ImportPlayersAsync(int teamId, List<PlayerDto> players) {
            var body = new {
                players = players.Select(player => new {
                    name = player.Name,
                    jerseyNumber = player.JerseyNumber,
                    phone = player.Phone,
                    email = player.Email,
                    position = player.Position,
                    dateOfBirth = player.DateOfBirth,
                    active = player.Active,
                    teamId = player.TeamId
                }).ToList()
            };

            return await PostAsync(
                $"/api/players/import/{teamId}",
                body);
        }

        public async Task<bool> EditPlayerAsync(int playerId, PlayerDto player) {

            var body = new {
                name = player.Name,
                jerseyNumber = player.JerseyNumber,
                phone = player.Phone,
                email = player.Email,
                position = player.Position,
                dateOfBirth = player.DateOfBirth,
                active = player.Active,
                teamId = player.TeamId
            };

            return await PutAsync(
                $"/api/players/{playerId}",
                body);
        }

        public async Task<bool> DeletePlayerAsync(int playerId) {

            return await DeleteAsync(
                $"/api/players/{playerId}");
        }
    }
}