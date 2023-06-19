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
    [Route("api/allocatedSubjects")]
    public class AllocatedSubjectsController : ControllerBase
    {
        private readonly ILogger<AllocatedSubjectsController> _logger;
        private readonly IConfiguration _configuration;

        public AllocatedSubjectsController(ILogger<AllocatedSubjectsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public IActionResult CreateAllocatedSubject([FromBody] AllocatedSubject allocatedSubject)
        {
            try
            {
                string query = "INSERT INTO AllocatedSubjects (Teacher, Subject) " +
                               "VALUES (@Teacher, @Subject)";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Teacher", allocatedSubject.Teacher);
                        command.Parameters.AddWithValue("@Subject", allocatedSubject.Subject);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Allocated subject created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an allocated subject.");
                return StatusCode(500, "An error occurred while creating the allocated subject.");
            }
        }

        [HttpGet("getAll")]
        public IActionResult GetAllAllocatedSubjects()
        {
            try
            {
                List<AllocatedSubject> allocatedSubjects = new List<AllocatedSubject>();

                string query = "SELECT * FROM AllocatedSubjects";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AllocatedSubject allocatedSubject = new AllocatedSubject
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Teacher = reader["Teacher"].ToString(),
                                    Subject = reader["Subject"].ToString()
                                };

                                allocatedSubjects.Add(allocatedSubject);
                            }
                        }
                    }
                }

                return Ok(allocatedSubjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving allocated subjects.");
                return StatusCode(500, "An error occurred while retrieving allocated subjects.");
            }
        }
    }
}
