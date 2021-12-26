using System.Collections.Generic;

namespace Core.DTO
{
    public class JobTaskDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CreationTime { get; set; }
        public string CurrentState { get; set; }
        public string Description { get; set; }
        public string AssignedEmployeeId { get; set; }
        public IReadOnlyCollection<TaskChangeDto> Changes { get; set; }
    }
}