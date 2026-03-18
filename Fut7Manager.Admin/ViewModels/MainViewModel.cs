using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class MainViewModel : BaseViewModel {
        private object? _currentView;

        public object? CurrentView
        {
            get => _currentView;
            set {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowPlayersCommand { get; }
        public ICommand ShowTeamsCommand { get; }
        public ICommand ShowLeaguesCommand { get; }
        public ICommand ShowMatchesCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public MainViewModel() {
            var loginVM = new LoginViewModel();

            loginVM.OnLoginSuccess = () =>
            {
                CurrentView = new PlayersViewModel();
            };

            CurrentView = loginVM;

            ShowPlayersCommand = new RelayCommand(
                () => CurrentView = new PlayersViewModel(),
                () => CurrentView is not PlayersViewModel
            );

            ShowTeamsCommand = new RelayCommand(
                () => CurrentView = new TeamsViewModel(),
                () => CurrentView is not TeamsViewModel
            );

            ShowLeaguesCommand = new RelayCommand(
                () => CurrentView = new LeaguesViewModel(),
                () => CurrentView is not LeaguesViewModel
            );

            ShowMatchesCommand = new RelayCommand(
                () => CurrentView = new MatchesViewModel(),
                () => CurrentView is not MatchesViewModel
            );

            //CurrentView = new PlayersViewModel();
        }
    }
}
