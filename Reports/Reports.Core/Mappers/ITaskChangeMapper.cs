using Core.Domain.Entities.TaskChanges;
using Core.DTO;

namespace Core.Mappers
{
    public interface ITaskChangeMapper : IMapper<TaskChangeDto, JobTaskChange>
    {
    }
}