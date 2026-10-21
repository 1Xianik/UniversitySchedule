using Microsoft.Data.SqlClient;
using UniversitySchedule.API.Models;

namespace UniversitySchedule.API.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Строка подключения не найдена");
        }

        // ==================== ГРУППЫ ====================
        public List<Group> GetGroups()
        {
            var result = new List<Group>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT GroupID, GroupName, Course, Specialty FROM KURS_Groups ORDER BY GroupName";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Group
                {
                    GroupID = (int)reader["GroupID"],
                    GroupName = reader["GroupName"]?.ToString() ?? "",
                    Course = (int)reader["Course"],
                    Specialty = reader["Specialty"]?.ToString() ?? ""
                });
            }
            return result;
        }

        // ==================== ПРЕПОДАВАТЕЛИ ====================
        public List<Teacher> GetTeachers()
        {
            var result = new List<Teacher>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT TeacherID, FullName, Department, Position, Login, Role FROM KURS_Teachers ORDER BY FullName";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Teacher
                {
                    TeacherID = (int)reader["TeacherID"],
                    FullName = reader["FullName"]?.ToString() ?? "",
                    Department = reader["Department"]?.ToString() ?? "",
                    Position = reader["Position"]?.ToString() ?? "",
                    Login = reader["Login"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? "teacher"
                });
            }
            return result;
        }

        // ==================== АУДИТОРИИ ====================
        public List<Classroom> GetClassrooms()
        {
            var result = new List<Classroom>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT ClassroomID, RoomNumber, Building, Capacity FROM KURS_Classrooms ORDER BY RoomNumber";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Classroom
                {
                    ClassroomID = (int)reader["ClassroomID"],
                    RoomNumber = reader["RoomNumber"]?.ToString() ?? "",
                    Building = reader["Building"]?.ToString() ?? "",
                    Capacity = reader["Capacity"] != DBNull.Value ? (int)reader["Capacity"] : 0
                });
            }
            return result;
        }

        // ==================== ДИСЦИПЛИНЫ ====================
        public List<Subject> GetSubjects()
        {
            var result = new List<Subject>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT SubjectID, SubjectName, TotalHours FROM KURS_Subjects ORDER BY SubjectName";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Subject
                {
                    SubjectID = (int)reader["SubjectID"],
                    SubjectName = reader["SubjectName"]?.ToString() ?? "",
                    TotalHours = reader["TotalHours"] != DBNull.Value ? (int?)reader["TotalHours"] : null
                });
            }
            return result;
        }

        // ==================== РАСПИСАНИЕ ДЛЯ ГРУППЫ ====================
        public List<ScheduleItem> GetScheduleForGroup(int groupId)
        {
            var result = new List<ScheduleItem>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                SELECT 
                    s.ScheduleID, g.GroupName, t.FullName AS TeacherName, 
                    sub.SubjectName, c.RoomNumber AS ClassroomNumber, 
                    s.DayOfWeek, s.PairNumber,
                    rt.FullName AS ReplacementTeacherName, 
                    rc.RoomNumber AS ReplacementClassroomNumber,
                    r.ReplacementDate, r.Reason AS ReplacementReason
                FROM KURS_Schedule s
                JOIN KURS_Groups g ON s.GroupID = g.GroupID
                JOIN KURS_Teachers t ON s.TeacherID = t.TeacherID
                JOIN KURS_Subjects sub ON s.SubjectID = sub.SubjectID
                JOIN KURS_Classrooms c ON s.ClassroomID = c.ClassroomID
                LEFT JOIN KURS_Replacements r ON s.ScheduleID = r.ScheduleID 
                    AND r.ReplacementDate = CAST(GETDATE() AS DATE)
                LEFT JOIN KURS_Teachers rt ON r.NewTeacherID = rt.TeacherID
                LEFT JOIN KURS_Classrooms rc ON r.NewClassroomID = rc.ClassroomID
                WHERE s.GroupID = @GroupID
                ORDER BY s.DayOfWeek, s.PairNumber";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupID", groupId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(MapScheduleItem(reader));
            }
            return result;
        }

        // ==================== РАСПИСАНИЕ ДЛЯ ПРЕПОДАВАТЕЛЯ ====================
        public List<ScheduleItem> GetScheduleForTeacher(int teacherId)
        {
            var result = new List<ScheduleItem>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                SELECT 
                    s.ScheduleID, g.GroupName, t.FullName AS TeacherName, 
                    sub.SubjectName, c.RoomNumber AS ClassroomNumber, 
                    s.DayOfWeek, s.PairNumber,
                    rt.FullName AS ReplacementTeacherName, 
                    rc.RoomNumber AS ReplacementClassroomNumber,
                    r.ReplacementDate, r.Reason AS ReplacementReason
                FROM KURS_Schedule s
                JOIN KURS_Groups g ON s.GroupID = g.GroupID
                JOIN KURS_Teachers t ON s.TeacherID = t.TeacherID
                JOIN KURS_Subjects sub ON s.SubjectID = sub.SubjectID
                JOIN KURS_Classrooms c ON s.ClassroomID = c.ClassroomID
                LEFT JOIN KURS_Replacements r ON s.ScheduleID = r.ScheduleID 
                    AND r.ReplacementDate = CAST(GETDATE() AS DATE)
                LEFT JOIN KURS_Teachers rt ON r.NewTeacherID = rt.TeacherID
                LEFT JOIN KURS_Classrooms rc ON r.NewClassroomID = rc.ClassroomID
                WHERE s.TeacherID = @TeacherID
                ORDER BY s.DayOfWeek, s.PairNumber";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TeacherID", teacherId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(MapScheduleItem(reader));
            }
            return result;
        }

        // ==================== СВОБОДНЫЕ АУДИТОРИИ ====================
        public List<Classroom> GetFreeClassrooms(int dayOfWeek, int pairNumber)
        {
            var result = new List<Classroom>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                SELECT c.ClassroomID, c.RoomNumber, c.Building, c.Capacity
                FROM KURS_Classrooms c
                WHERE c.ClassroomID NOT IN (
                    SELECT ClassroomID FROM KURS_Schedule 
                    WHERE DayOfWeek = @Day AND PairNumber = @Pair
                )
                ORDER BY c.RoomNumber";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Day", dayOfWeek);
            command.Parameters.AddWithValue("@Pair", pairNumber);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Classroom
                {
                    ClassroomID = (int)reader["ClassroomID"],
                    RoomNumber = reader["RoomNumber"]?.ToString() ?? "",
                    Building = reader["Building"]?.ToString() ?? "",
                    Capacity = reader["Capacity"] != DBNull.Value ? (int)reader["Capacity"] : 0
                });
            }
            return result;
        }

        // ==================== АУТЕНТИФИКАЦИЯ ====================
        public Teacher? AuthenticateUser(string login, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"SELECT TeacherID, FullName, Login, Role, Department, Position
                           FROM KURS_Teachers 
                           WHERE Login = @Login AND PasswordHash = @Password";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Login", login);
            command.Parameters.AddWithValue("@Password", password);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Teacher
                {
                    TeacherID = (int)reader["TeacherID"],
                    FullName = reader["FullName"]?.ToString() ?? "",
                    Login = reader["Login"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? "teacher",
                    Department = reader["Department"]?.ToString() ?? "",
                    Position = reader["Position"]?.ToString() ?? ""
                };
            }
            return null;
        }

        // ==================== ВСПОМОГАТЕЛЬНЫЙ МЕТОД ====================
        private ScheduleItem MapScheduleItem(SqlDataReader reader)
        {
            return new ScheduleItem
            {
                ScheduleID = (int)reader["ScheduleID"],
                GroupName = reader["GroupName"]?.ToString() ?? "",
                TeacherName = reader["TeacherName"]?.ToString() ?? "",
                SubjectName = reader["SubjectName"]?.ToString() ?? "",
                ClassroomNumber = reader["ClassroomNumber"]?.ToString() ?? "",
                DayOfWeek = (int)reader["DayOfWeek"],
                PairNumber = (int)reader["PairNumber"],
                ReplacementTeacherName = reader["ReplacementTeacherName"] != DBNull.Value
                    ? reader["ReplacementTeacherName"].ToString() : null,
                ReplacementClassroomNumber = reader["ReplacementClassroomNumber"] != DBNull.Value
                    ? reader["ReplacementClassroomNumber"].ToString() : null,
                ReplacementDate = reader["ReplacementDate"] != DBNull.Value
                    ? (DateTime?)reader["ReplacementDate"] : null,
                ReplacementReason = reader["ReplacementReason"] != DBNull.Value
                    ? reader["ReplacementReason"].ToString() : null
            };
        }
        // ==================== REPLACEMENTS: READ ====================
        public List<Replacement> GetAllReplacements()
        {
            var result = new List<Replacement>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        SELECT r.ReplacementID, r.ScheduleID, r.NewTeacherID, 
               t.FullName AS NewTeacherName,
               r.NewClassroomID, c.RoomNumber AS NewClassroomNumber,
               r.ReplacementDate, r.Reason
        FROM KURS_Replacements r
        JOIN KURS_Teachers t ON r.NewTeacherID = t.TeacherID
        JOIN KURS_Classrooms c ON r.NewClassroomID = c.ClassroomID
        ORDER BY r.ReplacementDate DESC, r.ReplacementID DESC";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(MapReplacement(reader));
            }
            return result;
        }

        public Replacement? GetReplacementById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        SELECT r.ReplacementID, r.ScheduleID, r.NewTeacherID, 
               t.FullName AS NewTeacherName,
               r.NewClassroomID, c.RoomNumber AS NewClassroomNumber,
               r.ReplacementDate, r.Reason
        FROM KURS_Replacements r
        JOIN KURS_Teachers t ON r.NewTeacherID = t.TeacherID
        JOIN KURS_Classrooms c ON r.NewClassroomID = c.ClassroomID
        WHERE r.ReplacementID = @ID";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
                return MapReplacement(reader);
            return null;
        }

        // ==================== REPLACEMENTS: CREATE ====================
        public int CreateReplacement(CreateReplacementDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        INSERT INTO KURS_Replacements (ScheduleID, NewTeacherID, NewClassroomID, ReplacementDate, Reason)
        OUTPUT INSERTED.ReplacementID
        VALUES (@ScheduleID, @NewTeacherID, @NewClassroomID, @ReplacementDate, @Reason)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ScheduleID", dto.ScheduleID);
            command.Parameters.AddWithValue("@NewTeacherID", dto.NewTeacherID);
            command.Parameters.AddWithValue("@NewClassroomID", dto.NewClassroomID);
            command.Parameters.AddWithValue("@ReplacementDate", dto.ReplacementDate);
            command.Parameters.AddWithValue("@Reason", string.IsNullOrEmpty(dto.Reason)
                ? (object)DBNull.Value : dto.Reason);

            var result = command.ExecuteScalar();
            return result != null ? (int)result : 0;
        }

        // ==================== REPLACEMENTS: UPDATE ====================
        public bool UpdateReplacement(int id, UpdateReplacementDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        UPDATE KURS_Replacements 
        SET NewTeacherID = @NewTeacherID, 
            NewClassroomID = @NewClassroomID,
            ReplacementDate = @ReplacementDate,
            Reason = @Reason
        WHERE ReplacementID = @ID";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@NewTeacherID", dto.NewTeacherID);
            command.Parameters.AddWithValue("@NewClassroomID", dto.NewClassroomID);
            command.Parameters.AddWithValue("@ReplacementDate", dto.ReplacementDate);
            command.Parameters.AddWithValue("@Reason", string.IsNullOrEmpty(dto.Reason)
                ? (object)DBNull.Value : dto.Reason);

            return command.ExecuteNonQuery() > 0;
        }

        // ==================== REPLACEMENTS: DELETE ====================
        public bool DeleteReplacement(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "DELETE FROM KURS_Replacements WHERE ReplacementID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            return command.ExecuteNonQuery() > 0;
        }

        // ==================== GROUPS: CREATE / UPDATE / DELETE ====================
        public int CreateGroup(CreateGroupDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        INSERT INTO KURS_Groups (GroupName, Course, Specialty)
        OUTPUT INSERTED.GroupID
        VALUES (@GroupName, @Course, @Specialty)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupName", dto.GroupName);
            command.Parameters.AddWithValue("@Course", dto.Course);
            command.Parameters.AddWithValue("@Specialty", string.IsNullOrEmpty(dto.Specialty)
                ? (object)DBNull.Value : dto.Specialty);

            var result = command.ExecuteScalar();
            return result != null ? (int)result : 0;
        }

        public bool UpdateGroup(int id, CreateGroupDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        UPDATE KURS_Groups 
        SET GroupName = @GroupName, Course = @Course, Specialty = @Specialty
        WHERE GroupID = @ID";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@GroupName", dto.GroupName);
            command.Parameters.AddWithValue("@Course", dto.Course);
            command.Parameters.AddWithValue("@Specialty", string.IsNullOrEmpty(dto.Specialty)
                ? (object)DBNull.Value : dto.Specialty);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteGroup(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "DELETE FROM KURS_Groups WHERE GroupID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SqlException)
            {
                // Например, есть связанные записи в расписании
                return false;
            }
        }

        // ==================== HELPER ====================
        private Replacement MapReplacement(SqlDataReader reader)
        {
            return new Replacement
            {
                ReplacementID = (int)reader["ReplacementID"],
                ScheduleID = (int)reader["ScheduleID"],
                NewTeacherID = (int)reader["NewTeacherID"],
                NewTeacherName = reader["NewTeacherName"]?.ToString() ?? "",
                NewClassroomID = (int)reader["NewClassroomID"],
                NewClassroomNumber = reader["NewClassroomNumber"]?.ToString() ?? "",
                ReplacementDate = (DateTime)reader["ReplacementDate"],
                Reason = reader["Reason"] != DBNull.Value ? reader["Reason"].ToString() : null
            };
        }
        // ==================== TEACHERS: READ SINGLE ====================
        public Teacher? GetTeacherById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT TeacherID, FullName, Department, Position, Login, Role FROM KURS_Teachers WHERE TeacherID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Teacher
                {
                    TeacherID = (int)reader["TeacherID"],
                    FullName = reader["FullName"]?.ToString() ?? "",
                    Department = reader["Department"]?.ToString() ?? "",
                    Position = reader["Position"]?.ToString() ?? "",
                    Login = reader["Login"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? "teacher"
                };
            }
            return null;
        }

        // Проверка уникальности логина
        public bool LoginExists(string login, int excludeId = 0)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT COUNT(*) FROM KURS_Teachers WHERE Login = @Login AND TeacherID <> @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Login", login);
            command.Parameters.AddWithValue("@ID", excludeId);
            return (int)command.ExecuteScalar() > 0;
        }

        // ==================== TEACHERS: CREATE ====================
        public int CreateTeacher(CreateTeacherDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        INSERT INTO KURS_Teachers (FullName, Department, Position, Login, PasswordHash, Role)
        OUTPUT INSERTED.TeacherID
        VALUES (@FullName, @Department, @Position, @Login, @Password, @Role)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FullName", dto.FullName);
            command.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(dto.Department) ? (object)DBNull.Value : dto.Department);
            command.Parameters.AddWithValue("@Position", string.IsNullOrEmpty(dto.Position) ? (object)DBNull.Value : dto.Position);
            command.Parameters.AddWithValue("@Login", dto.Login);
            command.Parameters.AddWithValue("@Password", dto.Password);
            command.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(dto.Role) ? "teacher" : dto.Role);

            var result = command.ExecuteScalar();
            return result != null ? (int)result : 0;
        }

        // ==================== TEACHERS: UPDATE ====================
        public bool UpdateTeacher(int id, UpdateTeacherDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Если пароль пустой — обновляем без пароля
            string sql;
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                sql = @"
            UPDATE KURS_Teachers 
            SET FullName = @FullName, Department = @Department, Position = @Position,
                Login = @Login, Role = @Role
            WHERE TeacherID = @ID";
            }
            else
            {
                sql = @"
            UPDATE KURS_Teachers 
            SET FullName = @FullName, Department = @Department, Position = @Position,
                Login = @Login, PasswordHash = @Password, Role = @Role
            WHERE TeacherID = @ID";
            }

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@FullName", dto.FullName);
            command.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(dto.Department) ? (object)DBNull.Value : dto.Department);
            command.Parameters.AddWithValue("@Position", string.IsNullOrEmpty(dto.Position) ? (object)DBNull.Value : dto.Position);
            command.Parameters.AddWithValue("@Login", dto.Login);
            command.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(dto.Role) ? "teacher" : dto.Role);
            if (!string.IsNullOrWhiteSpace(dto.Password))
                command.Parameters.AddWithValue("@Password", dto.Password);

            return command.ExecuteNonQuery() > 0;
        }

        // ==================== TEACHERS: DELETE ====================
        public bool DeleteTeacher(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "DELETE FROM KURS_Teachers WHERE TeacherID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SqlException)
            {
                // Есть связанные записи в расписании, заменах и т.п.
                return false;
            }
        }

        // ==================== CLASSROOMS: READ SINGLE ====================
        public Classroom? GetClassroomById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT ClassroomID, RoomNumber, Building, Capacity FROM KURS_Classrooms WHERE ClassroomID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Classroom
                {
                    ClassroomID = (int)reader["ClassroomID"],
                    RoomNumber = reader["RoomNumber"]?.ToString() ?? "",
                    Building = reader["Building"]?.ToString() ?? "",
                    Capacity = reader["Capacity"] != DBNull.Value ? (int)reader["Capacity"] : 0
                };
            }
            return null;
        }

        // ==================== CLASSROOMS: CREATE ====================
        public int CreateClassroom(CreateClassroomDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        INSERT INTO KURS_Classrooms (RoomNumber, Building, Capacity)
        OUTPUT INSERTED.ClassroomID
        VALUES (@RoomNumber, @Building, @Capacity)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@RoomNumber", dto.RoomNumber);
            command.Parameters.AddWithValue("@Building", string.IsNullOrEmpty(dto.Building) ? (object)DBNull.Value : dto.Building);
            command.Parameters.AddWithValue("@Capacity", dto.Capacity.HasValue ? dto.Capacity.Value : (object)DBNull.Value);

            var result = command.ExecuteScalar();
            return result != null ? (int)result : 0;
        }

        // ==================== CLASSROOMS: UPDATE ====================
        public bool UpdateClassroom(int id, CreateClassroomDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        UPDATE KURS_Classrooms 
        SET RoomNumber = @RoomNumber, Building = @Building, Capacity = @Capacity
        WHERE ClassroomID = @ID";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@RoomNumber", dto.RoomNumber);
            command.Parameters.AddWithValue("@Building", string.IsNullOrEmpty(dto.Building) ? (object)DBNull.Value : dto.Building);
            command.Parameters.AddWithValue("@Capacity", dto.Capacity.HasValue ? dto.Capacity.Value : (object)DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }

        // ==================== CLASSROOMS: DELETE ====================
        public bool DeleteClassroom(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "DELETE FROM KURS_Classrooms WHERE ClassroomID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        // ==================== SUBJECTS: READ SINGLE ====================
        public Subject? GetSubjectById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT SubjectID, SubjectName, TotalHours FROM KURS_Subjects WHERE SubjectID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Subject
                {
                    SubjectID = (int)reader["SubjectID"],
                    SubjectName = reader["SubjectName"]?.ToString() ?? "",
                    TotalHours = reader["TotalHours"] != DBNull.Value ? (int?)reader["TotalHours"] : null
                };
            }
            return null;
        }

        // ==================== SUBJECTS: CREATE ====================
        public int CreateSubject(CreateSubjectDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        INSERT INTO KURS_Subjects (SubjectName, TotalHours)
        OUTPUT INSERTED.SubjectID
        VALUES (@SubjectName, @TotalHours)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@SubjectName", dto.SubjectName);
            command.Parameters.AddWithValue("@TotalHours", dto.TotalHours.HasValue ? dto.TotalHours.Value : (object)DBNull.Value);

            var result = command.ExecuteScalar();
            return result != null ? (int)result : 0;
        }

        // ==================== SUBJECTS: UPDATE ====================
        public bool UpdateSubject(int id, CreateSubjectDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        UPDATE KURS_Subjects 
        SET SubjectName = @SubjectName, TotalHours = @TotalHours
        WHERE SubjectID = @ID";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@SubjectName", dto.SubjectName);
            command.Parameters.AddWithValue("@TotalHours", dto.TotalHours.HasValue ? dto.TotalHours.Value : (object)DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }

        // ==================== SUBJECTS: DELETE ====================
        public bool DeleteSubject(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "DELETE FROM KURS_Subjects WHERE SubjectID = @ID";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID", id);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SqlException)
            {
                return false;
            }
        }
        public List<ScheduleItem> GetAllSchedule()
        {
            var result = new List<ScheduleItem>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        SELECT 
            s.ScheduleID, g.GroupName, t.FullName AS TeacherName, 
            sub.SubjectName, c.RoomNumber AS ClassroomNumber, 
            s.DayOfWeek, s.PairNumber,
            rt.FullName AS ReplacementTeacherName, 
            rc.RoomNumber AS ReplacementClassroomNumber,
            r.ReplacementDate, r.Reason AS ReplacementReason
        FROM KURS_Schedule s
        JOIN KURS_Groups g ON s.GroupID = g.GroupID
        JOIN KURS_Teachers t ON s.TeacherID = t.TeacherID
        JOIN KURS_Subjects sub ON s.SubjectID = sub.SubjectID
        JOIN KURS_Classrooms c ON s.ClassroomID = c.ClassroomID
        LEFT JOIN KURS_Replacements r ON s.ScheduleID = r.ScheduleID 
            AND r.ReplacementDate = CAST(GETDATE() AS DATE)
        LEFT JOIN KURS_Teachers rt ON r.NewTeacherID = rt.TeacherID
        LEFT JOIN KURS_Classrooms rc ON r.NewClassroomID = rc.ClassroomID
        ORDER BY s.DayOfWeek, s.PairNumber, g.GroupName";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
                result.Add(MapScheduleItem(reader));
            return result;
        }
        public List<TeacherWorkload> GetTeacherWorkload()
        {
            var result = new List<TeacherWorkload>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
        SELECT t.FullName, COUNT(s.ScheduleID) AS PairCount
        FROM KURS_Teachers t
        LEFT JOIN KURS_Schedule s ON t.TeacherID = s.TeacherID
        GROUP BY t.FullName
        ORDER BY PairCount DESC";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new TeacherWorkload
                {
                    TeacherName = reader["FullName"]?.ToString() ?? "",
                    PairCount = (int)reader["PairCount"]
                });
            }
            return result;
        }
    }
}