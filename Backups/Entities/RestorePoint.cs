using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Backups.Entities
{
    public class RestorePoint
    {
        [JsonProperty("storages")]
        private readonly List<Storage> _storages;
        public RestorePoint(IReadOnlyCollection<Storage> storages, DateTime? dateTime = null)
        {
            _storages = storages.ToList();
            CreationTime = dateTime ?? DateTime.Now;
            Id = Guid.NewGuid();
        }

        [JsonConstructor]
        private RestorePoint(Guid id, DateTime creationTime, List<Storage> storages)
        {
            Id = id;
            CreationTime = creationTime;
            _storages = storages ?? throw new ArgumentNullException(nameof(storages));
        }

        [JsonIgnore]
        public IReadOnlyList<Storage> Storages => _storages;
        public DateTime CreationTime { get; }
        public Guid Id { get; }

        public void RelocateStorage(Storage storage, RestorePoint restorePointFrom)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
            if (restorePointFrom == null)
                throw new ArgumentNullException(nameof(restorePointFrom));

            if (!restorePointFrom.Storages.Contains(storage) || _storages.Contains(storage)) return;
            _storages.Add(storage);
            restorePointFrom._storages.Remove(storage);
        }
    }
}