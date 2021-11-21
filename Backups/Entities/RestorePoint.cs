using System;
using System.Collections.Generic;
using System.Linq;

namespace Backups.Entities
{
    public class RestorePoint
    {
        private readonly List<Storage> _storages;
        public RestorePoint(IReadOnlyCollection<Storage> storages, DateTime? dateTime = null)
        {
            _storages = storages.ToList();
            CreationTime = dateTime ?? DateTime.Now;
            Id = Guid.NewGuid();
        }

        public RestorePoint(Storage storage, DateTime? dateTime = null)
        {
            _storages = new List<Storage>(new[] { storage });
            CreationTime = dateTime ?? DateTime.Now;
            Id = Guid.NewGuid();
        }

        public IReadOnlyList<Storage> Storages => _storages;
        public DateTime CreationTime { get; }
        public Guid Id { get; }

        public void AddStorage(Storage storage)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            if (!_storages.Contains(storage))
                _storages.Add(storage);
        }
    }
}