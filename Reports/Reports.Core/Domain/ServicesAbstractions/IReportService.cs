using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Tools;

namespace Core.Domain.ServicesAbstractions
{
    public interface IReportService
    {
        Task<List<Report>> GetReports(Employee employee);
        Task<Report> CreateReport(Employee employee, ReportType type);
        Task AddTaskToReport(Report report, JobTask task);
        Task SetDescription(Report report, string description);
        Task ChangeState(Report report, ReportState state);
        Task<List<Report>> GetSubordinatesReportsForPeriod(Employee employee, TimeSpan period);
        Task<List<Employee>> GetSubordinatesWithoutReportsForPeriod(Employee employee, TimeSpan period);
    }
}