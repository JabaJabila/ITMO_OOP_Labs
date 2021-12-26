using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.ServicesAbstractions;
using Core.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Presentation.Controllers
{
    [ApiController]
    [Route("/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly IEmployeeMapper _mapper;

        public EmployeeController(IEmployeeService service, IEmployeeMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string name)
        {
            try
            {
                return Ok(_mapper.Map(await _service.CreateEmployee(name)));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("get-one")]
        public async Task<IActionResult> Find([FromQuery] Guid id, [FromQuery] string name)
        {
            Employee result;
            if (id != Guid.Empty)
            {
                result = await _service.GetById(id);
                if (result != null)
                    return Ok(_mapper.Map(result));
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(name)) return StatusCode((int) HttpStatusCode.BadRequest);
            result = await _service.FindByName(name);
            if (result != null)
                return Ok(_mapper.Map(result));
            
            return NotFound();
        }
        
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok((await _service.GetAll()).Select(e => _mapper.Map(e)).ToList());
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
                return Ok(_mapper.Map(result));
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
                return Ok(_mapper.Map(result));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
    }
}