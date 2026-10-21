using System;
using System.Windows;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnOk.IsEnabled = false;
            try
            {
                var request = new LoginRequest { Login = login, Password = password };
                var response = await AppServices.Api.PostAsync<LoginRequest, LoginResponse>(
                    "api/auth/login", request);

                if (response == null || string.IsNullOrEmpty(response.Token))
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохраняем токен в ApiClient
                AppServices.Api.SetToken(response.Token);

                // Формируем User
                var user = new User
                {
                    TeacherID = response.TeacherID,
                    FullName = response.FullName,
                    Role = response.Role,
                    Token = response.Token
                };

                // Открываем нужное окно
                if (user.Role == "admin")
                    new AdminWindow(user).Show();
                else
                    new TeacherWindow(user).Show();

                Close();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnOk.IsEnabled = true;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}