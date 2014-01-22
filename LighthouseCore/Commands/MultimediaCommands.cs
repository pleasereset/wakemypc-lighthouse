using CoreAudioApi;
using ree7.WakeMyPC.LighthouseCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ree7.WakeMyPC.LighthouseCore.Commands
{
	public class MuteCommand : CommandBase
	{
		protected override string GetName()
		{
			return LighthouseProtocolCommands.MuteCommand;
		}

		public override bool Execute(string receivedText)
		{
			try
			{
				MMDeviceEnumerator devEnumerator = new MMDeviceEnumerator();
				MMDevice dev = devEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
				if(dev != null)
				{
					dev.AudioEndpointVolume.Mute = !dev.AudioEndpointVolume.Mute;
				}

				return true;
			}
			catch(Exception e)
			{
				Log.d("Error in MuteCommand : " + e.ToString());
				return false;
			}
		}
	}
}
