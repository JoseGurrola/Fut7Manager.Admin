using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fut7Manager.Admin.Services {
    public class AuthService {
        private static readonly HttpClient _httpClient = new HttpClient {
            BaseAddress = new Uri("https://localhost:7202")
        };

        public async Task<LoginResult> LoginAsync(string username, string password) {
            try {
                var body = new LoginRequestDto {
                    Username = username,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) {
                    return new LoginResult {
                        Success = false,
                        Error = "Credenciales inválidas."
                    };
                }

                var result = JsonConvert.DeserializeObject<LoginResponseDto>(responseJson);

                return new LoginResult {
                    Success = result?.Token != null,
                    Token = result?.Token,
                    Error = result?.Token == null ? "Respuesta inválida del servidor." : null
                };
            }
            catch {
                return new LoginResult {
                    Success = false,
                    Error = "No se pudo conectar con el servidor."
                };
            }
        }
    }
}