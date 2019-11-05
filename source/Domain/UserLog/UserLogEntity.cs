using DotNetCore.Objects;
using DotNetCoreArchitecture.Model;
using System;

namespace DotNetCoreArchitecture.Domain
{
    public class UserLogEntity : Entity
    {
        protected internal UserLogEntity
        (
            long userId,
            LogType logType
        )
        {
            UserId = userId;
            LogType = logType;
            DateTime = DateTime.UtcNow;
        }

        public long UserId { get; private set; }

        public LogType LogType { get; private set; }

        public DateTime DateTime { get; private set; }

        public UserEntity User { get; private set; }
    }
}
