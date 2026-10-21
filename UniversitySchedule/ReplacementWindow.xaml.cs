using System;
using System.Collections.Generic;
using System.Windows;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class ReplacementWindow : Window
    {
        public ReplacementWindow()
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Today;
            Loaded += ReplacementWindow_Loaded;
        }

        private async void ReplacementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Загружаем всё расписание, чтобы выбрать пару для замены
                var schedule = await AppServices.Api.GetAsync<List<ScheduleItem>>("api/schedule/all");

                // Оставляем только те пары, у которых еще нет замены
                var mainSchedule = new List<ScheduleItem>();
                if (schedule != null)
                {
                    foreach (var item in schedule)
                    {
                        if (!item.HasReplacement) mainSchedule.Add(item);
                    }
                }
                dgSchedule.ItemsSource = mainSchedule;

                // 2. Загружаем преподавателей
                var teachers = await AppServices.Api.GetAsync<List<Teacher>>("api/teachers");
                cmbNewTeacher.ItemsSource = teachers;

                // 3. Загружаем аудитории
                var classrooms = await AppServices.Api.GetAsync<List<Classroom>>("api/classrooms");
                cmbNewClassroom.ItemsSource = classrooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgSchedule.SelectedItem is not ScheduleItem selected)
            {
                MessageBox.Show("Выберите занятие.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbNewTeacher.SelectedItem is not Teacher newTeacher)
            {
                MessageBox.Show("Выберите преподавателя.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbNewClassroom.SelectedItem is not Classroom newClassroom)
            {
                MessageBox.Show("Выберите аудиторию.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dto = new CreateReplacementDto
                {
                    ScheduleID = selected.ScheduleID,
                    NewTeacherID = newTeacher.TeacherID,
                    NewClassroomID = newClassroom.ClassroomID,
                    ReplacementDate = dpDate.SelectedDate.Value,
                    Reason = txtReason.Text.Trim()
                };

                // Отправляем POST-запрос в API
                bool ok = await AppServices.Api.PostAsync("api/replacements", dto);

                if (ok)
                {
                    MessageBox.Show("Замена внесена.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не удалось внести замену.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}