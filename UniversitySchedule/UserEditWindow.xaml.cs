using System;
using System.Windows;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class UserEditWindow : Window
    {
        private Teacher? existingTeacher;

        public UserEditWindow(Teacher? teacher)
        {
            InitializeComponent();
            existingTeacher = teacher;

            if (teacher != null)
            {
                txtHeader.Text = "Редактирование пользователя";
                txtFullName.Text = teacher.FullName;
                txtDepartment.Text = teacher.Department;
                txtPosition.Text = teacher.Position;
                txtLogin.Text = teacher.Login;

                foreach (System.Windows.Controls.ComboBoxItem item in cmbRole.Items)
                    if (item.Tag?.ToString() == teacher.Role)
                    {
                        item.IsSelected = true;
                        break;
                    }
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string role = (cmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Tag?.ToString() ?? "teacher";

            try
            {
                if (existingTeacher == null)
                {
                    // Создание
                    if (string.IsNullOrWhiteSpace(txtPassword.Password))
                    {
                        MessageBox.Show("Введите пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var dto = new CreateTeacherDto
                    {
                        FullName = txtFullName.Text.Trim(),
                        Department = txtDepartment.Text.Trim(),
                        Position = txtPosition.Text.Trim(),
                        Login = txtLogin.Text.Trim(),
                        Password = txtPassword.Password,
                        Role = role
                    };

                    var created = await AppServices.Api.PostAsync<CreateTeacherDto, Teacher>("api/teachers", dto);
                    if (created != null)
                    {
                        MessageBox.Show("Пользователь создан.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось создать.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Обновление
                    var dto = new UpdateTeacherDto
                    {
                        FullName = txtFullName.Text.Trim(),
                        Department = txtDepartment.Text.Trim(),
                        Position = txtPosition.Text.Trim(),
                        Login = txtLogin.Text.Trim(),
                        Password = string.IsNullOrWhiteSpace(txtPassword.Password) ? null : txtPassword.Password,
                        Role = role
                    };

                    bool ok = await AppServices.Api.PutAsync($"api/teachers/{existingTeacher.TeacherID}", dto);
                    if (ok)
                    {
                        MessageBox.Show("Сохранено.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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