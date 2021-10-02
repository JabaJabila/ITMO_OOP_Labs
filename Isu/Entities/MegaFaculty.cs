using System;
using System.Collections.Generic;

namespace Isu.Entities
{
    public class MegaFaculty
    {
        private readonly List<Faculty> _faculties;

        internal MegaFaculty(string name, int id)
        {
            Name = name ?? throw new ArgumentNullException(
                nameof(name),
                $"{nameof(name)} can't be null!");

            Id = id;
            _faculties = new List<Faculty>();
        }

        public string Name { get; }
        public int Id { get; }
        public IReadOnlyCollection<Faculty> Faculties => _faculties;

        internal void AddFacultyToMegaFaculty(Faculty faculty)
        {
            if (faculty == null)
                throw new ArgumentNullException(nameof(faculty), $"{nameof(faculty)} can't be null!");

            _faculties.Add(faculty);
        }
    }
}