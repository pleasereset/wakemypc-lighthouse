using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ree7.WakeMyPC.LighthouseCore.Commands
{
	public class ShutdownCommand : CommandBase
	{
		protected override string GetName()
		{
			return LighthouseProtocolCommands.ShutdownCommand;
		}

		public override bool Execute(string receivedText)
		{
			Utils.System.Shutdown();
			return true;
		}
	}

	public class ResetCommand : CommandBase
	{
		protected override string GetName()
		{
			return LighthouseProtocolCommands.ResetCommand;
		}

		public override bool Execute(string receivedText)
		{
			Utils.System.Reset();
			return true;
		}
	}

	public class StandbyCommand : CommandBase
	{
		protected override string GetName()
		{
			return LighthouseProtocolCommands.StandbyCommand;
		}

		public override bool Execute(string receivedText)
		{
			Utils.System.Standby();
			return true;
		}
	}

	public class SessionCloseCommand : CommandBase
	{
		protected override string GetName()
		{
			return LighthouseProtocolCommands.SessionCloseCommand;
		}

		public override bool Execute(string receivedText)
		{
			Utils.System.LogOff();
			return true;
		}
	}

	public class SessionLockCommand : CommandBase
	{
		protected override string GetName()
		{
			return LighthouseProtocolCommands.SessionLockCommand;
		}

		public override bool Execute(string receivedText)
		{
			Utils.System.LockSession();
			return true;
		}
	}
}
