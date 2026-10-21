namespace UniversitySchedule.API.Models
{
    public class Teacher
    {
        public int TeacherID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Role { get; set; } = "teacher";
    }
}