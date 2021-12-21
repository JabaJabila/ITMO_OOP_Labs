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
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        public async Task<Employee> Create([FromQuery] string name)
        {
            return await _service.CreateEmployee(name);
        }

        [HttpGet("get-one")]
        public async Task<IActionResult> Find([FromQuery] Guid id, [FromQuery] string name)
        {
            Employee result;
            if (id != Guid.Empty)
            {
                result = await _service.GetById(id);
                if (result != null)
                    return Ok(result);
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(name)) return StatusCode((int) HttpStatusCode.BadRequest);
            result = await _service.FindByName(name);
            if (result != null)
                return Ok(result);
            
            return NotFound();
        }
        
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            if (id == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            try
            {
                Employee result = await _service.Delete(id);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> SetSupervisor(
            [FromQuery] Guid currentEmployeeId,
            [FromQuery] Guid supervisorId)
        {
            if (currentEmployeeId == Guid.Empty || supervisorId == Guid.Empty)
                return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                Employee result = await _service.SetSupervisor(currentEmployeeId, supervisorId);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
    }
}