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
    [Route("api/classrooms")]
    public class ClassroomsController : ControllerBase
    {
        private readonly ILogger<ClassroomsController> _logger;
        private readonly IConfiguration _configuration;

        public ClassroomsController(ILogger<ClassroomsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public IActionResult CreateClassroom([FromBody] Classroom classroom)
        {
            try
            {
                string query = "INSERT INTO Classrooms (ClassroomName) " +
                               "VALUES (@ClassroomName)";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClassroomName", classroom.ClassroomName);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Classroom created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a classroom.");
                return StatusCode(500, "An error occurred while creating the classroom.");
            }
        }

        [HttpGet("getAll")]
        public IActionResult GetAllClassrooms()
        {
            try
            {
                List<Classroom> classrooms = new List<Classroom>();

                string query = "SELECT * FROM Classrooms";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Classroom classroom = new Classroom
                                {
                                    ClassroomID = Convert.ToInt32(reader["ClassroomID"]),
                                    ClassroomName = reader["ClassroomName"].ToString()
                                };

                                classrooms.Add(classroom);
                            }
                        }
                    }
                }

                return Ok(classrooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving classrooms.");
                return StatusCode(500, "An error occurred while retrieving classrooms.");
            }
        }
    }
}
