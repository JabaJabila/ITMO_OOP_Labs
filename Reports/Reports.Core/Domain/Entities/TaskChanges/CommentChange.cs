using System;

namespace Core.Domain.Entities.TaskChanges
{
    public class CommentChange : JobTaskChange
    {
        public string Message { get; private init; }

        private CommentChange()
        {
        }

        public CommentChange(Employee author, string message)
            : base(author)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}