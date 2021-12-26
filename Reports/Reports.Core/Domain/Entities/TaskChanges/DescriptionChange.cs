using System;

namespace Core.Domain.Entities.TaskChanges
{
    public class DescriptionChange : JobTaskChange
    {
        public string NewDescription { get; private init; }

        private DescriptionChange()
        {
        }
        
        public DescriptionChange(Employee author, string newDescription)
            : base(author)
        {
            NewDescription = newDescription ?? throw new ArgumentNullException(nameof(newDescription));
        }
    }
}