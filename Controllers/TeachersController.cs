using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Institute_Mng.Models;


namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly ILogger<TeachersController> _logger;
        private readonly IConfiguration _configuration;

        public TeachersController(ILogger<TeachersController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public IActionResult CreateTeacher([FromBody] Teacher teacher)
        {
            try
            {
                string query = "INSERT INTO Teachers (FirstName, LastName, ContactNo, EmailAddress, AllocatedClasses, AllocatedSubjects) " +
                               "VALUES (@FirstName, @LastName, @ContactNo, @EmailAddress, @AllocatedClasses, @AllocatedSubjects)";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", teacher.FirstName);
                        command.Parameters.AddWithValue("@LastName", teacher.LastName);
                        command.Parameters.AddWithValue("@ContactNo", teacher.ContactNo);
                        command.Parameters.AddWithValue("@EmailAddress", teacher.EmailAddress);
                        command.Parameters.AddWithValue("@AllocatedClasses", DBNull.Value);
                        command.Parameters.AddWithValue("@AllocatedSubjects", DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Teacher created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a teacher.");
                return StatusCode(500, "An error occurred while creating the teacher.");
            }
        }

        [HttpDelete("delete/{email}")]
        public IActionResult DeleteTeacher(string email)
        {
            try
            {
                string query = "DELETE FROM Teachers WHERE EmailAddress = @EmailAddress";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmailAddress", email);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Teacher deleted successfully.");
                        }
                        else
                        {
                            return NotFound("Teacher not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the teacher.");
                return StatusCode(500, "An error occurred while deleting the teacher.");
            }
        }

        [HttpGet("getAll")]
        public IActionResult GetAllTeachers()
        {
            try
            {
                List<Teacher> teachers = new List<Teacher>();

                string query = "SELECT * FROM Teachers";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Teacher teacher = new Teacher
                                {
                                    TeacherId = Convert.ToInt32(reader["TeacherId"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    ContactNo = reader["ContactNo"].ToString(),
                                    EmailAddress = reader["EmailAddress"].ToString(),
                                    AllocatedClasses = reader["AllocatedClasses"].ToString(),
                                    AllocatedSubjects = reader["AllocatedSubjects"].ToString()
                                };

                                teachers.Add(teacher);
                            }
                        }
                    }
                }

                return Ok(teachers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving teachers.");
                return StatusCode(500, "An error occurred while retrieving teachers.");
            }
        }
    }
}

  