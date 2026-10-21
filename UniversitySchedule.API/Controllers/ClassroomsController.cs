using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySchedule.API.Models;
using UniversitySchedule.API.Services;

namespace UniversitySchedule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassroomsController : ControllerBase
    {
        private readonly DatabaseService _db;
        public ClassroomsController(DatabaseService db) => _db = db;

        [HttpGet]
        public ActionResult<List<Classroom>> GetAll() => Ok(_db.GetClassrooms());

        [HttpGet("{id}")]
        public ActionResult<Classroom> GetById(int id)
        {
            var c = _db.GetClassroomById(id);
            if (c == null)
                return NotFound($"Аудитория с ID={id} не найдена");
            return Ok(c);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult<Classroom> Create([FromBody] CreateClassroomDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoomNumber))
                return BadRequest("Номер аудитории обязателен");
            if (dto.Capacity.HasValue && dto.Capacity.Value < 0)
                return BadRequest("Вместимость не может быть отрицательной");

            try
            {
                int newId = _db.CreateClassroom(dto);
                if (newId == 0)
                    return StatusCode(500, "Не удалось создать аудиторию");

                var created = _db.GetClassroomById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id, [FromBody] CreateClassroomDto dto)
        {
            var existing = _db.GetClassroomById(id);
            if (existing == null)
                return NotFound($"Аудитория с ID={id} не найдена");

            if (string.IsNullOrWhiteSpace(dto.RoomNumber))
                return BadRequest("Номер аудитории обязателен");

            try
            {
                if (!_db.UpdateClassroom(id, dto))
                    return StatusCode(500, "Не удалось обновить аудиторию");
                return Ok(_db.GetClassroomById(id));
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var existing = _db.GetClassroomById(id);
            if (existing == null)
                return NotFound($"Аудитория с ID={id} не найдена");

            try
            {
                if (!_db.DeleteClassroom(id))
                    return BadRequest("Не удалось удалить (возможно, есть связанные записи в расписании)");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }
    }
}