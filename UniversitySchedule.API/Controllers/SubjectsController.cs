using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySchedule.API.Models;
using UniversitySchedule.API.Services;

namespace UniversitySchedule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly DatabaseService _db;
        public SubjectsController(DatabaseService db) => _db = db;

        [HttpGet]
        public ActionResult<List<Subject>> GetAll() => Ok(_db.GetSubjects());

        [HttpGet("{id}")]
        public ActionResult<Subject> GetById(int id)
        {
            var s = _db.GetSubjectById(id);
            if (s == null)
                return NotFound($"Дисциплина с ID={id} не найдена");
            return Ok(s);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult<Subject> Create([FromBody] CreateSubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SubjectName))
                return BadRequest("Название дисциплины обязательно");
            if (dto.TotalHours.HasValue && dto.TotalHours.Value < 0)
                return BadRequest("Количество часов не может быть отрицательным");

            try
            {
                int newId = _db.CreateSubject(dto);
                if (newId == 0)
                    return StatusCode(500, "Не удалось создать дисциплину");

                var created = _db.GetSubjectById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id, [FromBody] CreateSubjectDto dto)
        {
            var existing = _db.GetSubjectById(id);
            if (existing == null)
                return NotFound($"Дисциплина с ID={id} не найдена");

            if (string.IsNullOrWhiteSpace(dto.SubjectName))
                return BadRequest("Название дисциплины обязательно");

            try
            {
                if (!_db.UpdateSubject(id, dto))
                    return StatusCode(500, "Не удалось обновить дисциплину");
                return Ok(_db.GetSubjectById(id));
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
            var existing = _db.GetSubjectById(id);
            if (existing == null)
                return NotFound($"Дисциплина с ID={id} не найдена");

            try
            {
                if (!_db.DeleteSubject(id))
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