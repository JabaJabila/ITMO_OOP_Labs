using System;

namespace Core.Domain.Tools.TaskChanges
{
    public class DescriptionChange : JobTaskChange
    {
        public string NewDescription { get; }

        public DescriptionChange(string newDescription)
        {
            NewDescription = newDescription ?? throw new ArgumentNullException(nameof(newDescription));
        }
    }
}