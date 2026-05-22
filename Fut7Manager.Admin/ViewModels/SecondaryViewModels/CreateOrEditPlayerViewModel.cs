using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Fut7Manager.Admin.Models.SecondaryModels;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels
{
    public class CreateOrEditPlayerViewModel : BaseViewModel {
        private readonly TeamService _teamService = new TeamService();
        private readonly LeagueDto _league;
        private string _name = "";
        private string _email = "";
        private PlayerPosition _position;
        private int _selectedTeamId;
        //private PositionItem _selectedPosition;
        private DateTime? _dateOfBirth;
        private readonly PlayerDto? _editingPlayer;
        

        public string Email
        {
            get => _email;
            set {
                _email = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public List<PositionItem> Positions { get; } = new()
        {
            new() { Value = PlayerPosition.Goalkeeper, Label = "Portero" },
            new() { Value = PlayerPosition.Defender, Label = "Defensa" },
            new() { Value = PlayerPosition.Midfielder, Label = "Medio" },
            new() { Value = PlayerPosition.Forward, Label = "Delantero" }
        };

        public ObservableCollection<TeamDto> Teams { get; } = new();

        public Action<bool>? CloseAction { get; set; }


        public PlayerPosition Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set {

                if (value > DateTime.Today)
                    value = DateTime.Today;

                SetProperty(ref _dateOfBirth, value);
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateOrEditPlayerViewModel(LeagueDto league, PlayerDto? player = null) {

            _league = league;
            _editingPlayer = player;

            SaveCommand = new RelayCommand(Save, CanSave);
 
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));

            if (player != null) {
                Name = player.Name;
                JerseyNumber = player.JerseyNumber;
                Phone = player.Phone ?? "";
                Email = player.Email ?? "";
                Position = player.Position;
                SelectedTeamId = player.TeamId;
                Active = player.Active;

                if (player.DateOfBirth != DateTime.MinValue)
                    DateOfBirth = player.DateOfBirth;
            }

            _ = LoadTeams();
        }

        private async Task LoadTeams() {
            var teams = await _teamService.GetTeamsAsync(_league.Id);

            Teams.Clear();

            foreach (var t in teams)
                Teams.Add(t);
        }

        // PROPERTIES


        public int JerseyNumber { get; set; }

        public string Phone { get; set; } = "";
        public bool Active { get; set; } = true;

        
        public int SelectedTeamId
        {
            get => _selectedTeamId;
            set {
                _selectedTeamId = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private bool CanSave() {
            return !string.IsNullOrWhiteSpace(Name)
            && SelectedTeamId > 0
            && IsValidEmail(Email);
        }

        private void Save() {
            CloseAction?.Invoke(true);
        }

        private bool IsValidEmail(string email) {
            if (string.IsNullOrWhiteSpace(email))
                return true; // opcional: vacío permitido

            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }

    public class PositionItem {
        public PlayerPosition Value { get; set; }
        public string Label { get; set; } = "";

    }
}
