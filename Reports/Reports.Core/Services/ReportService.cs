using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.ServicesAbstractions;
using Core.Domain.Tools;

namespace Core.Services
{
    public class ReportService : IReportService
    {
        public Task<List<Report>> GetReports(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<Report> CreateReport(Employee employee, ReportType type)
        {
            throw new NotImplementedException();
        }

        public Task AddTaskToReport(Report report, JobTask task)
        {
            throw new NotImplementedException();
        }

        public Task SetDescription(Report report, string description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeState(Report report, ReportState state)
        {
            throw new NotImplementedException();
        }

        public Task<List<Report>> GetSubordinatesReportsForPeriod(Employee employee, TimeSpan period)
        {
            throw new NotImplementedException();
        }

        public Task<List<Employee>> GetSubordinatesWithoutReportsForPeriod(Employee employee, TimeSpan period)
        {
            throw new NotImplementedException();
        }
    }
}