namespace UniversitySchedule.API.Models
{
    public class UpdateReplacementDto
    {
        public int NewTeacherID { get; set; }
        public int NewClassroomID { get; set; }
        public DateTime ReplacementDate { get; set; }
        public string? Reason { get; set; }
    }
}