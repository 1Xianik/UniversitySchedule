using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class FreeClassroomsWindow : Window
    {
        public FreeClassroomsWindow()
        {
            InitializeComponent();
            if (cmbDay.Items.Count > 0) cmbDay.SelectedIndex = 0;
            if (cmbPair.Items.Count > 0) cmbPair.SelectedIndex = 0;
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDay.SelectedItem is not ComboBoxItem dayItem ||
                cmbPair.SelectedItem is not ComboBoxItem pairItem)
            {
                MessageBox.Show("Выберите день и пару.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int day = int.Parse(dayItem.Tag.ToString()!);
                int pair = int.Parse(pairItem.Tag.ToString()!);

                // Запрос к API для поиска свободных аудиторий
                var freeRooms = await AppServices.Api.GetAsync<List<Classroom>>(
                    $"api/schedule/free-classrooms?dayOfWeek={day}&pairNumber={pair}");

                dgFreeRooms.ItemsSource = freeRooms;

                if (freeRooms == null || freeRooms.Count == 0)
                {
                    MessageBox.Show("Свободных аудиторий на выбранное время не найдено.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}