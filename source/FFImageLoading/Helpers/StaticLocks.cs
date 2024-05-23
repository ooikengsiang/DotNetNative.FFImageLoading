using System;
using System.Threading;

namespace FFImageLoading.Helpers
{
	public static class StaticLocks
	{
		public static SemaphoreSlim DecodingLock { get; set; }
	}
}
