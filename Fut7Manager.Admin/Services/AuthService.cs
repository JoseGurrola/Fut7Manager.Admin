using Fut7Manager.Admin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fut7Manager.Admin.Services {
    public class AuthService {
        private readonly HttpClient _httpClient;

        public AuthService() {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7202");
        }

        public async Task<string?> LoginAsync(string username, string password) {
            var body = new LoginRequestDto {
                Username = username,
                Password = password
            };

            var json = JsonConvert.SerializeObject(body);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content);

            var responseJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"LOGIN STATUS: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"LOGIN RESPONSE: {responseJson}");

            if (!response.IsSuccessStatusCode)
                return null;

            var result = JsonConvert.DeserializeObject<LoginResponseDto>(responseJson);

            return result?.Token;
        }
    }
}
