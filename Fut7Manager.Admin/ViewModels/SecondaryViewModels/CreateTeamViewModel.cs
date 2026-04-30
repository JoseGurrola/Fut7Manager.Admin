using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Fut7Manager.Admin.ViewModels {
    public class CreateOrEditTeamViewModel : BaseViewModel {
        private string _teamName = string.Empty;
        private readonly int? _teamId;
        private string? _logoUrl = string.Empty;
        private GroupService _groupService = new GroupService();
        private string _teamManager = string.Empty;
        private string _teamManagerPhone = string.Empty;
        private decimal _paid;
        private decimal _remaining;
        private LeagueDto _league;
        public ICommand UploadLogoCommand { get; }
        private string? _logoFileName;
        private string? _localImagePath;
        


        public ObservableCollection<GroupDto> AvailableGroupNumbers { get; }
        = new ObservableCollection<GroupDto>();

        public bool CanEditTeamInfo => _league.Status == LeagueStatus.Upcoming;

        private int? _selectedGroup;
        public int? SelectedGroup
        {
            get => _selectedGroup;
            set {
                _selectedGroup = value;
                System.Diagnostics.Debug.WriteLine($"SelectedGroup changed to: {_selectedGroup}");
                OnPropertyChanged();
            }
        }

        private readonly TeamService _teamService = new TeamService();

        public string TeamName
        {
            get => _teamName;
            set {
                if (SetProperty(ref _teamName, value)) {
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

        public decimal Paid
        {
            get => _paid;
            set {
                if (SetProperty(ref _paid, value)) {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal Remaining
        {
            get => _remaining;
            set {
                if (SetProperty(ref _remaining, value)) {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }


        public string TeamManager
        {
            get => _teamManager;
            set {
                if (SetProperty(ref _teamManager, value))
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        
        public string TeamManagerPhone
        {
            get => _teamManagerPhone;
            set {
                if (SetProperty(ref _teamManagerPhone, value))
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

        private int? _originalGroupId;
        public CreateOrEditTeamViewModel(TeamDto? team = null, LeagueDto league = default!) {
            _league = league;
            if (team != null) {
                _teamId = team.Id;
                TeamName = team.Name;
                LogoUrl = team.LogoUrl;
                ButtonText = "Guardar";
                _originalGroupId = team.GroupId;
                Remaining = team.Remaining;
                Paid = team.Paid;
                TeamManager = team.TeamManagerName;
                TeamManagerPhone = team.TeamManagerPhone;
            }



             _ = LoadGroups(_league.Id);


            // Use RelayCommand that supports async properly with fire-and-forget
            //SaveCommand = new RelayCommand(SaveTeam);
            SaveCommand = new RelayCommand(SaveTeam, CanSaveTeam);
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
            UploadLogoCommand = new RelayCommand(async () => await UploadLogo());
        }

        private bool CanSaveTeam() {
            return !string.IsNullOrWhiteSpace(TeamName)
                && !string.IsNullOrWhiteSpace(TeamManager)
                && !string.IsNullOrWhiteSpace(TeamManagerPhone)
                && TeamManagerPhone.Length == 10
                && SelectedGroup != null;
        }

        public async Task LoadGroups(int leagueId) {
            //IsLoading = true;
            var groups = await _groupService.GetGroupsAsync(leagueId);

            LoadGroupsFromApi(groups);
            
            // IsLoading = false;
        }

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

        void LoadGroupsFromApi(IEnumerable<GroupDto> groups) {
            AvailableGroupNumbers.Clear();

            foreach (var g in groups)
                AvailableGroupNumbers.Add(g);

            // Seleccionar primer grupo por defecto
            if (_originalGroupId.HasValue)
                SelectedGroup = _originalGroupId.Value;
            else {
            if (AvailableGroupNumbers.Count > 0) {

                SelectedGroup = AvailableGroupNumbers[0].Id;
            } 
            }
        }

        private async void SaveTeam() {
            try {
                // 🔥 Subir imagen SOLO si seleccionaron una nueva
                if (!string.IsNullOrEmpty(LocalImagePath)) {
                    var uploadService = new UploadFileService();
                    var url = await uploadService.UploadLogoAsync(LocalImagePath, "team");

                    if (string.IsNullOrEmpty(url)) {
                        // 🔥 Aquí está el control que te faltaba
                        MessageBox.Show("Error al subir la imagen");
                        return;
                    }

                    LogoUrl = url;
                }

                //var team = new TeamDto {
                //    Id = _teamId ?? 0,
                //    Name = TeamName,
                //    LogoUrl = LogoUrl,
                //    GroupId = SelectedGroup,
                //    LeagueId = _league.Id,
                //    TeamManagerName = TeamManager,
                //    TeamManagerPhone = TeamManagerPhone
                //};

                //if (_teamId.HasValue)
                //    await _teamService.EditTeamAsync(team);
                //else
                //    await _teamService.CreateTeamAsync(team);

                CloseAction?.Invoke(true);
            }
            catch (Exception ex) {
                Debug.WriteLine($"Error al guardar equipo: {ex.Message}");
            }
        }


    }
}