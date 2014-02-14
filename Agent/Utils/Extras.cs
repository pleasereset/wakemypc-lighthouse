using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Input;

namespace ree7.WakeMyPC.Agent.Utils
{
	public class IconExtractor
	{

		public static Icon Extract(string file, int number, bool largeIcon)
		{
			IntPtr large;
			IntPtr small;
			ExtractIconEx(file, number, out large, out small, 1);
			try
			{
				return Icon.FromHandle(largeIcon ? large : small);
			}
			catch
			{
				return null;
			}

		}
		[DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

	}

	namespace Converters
	{
		public class BusyStateBoolCursorConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if (value is bool)
				{
					bool b = (bool)value;
					return b ? Cursors.Wait : Cursors.Arrow;
				}
				else
				{
					throw new ArgumentException();
				}
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}
