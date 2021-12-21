using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Tools;

namespace Core.Domain.ServicesAbstractions
{
    public interface IReportService
    {
        Task<IReadOnlyCollection<Report>> GetReports(Employee employee);
        Task<Report> GetById(Guid id);
        Task<Report> CreateReport(Employee employee, ReportType type);
        Task<Report> AddTaskToReport(Report report, JobTask task);
        Task<Report> SetDescription(Report report, string description);
        Task<Report> ChangeState(Report report, ReportState state);
        Task<IReadOnlyCollection<Report>> GetSubordinatesReportsForPeriod(Employee employee, uint countOfDays);
        Task<IReadOnlyCollection<Employee>> GetSubordinatesWithoutReportsForPeriod(Employee employee, uint countOfDays);
    }
}