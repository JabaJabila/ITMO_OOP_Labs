using System;
using System.Collections.Generic;
using Backups.Entities;
using Backups.Tools;
using BackupsExtra.Algorithms;

namespace BackupsExtra.Limits
{
    public class LimiterByCount : IRestorePointLimiter
    {
        private readonly IRestorePointsCleaningAlgorithm _algorithm;

        public LimiterByCount(uint limitAmount, IRestorePointsCleaningAlgorithm algorithm)
        {
            if (limitAmount == 0)
                throw new BackupException("Impossible to set limit to store 0 points maximum!");

            LimitAmount = limitAmount;
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        public uint LimitAmount { get; }

        public void ControlRestorePoints(IReadOnlyList<RestorePoint> backupRestorePoints)
        {
            // TODO
        }
    }
}