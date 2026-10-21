using System;
using System.Collections.Generic;
using System.Windows;
using UniversitySchedule.WPF.Models;
using UniversitySchedule.WPF.Services;

namespace UniversitySchedule.WPF
{
    public partial class WorkloadReportWindow : Window
    {
        public WorkloadReportWindow()
        {
            InitializeComponent();
            Loaded += WorkloadReportWindow_Loaded;
        }

        private async void WorkloadReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var workload = await AppServices.Api.GetAsync<List<TeacherWorkload>>("api/schedule/workload");
                dgWorkload.ItemsSource = workload;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}