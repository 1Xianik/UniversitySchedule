namespace UniversitySchedule.API.Models
{
    public class CreateSubjectDto
    {
        public string SubjectName { get; set; } = string.Empty;
        public int? TotalHours { get; set; }
    }
}