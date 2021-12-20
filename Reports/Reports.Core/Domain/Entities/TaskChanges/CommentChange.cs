using System;

namespace Core.Domain.Entities.TaskChanges
{
    public class CommentChange : JobTaskChange
    {
        public Employee Author { get; private init; }
        public string Message { get; private init; }

        private CommentChange()
        {
        }

        public CommentChange(Employee author, string message)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}