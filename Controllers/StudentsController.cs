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
                string query = "INSERT INTO Students (FirstName, LastName, ContactPerson, ContactNo, EmailAddress, DateOfBirth, Age) " +
                               "VALUES (@FirstName, @LastName, @ContactPerson, @ContactNo, @EmailAddress, @DateOfBirth, @Age)";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", student.FirstName);
                        command.Parameters.AddWithValue("@LastName", student.LastName);
                        command.Parameters.AddWithValue("@ContactPerson", student.ContactPerson);
                        command.Parameters.AddWithValue("@ContactNo", student.ContactNo);
                        command.Parameters.AddWithValue("@EmailAddress", student.EmailAddress);
                        command.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
                        command.Parameters.AddWithValue("@Age", student.Age);

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
                                    Age = Convert.ToInt32(reader["Age"])
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
