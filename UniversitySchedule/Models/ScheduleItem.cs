using System;

namespace UniversitySchedule.WPF.Models
{
    public class ScheduleItem
    {
        public int ScheduleID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string ClassroomNumber { get; set; } = string.Empty;
        public int DayOfWeek { get; set; }
        public int PairNumber { get; set; }

        // Для замен
        public string? ReplacementTeacherName { get; set; }
        public string? ReplacementClassroomNumber { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string? ReplacementReason { get; set; }

        public string DayOfWeekName => DayOfWeek switch
        {
            1 => "Понедельник",
            2 => "Вторник",
            3 => "Среда",
            4 => "Четверг",
            5 => "Пятница",
            6 => "Суббота",
            _ => "Воскресенье"
        };

        // Время пары — автоматически вычисляется по номеру
        public string PairTime => PairNumber switch
        {
            1 => "08:30 – 10:00",
            2 => "10:10 – 11:40",
            3 => "12:10 – 13:40",
            4 => "13:50 – 15:20",
            5 => "15:30 – 17:00",
            _ => "—"
        };

        // Есть ли замена
        public bool HasReplacement => !string.IsNullOrEmpty(ReplacementTeacherName)
                                   || !string.IsNullOrEmpty(ReplacementClassroomNumber);
    }
}