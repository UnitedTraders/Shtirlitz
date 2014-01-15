using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shtirlitz.Reporter
{
	class CurrentTimeProvider : ITimeProvider
	{
		public DateTimeOffset GetTime()
		{
			return DateTimeOffset.Now;
		}
	}
}
