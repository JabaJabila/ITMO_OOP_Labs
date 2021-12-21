using System;
using System.Globalization;
using Core.Domain.Entities.TaskChanges;
using Core.DTO;

namespace Core.Mappers
{
    public class MyTaskChangeToDtoMapper : IMapper<TaskChangeDto,JobTaskChange>
    {
        public TaskChangeDto Map(JobTaskChange entity)
        {
            if (entity == null) return new TaskChangeDto();
            var dto = new TaskChangeDto
            {
                Id = entity.Id.ToString(),
                Type = entity.GetType().Name,
                AuthorId = entity.Author.Id.ToString(),
                ChangeTime = entity.ChangeTime.ToString(CultureInfo.InvariantCulture), 
                ChangeInfo = entity.GetType().Name switch
                {
                    "AssignedEmployeeChange"
                        => $"assigned employee {((AssignedEmployeeChange) entity).NewAssignedEmployee}",
                    "DescriptionChange" => $"new description: {((DescriptionChange) entity).NewDescription}",
                    "StateChange" => $"new state: {((StateChange) entity).NewState}",
                    "CommentChange" => $"new comment: {((CommentChange) entity).Message}",
                    _ => throw new ArgumentOutOfRangeException()
                }
            };

            return dto;
        }
    }
}