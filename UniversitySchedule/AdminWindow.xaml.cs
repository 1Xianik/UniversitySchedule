using System;
using System.Collections.Generic;
using System.Windows;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class AdminWindow : Window
    {
        private User currentUser;

        public AdminWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            txtGreeting.Text = $"Здравствуйте, {user.FullName} (администратор)!";
            Loaded += AdminWindow_Loaded;
        }

        private async void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllScheduleAsync();
        }

        private async System.Threading.Tasks.Task LoadAllScheduleAsync()
        {
            try
            {
                var schedule = await AppServices.Api.GetAsync<List<ScheduleItem>>("api/schedule/all");
                dgAllSchedule.ItemsSource = schedule;
                txtStatus.Text = $"Всего записей: {schedule?.Count ?? 0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadAllScheduleAsync();
        }

        private void BtnAddReplacement_Click(object sender, RoutedEventArgs e)
        {
            var replacementWindow = new ReplacementWindow();
            if (replacementWindow.ShowDialog() == true)
            {
                // Обновить после закрытия
                _ = LoadAllScheduleAsync();
            }
        }

        private void BtnFreeClassrooms_Click(object sender, RoutedEventArgs e)
        {
            new FreeClassroomsWindow().ShowDialog();
        }

        private void BtnWorkloadReport_Click(object sender, RoutedEventArgs e)
        {
            new WorkloadReportWindow().ShowDialog();
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            new UsersWindow().ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppServices.Api.ClearToken();
                new LoginWindow().Show();
                Close();
            }
        }
    }
}