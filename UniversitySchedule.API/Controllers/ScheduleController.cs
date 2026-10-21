using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySchedule.API.Models;
using UniversitySchedule.API.Services;

namespace UniversitySchedule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // <-- ВСЕ endpoints требуют токен
    public class ScheduleController : ControllerBase
    {
        private readonly DatabaseService _db;
        public ScheduleController(DatabaseService db) => _db = db;

        [HttpGet("group/{groupId}")]
        [AllowAnonymous]
        public ActionResult<List<ScheduleItem>> GetForGroup(int groupId)
            => Ok(_db.GetScheduleForGroup(groupId));

        [HttpGet("teacher/{teacherId}")]
        public ActionResult<List<ScheduleItem>> GetForTeacher(int teacherId)
            => Ok(_db.GetScheduleForTeacher(teacherId));

        [HttpGet("free-classrooms")]
        public ActionResult<List<Classroom>> GetFreeClassrooms(
            [FromQuery] int dayOfWeek, [FromQuery] int pairNumber)
        {
            if (dayOfWeek < 1 || dayOfWeek > 6)
                return BadRequest("dayOfWeek должен быть от 1 до 6");
            if (pairNumber < 1 || pairNumber > 5)
                return BadRequest("pairNumber должен быть от 1 до 5");

            return Ok(_db.GetFreeClassrooms(dayOfWeek, pairNumber));
        }
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public ActionResult<List<ScheduleItem>> GetAll()
    => Ok(_db.GetAllSchedule());
        [HttpGet("workload")]
        [Authorize(Roles = "admin")] // Отчет только для админа
        public ActionResult<List<TeacherWorkload>> GetWorkload()
    => Ok(_db.GetTeacherWorkload());
    }
}