using System;
using System.Globalization;
using System.Linq;
using Core.Domain.Entities;
using Core.Domain.Entities.TaskChanges;
using Core.Domain.Tools;
using Core.DTO;

namespace Core.Mappers
{
    public class MyJobTaskToDtoMapper : IMapper<JobTaskDto, JobTask>
    {
        private readonly IMapper<TaskChangeDto, JobTaskChange> _changeMapper;

        public MyJobTaskToDtoMapper(IMapper<TaskChangeDto, JobTaskChange> changeMapper)
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