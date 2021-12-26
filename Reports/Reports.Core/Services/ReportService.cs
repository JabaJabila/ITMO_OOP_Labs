using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.Enums;
using Core.Domain.ServicesAbstractions;
using Core.Domain.Tools;
using Core.RepositoryAbstractions;

namespace Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ReportService(IReportRepository reportRepository, IEmployeeRepository employeeRepository)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }
        
        public async Task<IReadOnlyCollection<Report>> GetReports(Employee employee)
        {
            return (await _reportRepository.GetAll())
                .Where(r => r.AssignedEmployee.Id == employee.Id)
                .ToList();
        }

        public async Task<Report> GetById(Guid id)
        {
            return await _reportRepository.GetById(id);
        }

        public async Task<Report> CreateReport(Employee employee, ReportType type)
        {
            var report = new Report(employee, type);
            return await _reportRepository.Add(report);
        }

        public async Task<Report> AddTaskToReport(Report report, JobTask task)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (task == null) throw new ArgumentNullException(nameof(task));
            
            report.Tasks.Add(task);
            await _reportRepository.SaveChanges();
            return report;
        }

        public async Task<Report> SetDescription(Report report, string description)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (description == null) throw new ArgumentNullException(nameof(description));

            report.Description = description;
            await _reportRepository.SaveChanges();
            return report;
        }

        public async Task<Report> ChangeState(Report report, ReportState state)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            report.State = state;
            await _reportRepository.SaveChanges();
            return report;
        }

        public async Task<IReadOnlyCollection<Report>> GetSubordinatesReportsForPeriod(
            Employee employee,
            uint countOfDays)
        {
            if (countOfDays == 0) throw new ReportsAppException("Count of days must be > 0!");
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            return (await _reportRepository.GetAll())
                .Where(r => r.State == ReportState.Finished &&
                            r.AssignedEmployee.SupervisorId == employee.Id &&
                            DateTime.Now <= r.CreationTime.AddDays(countOfDays))
                .ToList();
        }

        public async Task<IReadOnlyCollection<Employee>> GetSubordinatesWithoutReportsForPeriod(
            Employee employee,
            uint countOfDays)
        {
            if (countOfDays == 0) throw new ReportsAppException("Count of days must be > 0!");
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            
            var subordinatesWithReport = (await _reportRepository.GetAll())
                .Where(r => r.State == ReportState.Finished &&
                            r.AssignedEmployee.SupervisorId == employee.Id &&
                            DateTime.Now <= r.CreationTime.AddDays(countOfDays))
                .Select(r => r.AssignedEmployee)
                .ToList();

            var allSubordinates = (await _employeeRepository.GetAll())
                .Where(e => e.SupervisorId == employee.Id)
                .ToList();

            return allSubordinates.Except(subordinatesWithReport).ToList();
        }
    }
}