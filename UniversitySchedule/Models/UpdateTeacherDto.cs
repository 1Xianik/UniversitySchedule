namespace UniversitySchedule.WPF.Models
{
    public class UpdateTeacherDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string Login { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string Role { get; set; } = "teacher";
    }
}