namespace UniversitySchedule.API.Models
{
    public class Replacement
    {
        public int ReplacementID { get; set; }
        public int ScheduleID { get; set; }
        public int NewTeacherID { get; set; }
        public string NewTeacherName { get; set; } = string.Empty;
        public int NewClassroomID { get; set; }
        public string NewClassroomNumber { get; set; } = string.Empty;
        public DateTime ReplacementDate { get; set; }
        public string? Reason { get; set; }
    }
}