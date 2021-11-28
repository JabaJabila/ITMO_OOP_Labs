using Backups.Entities;

namespace BackupsExtra.Extensions
{
    public static class JobObjectExtensions
    {
        public static string JobObjectInfo(this JobObject jobObject)
        {
            return $"job object at: {jobObject.FullName}";
        }
    }
}