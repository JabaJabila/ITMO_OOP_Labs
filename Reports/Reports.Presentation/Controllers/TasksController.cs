using System;
using System.Net;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.ServicesAbstractions;
using Core.Domain.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Presentation.Controllers
{
    [ApiController]
    [Route("/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly IJobTaskService _taskService;
        private readonly IEmployeeService _employeeService;

        public TasksController(IJobTaskService taskService, IEmployeeService employeeService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }
        
        [HttpGet("get-one")]
        public async Task<IActionResult> Find(
            [FromQuery] Guid id,
            [FromQuery] string name,
            [FromQuery] DateTime? creationDateTime,
            [FromQuery] DateTime? lastChangeDateTime)
        {
            JobTask result;
            if (id != Guid.Empty)
            {
                try
                {
                    return Ok(await _taskService.GetById(id));
                }
                catch (Exception)
                {
                    return NotFound();
                }
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                result = await _taskService.FindByName(name);
                if (result != null)
                    return Ok(result);
                return NotFound();
            }

            if (creationDateTime != null)
            {
                result = await _taskService.FindByCreationTime((DateTime)creationDateTime);
                if (result != null)
                    return Ok(result);
                return NotFound();
            }
            
            if (lastChangeDateTime == null) return StatusCode((int)HttpStatusCode.BadRequest);
            result = await _taskService.FindByLastChangeTime((DateTime)lastChangeDateTime);
            if (result != null)
                return Ok(result);
            return NotFound();
        }
        
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _taskService.GetAll());
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> Create(
            [FromQuery] string name,
            [FromQuery] Guid assignedEmployeeId,
            [FromQuery] string description)
        {
            if (assignedEmployeeId == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            Employee employee = await _employeeService.GetById(assignedEmployeeId);
            if (employee == null)
                return NotFound();

            try
            {
                return Ok(await _taskService.CreateTask(name, employee, description));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }

        [HttpPatch("description")]
        public async Task<IActionResult> ChangeDescription(
            [FromQuery] Guid id,
            [FromQuery] string newDescription)
        {
            if (id == Guid.Empty)
                return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                JobTask task = await _taskService.GetById(id);
                if (task == null)
                    return NotFound();
                return Ok(await _taskService.ChangeDescription(task, newDescription));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpPatch("state")]
        public async Task<IActionResult> ChangeState(
            [FromQuery] Guid id,
            [FromQuery] JobTaskState state)
        {
            if (id == Guid.Empty)
                return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                JobTask task = await _taskService.GetById(id);
                if (task == null)
                    return NotFound();
                return Ok(await _taskService.ChangeState(task, state));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpPatch("assigned-employee")]
        public async Task<IActionResult> ChangeState(
            [FromQuery] Guid taskId,
            [FromQuery] Guid employeeId)
        {
            if (taskId == Guid.Empty || employeeId == Guid.Empty)
                return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                JobTask task = await _taskService.GetById(taskId);
                Employee employee = await _employeeService.GetById(employeeId);
                if (task == null || employee == null)
                    return NotFound();
                return Ok(await _taskService.ChangeAssignedEmployee(task, employee));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpPatch("add-comment")]
        public async Task<IActionResult> AddComment(
            [FromQuery] Guid taskId,
            [FromQuery] Guid employeeId,
            [FromQuery] string text)
        {
            if (taskId == Guid.Empty || employeeId == Guid.Empty || string.IsNullOrWhiteSpace(text))
                return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                JobTask task = await _taskService.GetById(taskId);
                Employee employee = await _employeeService.GetById(employeeId);
                if (task == null || employee == null)
                    return NotFound();
                return Ok(await _taskService.AddComment(task, employee, text));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("assigned-tasks")]
        public async Task<IActionResult> GetAssignedTasks([FromQuery] Guid employeeId)
        {
            if (employeeId == Guid.Empty) return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                Employee employee = await _employeeService.GetById(employeeId);
                if (employee == null)
                    return NotFound();
                return Ok(await _taskService.FindAssignedTasks(employee));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpGet("contributed-tasks")]
        public async Task<IActionResult> GetContributedTasks([FromQuery] Guid employeeId)
        {
            if (employeeId == Guid.Empty) return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                Employee employee = await _employeeService.GetById(employeeId);
                if (employee == null)
                    return NotFound();
                return Ok(await _taskService.FindContributedTasks(employee));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpGet("assigned-to-subordinates-tasks")]
        public async Task<IActionResult> GetAssignedToSubordinatesTasks([FromQuery] Guid employeeId)
        {
            if (employeeId == Guid.Empty) return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                Employee employee = await _employeeService.GetById(employeeId);
                if (employee == null)
                    return NotFound();
                return Ok(await _taskService.FindAssignedToSubordinatesTasks(employee));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
    }
}