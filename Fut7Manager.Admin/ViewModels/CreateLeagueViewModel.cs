using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class CreateOrEditLeagueViewModel : BaseViewModel {
        private string _leagueName = string.Empty;
        private readonly int? _leagueId;
        private decimal _registrationFee;
        private int _numberOfGroups = 0;

        //private readonly LeagueService _leagueService = new LeagueService();

        public string LeagueName
        {
            get => _leagueName;
            set {
                if (SetProperty(ref _leagueName, value)) {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal RegistrationFee
        {
            get => _registrationFee;
            set {
                if (SetProperty(ref _registrationFee, value)) {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public int NumberOfGroups
        {
            get => _numberOfGroups;
            set {
                if (SetProperty(ref _numberOfGroups, value)) {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string ButtonText { get; set; } = "Crear";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<bool> CloseAction { get; set; } = default!;

        public CreateOrEditLeagueViewModel(LeagueDto? league = null) {
            if (league != null) {
                _leagueId = league.Id;
                LeagueName = league.Name;
                RegistrationFee = league.RegistrationFee;

                ButtonText = "Guardar";
            }

            // Use RelayCommand that supports async properly with fire-and-forget
            SaveCommand = new RelayCommand(SaveLeague);
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
        }

        //private bool CanSave() => !string.IsNullOrWhiteSpace(LeagueName);

        private void SaveLeague() {
            if (string.IsNullOrWhiteSpace(LeagueName))
                return;

            if (RegistrationFee < 0) return;

            CloseAction?.Invoke(true);
        }

        
    }
}