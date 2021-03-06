using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.ServicesAbstractions;
using Core.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Presentation.Controllers
{
    [ApiController]
    [Route("/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportsService;
        private readonly IEmployeeService _employeeService;
        private readonly IJobTaskService _taskService;
        private readonly IReportMapper _reportMapper;
        private readonly IEmployeeMapper _employeeMapper;

        public ReportsController(
            IReportService reportService,
            IEmployeeService employeeService,
            IJobTaskService taskService,
            IReportMapper reportMapper,
            IEmployeeMapper employeeMapper)
        {
            _reportsService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _reportMapper = reportMapper ?? throw new ArgumentNullException(nameof(reportMapper));
            _employeeMapper = employeeMapper ?? throw new ArgumentNullException(nameof(employeeMapper));
        }

        [HttpGet("from-employee")]
        public async Task<IActionResult> GetReports(Guid employeeId)
        {
            if (employeeId == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                Employee employee = await _employeeService.GetById(employeeId);
                if (employee == null)
                    return NotFound();
                return Ok((await _reportsService.GetReports(employee))
                    .Select(r => _reportMapper.Map(r))
                    .ToList());
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
        }
        
        [HttpGet("by-id")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            try
            {
                return Ok(_reportMapper.Map(await _reportsService.GetById(id)));
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromQuery] Guid assignedEmployeeId,
            [FromQuery] ReportType type)
        {
            if (assignedEmployeeId == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            Employee employee = await _employeeService.GetById(assignedEmployeeId);
            if (employee == null)
                return NotFound();

            try
            {
                return Ok(_reportMapper.Map(await _reportsService.CreateReport(employee, type)));
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
                Report report = await _reportsService.GetById(id);
                if (report == null)
                    return NotFound();
                return Ok(_reportMapper.Map(await _reportsService.SetDescription(report, newDescription)));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpPatch("state")]
        public async Task<IActionResult> ChangeState(
            [FromQuery] Guid id,
            [FromQuery] ReportState state)
        {
            if (id == Guid.Empty)
                return StatusCode((int) HttpStatusCode.BadRequest);

            try
            {
                Report report = await _reportsService.GetById(id);
                if (report == null)
                    return NotFound();
                return Ok(_reportMapper.Map(await _reportsService.ChangeState(report, state)));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpPatch("tasks")]
        public async Task<IActionResult> ChangeDescription(
            [FromQuery] Guid reportId,
            [FromQuery] Guid taskId)
        {
            if (reportId == Guid.Empty || taskId == Guid.Empty) 
                return StatusCode((int) HttpStatusCode.BadRequest);
            try
            {
                Report report = await _reportsService.GetById(reportId);
                JobTask task = await _taskService.GetById(taskId);
                if (report == null || task == null)
                    return NotFound();
                return Ok(_reportMapper.Map(await _reportsService.AddTaskToReport(report, task)));
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("get-subordinates-reports")]
        public async Task<IActionResult> GetSubordinatesReports(
            [FromQuery] Guid employeeId,
            [FromQuery] uint countOfDays)
        {
            if (employeeId == Guid.Empty) 
                return StatusCode((int) HttpStatusCode.BadRequest);
            try
            {
                Employee employee = await _employeeService.GetById(employeeId);
                if (employee == null)
                    return NotFound();
                return Ok((await _reportsService.GetSubordinatesReportsForPeriod(employee, countOfDays))
                    .Select(r => _reportMapper.Map(r))
                    .ToList());
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
        
        [HttpGet("get-subordinates-without-reports")]
        public async Task<IActionResult> GetSubordinatesWithoutReports(
            [FromQuery] Guid employeeId,
            [FromQuery] uint countOfDays)
        {
            if (employeeId == Guid.Empty) 
                return StatusCode((int) HttpStatusCode.BadRequest);
            try
            {
                Employee employee = await _employeeService.GetById(employeeId);
                if (employee == null)
                    return NotFound();
                return Ok((await _reportsService.GetSubordinatesWithoutReportsForPeriod(employee, countOfDays))
                    .Select(e => _employeeMapper.Map(e))
                    .ToList());
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }
        }
    }
}