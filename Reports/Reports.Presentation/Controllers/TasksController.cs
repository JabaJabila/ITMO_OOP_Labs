using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Reports.Presentation.Controllers
{
    [ApiController]
    [Route("/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ReportsDatabaseContext _context;

        public TasksController(ReportsDatabaseContext context) {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestModel requestModel)
        {
            string employeeId = User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            Employee employee = await _context.Employees.SingleAsync(x => x.Id == Guid.Parse(employeeId));
            EntityEntry<TaskModel> task = await _context.Tasks.AddAsync(new TaskModel
            {
                Id = Guid.NewGuid(),
                AssignedEmployee = employee
            });
            await _context.SaveChangesAsync();

            return Ok(task.Entity);
        }
    }

    public class CreateTaskRequestModel
    {
        public string Name { get; set; }
        public int Priority { get; set; }
    }
}