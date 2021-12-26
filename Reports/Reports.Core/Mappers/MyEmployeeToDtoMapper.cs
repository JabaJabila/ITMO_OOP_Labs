using System;
using Core.Domain.Entities;
using Core.Domain.Entities.Enums;
using Core.DTO;

namespace Core.Mappers
{
    public class MyEmployeeToDtoMapper : IEmployeeMapper
    {
        public EmployeeDto Map(Employee entity)
        {
            if (entity == null) return new EmployeeDto();
            var dto = new EmployeeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Status = Enum.GetName(typeof(EmployeeStatus), entity.Status),
                SupervisorId = entity.SupervisorId.ToString()
            };

            return dto;
        }
    }
}