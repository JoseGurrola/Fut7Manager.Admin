using Fut7Manager.Admin.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fut7Manager.Admin.Services {
    public class PaymentService : BaseService {

        public async Task<List<PaymentDto>> GetPaymentsAsync(int teamId) {

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/Payments?teamId={teamId}");

            var result =
                await SendAsync<List<PaymentDto>>(request);

            return result ?? new List<PaymentDto>();
        }

        public async Task<PaymentDto?> CreatePaymentAsync(PaymentDto payment) {

            var body = new {
                teamId = payment.TeamId,
                amount = payment.Amount
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/Payments") {
                Content = JsonContent.Create(body)
            };

            return await SendAsync<PaymentDto>(request);
        }

        public async Task<bool> DeletePaymentAsync(int id) {

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/Payments/{id}");

            var result =
                await SendAsync<object>(request);

            return result != null;
        }
    }
}