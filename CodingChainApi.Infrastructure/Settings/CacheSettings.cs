﻿namespace CodingChainApi.Infrastructure.Settings
{
    public interface ICacheSettings
    {
        public int ParticipationSecondDuration { get; set; }
    }
    public class CacheSettings : ICacheSettings
    {
        public int ParticipationSecondDuration { get; set; }
    }
}