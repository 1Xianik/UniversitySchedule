namespace UniversitySchedule.API.Models
{
    public class CreateClassroomDto
    {
        public string RoomNumber { get; set; } = string.Empty;
        public string? Building { get; set; }
        public int? Capacity { get; set; }
    }
}