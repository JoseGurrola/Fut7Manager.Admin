// ============================================
// GroupAssignmentViewModel.cs
// ============================================

using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels {

    public class GroupAssignmentViewModel : BaseViewModel {

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
            set {
                _currentStep = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MatchdayDto> Matchdays { get; set; } = new();

        private bool _interGroupMatches;

        public bool InterGroupMatches
        {
            get => _interGroupMatches;
            set {
                _interGroupMatches = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set {

                _isLoading = value;

                OnPropertyChanged();

                (GenerateScheduleCommand as RelayCommand)?.RaiseCanExecuteChanged();

                (FinishCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool HasSchedule => Matchdays?.Any() == true;

        // ============================================
        // COMMANDS
        // ============================================

        public ICommand RandomizeCommand { get; }

        public ICommand ConfirmCommand { get; }

        public ICommand GenerateScheduleCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand FinishCommand { get; }

        public ICommand BackCommand { get; }

        public GroupAssignmentViewModel(
            List<TeamDto> teams,
            List<GroupDto> groups,
            int leagueId) {

            _leagueId = leagueId;

            _leagueService = new LeagueService();

            CurrentStep = SetupStep.Groups;

            RandomizeCommand =
                new RelayCommand(Randomize);

            ConfirmCommand =
                new RelayCommand(async () => await Confirm());

            GenerateScheduleCommand =
                new RelayCommand(
                    async () => await GenerateSchedule(),
                    () => !IsLoading);

            FinishCommand =
                new RelayCommand(
                    async () => await FinalizeSetup(),
                    () => HasSchedule && !IsLoading);

            CancelCommand =
                new RelayCommand(Cancel);

            BackCommand =
                new RelayCommand(GoBack);

            // ============================================
            // CREAR GRUPOS
            // ============================================

            foreach (var group in groups) {

                var groupVM = new GroupWithTeams {

                    Id = group.Id ?? 0,

                    Name = group.Name,

                    Teams = new ObservableCollection<TeamDto>()
                };

                Groups.Add(groupVM);
            }

            // ============================================
            // METER EQUIPOS TEMPORALMENTE
            // ============================================

            if (Groups.Any()) {

                foreach (var team in teams) {
                    Groups[0].Teams.Add(team);
                }

                Randomize();
            }
        }

        // ============================================
        // GENERATE SCHEDULE
        // ============================================

        private async Task GenerateSchedule() {

            if (IsLoading)
                return;

            try {

                IsLoading = true;

                var assignments = Groups
                    .SelectMany(g => g.Teams.Select(t =>
                        new TeamGroupAssignmentDto {

                            TeamId = t.Id,

                            GroupId = g.Id
                        }))
                    .ToList();

                var result =
                    await _leagueService.PreviewScheduleAsync(
                        _leagueId,
                        InterGroupMatches,
                        assignments);

                Matchdays.Clear();

                if (result != null) {

                    foreach (var md in result)
                        Matchdays.Add(md);
                }

                OnPropertyChanged(nameof(HasSchedule));
            }
            finally {

                IsLoading = false;
            }
        }

        // ============================================
        // FINALIZE
        // ============================================

        private async Task FinalizeSetup() {
            try {
                IsLoading = true;

                var assignments = Groups
                    .SelectMany(g => g.Teams.Select(t =>
                        new TeamGroupAssignmentDto {
                            TeamId = t.Id,
                            GroupId = g.Id
                        }))
                    .ToList();

                await _leagueService.FinalizeSetupAsync(
                    _leagueId,
                    InterGroupMatches,
                    assignments);

                CompletedSuccessfully = true;
                CloseAction?.Invoke(true);
            }
            catch (Exception) {
                MessageService.Show("No se pudo finalizar la liga");
            }
            finally {
                IsLoading = false;
            }
        }
        // ============================================
        // BACK
        // ============================================

        private void GoBack() {

            CurrentStep = SetupStep.Groups;
        }

        // ============================================
        // RANDOMIZE
        // ============================================

        private void Randomize() {

            var allTeams = Groups
                .SelectMany(g => g.Teams)
                .ToList();

            var rnd = new Random();

            var shuffled =
                allTeams
                .OrderBy(x => rnd.Next())
                .ToList();

            int groupCount = Groups.Count;

            int baseSize = shuffled.Count / groupCount;

            int extra = shuffled.Count % groupCount;

            int index = 0;

            foreach (var group in Groups) {

                group.Teams.Clear();

                int size =
                    baseSize + (extra-- > 0 ? 1 : 0);

                for (int i = 0; i < size; i++) {

                    var team = shuffled[index++];

                    team.GroupId = group.Id;

                    group.Teams.Add(team);
                }
            }
        }

        // ============================================
        // VALIDATE BALANCE
        // ============================================

        private bool IsBalanced() {

            var counts =
                Groups.Select(g => g.Teams.Count).ToList();

            int min = counts.Min();

            int max = counts.Max();

            return (max - min) <= 1;
        }

        // ============================================
        // CONFIRM
        // ============================================

        private async Task Confirm() {

            if (!IsBalanced()) {

                MessageService.Show(
                    "Los grupos deben tener máximo 1 equipo de diferencia",
                    "Validación");

                return;
            }

            CurrentStep = SetupStep.Schedule;

            await Task.CompletedTask;
        }

        // ============================================
        // CANCEL
        // ============================================

        private void Cancel() {

            CompletedSuccessfully = false;

            CloseAction?.Invoke(false);
        }

        // ============================================
        // MOVE TEAM
        // ============================================

        public void MoveTeam(
            TeamDto team,
            GroupWithTeams targetGroup) {

            var sourceGroup =
                Groups.FirstOrDefault(g =>
                    g.Teams.Contains(team));

            if (sourceGroup == null ||
                sourceGroup == targetGroup)
                return;

            sourceGroup.Teams.Remove(team);

            team.GroupId = targetGroup.Id;

            targetGroup.Teams.Add(team);
        }
    }
}