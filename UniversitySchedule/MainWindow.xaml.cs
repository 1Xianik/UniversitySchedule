using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;
using Group = UniversitySchedule.WPF.Models.Group;

namespace UniversitySchedule.WPF
{
    public partial class MainWindow : Window
    {
        private List<Group> groups = new List<Group>();
        private List<ScheduleItem> currentSchedule = new List<ScheduleItem>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGroupsAsync();
            SetDefaultDay();
        }

        private async System.Threading.Tasks.Task LoadGroupsAsync()
        {
            try
            {
                var list = await AppServices.Api.GetAsync<List<Group>>("api/groups");
                if (list == null)
                {
                    txtStatus.Text = "Не удалось получить список групп.";
                    return;
                }

                groups = list;
                cmbGroups.ItemsSource = groups;
                if (groups.Count > 0)
                    cmbGroups.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetDefaultDay()
        {
            int today = (int)DateTime.Now.DayOfWeek;
            cmbDay.SelectedIndex = today; // 0 = вся неделя, 1-6 = дни
        }

        private async void BtnShowSchedule_Click(object sender, RoutedEventArgs e)
        {
            await LoadScheduleAsync();
        }

        private async System.Threading.Tasks.Task LoadScheduleAsync()
        {
            if (cmbGroups.SelectedItem is not Group selectedGroup)
            {
                MessageBox.Show("Выберите группу.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var list = await AppServices.Api.GetAsync<List<ScheduleItem>>(
                    $"api/schedule/group/{selectedGroup.GroupID}");
                currentSchedule = list ?? new List<ScheduleItem>();
                ApplyDayFilter();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Требуется авторизация. Войдите в систему.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyDayFilter()
        {
            if (currentSchedule == null) return;

            int selectedDay = GetSelectedDay();

            List<ScheduleItem> filtered = selectedDay == 0
                ? currentSchedule
                : currentSchedule.Where(x => x.DayOfWeek == selectedDay).ToList();

            dgMainSchedule.ItemsSource = filtered.Where(x => !x.HasReplacement).ToList();
            dgReplacements.ItemsSource = filtered.Where(x => x.HasReplacement).ToList();

            string dayName = cmbDay.SelectedItem is ComboBoxItem item ? item.Content.ToString() ?? "" : "";
            txtStatus.Text = $"{dayName}: " +
                             $"{filtered.Count(x => !x.HasReplacement)} пар, " +
                             $"{filtered.Count(x => x.HasReplacement)} замен";
        }

        private int GetSelectedDay()
        {
            if (cmbDay.SelectedItem is ComboBoxItem item && item.Tag != null)
                return int.Parse(item.Tag.ToString()!);
            return 0;
        }

        private void CmbDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentSchedule != null && currentSchedule.Count > 0)
                ApplyDayFilter();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
    }
}