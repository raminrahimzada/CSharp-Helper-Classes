using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

// This class provides all the necessary support for communicating
//   with the Comm Port (otherwise known the Serial Port, or 
//   RS232 port).

public class Rs232
{
	// Declare the necessary class variables, and their initial values.		
	private int mhRS = -1;   // Handle to Com Port									;
	private int miPort = 1;   // Default is COM1	;
	private int miTimeout = 70;   // Timeout in ms;
	private int miBaudRate = 9600;
	private DataParity meParity = 0;
	private DataStopBit meStopBit = 0;
	private int miDataBit = 8;
	private int miBufferSize = 512;   // Buffers size default to 512 bytes
	private byte[] mabtRxBuf;  // Receive buffer
	private Mode meMode;  // class working mode
	private bool mbWaitOnRead;
	private bool mbWaitOnWrite;
	private bool mbWriteErr;
	private OVERLAPPED muOverlapped;
	private OVERLAPPED muOverlappedW;
	private OVERLAPPED muOverlappedE;
	private byte[] mabtTmpTxBuf;  // Temporary buffer used by Async Tx;
	private Thread moThreadTx;
	private Thread moThreadRx;
	private int miTmpBytes2Read;
	private EventMasks meMask;

	#region "Enums";

	// This enumeration provides Data Parity values.

	public enum DataParity
	{
		Parity_None,
		Paritiy_Odd,
		Parity_Even,
		Parity_Mark
	}

	// This enumeration provides Data Stop Bit values.
	//   It is set to begin with a one, so that the enumeration values
	//   match the actual values.

	public enum DataStopBit
	{

		StopBit_1 = 1,
		StopBit_2

	}

	// This enumeration contains values used to purge the various buffers.

	private enum PurgeBuffers
	{
		RXAbort = 0x02,
		RXClear = 0x08,
		TxAbort = 0x01,
		TxClear = 0x04
	}

	// This enumeration provides values for the lines sent to the Comm Port

	private enum Lines
	{

		SetRts = 3,
		ClearRts = 4,
		SetDtr = 5,
		ClearDtr = 6,
		ResetDev = 7,	//	 Reset device if possible
		SetBreak = 8,   //	 set {the device break line.
		ClearBreak = 9	//	 Clear the device break line.;
	}

	// This enumeration provides values for the Modem Status, since
	//   we'll be communicating primarily with a modem.
	// Note that the Flags() attribute is set to allow for a bitwise
	//   combination of values.

	[Flags()] public enum ModemStatusBits
	{
		ClearToSendOn = 0x10,
		DataSetReadyOn = 0x20,
		RingIndicatorOn = 0x40,
		CarrierDetect = 0x80
	}

	// This enumeration provides values for the Working mode

	public enum Mode
	{

		NonOverlapped,
		Overlapped

	}

	// This enumeration provides values for the Comm Masks used.
	// Note that the Flags() attribute is set to allow for a bitwise
	//   combination of values.

	[Flags()] public enum EventMasks
	{

		RxChar = 0x01,

		RXFlag = 0x02,

		TxBufferEmpty = 0x04,

		ClearToSend = 0x08,

		DataSetReady = 0x10,

		ReceiveLine = 0x20,

		Break = 0x40,

		StatusError = 0x80,

		Ring = 0x100,

	}

	#endregion

	#region "Structures";

	// This is the DCB structure used by the calls to the Windows API.

	[StructLayout(LayoutKind.Sequential, Pack=1)] private struct DCB
	{
		public int DCBlength;
		public int BaudRate;
		public int Bits1;
		public Int16 wReserved;
		public Int16 XonLim;
		public Int16 XoffLim;
		public byte ByteSize;
		public byte Parity;
		public byte StopBits;
		public byte XonChar;
		public byte XoffChar;
		public byte ErrorChar;
		public byte EofChar;
		public byte EvtChar;
		public Int16 wReserved2;

	}

	// This is the CommTimeOuts structure used by the calls to the Windows API.

	[StructLayout(LayoutKind.Sequential, Pack=1)] private struct COMMTIMEOUTS
	{
		public int ReadIntervalTimeout;
		public int ReadTotalTimeoutMultiplier;
		public int ReadTotalTimeoutConstant;
		public int WriteTotalTimeoutMultiplier;
		public int WriteTotalTimeoutConstant;
	}

	// This is the CommConfig structure used by the calls to the Windows API.

	[StructLayout(LayoutKind.Sequential, Pack=1)] private struct COMMCONFIG
	{

		public int dwSize;

		public Int16 wVersion;

		public Int16 wReserved;

		public DCB dcbx;

		public int dwProviderSubType;

		public int dwProviderOffset;

		public int dwProviderSize;

		public byte wcProviderData;

	}

	// This is the OverLapped structure used by the calls to the Windows API.

	[StructLayout(LayoutKind.Sequential, Pack=1)] public struct OVERLAPPED
	{
		public int interna;
		public int InternalHigh;
		public int Offset;
		public int OffsetHigh;
		public int hEvent;
	}

	#endregion

	#region "Exceptions";

	// This class defines a customized channel exception. This exception is
	//   raised when a NACK is raised.

	public class CIOChannelException: ApplicationException
	{

		public CIOChannelException(string Message)
		{
			new ApplicationException(Message);
			
			

		}

		public CIOChannelException(string Message, Exception InnerException)
		{
			new ApplicationException(Message, InnerException);
		}

	}

	// This class defines a customized timeout exception.

	public class IOTimeoutException:ApplicationException
	{

		public IOTimeoutException(string Message)
		{
		
			new CIOChannelException(Message);
			

		}

		public IOTimeoutException(string Message, Exception InnerException)
		{

			new CIOChannelException(Message, InnerException);

		}

	}

	#endregion

	#region "Events";

	// These events allow the program using this class to react to Comm Port
	//   events.
	public delegate void DataReceivedDelegate(Rs232 Source, byte[] DataBuffer);
	public event DataReceivedDelegate DataReceived;

	public delegate void TxCompletedDelegate(Rs232 Source);
	public event TxCompletedDelegate TxCompleted;

	public delegate void CommEventDelegate(Rs232 Source, EventMasks Mask);
	public event CommEventDelegate CommEvent;

	#endregion

	#region "Constants";

	// These constants are used to make the code clearer.

	private const int PURGE_RXABORT = 0x2;

	private const int PURGE_RXCLEAR = 0x08;

	private const int PURGE_TXABORT = 0x1;

	private const int PURGE_TXCLEAR = 0x4;

	private const int GENERIC_READ = unchecked((int)0x80000000);

	private const int GENERIC_WRITE = 0x40000000;

	private const int OPEN_EXISTING = 3;

	private const int INVALID_HANDLE_VALUE = -1;

	private const int IO_BUFFER_SIZE = 1024;

	private const int FILE_FLAG_OVERLAPPED = 0x40000000;

	private const int ERROR_IO_PENDING = 997;

	private const int WAIT_OBJECT_0 = 0;

	private const int ERROR_IO_INCOMPLETE = 996;

	private const int WAIT_TIMEOUT = 0x102;

	private const int INFINITE = unchecked((int)0xFFFFFFFF);

	#endregion

	#region "Properties";

	// This property gets or sets the BaudRate

	public int BaudRate
	{

		get 
		{

			return miBaudRate;

		}

		set
		{

			miBaudRate = value;

		}

	}

	// This property gets or sets the BufferSize

	public int BufferSize
	{

		get 
		{

			return miBufferSize;

		}

		set
		{

			miBufferSize = value;

		}

	}

	// This property gets or sets the DataBit.

	public int DataBit
	{
		get 
		{
			return miDataBit;
		}
		set
		{
			miDataBit = value;
		}
	}

	// This write-only property sets or resets the DTR line.

	public bool Dtr
	{

		set
		{

			if (mhRS != -1) 
			{

				if (value) 
				{

					EscapeCommFunction(mhRS, (long)Lines.SetDtr);
				}
				else 
				{

					EscapeCommFunction(mhRS, (long)Lines.ClearDtr);

				}

			}

		}

	}

	// This read-only property returns an array of bytes that represents
	//   the input coming into the Comm Port.

	public byte[] InputStream
	{

		get 
		{

			return mabtRxBuf;

		}

	}

	// This read-only property returns a string that represents
	//   the data coming into to the Comm Port.

	public string InputStreamstring
	{

		get 
		{

			System.Text.ASCIIEncoding oEncoder = new System.Text.ASCIIEncoding();
			return oEncoder.GetString(this.InputStream);

		}

	}

	// This property returns the open status of the Comm Port.

	public bool IsOpen
	{

		get 
		{

			return Convert.ToBoolean(mhRS != -1);

		}

	}

	// This read-only property returns the status of the modem.

	public ModemStatusBits ModemStatus
	{

		get 
		{

			if (mhRS == -1) 
			{

				throw new ApplicationException("Please initialize and open " +
					"port before using this method");
			}
			else 
			{

				// Retrieve modem status

				int lpModemStatus = 0;
				if (!GetCommModemStatus(mhRS, ref lpModemStatus))
				{
					throw new ApplicationException("Unable to get modem status");
				}
				else 
				{
					return (ModemStatusBits) lpModemStatus;

				}
			}

		}
	}

	// This property gets or sets the Parity

	public DataParity Parity
	{

		get 
		{

			return meParity;

		}

		set
		{
			meParity = value;

		}

	}

	// This property gets or sets the Port

	public int Port
	{

		get 
		{

			return miPort;

		}

		set
		{
			miPort = value;
		}

	}

	// This write-only property sets or resets the RTS line.

	public bool Rts
	{

		set
		{

			if (mhRS != -1) 
			{

				if (value) 
				{

					EscapeCommFunction(mhRS, (long)Lines.SetRts);
				}
				else 
				{

					EscapeCommFunction(mhRS, (long)Lines.ClearRts);

				}

			}

		}

	}

	// This property gets or sets the StopBit

	public DataStopBit StopBit
	{
		get 
		{

			return meStopBit;

		}

		set
		{
			meStopBit = value;
		}

	}

	// This property gets or sets the Timeout

	public int Timeout
	{
		get 
		{
			return miTimeout;
		}
		set
		{
			
			if (value == 0)
			{
				miTimeout=Convert.ToInt32(500);
			}
			else
			{
				miTimeout = Convert.ToInt32(value);
			}

			// if Port is open updates it on the fly

			pSetTimeout();

		}

	}

	// This property gets or sets the working mode to overlapped

	//   or non-overlapped.

	public Mode WorkingMode
	{

		get 
		{

			return meMode;

		}

		set
		{

			meMode = value;

		}

	}

	#endregion

	#region "Win32API";

	// The following functions are the required Win32 functions needed to 
	//   make communication with the Comm Port possible.

	[DllImport("kernel32.dll")] private static extern int BuildCommDCB(string lpDef, ref DCB lpDCB);
	[DllImport("kernel32.dll")] private static extern int ClearCommError(int hFile, int lpErrors, int l);
	[DllImport("kernel32.dll")] private static extern int CloseHandle(int hObject);
	[DllImport("kernel32.dll")] private static extern int CreateEvent(int lpEventAttributes, int bManualReset, int bInitialState, 
		[MarshalAs(UnmanagedType.LPStr)] string lpName);
	[DllImport("kernel32.dll")] private static extern int CreateFile( 
		[MarshalAs(UnmanagedType.LPStr)] string lpFileName, 
		int dwDesiredAccess, int dwShareMode, 
		int lpSecurityAttributes, int dwCreationDisposition, 
		int dwFlagsAndAttributes, int hTemplateFile);
	[DllImport("kernel32.dll")] private static extern bool EscapeCommFunction(int hFile, long ifunc);
	[DllImport("kernel32.dll")] private static extern int FormatMessage( 
		int dwFlags, int lpSource, int dwMessageId, int dwLanguageId, 
		[MarshalAs(UnmanagedType.LPStr)] string lpBuffer, 
		int nSize, int Arguments);
	[DllImport("kernel32.dll")] private static extern int FormatMessageA( 
	  int dwFlags, int lpSource, int dwMessageId, int dwLanguageId, 
     StringBuilder lpBuffer, int nSize, int Arguments);
    [DllImport("kernel32.dll")] public static extern bool GetCommModemStatus(int hFile, ref int lpModemStatus) ;
    [DllImport("kernel32.dll")] private static extern int GetCommState(int hCommDev, ref DCB lpDCB);
    [DllImport("kernel32.dll")] private static extern int GetCommTimeouts(int hFile, ref COMMTIMEOUTS lpCommTimeouts);
    [DllImport("kernel32.dll")] private static extern int GetLastError();
    [DllImport("kernel32.dll")] private static extern int GetOverlappedResult(int hFile, ref OVERLAPPED lpOverlapped, ref int lpNumberOfBytesTransferred, int bWait);
    [DllImport("kernel32.dll")] private static extern int PurgeComm(int hFile, int dwFlags);
    [DllImport("kernel32.dll")] private static extern int ReadFile(int hFile, byte[] Buffer, int nNumberOfBytesToRead, 
		ref int lpNumberOfBytesRead, ref OVERLAPPED lpOverlapped);
    [DllImport("kernel32.dll")] private static extern int SetCommTimeouts(int hFile, ref COMMTIMEOUTS lpCommTimeouts);
    [DllImport("kernel32.dll")] private static extern int SetCommState(int hCommDev, ref DCB lpDCB);
    [DllImport("kernel32.dll")] private static extern int SetupComm(int hFile, int dwInQueue, int dwOutQueue);
    [DllImport("kernel32.dll")] private static extern int SetCommMask(int hFile, int lpEvtMask);
    [DllImport("kernel32.dll")] private static extern int WaitCommEvent(int hFile, ref EventMasks Mask, 
        ref OVERLAPPED lpOverlap);
    [DllImport("kernel32.dll")] private static extern int WaitForSingleObject(int hHandle, int dwMilliseconds);
    [DllImport("kernel32.dll")] private static extern int WriteFile( 
        int hFile, byte[] Buffer, int nNumberOfBytesToWrite, 
        ref int lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);

#endregion

#region "Methods";

    // This subroutine invokes a thread to perform an asynchronous read.
    //   This routine should not be called directly, but is used
    //   by the class.

    public void _R()
	{
        int iRet = Read(miTmpBytes2Read);
    }

    // This subroutine invokes a thread to perform an asynchronous write.
    //   This routine should not be called directly, but is used
    //   by the class.

    public void _W()
	{
        Write(mabtTmpTxBuf);
    }

    // This subroutine uses another thread to read from the Comm Port. It 
    //   raises RxCompleted when done. It reads an integer.

    public void AsyncRead(int Bytes2Read)
{
		if (meMode != Mode.Overlapped) 
		{ 
			throw new ApplicationException("Async Methods allowed only when WorkingMode=Overlapped");
		}

        miTmpBytes2Read = Bytes2Read;
        Thread moThreadTx = new Thread(new ThreadStart(this._R));
        moThreadTx.Start();

    }

    // This subroutine uses another thread to write to the Comm Port. It 

    //   raises TxCompleted when done. It writes an array of bytes.

    public void AsyncWrite(byte[]  Buffer)
	{
		if (meMode != Mode.Overlapped) 
		{
				throw new ApplicationException( 
			  "Async Methods allowed only when WorkingMode=Overlapped");
		}
		if (mbWaitOnWrite == true) 
		{ 
			throw new ApplicationException( 
				"Unable to send message because of pending transmission.");
		}
        mabtTmpTxBuf = Buffer;
        Thread moThreadTx = new Thread(new ThreadStart(this._W));
		moThreadTx.Start();

    }

    // This subroutine uses another thread to write to the Comm Port. It 
    //   raises TxCompleted when done. It writes a string.

    public void AsyncWrite(string Buffer)
	{
		System.Text.ASCIIEncoding oEncoder = new System.Text.ASCIIEncoding();
        byte[] aByte = oEncoder.GetBytes(Buffer);
        this.AsyncWrite(aByte);

    }

    // This function takes the ModemStatusBits and returns a boolean value
    //   signifying whether the Modem is active.

    public bool CheckLineStatus(ModemStatusBits Line)
	{
        return Convert.ToBoolean((int)ModemStatus + (int)Line);
    }

    // This subroutine clears the input buffer.

    public void ClearInputBuffer()
	{

        if (mhRS != -1) {

            PurgeComm(mhRS, PURGE_RXCLEAR);

        }

    }

    // This subroutine closes the Comm Port.

    public void Close()
	{

        if (mhRS != -1) {

            CloseHandle(mhRS);

            mhRS = -1;

        }

    }

    // This subroutine opens and initializes the Comm Port

    public void Open()
	{
        // Get Dcb block,Update with current data

        DCB uDcb=new DCB(); 
		int iRc;

        // Set working mode

        int iMode;
		if(meMode == Mode.Overlapped)
		{
			iMode = Convert.ToInt32(FILE_FLAG_OVERLAPPED);
		}
		else
		{
			iMode = 0;
		}

			// Initializes Com Port

			if (miPort > 0) 
		{

			try 
			{

				// Creates a COM Port stream handle 

				mhRS = CreateFile("COM" + miPort.ToString(), 
					GENERIC_READ | GENERIC_WRITE, 0, 0, 
					OPEN_EXISTING, iMode, 0);

				if (mhRS != -1) 
				{
					// Clear all comunication errors
					int lpErrCode = 0;

					iRc = ClearCommError(mhRS, lpErrCode, 0);

					// Clears I/O buffers

					iRc = PurgeComm(mhRS, (int) PurgeBuffers.RXClear | (int) PurgeBuffers.TxClear);

					// Gets COM Settings

					iRc = GetCommState(mhRS, ref uDcb);

					// Updates COM Settings

					string sParity = "NOEM";

					sParity = sParity.Substring((int) meParity, 1);

					// Set DCB State

					string sDCBState = string.Format( 
						"baud={0} parity={1} data={2} stop={3}", 
						miBaudRate, sParity, miDataBit, (int)meStopBit);

					iRc = BuildCommDCB(sDCBState, ref uDcb);
					iRc = SetCommState(mhRS, ref uDcb);

					if (iRc == 0) 
					{
						string sErrTxt = pErr2Text(GetLastError());
						throw new CIOChannelException( 
							"Unable to set COM state0" + sErrTxt);

					}

					// Setup Buffers (Rx,Tx)

					iRc = SetupComm(mhRS, miBufferSize, miBufferSize);

					// Set Timeouts

					pSetTimeout();
				}
				else 
				{

					// Raise Initialization problems

					throw new CIOChannelException( 
						"Unable to open COM" + miPort.ToString());

				}

			} 
			catch(Exception Ex)
			{
				// Generic error

				throw new CIOChannelException(Ex.Message, Ex);

			}
		}
		else 
		{

			// Port not defined, can! open

			throw new ApplicationException("COM Port not defined, " + 
				"use Port property to set it before invoking InitPort");

		}

    }

    // This subroutine opens and initializes the Comm Port (overloaded
    //   to support parameters).

    public void Open(int Port, int BaudRate, int DataBit,
        DataParity Parity, DataStopBit StopBit, int BufferSize)
	{
        this.Port = Port;
        this.BaudRate = BaudRate;
        this.DataBit = DataBit;
        this.Parity = Parity;
        this.StopBit = StopBit;
        this.BufferSize = BufferSize;
        Open();

    }

    // This function translates an API error code to text.

    private string pErr2Text(int lCode)
	{
		
        StringBuilder sRtrnCode = new StringBuilder(256);
        int lRet = FormatMessage(0x00001000, 0, lCode, 0, sRtrnCode.ToString(), 256, 0);

		if (lRet > 0) 
		{
			return sRtrnCode.ToString();
		}
		else 
		{
			return "Error not found.";
		}

    }

    // This subroutine handles overlapped reads.

    private void pHandleOverlappedRead(int Bytes2Read)
	{

        int iReadChars = 0;
		int iRc; 
		int iRes;
		int iLastErr;
		


        muOverlapped.hEvent = CreateEvent(0, 1, 0, null);

        if (muOverlapped.hEvent == 0) {

            // Can't create event

            throw new ApplicationException( 
                "Error creating event for overlapped read.");
			}
        else {

            // Ovellaped reading

            if (mbWaitOnRead == false) {
				
                
				
                iRc = ReadFile(mhRS, mabtRxBuf, Bytes2Read, 
                    ref iReadChars, ref muOverlapped);

				if (iRc == 0) 
				{

					iLastErr = GetLastError();

					if (iLastErr != ERROR_IO_PENDING) 
					{

						throw new ArgumentException("Overlapped Read Error: " + 
							pErr2Text(iLastErr));
					}
					else 
					{

						// Set Flag

						mbWaitOnRead = true;

					}
				}
				else 
				{

					// Read completed successfully

					DataReceived(this, mabtRxBuf);

				}

            }

        }

        // Wait for operation to be completed

        if (mbWaitOnRead) {

            iRes = WaitForSingleObject(muOverlapped.hEvent, miTimeout);

            switch(iRes)
			{
				case WAIT_OBJECT_0:
				{
					// Object signaled,operation completed
					if (GetOverlappedResult(mhRS, ref muOverlapped, 
						ref iReadChars, 0) == 0) 
					{
						// Operation error
						iLastErr = GetLastError();
						if (iLastErr == ERROR_IO_INCOMPLETE) 
						{
							throw new ApplicationException( 
								"Read operation incomplete");
						}
						else 
						{
							throw new ApplicationException( 
								"Read operation error " + iLastErr.ToString());

						}
					}
					else 
					{
						// Operation completed
						DataReceived(this, mabtRxBuf);
						mbWaitOnRead = false;
					}
					break;
				}
				case WAIT_TIMEOUT:
				{
					throw new IOTimeoutException("Timeout error");
					break;
				}
				default: 
				{
					throw new ApplicationException("Overlapped read error");
					break;
				}
            }

        }

    }

    // This subroutine handles overlapped writes.

    private bool pHandleOverlappedWrite(byte[] Buffer)
	{

        int iBytesWritten = 0;
		int iRc;
		int iLastErr;
		int iRes;
		bool bErr = false;

        muOverlappedW.hEvent = CreateEvent(0, 1, 0, null);

        if (muOverlappedW.hEvent == 0) {
            // Can't create event
            throw new ApplicationException( 
                "Error creating event for overlapped write.");
			}
        else {
            // Overllaped write
            PurgeComm(mhRS, (int)PURGE_RXCLEAR | (int)PURGE_TXCLEAR);
            mbWaitOnRead = true;
            iRc = WriteFile(mhRS, Buffer, Buffer.Length, 
                ref iBytesWritten, ref muOverlappedW);
			if (iRc == 0) 
			{

				iLastErr = GetLastError();

				if (iLastErr != ERROR_IO_PENDING) 
				{
					throw new ArgumentException("Overlapped Read Error: " + 
						pErr2Text(iLastErr));
				}
				else 
				{
					// Write is pending
					iRes = WaitForSingleObject(muOverlappedW.hEvent, INFINITE);
					switch(iRes)
					{
						case WAIT_OBJECT_0:
						{
							// Object signaled,operation completed
							if (GetOverlappedResult(mhRS, ref muOverlappedW, 

								ref iBytesWritten, 0) == 0)
							{
								bErr = true;
							}
							else 
							{
								// Notifies Async tx completion,stops thread
								mbWaitOnRead = false;
								TxCompleted(this);
							}
							break;
						}
					}
				}
			}
			else 
			{
				// Wait operation completed immediatly
				bErr = false;
			}
				}

        CloseHandle(muOverlappedW.hEvent);

        return bErr;

    }

    // This subroutine sets the Comm Port timeouts.

    private void pSetTimeout()
	{

        COMMTIMEOUTS uCtm;

        // Set ComTimeout

		if (mhRS == -1) 
		{

			return;
		}
		else 
		{

			// Changes setup on the fly
			uCtm.ReadIntervalTimeout = 0;
			uCtm.ReadTotalTimeoutMultiplier = 0;
			uCtm.ReadTotalTimeoutConstant = miTimeout;
			uCtm.WriteTotalTimeoutMultiplier = 10;
			uCtm.WriteTotalTimeoutConstant = 100;
			SetCommTimeouts(mhRS, ref uCtm);
		}

    }

    // This function returns an integer specifying the number of bytes 
    //   read from the Comm Port. It accepts a parameter specifying the number
    //   of desired bytes to read.

    public int Read(int Bytes2Read)
		{

        int iReadChars = 0;
		int iRc;

        // if Bytes2Read not specified uses Buffersize

		if (Bytes2Read == 0) 
		{ 
			Bytes2Read = miBufferSize;
		}
		if (mhRS == -1) 
		{

			throw new ApplicationException( 
				"Please initialize and open port before using this method");
		}
		else 
		{
			// Get bytes from port
			try 
			{
				// Purge buffers
				//PurgeComm(mhRS, PURGE_RXCLEAR || PURGE_TXCLEAR)
				// Creates an event for overlapped operations
				if (meMode == Mode.Overlapped) 
				{
					pHandleOverlappedRead(Bytes2Read);
				}
				else 
				{
					// Non overlapped mode
					
					iRc = ReadFile(mhRS, mabtRxBuf, Bytes2Read, ref iReadChars, ref muOverlappedW);

					if (iRc == 0) 
					{
						// Read Error
						throw new ApplicationException( 
							"ReadFile error " + iRc.ToString());
					}
					else 
					{

						// timeout or returns input chars
						if (iReadChars < Bytes2Read) 
						{
							throw new IOTimeoutException("Timeout error");
						}
						else 
						{
							mbWaitOnRead = true;
							return (iReadChars);
						}
							
						}
		
					}
						return (iReadChars);
					} 
			catch(Exception Ex)
			{
				// Others generic erroes
				
				throw new ApplicationException("Read Error: " + Ex.Message, Ex);
				
			}

		}

    }

    // This subroutine writes the passed array of bytes to the 

    //   Comm Port to be written.

    public void Write(byte[] Buffer)
	{
        int iBytesWritten = 0;
		int iRc;

		if (mhRS == -1) 
		{

			throw new ApplicationException( 
				"Please initialize and open port before using this method");
		}
		else 
		{
			// Transmit data to COM Port
			try 
			{

				if (meMode == Mode.Overlapped)
				{

					// Overlapped write

					if (pHandleOverlappedWrite(Buffer)) 
					{
						throw new ApplicationException( 
							"Error in overllapped write");
					}
					else 
					{
						// Clears IO buffers
						iRc = PurgeComm(mhRS, PURGE_RXCLEAR) | PURGE_TXCLEAR;

						iRc = WriteFile(mhRS, Buffer, Buffer.Length, 
							ref iBytesWritten, ref muOverlappedW);

						if (iRc == 0) 
						{

							throw new ApplicationException( 
								"Write Error - Bytes Written " + 
								iBytesWritten.ToString() + " of " + 
								Buffer.Length.ToString());
						}

					}

				} 
			}
			catch(Exception Ex)
			{
				throw;

			}

		}

    }

    // This subroutine writes the passed string to the 
    //   Comm Port to be written.

    public void Write(string Buffer)
	{
        System.Text.ASCIIEncoding oEncoder = new System.Text.ASCIIEncoding();
        byte[] aByte  = oEncoder.GetBytes(Buffer);
        this.Write(aByte);

    }

#endregion

}

