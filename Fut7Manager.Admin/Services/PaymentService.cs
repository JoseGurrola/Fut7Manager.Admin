using Fut7Manager.Admin.Models;

namespace Fut7Manager.Admin.Services {

    public class PaymentService : BaseService {

        public async Task<List<PaymentDto>> GetPaymentsAsync(
            int teamId) {

            return await GetAsync<List<PaymentDto>>(
                $"/api/Payments?teamId={teamId}")
                ?? new List<PaymentDto>();
        }

        public async Task<PaymentDto?> CreatePaymentAsync(
            PaymentDto payment) {

            var body = new {

                teamId = payment.TeamId,
                amount = payment.Amount
            };

            return await PostAsync<PaymentDto>(
                "/api/Payments",
                body);
        }

        public async Task<bool> DeletePaymentAsync(
            int id) {

            return await DeleteAsync(
                $"/api/Payments/{id}");
        }
    }
}