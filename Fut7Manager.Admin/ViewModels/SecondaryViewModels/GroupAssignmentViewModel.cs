using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels {
    public class GroupAssignmentViewModel : BaseViewModel {
        private readonly TeamService _teamService;
        private readonly LeagueService _leagueService;
        private readonly int _leagueId;
        private bool _isLoading;
        public bool CompletedSuccessfully { get; private set; }
        public ObservableCollection<GroupWithTeams> Groups { get; set; } = new();
        public Action<bool>? CloseAction { get; set; }

        public enum SetupStep {
            Groups,
            Schedule
        }

        private SetupStep _currentStep;
        public SetupStep CurrentStep
        {
            get => _currentStep;
            set { _currentStep = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MatchdayDto> Matchdays { get; set; } = new();

        private bool _interGroupMatches;
        public bool InterGroupMatches
        {
            get => _interGroupMatches;
            set { _interGroupMatches = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set {
                _isLoading = value;
                OnPropertyChanged();
                (GenerateScheduleCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        //comandos
        public ICommand RandomizeCommand { get; }
        public ICommand ConfirmCommand { get; }
        //public ICommand NextStepCommand { get; }
        public ICommand GenerateScheduleCommand { get; }

        public ICommand FinishCommand { get; }

        public GroupAssignmentViewModel(List<TeamDto> teams, List<GroupDto> groups, int leagueId) {
            _leagueId = leagueId;
            _teamService = new TeamService();
            _leagueService = new LeagueService();

            CurrentStep = SetupStep.Groups;

            RandomizeCommand = new RelayCommand(Randomize);
            ConfirmCommand = new RelayCommand(async () => await Confirm());
            GenerateScheduleCommand = new RelayCommand(async () => await GenerateSchedule(), () => !IsLoading);
            FinishCommand = new RelayCommand(FinalizeSetup);
            foreach (var group in groups) {
                var groupVM = new GroupWithTeams {
                    Id = group.Id ?? 0,
                    Name = group.Name,
                    Teams = new ObservableCollection<TeamDto>(
                        teams.Where(t => t.GroupId == group.Id)
                    )
                };

                Groups.Add(groupVM);
            }
        }

        private void FinalizeSetup() {
            CompletedSuccessfully = true;

            CloseAction?.Invoke(true);
        }

        private async Task GenerateSchedule() {
            if (IsLoading) return;
            try {
                IsLoading = true;

                var result = await _leagueService.GenerateSchedule(_leagueId, InterGroupMatches);

            if (result != null) {

                Matchdays.Clear();

                foreach (var md in result)
                    Matchdays.Add(md);
                }
            }
            finally {
                IsLoading = false;
            }
        }

        private void GoToSchedule() {
            if (!IsBalanced()) {
                MessageBox.Show("Los grupos deben estar balanceados");
                return;
            }

            CurrentStep = SetupStep.Schedule;
        }

        private void Randomize() {
            var allTeams = Groups.SelectMany(g => g.Teams).ToList();
            var rnd = new Random();

            var shuffled = allTeams.OrderBy(x => rnd.Next()).ToList();

            int groupCount = Groups.Count;
            int baseSize = shuffled.Count / groupCount;
            int extra = shuffled.Count % groupCount;

            int index = 0;

            foreach (var group in Groups) {
                group.Teams.Clear();

                int size = baseSize + (extra-- > 0 ? 1 : 0);

                for (int i = 0; i < size; i++) {
                    var team = shuffled[index++];
                    team.GroupId = group.Id; // 🔥 importante
                    group.Teams.Add(team);
                }
            }
        }

        private bool IsBalanced() {
            var counts = Groups.Select(g => g.Teams.Count).ToList();

            int min = counts.Min();
            int max = counts.Max();

            return (max - min) <= 1;
        }

        private async Task Confirm() {
            if (!IsBalanced()) {
                MessageBox.Show("Los grupos deben tener máximo 1 equipo de diferencia");
                return;
            }

            foreach (var group in Groups) {
                foreach (var team in group.Teams) {
                    await _teamService.EditTeamAsync(new TeamDto {
                        Id = team.Id,
                        GroupId = group.Id,
                        Name = team.Name,
                        LeagueId = team.LeagueId,
                        TeamManagerName = team.TeamManagerName,
                        TeamManagerPhone = team.TeamManagerPhone,
                        TeamPrimaryColor = team.TeamPrimaryColor,
                        Paid = team.Paid,
                        Remaining = team.Remaining
                    });
                }
            }

            // 🔥 en lugar de cerrar:
            CurrentStep = SetupStep.Schedule;
        }

        public void MoveTeam(TeamDto team, GroupWithTeams targetGroup) {
            var sourceGroup = Groups.FirstOrDefault(g => g.Teams.Contains(team));

            if (sourceGroup == null || sourceGroup == targetGroup)
                return;

            sourceGroup.Teams.Remove(team);

            team.GroupId = targetGroup.Id;
            targetGroup.Teams.Add(team);
        }

        public void MoveMatch(Fut7MatchDto match, MatchdayDto targetMatchday) {
            var source = Matchdays.FirstOrDefault(md => md.Matches.Contains(match));

            if (source == null || targetMatchday == null || source == targetMatchday)
                return;

            source.Matches.Remove(match);
            targetMatchday.Matches.Add(match);
        }
    }
}