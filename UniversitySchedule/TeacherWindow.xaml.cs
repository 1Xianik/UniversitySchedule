using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class TeacherWindow : Window
    {
        private User currentUser;

        public TeacherWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            txtGreeting.Text = $"Здравствуйте, {currentUser.FullName}!";
            Loaded += TeacherWindow_Loaded;
        }

        private async void TeacherWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Запрос к API для получения расписания преподавателя
                var schedule = await AppServices.Api.GetAsync<List<ScheduleItem>>(
                    $"api/schedule/teacher/{currentUser.TeacherID}");

                int today = (int)DateTime.Now.DayOfWeek;
                if (today == 0) today = 7;

                var todaySchedule = schedule?
                    .Where(x => x.DayOfWeek == today)
                    .ToList() ?? new List<ScheduleItem>();

                dgTeacherSchedule.ItemsSource = todaySchedule;

                if (todaySchedule.Count == 0)
                {
                    MessageBox.Show("На сегодня занятий нет.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}