namespace UniversitySchedule.API.Models
{
    public class Classroom
    {
        public int ClassroomID { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}