// ============================================
// StartPlayoffViewModel.cs
// ============================================

using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Models.SecondaryModels;
using Fut7Manager.Admin.Models.SecondaryModels.PlayoffBracketModels;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using GroupStandingDto = Fut7Manager.Admin.Models.GroupStandingDto;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels {

    public class StartPlayoffViewModel : BaseViewModel {
        private readonly LeagueDto _league;

        public bool CompletedSuccessfully { get; private set; }

        public Action<bool>? CloseAction { get; set; }

        public enum PlayoffStep {
            Teams,
            Matchups
        }

        private PlayoffStep _currentStep;

        public PlayoffStep CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }

        public ObservableCollection<PlayoffTeamDto> QualifiedTeams { get; set; }
            = new();

        public ObservableCollection<PlayoffTeamDto> EliminatedTeams { get; set; }
            = new();

        public ObservableCollection<PlayoffBracketRoundDto> LeftRounds
        { get; set; } = new();

        public ObservableCollection<PlayoffBracketRoundDto> RightRounds
        { get; set; } = new();

        private PlayoffBracketRoundDto? _finalRound;
        public PlayoffBracketRoundDto? FinalRound
        {
            get => _finalRound;
            set => SetProperty(ref _finalRound, value);
        }

        //=========================
        // COMMANDS
        //=========================
        public ICommand CancelCommand { get; }

        public ICommand NextCommand { get; }

        public ICommand BackCommand { get; }

        public ICommand FinishCommand { get; }

        public StartPlayoffViewModel(
            IEnumerable<GroupStandingDto> groupedStandings, LeagueDto league) {
            _league = league;

            LoadTeams(groupedStandings);

            CurrentStep = PlayoffStep.Teams;

            CancelCommand =
                new RelayCommand(Cancel);

            NextCommand =
                new RelayCommand(GoNext);

            BackCommand =
                new RelayCommand(GoBack);

            FinishCommand =
                new RelayCommand(Finish);
        }

        private void LoadTeams(
    IEnumerable<GroupStandingDto> groupedStandings) {
            foreach (var group in groupedStandings) {
                foreach (var standing in group.Standings) {
                    var team = new PlayoffTeamDto {
                        TeamId = standing.TeamId,
                        Name = standing.TeamName,
                        LogoUrl = standing.LogoUrl,
                        Position = standing.Position,
                        Points = standing.Points,
                        GroupName = group.GroupName,
                        IsQualified = standing.IsQualified
                    };

                    if (standing.IsQualified)
                        QualifiedTeams.Add(team);
                    else
                        EliminatedTeams.Add(team);
                }
            }
        }

        private bool IsPowerOfTwo(int x) {
            return x > 0 &&
                   (x & (x - 1)) == 0;
        }

        private void GoNext() {
            if (CurrentStep == PlayoffStep.Teams) {
                if (QualifiedTeams.Count != _league.TotalQualifiedTeams) {
                    MessageService.Show(
                        $"La cantidad de clasificados debe ser {_league.TotalQualifiedTeams}",
                        "Validación");

                    return;
                }

                GenerateBracket();

                CurrentStep = PlayoffStep.Matchups;
            }
        }

        private void GenerateBracket() {
            LeftRounds.Clear();
            RightRounds.Clear();

            FinalRound = null;

            var teams = GenerateFirstRound();

            int rounds = (int)Math.Log2(teams.Count);

            var allRounds = new List<PlayoffBracketRoundDto>();

            int matches = teams.Count / 2;

            for (int round = 1; round <= rounds; round++) {
                var dto = new PlayoffBracketRoundDto {
                    RoundNumber = round,
                    Name = GetRoundName(matches)
                };

                for (int i = 0; i < matches; i++) {
                    dto.Matches.Add(
                        new PlayoffBracketMatchDto());

                }


                allRounds.Add(dto);

                matches /= 2;
            }

            SplitRounds(allRounds);

            FillFirstRound(teams);
        }

        private void SplitRounds(
    List<PlayoffBracketRoundDto> rounds) {
            if (rounds.Count == 1) {
                FinalRound = rounds[0];
                return;
            }

            FinalRound = rounds.Last();

            var sideRounds =
                rounds.Take(rounds.Count - 1).ToList();

            for (int i = 0; i < sideRounds.Count; i++) {
                var source = sideRounds[i];

                int matchesPerSide =
                    Math.Max(1, source.Matches.Count / 2);

                var left =
                    new PlayoffBracketRoundDto {
                        Name = source.Name,
                        RoundNumber = source.RoundNumber
                    };

                var right =
                    new PlayoffBracketRoundDto {
                        Name = source.Name,
                        RoundNumber = source.RoundNumber,
                        IsMirrored = true
                    };

                for (int j = 0; j < matchesPerSide; j++) {
                    left.Matches.Add(
                        new PlayoffBracketMatchDto());

                    right.Matches.Add(
                        new PlayoffBracketMatchDto());
                }

                LeftRounds.Add(left);
                RightRounds.Add(right);
            }

            CalculateSpacing();
        }

        private PlayoffBracketRoundDto CloneRound(
    PlayoffBracketRoundDto source) {
            var clone =
                new PlayoffBracketRoundDto {
                    Name = source.Name,
                    RoundNumber = source.RoundNumber,
                    IsMirrored = source.IsMirrored
                };

            foreach (var match in source.Matches) {
                clone.Matches.Add(
                    new PlayoffBracketMatchDto());
            }

            return clone;
        }

        private void CalculateSpacing() {
            CalculateSideSpacing(LeftRounds);
            CalculateSideSpacing(RightRounds);

            if (FinalRound != null) {
                int factor =
                    (int)Math.Pow(2, LeftRounds.Count);

                FinalRound.MarginTop = 80;
            }
        }
        private void CalculateSideSpacing(
    ObservableCollection<PlayoffBracketRoundDto> rounds) {
            const double cardHeight = 85;
            const double cardSpacing = 25;

            for (int i = 0; i < rounds.Count; i++) {
                var round = rounds[i];

                int factor = (int)Math.Pow(2, i);

                round.MarginTop =
     factor == 1
         ? 0
         : ((cardHeight + cardSpacing)
             * (factor - 1)
             / 2);

                round.MatchMargin =
     factor == 1
         ? 15
         : ((cardHeight + cardSpacing) * factor / 2);

                foreach (var match in round.Matches) {
                    match.MarginBottom =
                        round.MatchMargin;
                }
            }
        }

        private string GetRoundName(int matches) {
            return matches switch {
                1 => "Final",
                2 => "Semifinal",
                4 => "Cuartos",
                8 => "Octavos",
                _ => $"Ronda {matches}"
            };
        }

        private List<PlayoffTeamDto> GenerateFirstRound() {
            return QualifiedTeams
                .OrderBy(t => t.Position)
                .ToList();
        }

        private void FillFirstRound(
    List<PlayoffTeamDto> teams) {
            if (!LeftRounds.Any())
                return;

            var firstLeft = LeftRounds.First();
            var firstRight = RightRounds.FirstOrDefault();

            int totalMatches =
                teams.Count / 2;

            int leftMatches =
                totalMatches / 2;

            int rightMatches =
                totalMatches - leftMatches;

            var pairings =
                GeneratePairings(teams);

            for (int i = 0; i < leftMatches; i++) {
                firstLeft.Matches[i].HomeTeam =
                    pairings[i].Item1;

                firstLeft.Matches[i].AwayTeam =
                    pairings[i].Item2;
            }

            if (firstRight != null) {
                for (int i = 0; i < rightMatches; i++) {
                    var pairing =
                        pairings[leftMatches + i];

                    firstRight.Matches[i].HomeTeam =
                        pairing.Item1;

                    firstRight.Matches[i].AwayTeam =
                        pairing.Item2;
                }
            }
        }

        private List<(PlayoffTeamDto, PlayoffTeamDto)>
GeneratePairings(
    List<PlayoffTeamDto> teams) {
            var result =
                new List<(PlayoffTeamDto, PlayoffTeamDto)>();

            int count = teams.Count;

            for (int i = 0; i < count / 2; i++) {
                result.Add(
                    (
                        teams[i],
                        teams[count - i - 1]
                    ));
            }

            return result;
        }

        private void GoBack() {
            if (CurrentStep == PlayoffStep.Matchups) {
                CurrentStep = PlayoffStep.Teams;
            }
        }

        private void Cancel() {
            CompletedSuccessfully = false;

            CloseAction?.Invoke(false);
        }


        private void Finish() {
            CompletedSuccessfully = true;

            CloseAction?.Invoke(true);
        }

        public void MoveTeam(
    PlayoffTeamDto team,
    bool toQualified) {
            if (toQualified) {
                if (QualifiedTeams.Contains(team))
                    return;

                EliminatedTeams.Remove(team);

                team.IsQualified = true;

                QualifiedTeams.Add(team);
            } else {
                if (EliminatedTeams.Contains(team))
                    return;

                QualifiedTeams.Remove(team);

                team.IsQualified = false;

                EliminatedTeams.Add(team);
            }
        }
    }
}