namespace UniversitySchedule.WPF.Models
{
    public class Teacher
    {
        public int TeacherID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Role { get; set; } = "teacher";
        public string Login { get; set; } = string.Empty;

        // Отображаемое название роли
        public string RoleDisplay => Role == "admin" ? "Администратор" : "Преподаватель";
    }
}