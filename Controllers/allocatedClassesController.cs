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
    [Route("api/allocatedClasses")]
    public class AllocatedClassesController : ControllerBase
    {
        private readonly ILogger<AllocatedClassesController> _logger;
        private readonly IConfiguration _configuration;

        public AllocatedClassesController(ILogger<AllocatedClassesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public IActionResult CreateAllocatedClass([FromBody] AllocatedClass allocatedClass)
        {
            try
            {
                string query = "INSERT INTO AllocatedClasses (Teacher, Class) " +
                               "VALUES (@Teacher, @Class)";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Teacher", allocatedClass.Teacher);
                        command.Parameters.AddWithValue("@Class", allocatedClass.Class);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Allocated class created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an allocated class.");
                return StatusCode(500, "An error occurred while creating the allocated class.");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteAllocatedClass(int id)
        {
            try
            {
                string query = "DELETE FROM AllocatedClasses WHERE Id = @Id";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Allocated class deleted successfully.");
                        }
                        else
                        {
                            return NotFound("Allocated class not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting an allocated class.");
                return StatusCode(500, "An error occurred while deleting the allocated class.");
            }
        }

        [HttpGet("getAll")]
        public IActionResult GetAllAllocatedClasses()
        {
            try
            {
                List<AllocatedClass> allocatedClasses = new List<AllocatedClass>();

                string query = "SELECT * FROM AllocatedClasses";

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AllocatedClass allocatedClass = new AllocatedClass
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Teacher = reader["Teacher"].ToString(),
                                    Class = reader["Class"].ToString()
                                };

                                allocatedClasses.Add(allocatedClass);
                            }
                        }
                    }
                }

                return Ok(allocatedClasses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving allocated classes.");
                return StatusCode(500, "An error occurred while retrieving allocated classes.");
            }
        }
    }
}
