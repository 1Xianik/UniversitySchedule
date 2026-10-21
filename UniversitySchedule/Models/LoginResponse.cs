namespace UniversitySchedule.WPF.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int TeacherID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}