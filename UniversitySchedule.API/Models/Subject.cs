namespace UniversitySchedule.API.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int? TotalHours { get; set; }
    }
}