using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class UsersWindow : Window
    {
        public UsersWindow()
        {
            InitializeComponent();
            Loaded += UsersWindow_Loaded;
        }

        private async void UsersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();
        }

        private async System.Threading.Tasks.Task LoadUsersAsync()
        {
            try
            {
                var users = await AppServices.Api.GetAsync<List<Teacher>>("api/teachers");
                dgUsers.ItemsSource = users;
                txtStatus.Text = $"Всего пользователей: {users?.Count ?? 0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new UserEditWindow(null).ShowDialog();
            _ = LoadUsersAsync();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is not Teacher selected)
            {
                MessageBox.Show("Выберите пользователя.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            new UserEditWindow(selected).ShowDialog();
            _ = LoadUsersAsync();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is not Teacher selected)
            {
                MessageBox.Show("Выберите пользователя.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить \"{selected.FullName}\"?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                bool ok = await AppServices.Api.DeleteAsync($"api/teachers/{selected.TeacherID}");
                if (ok)
                {
                    MessageBox.Show("Удалено.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadUsersAsync();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить (возможно, есть связанные записи).", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}