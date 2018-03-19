using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Util
{
    public sealed class TimeManager : SingletonBehavior<TimeManager>
    {
        public readonly DateTime Epoch = new DateTime(1970, 1, 1);

        public static long SecondsToMilliseconds(long seconds)
        {
            return seconds * 1000;
        }

        [SerializeField]
        private int _offsetSeconds = 0;

        public long CurrentUnixSeconds => (long)DateTime.UtcNow.Subtract(Epoch).TotalSeconds + _offsetSeconds;

        public long CurrentUnixMs => (long)DateTime.UtcNow.Subtract(Epoch).TotalMilliseconds + SecondsToMilliseconds(_offsetSeconds);
    }
}
