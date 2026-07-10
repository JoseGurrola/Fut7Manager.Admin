using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Models.SecondaryModels;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fut7Manager.Admin.Views.SecondaryWindows {
    public partial class StartPlayoffWindow : Window {

        //private Point _startPoint;

        public StartPlayoffWindow() {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseMove(
         object sender,
         MouseEventArgs e) {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var listBox = sender as ListBox;

            if (listBox?.SelectedItem == null)
                return;

            DragDrop.DoDragDrop(
                listBox,
                listBox.SelectedItem,
                DragDropEffects.Move);
        }

        private void Qualified_Drop(
            object sender,
            DragEventArgs e) {
            if (!e.Data.GetDataPresent(typeof(PlayoffTeamDto)))
                return;

            var team =
                (PlayoffTeamDto)e.Data.GetData(
                    typeof(PlayoffTeamDto));

            if (DataContext is StartPlayoffViewModel vm) {
                vm.MoveTeam(team, true);
            }
        }

        private void Eliminated_Drop(
            object sender,
            DragEventArgs e) {
            if (!e.Data.GetDataPresent(typeof(PlayoffTeamDto)))
                return;

            var team =
                (PlayoffTeamDto)e.Data.GetData(
                    typeof(PlayoffTeamDto));

            if (DataContext is StartPlayoffViewModel vm) {
                vm.MoveTeam(team, false);
            }
        }
    }
}