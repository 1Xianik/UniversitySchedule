namespace UniversitySchedule.API.Models
{
    public class CreateGroupDto
    {
        public string GroupName { get; set; } = string.Empty;
        public int Course { get; set; }
        public string? Specialty { get; set; }
    }
}