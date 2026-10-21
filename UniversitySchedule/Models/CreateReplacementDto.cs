using System;

namespace UniversitySchedule.WPF.Models
{
    public class CreateReplacementDto
    {
        public int ScheduleID { get; set; }
        public int NewTeacherID { get; set; }
        public int NewClassroomID { get; set; }
        public DateTime ReplacementDate { get; set; }
        public string? Reason { get; set; }
    }
}