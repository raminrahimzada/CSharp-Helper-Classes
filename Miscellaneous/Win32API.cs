using System;
using System.Runtime.InteropServices;
using System.Text;

// class to wrap up Windows 32 API constants and functions.
public class Win32API
{
	[StructLayout(LayoutKind.Sequential)]
		public struct OSVersionInfo
	{
		public int OSVersionInfoSize;
		public int majorVersion;
		public int minorVersion;
		public int buildNumber;
		public int platformId;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
		public string versionstring;
	}

	[StructLayout(LayoutKind.Sequential)]
		public struct SECURITY_ATTRIBUTES
	{
		public int nLength;
		public int lpSecurityDescriptor;
		public int bInheritHandle;
	}

    public const int GWL_EXSTYLE = (-20);
    public const int GW_OWNER = 4;
    public const int SW_RESTORE = 9;
    public const int SW_SHOW = 5;
    public const int WS_EX_TOOLWINDOW = 0x80;
    public const int WS_EX_APPWINDOW = 0x40000;

	[DllImport("kernel32.dll", EntryPoint="CreateDirectoryA")]
	public static extern bool CreateDirectory(string lpPathName, SECURITY_ATTRIBUTES lpSecurityAttribut);

    public delegate bool EnumWindowsCallback(int hWnd, int lParam) ;

	[DllImport("user32.dll", EntryPoint="EnumWindows")]
	public static extern int EnumWindows(EnumWindowsCallback callback, int lParam);
    
    [DllImport("user32.dll", EntryPoint="EnumWindows", SetLastError=true,
    CharSet=CharSet.Ansi, ExactSpelling=true,
    CallingConvention=CallingConvention.StdCall)]
    public static extern int EnumWindowsDllImport(EnumWindowsCallback callback, int lParam);

	[DllImport( "user32.dll", EntryPoint="FindWindow", CharSet=CharSet.Auto )]
	public static extern int FindWindow(string lpClassName, string lpWindowName);
    
	[DllImport( "user32.dll", EntryPoint="FindWindow", CharSet=CharSet.Auto )]
	public static extern int FindWindowAny(int lpClassName, int lpWindowName);

	[DllImport( "user32.dll", EntryPoint="FindWindow", CharSet=CharSet.Auto )]
	public static extern int FindWindowNullClassName(int lpClassName, string lpWindowName);

	[DllImport( "user32.dll", EntryPoint="FindWindow", CharSet=CharSet.Auto )]
	public static extern int FindWindowNullWindowCaption(string lpClassName, int lpWindowName);

	[DllImport( "user32.dll", EntryPoint="GetClassNameA" )]
	public static extern int GetClassName(int hwnd, StringBuilder lpClassName, int cch);
    
	[DllImport( "kernel32.dll", EntryPoint="GetDiskFreeSpaceA" )]
	public static extern int GetDiskFreeSpace(string lpRootPathName,
												ref int lpSectorsPerCluster,
												ref int lpBytesPerSector,
												ref int lpNumberOfFreeClusters,
												ref int lpTotalNumberOfClusters);

	[DllImport( "kernel32.dll", EntryPoint="GetDiskFreeSpaceExA" )]
	[CLSCompliant(false)]
	public static extern int GetDiskFreeSpaceEx(string lpRootPathName,
													ref int lpFreeBytesAvailableToCaller,
													ref int lpTotalNumberOfBytes,
													ref UInt32 lpTotalNumberOfFreeBytes);

	[DllImport( "kernel32.dll", EntryPoint="GetDriveTypeA" )]
	public static extern int GetDriveType(string nDrive);

	[DllImport( "user32.dll", EntryPoint="GetParent" )]
	public static extern int GetParent(int hwnd);
    
	[DllImport( "Kernel32.dll", EntryPoint="GetVersionExA", CharSet=CharSet.Ansi )]
	public static extern bool GetVersionEx(ref OSVersionInfo osvi);
    
	[DllImport( "user32.dll", EntryPoint="GetWindow" )]
	public static extern int GetWindow(int hwnd, int wCmd);
    
	[DllImport( "user32.dll", EntryPoint="GetWindowLongA" )]
	public static extern int GetWindowLong(int hwnd, int nIndex);
    
	[DllImport( "user32.dll", EntryPoint="GetWindowTextA" )]
	public static extern void GetWindowText(int hWnd, StringBuilder lpstring, int nMaxCount);
    
	[DllImport( "user32.dll", EntryPoint="IsIconic" )]
	public static extern bool IsIconic(int hwnd);
    
	[DllImport( "Powrprof.dll", EntryPoint="IsPwrHibernateAllowed" )]
	public static extern int IsPwrHibernateAllowed();
    
	[DllImport( "user32.dll", EntryPoint="IsWindowVisible" )]
	public static extern bool IsWindowVisible(int hwnd);
    
	[DllImport( "user32.dll", EntryPoint="SetForegroundWindow" )]
	public static extern int SetForegroundWindow(int hwnd);
    
	[DllImport( "Powrprof.dll", EntryPoint="SetSuspendState" )]
	public static extern int SetSuspendState(int Hibernate, int ForceCritical, int DisableWakeEvent);
    
	[DllImport( "user32.dll", EntryPoint="ShowWindow" )]
	public static extern int ShowWindow(int hwnd, int nCmdShow);
    
	[DllImport( "user32.dll", EntryPoint="SwapMouseButton" )]
	public static extern int SwapMouseButton(int bSwap);
  
}

