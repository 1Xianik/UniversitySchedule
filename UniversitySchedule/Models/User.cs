namespace UniversitySchedule.WPF.Models
{
    public class User
    {
        public int TeacherID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Role { get; set; } = "teacher";
        public string Token { get; set; } = string.Empty;
    }
}