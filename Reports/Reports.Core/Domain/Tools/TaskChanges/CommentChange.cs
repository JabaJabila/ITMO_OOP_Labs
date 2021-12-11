using System;
using Core.Domain.Entities;

namespace Core.Domain.Tools.TaskChanges
{
    public class CommentChange : JobTaskChange
    {
        public Employee Author { get; }
        public string Message { get; }

        public CommentChange(Employee author, string message)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}