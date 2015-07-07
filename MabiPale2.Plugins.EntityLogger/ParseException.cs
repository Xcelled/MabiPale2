using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MabiPale2.Shared;

namespace MabiPale2.Plugins.EntityLogger
{
	class ParseException : Exception
	{
		public ParseException(Exception problem, Packet cause)
		{
			Cause = cause;
			Problem = problem;
		}

		public Exception Problem { get; private set; }
		public Packet Cause { get; private set; }

		public override string ToString()
		{
			Cause.Rewind();

			return string.Format(@"There has been a problem handling the following packet:

{0}

The problem was:
{1}", Cause, Problem);
		}
	}
}
