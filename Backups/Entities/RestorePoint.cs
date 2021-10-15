using System;
using System.Collections.Generic;
using System.Linq;

namespace Backups.Entities
{
    public class RestorePoint
    {
        private readonly List<Storage> _storages;
        public RestorePoint(IEnumerable<Storage> storages)
        {
            _storages = storages.ToList();
            CreationTime = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public RestorePoint(Storage storage)
        {
            _storages = new List<Storage>(new[] { storage });
            CreationTime = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public RestorePoint(Storage storage, DateTime dateTime)
        {
            _storages = new List<Storage>(new[] { storage });
            CreationTime = dateTime;
            Id = Guid.NewGuid();
        }

        public IReadOnlyList<Storage> Storages => _storages;
        public DateTime CreationTime { get; }
        public Guid Id { get; }
    }
}