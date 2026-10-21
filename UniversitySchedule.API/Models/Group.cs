namespace UniversitySchedule.API.Models
{
    public class Group
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int Course { get; set; }
        public string Specialty { get; set; } = string.Empty;
    }
}