using System;
using System.Net;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.ServicesAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Presentation.Controllers
{
    [ApiController]
    [Route("/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<Employee> Create([FromQuery] string name)
        {
            return await _service.CreateEmployee(name);
        }

        [HttpGet]
        public async Task<IActionResult> Find([FromQuery] string name, [FromQuery] Guid id)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Employee result = await _service.FindByName(name);
                if (result != null)
                {
                    return Ok(result);
                }

                return NotFound();
            }

            if (id == Guid.Empty) return StatusCode((int) HttpStatusCode.BadRequest);
            {
                Employee result = await _service.GetById(id);
                if (result != null)
                {
                    return Ok(result);
                }

                return NotFound();
            }

        }
    }
}