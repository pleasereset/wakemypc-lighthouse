using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ree7.WakeMyPC.LighthouseCore
{
	/// <summary>
	/// Lighthouse Protocol v1.0
	/// List of the string commands supported by the protocol. 
	/// - Adds do not change the protocol version.
	/// - Editing the syntax of a previously used command changes the protocol version.
	/// - Changing the protocol behavior changes the protocol version.
	/// </summary>
	public static class LighthouseProtocolCommands
	{
		public const string V = "1.0";

		public const string PasswordCommand = "PASSWORD";

		public const string ShutdownCommand = "SHUTDOWN";
		public const string ResetCommand = "RESET";
		public const string StandbyCommand = "STANDBY";
		public const string SessionLockCommand = "WIN_SESSION_LOCK";
		public const string SessionCloseCommand = "WIN_SESSION_CLOSE";

		public const string MuteCommand = "VOL_MUTE";
	}
}
