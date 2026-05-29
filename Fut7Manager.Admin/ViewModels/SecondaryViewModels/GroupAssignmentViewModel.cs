// ============================================
// GroupAssignmentViewModel.cs
// ============================================

using DocumentFormat.OpenXml.Drawing;
using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels {

    public class GroupAssignmentViewModel : BaseViewModel {

        private readonly LeagueService _leagueService;
        private readonly GroupService _groupService;
        private readonly int _leagueId;
        private readonly List<TeamDto> _teams;

        private bool _isLoading;

        public bool CompletedSuccessfully { get; private set; }

        public ObservableCollection<GroupWithTeams> Groups { get; set; } = new();

        public Action<bool>? CloseAction { get; set; }

        public enum SetupStep {
            Configuration,
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

        private int _numberOfGroups = 2;

        public int NumberOfGroups
        {
            get => _numberOfGroups;
            set {
                if (SetProperty(ref _numberOfGroups, value)) {
                    OnPropertyChanged(nameof(QualifiedTeamsOptions));

                    var options = QualifiedTeamsOptions;

                    if (options.Any()) {
                        QualifiedTeamsPerGroup = options.First();
                    }
                }
            }
        }

        private int _qualifiedTeamsPerGroup = 2;

        public int QualifiedTeamsPerGroup
        {
            get => _qualifiedTeamsPerGroup;
            set => SetProperty(ref _qualifiedTeamsPerGroup, value);
        }

        public List<int> NumberOfGroupsOptions =>
            new() { 1, 2, 4, 8 };

        public List<int> QualifiedTeamsOptions =>
            Enumerable.Range(1, 8)
                .Where(IsValidConfiguration)
                .ToList();

        private bool IsPowerOfTwo(int number) {
            return number > 1 &&
                   (number & (number - 1)) == 0;
        }
        private bool IsValidConfiguration(int qualified) {
            if (NumberOfGroups <= 0)
                return false;

            int totalTeams = _teams.Count;

            int teamsPerGroup =
                totalTeams / NumberOfGroups;

            if (teamsPerGroup <= 0)
                return false;

            if (qualified > teamsPerGroup)
                return false;

            int totalQualified =
                NumberOfGroups * qualified;

            return IsPowerOfTwo(totalQualified);
        }

        // ============================================
        // COMMANDS
        // ============================================

        public ICommand RandomizeCommand { get; }

        public ICommand ConfirmCommand { get; }

        public ICommand GenerateScheduleCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand FinishCommand { get; }

        public ICommand BackCommand { get; }

        public ICommand ContinueConfigurationCommand { get; }

        public GroupAssignmentViewModel(IEnumerable<TeamDto> teams, IEnumerable<GroupDto> groups, int leagueId) {

            _leagueId = leagueId;

            _leagueService = new LeagueService();
            _groupService = new GroupService();
            _teams = teams.ToList();
            CurrentStep = SetupStep.Configuration;

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

            ContinueConfigurationCommand = new RelayCommand(ContinueConfiguration);
            // ============================================
            // CREAR GRUPOS
            // ============================================

            GenerateGroups();


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

                await PersistGroups();

                var league = await _leagueService.GetLeagueByIdAsync(_leagueId);

                league.QualifiedTeamsPerGroup = QualifiedTeamsPerGroup;
                league.Status = LeagueStatus.InProgress;

                await _leagueService.EditLeagueAsync(league);

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

            if (CurrentStep == SetupStep.Groups) CurrentStep = SetupStep.Configuration;
            else
            CurrentStep = SetupStep.Groups;
        }

        // ============================================
        // RANDOMIZE
        // ============================================

        private void Randomize() {

            var allTeams = _teams.ToList();

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

        private void ContinueConfiguration() {
            if (!QualifiedTeamsOptions.Any()) {
                MessageService.Show("Configuración inválida", "Validación");

                return;
            }

            GenerateGroups();

            Randomize();

            CurrentStep = SetupStep.Groups;
        }

        private void GenerateGroups() {
            Groups.Clear();

            for (int i = 0; i < NumberOfGroups; i++) {
                Groups.Add(new GroupWithTeams {
                    Id = i + 1,
                    Name = $"Grupo {i + 1}",
                    Teams = new ObservableCollection<TeamDto>()
                });
            }
        }

        private async Task PersistGroups() {
            var existingGroups =
                await _groupService.GetGroupsAsync(_leagueId);

            foreach (var g in existingGroups) {
                if (g.Id.HasValue) {
                    await _groupService.DeleteGroupAsync(g.Id.Value);
                }
            }

            var updatedGroups =
                new List<GroupWithTeams>();

            foreach (var group in Groups) {
               
                var created = await _groupService.CreateGroupAsync(
                        new GroupDto {
                            Name = group.Name ?? "group",
                            LeagueId = _leagueId
                        });

                if (created == null) {
                    // If creation failed, preserve original name and teams
                    updatedGroups.Add(new GroupWithTeams {
                        Id = 0,
                        Name = group.Name,
                        Teams = group.Teams
                    });
                    continue;
                }

                updatedGroups.Add(new GroupWithTeams {
                    Id = created.Id ?? 0,
                    Name = created.Name ?? group.Name,
                    Teams = group.Teams
                });
            }

            Groups.Clear();

            foreach (var g in updatedGroups) {
                Groups.Add(g);
            }
        }
    }
}