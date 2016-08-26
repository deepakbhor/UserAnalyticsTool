using System;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace UserAnalytics
{
    /// <summary>
    /// Summary description for APIFuncs.
    /// </summary>
    /// 
    
    public class APIFuncs
	{
		#region Windows API Functions Declarations
		//This Function is used to get Active Window Title...
		[System.Runtime.InteropServices.DllImport("user32.dll",CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hwnd,string lpString, int cch);

		//This Function is used to get Handle for Active Window...
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		private static extern IntPtr GetForegroundWindow();
	
		//This Function is used to get Active process ID...
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd,out Int32 lpdwProcessId);

        //This Function is used to get Active process ID...
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT lprect);

        [System.Runtime.InteropServices.DllImport("WinRobotCorex64.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr GetActiveConsoleSession(IntPtr hWnd, out IntPtr lptre);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner  
            public int Top;         // y position of upper-left corner  
            public int Right;       // x position of lower-right corner  
            public int Bottom;      // y position of lower-right corner  
        }
        #endregion
        
        #region User-defined Functions
        public static  Int32 GetWindowProcessID(IntPtr hwnd)
		{
			//This Function is used to get Active process ID...
			Int32 pid;
			GetWindowThreadProcessId(hwnd, out pid);
			return pid;
		}

        public static IntPtr getWindowRectangle(IntPtr hwnd)
        {
            RECT rect;
            return GetWindowRect(hwnd , out rect);
        }


        public static IntPtr getforegroundWindow()
		{
			//This method is used to get Handle for Active Window using GetForegroundWindow() method present in user32.dll
			return GetForegroundWindow();
		}

		public static string ActiveApplTitle()
		{
			//This method is used to get active application's title using GetWindowText() method present in user32.dll
			IntPtr hwnd =getforegroundWindow();
			if (hwnd.Equals(IntPtr.Zero)) return "";
			string lpText = new string((char) 0, 100);
			int intLength = GetWindowText(hwnd, lpText, lpText.Length);
			if ((intLength <= 0) || (intLength > lpText.Length)) return "unknown";
			return lpText.Trim();
		}
        #endregion
        public APIFuncs()
		{
		}
	}
}
