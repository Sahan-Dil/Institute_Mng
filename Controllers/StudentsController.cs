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
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IConfiguration _configuration;

        public StudentsController(ILogger<StudentsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public IActionResult CreateStudent([FromBody] Student student)
        {
            try
            {
                string query = "INSERT INTO Students (FirstName, LastName, ContactPerson, ContactNo, EmailAddress, DateOfBirth, Age, Classroom) " +
               "VALUES (@FirstName, @LastName, @ContactPerson, @ContactNo, @EmailAddress, @DateOfBirth, @Age, @Classroom)";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", student.FirstName);
                        command.Parameters.AddWithValue("@LastName", student.LastName);
                        command.Parameters.AddWithValue("@ContactPerson", student.ContactPerson);
                        command.Parameters.AddWithValue("@ContactNo", student.ContactNo);
                        command.Parameters.AddWithValue("@EmailAddress", student.EmailAddress);
                        command.Parameters.AddWithValue("@DateOfBirth", Convert.ToDateTime(student.DateOfBirth));
                        command.Parameters.AddWithValue("@Age", student.Age);
                        command.Parameters.AddWithValue("@Classroom", student.Classroom);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Student created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a student.");
                return StatusCode(500, "An error occurred while creating the student.");
            }
        }

        [HttpDelete("delete/{email}")]
        public IActionResult DeleteStudent(string email)
        {
            try
            {
                string query = "DELETE FROM Students WHERE EmailAddress = @EmailAddress";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmailAddress", email);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Student deleted successfully.");
                        }
                        else
                        {
                            return NotFound("Student not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the student.");
                return StatusCode(500, "An error occurred while deleting the student.");
            }
        }

        [HttpGet("getAll")]
        public IActionResult GetAllStudents()
        {
            try
            {
                List<Student> students = new List<Student>();

                string query = "SELECT * FROM Students";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Student student = new Student
                                {
                                    StudentID = Convert.ToInt32(reader["StudentID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    ContactPerson = reader["ContactPerson"].ToString(),
                                    ContactNo = reader["ContactNo"].ToString(),
                                    EmailAddress = reader["EmailAddress"].ToString(),
                                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                    Age = Convert.ToInt32(reader["Age"]),
                                    Classroom = reader["Classroom"].ToString()
                                };

                                students.Add(student);
                            }
                        }
                    }
                }

                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving students.");
                return StatusCode(500, "An error occurred while retrieving students.");
            }
        }
    }
}
