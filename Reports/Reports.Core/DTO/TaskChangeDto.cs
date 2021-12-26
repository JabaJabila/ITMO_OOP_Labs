namespace Core.DTO
{
    public class TaskChangeDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string AuthorId { get; set; }
        public string ChangeTime { get; set; }
        public string ChangeInfo { get; set; }
    }
}