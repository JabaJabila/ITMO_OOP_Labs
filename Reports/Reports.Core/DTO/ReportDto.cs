using System.Collections.Generic;

namespace Core.DTO
{
    public class ReportDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string CreationTime { get; set; }
        public string AssignedEmployeeId { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public IReadOnlyCollection<string> TasksIds { get; set; }
    }
}