using System;
using System.Globalization;
using System.Linq;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.DTO;

namespace Core.Mappers
{
    public class MyJobTaskToDtoMapper : IJobTaskMapper
    {
        private readonly ITaskChangeMapper _changeMapper;

        public MyJobTaskToDtoMapper(ITaskChangeMapper changeMapper)
        {
            _changeMapper = changeMapper ?? throw new ArgumentNullException(nameof(changeMapper));
        }

        public JobTaskDto Map(JobTask entity)
        {
            if (entity == null) return new JobTaskDto();
            var dto = new JobTaskDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                CreationTime = entity.CreationTime.ToString(CultureInfo.InvariantCulture),
                CurrentState = Enum.GetName(typeof(JobTaskState), entity.CurrentState),
                Description = entity.Description,
                AssignedEmployeeId = entity.AssignedEmployee.Id.ToString(),
                Changes = entity.Changes.Select(c => _changeMapper.Map(c)).ToList()
            };

            return dto;
        }
    }
}