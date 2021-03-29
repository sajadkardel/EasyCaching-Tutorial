using System;
using EasyCaching.Core;

namespace InMemoryProvider.Services
{
    public class DateTimeService : IDateTimeService
    {
        private readonly IEasyCachingProvider _easyCachingProvider;

        public DateTimeService(IEasyCachingProvider easyCachingProvider)
        {
            _easyCachingProvider = easyCachingProvider;
        }

        public int NowSecond()
        {
            var cache = _easyCachingProvider.Get<int>($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            if (cache.HasValue)
            {
                return cache.Value;
            }

            var data = DateTime.Now.Second;
            _easyCachingProvider.Set($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}", data, TimeSpan.FromMinutes(1));
            return data;
        }
    }
}
