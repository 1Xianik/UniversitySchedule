using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySchedule.API.Models;
using UniversitySchedule.API.Services;

namespace UniversitySchedule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // любой авторизованный может читать
    public class ReplacementsController : ControllerBase
    {
        private readonly DatabaseService _db;
        public ReplacementsController(DatabaseService db) => _db = db;

        /// <summary>
        /// Получить все замены
        /// </summary>
        [HttpGet]
        public ActionResult<List<Replacement>> GetAll()
            => Ok(_db.GetAllReplacements());

        /// <summary>
        /// Получить замену по ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Replacement> GetById(int id)
        {
            var replacement = _db.GetReplacementById(id);
            if (replacement == null)
                return NotFound($"Замена с ID={id} не найдена");
            return Ok(replacement);
        }

        /// <summary>
        /// Создать замену (только админ)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult<Replacement> Create([FromBody] CreateReplacementDto dto)
        {
            if (dto.ScheduleID <= 0 || dto.NewTeacherID <= 0 || dto.NewClassroomID <= 0)
                return BadRequest("Некорректные данные: проверьте ScheduleID, NewTeacherID, NewClassroomID");
            if (dto.ReplacementDate == default)
                return BadRequest("Укажите дату замены");

            try
            {
                int newId = _db.CreateReplacement(dto);
                if (newId == 0)
                    return StatusCode(500, "Не удалось создать замену");

                var created = _db.GetReplacementById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить замену (только админ)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult<Replacement> Update(int id, [FromBody] UpdateReplacementDto dto)
        {
            var existing = _db.GetReplacementById(id);
            if (existing == null)
                return NotFound($"Замена с ID={id} не найдена");

            if (dto.NewTeacherID <= 0 || dto.NewClassroomID <= 0)
                return BadRequest("Некорректные данные: проверьте NewTeacherID, NewClassroomID");
            if (dto.ReplacementDate == default)
                return BadRequest("Укажите дату замены");

            try
            {
                if (!_db.UpdateReplacement(id, dto))
                    return StatusCode(500, "Не удалось обновить замену");

                var updated = _db.GetReplacementById(id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить замену (только админ)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var existing = _db.GetReplacementById(id);
            if (existing == null)
                return NotFound($"Замена с ID={id} не найдена");

            try
            {
                if (!_db.DeleteReplacement(id))
                    return StatusCode(500, "Не удалось удалить замену");

                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении: {ex.Message}");
            }
        }
    }
}