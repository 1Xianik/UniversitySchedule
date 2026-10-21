using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySchedule.API.Models;
using UniversitySchedule.API.Services;

namespace UniversitySchedule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly DatabaseService _db;
        public GroupsController(DatabaseService db) => _db = db;

        /// <summary>
        /// Получить список всех групп (публичный)
        /// </summary>
        [HttpGet]
        public ActionResult<List<Group>> GetAll() => Ok(_db.GetGroups());

        /// <summary>
        /// Создать группу (только админ)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult<Group> Create([FromBody] CreateGroupDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.GroupName))
                return BadRequest("Название группы обязательно");
            if (dto.Course < 1 || dto.Course > 5)
                return BadRequest("Курс должен быть от 1 до 5");

            try
            {
                int newId = _db.CreateGroup(dto);
                if (newId == 0)
                    return StatusCode(500, "Не удалось создать группу");

                var created = new Group
                {
                    GroupID = newId,
                    GroupName = dto.GroupName,
                    Course = dto.Course,
                    Specialty = dto.Specialty ?? ""
                };
                return CreatedAtAction(nameof(GetAll), new { id = newId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить группу (только админ)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id, [FromBody] CreateGroupDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.GroupName))
                return BadRequest("Название группы обязательно");

            try
            {
                if (!_db.UpdateGroup(id, dto))
                    return NotFound($"Группа с ID={id} не найдена");
                return Ok("Группа обновлена");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить группу (только админ)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (!_db.DeleteGroup(id))
                    return NotFound($"Группа с ID={id} не найдена");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Вернет красивый текст ошибки
            }
        }
    }
}