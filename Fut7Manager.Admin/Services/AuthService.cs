using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fut7Manager.Admin.Services {
    public class AuthService : BaseService {
        public async Task<LoginResult> LoginAsync(string username, string password) {
            try {
                var body = new LoginRequestDto {
                    Username = username,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(body);

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "/api/auth/login") {
                    Content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json")
                };

                // 🔥 login no necesita bearer
                request.Headers.Authorization = null;

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode) {
                    return new LoginResult {
                        Success = false,
                        Error = "Credenciales inválidas."
                    };
                }

                var responseJson =
                    await response.Content.ReadAsStringAsync();

                var result =
                    JsonConvert.DeserializeObject<LoginResponseDto>(
                        responseJson);

                return new LoginResult {
                    Success = result?.Token != null,
                    Token = result?.Token,
                    Error = result?.Token == null
                        ? "Respuesta inválida del servidor."
                        : null
                };
            }
            catch (HttpRequestException) {
                return new LoginResult {
                    Success = false,
                    Error = "No se pudo conectar con el servidor."
                };
            }
            catch (TaskCanceledException) {
                return new LoginResult {
                    Success = false,
                    Error = "El servidor tardó demasiado en responder."
                };
            }
            catch (Exception ex) {
                return new LoginResult {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
    }
}