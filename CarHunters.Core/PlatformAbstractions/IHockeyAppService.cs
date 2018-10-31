﻿using System;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IHockeyAppService
    {
		void LogError(Exception e);
	}
}
