using System;

namespace MD5
{


	
	/*******************************************************
	 * Programmed by
	 *			 syed Faraz mahmood
	 *					Student NU FAST ICS
	 * can be reached at s_faraz_mahmood@hotmail.com
	 * 
	 * 
	 * 
	 * *******************************************************/

	/// <summary>
	/// constants for md5
	/// </summary>
	public enum MD5InitializerConstant : uint
	{
		A=0x67452301,
		B=0xEFCDAB89,
		C=0x98BADCFE,
		D=0X10325476
	}

	/// <summary>
	/// Represent digest with ABCD
	/// </summary>
	sealed public class Digest
	{
		public uint A;
		public uint B;
		public uint C;
		public uint D;

		public Digest()
		{
			A=(uint)MD5InitializerConstant.A;
			B=(uint)MD5InitializerConstant.B;
			C=(uint)MD5InitializerConstant.C;
			D=(uint)MD5InitializerConstant.D;
       	}
		public override string ToString()
		{
			string st ;
			st= MD5Helper.ReverseByte(A).ToString("X8")+
				MD5Helper.ReverseByte(B).ToString("X8")+
                MD5Helper.ReverseByte(C).ToString("X8")+
				MD5Helper.ReverseByte(D).ToString("X8");
			return st;
			
		}

	}


	/// <summary>
	/// helper class providing suporting function
	/// </summary>
	sealed public class MD5Helper
	{

		private MD5Helper(){}

		/// <summary>
		/// Left rotates the input word
		/// </summary>
		/// <param name="uiNumber">a value to be rotated</param>
		/// <param name="shift">no of bits to be rotated</param>
		/// <returns>the rotated value</returns>
		public static uint RotateLeft(uint uiNumber , ushort shift)
		{
			return ((uiNumber >> 32-shift)|(uiNumber<<shift));
		}

		/// <summary>
		/// perform a ByteReversal on a number
		/// </summary>
		/// <param name="uiNumber">value to be reversed</param>
		/// <returns>reversed value</returns>
		public static uint ReverseByte(uint uiNumber)
		{
			return ( (	(uiNumber & 0x000000ff) <<24) |
						(uiNumber >>24) |
					(	(uiNumber & 0x00ff0000) >>8)  |
					(	(uiNumber & 0x0000ff00) <<8) );
		}
	}

	/// <summary>
	/// class for changing event args
	/// </summary>
	public class MD5ChangingEventArgs:EventArgs
	{
		public readonly byte[] NewData;
		
		public MD5ChangingEventArgs(byte [] data)
		{
			byte [] NewData = new byte[data.Length];
			for (int i =0; i<data.Length;i++)
				NewData[i]=data[i];
		}

		public MD5ChangingEventArgs(string data)
		{
			byte [] NewData = new byte[data.Length];
			for (int i =0; i<data.Length;i++)
				NewData[i]=(byte)data[i];
		}

	}

	/// <summary>
	/// class for cahnged event args
	/// </summary>
	public class MD5ChangedEventArgs:EventArgs
	{
		public readonly byte[] NewData;
		public readonly string FingerPrint;
		
		public MD5ChangedEventArgs(byte [] data,string HashedValue)
		{
			byte [] NewData = new byte[data.Length];
			for (int i =0; i<data.Length;i++)
				NewData[i]=data[i];
			FingerPrint=HashedValue;
		}

		public MD5ChangedEventArgs(string data,string HashedValue)
		{
			byte [] NewData = new byte[data.Length];
			for (int i =0; i<data.Length;i++)
				NewData[i]=(byte)data[i];

			FingerPrint=HashedValue;
		}

	}




}