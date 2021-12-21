using System;
using System.Globalization;
using System.Linq;
using Core.Domain.Entities;
using Core.Domain.Tools;
using Core.DTO;

namespace Core.Mappers
{
    public class MyReportToDtoMapper : IReportMapper
    {
        public ReportDto Map(Report entity)
        {
            if (entity == null) return new ReportDto();
            var dto = new ReportDto
            {
                Id = entity.Id.ToString(),
                Type = Enum.GetName(typeof(ReportType), entity.Type),
                CreationTime = entity.CreationTime.ToString(CultureInfo.InvariantCulture),
                AssignedEmployeeId = entity.AssignedEmployee.Id.ToString(),
                Description = entity.Description,
                State = Enum.GetName(typeof(ReportState), entity.State),
                TasksIds = entity.Tasks.Select(t => t.Id.ToString()).ToList()
            };

            return dto;
        }
    }
}