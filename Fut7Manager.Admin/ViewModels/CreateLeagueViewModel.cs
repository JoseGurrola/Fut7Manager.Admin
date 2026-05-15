using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class CreateOrEditLeagueViewModel : BaseViewModel {
        private string _leagueName = string.Empty;
        private readonly int? _leagueId;
        private decimal _registrationFee;
        private int _numberOfGroups = 1;
        private LeagueStatus _status = LeagueStatus.Upcoming;
        private string? _logoUrl = string.Empty;
        public ICommand UploadLogoCommand { get; }
        private string? _logoFileName;
        private string? _localImagePath;
        public string? FinalLogoUrl { get; private set; }

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

        public LeagueStatus Status
        {
            get => _status;
            set {
                if (SetProperty(ref _status, value)) {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string? LogoUrl
        {
            get => _logoUrl;
            set {
                if (SetProperty(ref _logoUrl, value)) {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string? LogoFileName
        {
            get => _logoFileName;
            set => SetProperty(ref _logoFileName, value);
        }
        public string? LocalImagePath
        {
            get => _localImagePath;
            set => SetProperty(ref _localImagePath, value);
        }

        public string ButtonText { get; set; } = "Crear";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<bool> CloseAction { get; set; } = default!;

        public CreateOrEditLeagueViewModel(LeagueDto? league = null, int? numberOfGroups = 1) {
            if (league != null) {
                _leagueId = league.Id;
                LeagueName = league.Name;
                RegistrationFee = league.RegistrationFee;
                Status = league.Status;
                LogoUrl = league.LogoUrl;
                if(numberOfGroups.HasValue)
                    NumberOfGroups = numberOfGroups.Value;

                ButtonText = "Guardar";
            }

            // Use RelayCommand that supports async properly with fire-and-forget
            SaveCommand = new RelayCommand(SaveLeague);
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
            UploadLogoCommand = new RelayCommand(async () => await UploadLogo());
        }

        //private bool CanSave() => !string.IsNullOrWhiteSpace(LeagueName);

        public async Task UploadLogo() {
            var dialog = new Microsoft.Win32.OpenFileDialog {
                Filter = "Imágenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() == true) {
                LocalImagePath = dialog.FileName;
                LogoFileName = System.IO.Path.GetFileName(dialog.FileName);

                LogoUrl = LocalImagePath;
            }
        }

        private async void SaveLeague() {

            if (string.IsNullOrWhiteSpace(LeagueName))
                return;

            if (RegistrationFee < 0)
                return;

            FinalLogoUrl = LogoUrl;

            try {

                // Subir imagen si seleccionó una nueva
                if (!string.IsNullOrEmpty(LocalImagePath)) {

                    var uploadService = new UploadFileService();

                    var uploadedUrl =
                        await uploadService.UploadLogoAsync(
                            LocalImagePath,
                            "league");

                    if (!string.IsNullOrEmpty(uploadedUrl)) {
                        FinalLogoUrl = uploadedUrl;
                    }
                }

                var league = new LeagueDto {

                    Id = _leagueId ?? 0,
                    Name = LeagueName,
                    RegistrationFee = RegistrationFee,
                    //NumberOfGroups = NumberOfGroups,
                    Status = Status,
                    LogoUrl = FinalLogoUrl
                };

                CloseAction?.Invoke(true);
            }
            catch (Exception ex) {

                MessageService.Show(
                    $"[SaveLeague] Error: {ex.Message}",
                    "Error");
            }
        }


    }
}