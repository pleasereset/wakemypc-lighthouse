using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ree7.WakeMyPC.ProbeServer.Commands
{
	public abstract class CommandBase
	{
		/// <summary>
		/// Returns the string triggering the command (ex :  SHUTDOWN)
		/// for basic Check() implementation
		/// </summary>
		/// <returns></returns>
		protected virtual string GetName()
		{
			throw new NotImplementedException();
		}

		public virtual bool Check(string receivedText)
		{
			return receivedText.StartsWith(GetName());
		}

		public abstract bool Execute(string receivedText);
	}
}
