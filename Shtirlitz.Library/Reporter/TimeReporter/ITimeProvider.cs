using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shtirlitz.Reporter
{
	public interface ITimeProvider
	{
		DateTimeOffset GetTime();
	}
}
