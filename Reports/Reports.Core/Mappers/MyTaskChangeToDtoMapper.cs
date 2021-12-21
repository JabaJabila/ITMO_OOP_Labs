using System;
using System.Globalization;
using Core.Domain.Entities.TaskChanges;
using Core.Domain.Tools;
using Core.DTO;

namespace Core.Mappers
{
    public class MyTaskChangeToDtoMapper : ITaskChangeMapper
    {
        public TaskChangeDto Map(JobTaskChange entity)
        {
            if (entity == null) return new TaskChangeDto();
            var dto = new TaskChangeDto();
            dto.Id = entity.Id.ToString();
            dto.Type = entity.GetType().Name;
            dto.AuthorId = entity.Author.Id.ToString();
            dto.ChangeTime = entity.ChangeTime.ToString(CultureInfo.InvariantCulture);
            dto.ChangeInfo = entity.GetType().Name switch
            {
                "AssignedEmployeeChange"
                    => $"assigned employee {((AssignedEmployeeChange) entity).NewAssignedEmployee.Id.ToString()}",
                "DescriptionChange" => $"new description: {((DescriptionChange) entity).NewDescription}",
                "StateChange"
                    => $"new state: {Enum.GetName(typeof(JobTaskState), ((StateChange) entity).NewState)}",
                "CommentChange" => $"new comment: {((CommentChange) entity).Message}",
                _ => throw new ArgumentOutOfRangeException()
            };

            return dto;
        }
    }
}