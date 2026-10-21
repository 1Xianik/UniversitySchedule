namespace UniversitySchedule.API.Models
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
        public string? ReplacementTeacherName { get; set; }
        public string? ReplacementClassroomNumber { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string? ReplacementReason { get; set; }
    }
}