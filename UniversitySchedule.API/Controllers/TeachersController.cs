using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySchedule.API.Models;
using UniversitySchedule.API.Services;

namespace UniversitySchedule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly DatabaseService _db;
        public TeachersController(DatabaseService db) => _db = db;

        /// <summary>
        /// Список всех преподавателей (публичный)
        /// </summary>
        [HttpGet]
        public ActionResult<List<Teacher>> GetAll() => Ok(_db.GetTeachers());

        /// <summary>
        /// Преподаватель по ID (публичный)
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Teacher> GetById(int id)
        {
            var teacher = _db.GetTeacherById(id);
            if (teacher == null)
                return NotFound($"Преподаватель с ID={id} не найден");
            return Ok(teacher);
        }

        /// <summary>
        /// Создать преподавателя (только админ)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult<Teacher> Create([FromBody] CreateTeacherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest("ФИО обязательно");
            if (string.IsNullOrWhiteSpace(dto.Login))
                return BadRequest("Логин обязателен");
            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Пароль обязателен");
            if (dto.Role != "admin" && dto.Role != "teacher")
                return BadRequest("Роль должна быть 'admin' или 'teacher'");

            if (_db.LoginExists(dto.Login))
                return Conflict($"Логин '{dto.Login}' уже занят");

            try
            {
                int newId = _db.CreateTeacher(dto);
                if (newId == 0)
                    return StatusCode(500, "Не удалось создать преподавателя");

                var created = _db.GetTeacherById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить преподавателя (только админ)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult<Teacher> Update(int id, [FromBody] UpdateTeacherDto dto)
        {
            var existing = _db.GetTeacherById(id);
            if (existing == null)
                return NotFound($"Преподаватель с ID={id} не найден");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest("ФИО обязательно");
            if (string.IsNullOrWhiteSpace(dto.Login))
                return BadRequest("Логин обязателен");
            if (dto.Role != "admin" && dto.Role != "teacher")
                return BadRequest("Роль должна быть 'admin' или 'teacher'");

            if (_db.LoginExists(dto.Login, id))
                return Conflict($"Логин '{dto.Login}' уже занят другим пользователем");

            try
            {
                if (!_db.UpdateTeacher(id, dto))
                    return StatusCode(500, "Не удалось обновить преподавателя");

                var updated = _db.GetTeacherById(id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить преподавателя (только админ)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var existing = _db.GetTeacherById(id);
            if (existing == null)
                return NotFound($"Преподаватель с ID={id} не найден");

            try
            {
                if (!_db.DeleteTeacher(id))
                    return BadRequest("Не удалось удалить (возможно, есть связанные записи в расписании или заменах)");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }
    }
}