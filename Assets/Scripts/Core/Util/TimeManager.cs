using System;

namespace pdxpartyparrot.Core.Util
{
    public sealed class TimeManager : SingletonBehavior<TimeManager>
    {
        public readonly DateTime Epoch = new DateTime(1970, 1, 1);

        public long CurrentUnixSeconds => (long)DateTime.UtcNow.Subtract(Epoch).TotalSeconds;

        public long CurrentUnixMs => (long)DateTime.UtcNow.Subtract(Epoch).TotalMilliseconds;
    }
}
