using System;
using System.Runtime.InteropServices;

// This class encapsulates the calling variations of the function Beep
public class BeepCallingVariations
{
    // Declare version 

	[DllImport("kernel32.dll", EntryPoint="Beep")]
		public static extern int DeclareBeep(int dwFreq, int dwDuration);

    // DLLImport version
    [DllImport("kernel32.dll", EntryPoint="Beep")]
		public static extern int DLLImportBeep(int dwFreq, int dwDuration);

    // Specifying  Unicode
	[DllImport( "Kernel32.dll", EntryPoint="Beep", CharSet=CharSet.Unicode )]
		public static extern int UnicodeBeep(int dwFreq, int dwDuration);

    // Specifying Ansi
	[DllImport( "Kernel32.dll", EntryPoint="Beep", CharSet=CharSet.Ansi )]
		public static extern int ANSIBeep(int dwFreq, int dwDuration);
    
    // Specifying Auto
	[DllImport( "Kernel32.dll", EntryPoint="Beep", CharSet=CharSet.Auto )]
		public static extern int AutoBeep(int dwFreq, int dwDuration);
   
    // Using Exact Spelling
    // Default is FALSE
    // if FALSE an A is appended under ANSI and a W under Unicode so MessageBox becomes 
    // MessageBoxW

    [DllImport("kernel32.dll", EntryPoint="Beep", ExactSpelling=true, CharSet=CharSet.Ansi)]
		public static extern int ExactSpellingBeep(int dwFreq, int dwDuration);

}

