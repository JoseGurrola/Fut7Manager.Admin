using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels {
    public class PaymentsViewModel : BaseViewModel {
        private readonly PaymentService _paymentService;
        private readonly int _teamId;

        public string TeamName { get; }
        public ObservableCollection<PaymentDto> Payments { get; } = new();

        public decimal NewAmount { get; set; }

        public ICommand AddPaymentCommand { get; }
        public ICommand DeletePaymentCommand { get; }
        public ICommand CloseCommand { get; }

        public Action<bool> CloseAction { get; set; } = default!;

        public PaymentsViewModel(PaymentService paymentService, int teamId, string teamName) {
            _paymentService = paymentService;
            _teamId = teamId;
            TeamName = teamName;

            AddPaymentCommand = new RelayCommand(async () => await AddPaymentAsync());
            DeletePaymentCommand = new RelayCommand<PaymentDto>(async p => await DeletePaymentAsync(p));
            CloseCommand = new RelayCommand(() => CloseAction?.Invoke(false));

            _ = LoadPayments();
        }

        private async Task LoadPayments() {
            var payments = await _paymentService.GetPaymentsAsync(_teamId);

            Payments.Clear();
            foreach (var p in payments
        .OrderByDescending(p => p.Date)) {
                Payments.Add(p);
            }
        }

        private bool CanAddPayment() => NewAmount > 0;

        private async Task AddPaymentAsync() {
            if (NewAmount <= 0)
                return;

            var created = await _paymentService.CreatePaymentAsync(new PaymentDto {
                TeamId = _teamId,
                Amount = NewAmount
            });

            if (created != null) {
                Payments.Insert(0, created);
                NewAmount = 0;
                OnPropertyChanged(nameof(NewAmount));
            }
        }

        private async Task DeletePaymentAsync(PaymentDto payment) {
            var success = await _paymentService.DeletePaymentAsync(payment.Id);
            if (success)
                Payments.Remove(payment);
        }

    }
}
