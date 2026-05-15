using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fut7Manager.Admin.Views.SecondaryWindows {
    public partial class GroupAssignmentWindow : Window {

        //private Point _startPoint;

        public GroupAssignmentWindow() {
            InitializeComponent();
        }

        // ============================
        // 🔹 DRAG EQUIPOS
        // ============================
        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var listBox = sender as ListBox;
            if (listBox?.SelectedItem == null)
                return;

            DragDrop.DoDragDrop(listBox, listBox.SelectedItem, DragDropEffects.Move);
        }

        private void ListBox_Drop(object sender, DragEventArgs e) {
            if (!e.Data.GetDataPresent(typeof(TeamDto)))
                return;

            var team = (TeamDto)e.Data.GetData(typeof(TeamDto));
            var listBox = sender as ListBox;

            if (listBox?.DataContext is GroupWithTeams targetGroup &&
                DataContext is GroupAssignmentViewModel vm) {

                vm.MoveTeam(team, targetGroup);
            }
        }

        // ============================
        // 🔹 DRAG PARTIDOS
        // ============================

        private void Close_Click(object sender, RoutedEventArgs e) {

            this.Close();
        }
    }
}