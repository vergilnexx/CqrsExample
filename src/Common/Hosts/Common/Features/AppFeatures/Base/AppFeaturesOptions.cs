﻿using System.Collections.Generic;

namespace Meta.Common.Hosts.Features.AppFeatures.Base
{
    /// <summary>
    /// Настройки функциональностей.
    /// </summary>
    public class AppFeaturesOptions
    {
        /// <summary>
        /// Функциональности.
        /// </summary>
        public Dictionary<string, AppFeatureOptions> Features { get; set; } = [];
    }
}
