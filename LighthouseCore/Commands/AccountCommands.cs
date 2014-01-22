using ree7.WakeMyPC.LighthouseCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ree7.WakeMyPC.LighthouseCore.Commands
{
	public class PasswordCommand : CommandBase
	{
		protected override string GetName()
		{
			return LighthouseProtocolCommands.PasswordCommand;
		}

		public override bool Execute(string receivedText)
		{
			// If password command have been invoked, it means the user passed
			// the password check in Server.OnData()
			return true;
		}
	}
}
