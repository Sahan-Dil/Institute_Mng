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
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly ILogger<SubjectsController> _logger;
        private readonly IConfiguration _configuration;

        public SubjectsController(ILogger<SubjectsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public IActionResult CreateSubject([FromBody] Subject subject)
        {
            try
            {
                string query = "INSERT INTO Subjects (SubjectName) " +
                               "VALUES (@SubjectName)";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectName", subject.SubjectName);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Subject created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a subject.");
                return StatusCode(500, "An error occurred while creating the subject.");
            }
        }

        [HttpGet("getAll")]
        public IActionResult GetAllSubjects()
        {
            try
            {
                List<Subject> subjects = new List<Subject>();

                string query = "SELECT * FROM Subjects";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Subject subject = new Subject
                                {
                                    SubjectID = Convert.ToInt32(reader["SubjectID"]),
                                    SubjectName = reader["SubjectName"].ToString()
                                };

                                subjects.Add(subject);
                            }
                        }
                    }
                }

                return Ok(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving subjects.");
                return StatusCode(500, "An error occurred while retrieving subjects.");
            }
        }
    }
}
