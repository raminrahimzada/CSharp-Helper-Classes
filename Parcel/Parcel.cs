public class Parcel
    {
        private readonly Stream _stream;

        public long Position
        {
            get
            {
                return _stream.Position;
            }
            private set
            {
                _stream.Seek(value, SeekOrigin.Begin);
            }
        }
        public void ResetToOrigin()
        {
            Position = 0;
        }
        public void Flush()
        {
            _stream.Flush();
        }
        public void WriteBytes(params byte[] newdata)
        {
            _stream.Write(newdata, 0, newdata.Length);
        }
        public byte[] ReadBytes(int length)
        {
            var buffer = new byte[length];
            var count = _stream.Read(buffer, 0, length);
            if (count== length)
            {
                return buffer;
            }
            throw new IOException("file read fault");
        }
        public Parcel(Stream stream)
        {
            if (stream==null)
            {
                throw new ArgumentException(nameof(stream));
            }
            _stream = stream;
        }

        #region array global
        public T Read<T>() where T:IWSerializer
        {
            var t = Activator.CreateInstance<T>();
            t.ReadFromParcel(this);
            return t;
        }
        public T[] ReadArrayOf<T>() where T : IWSerializer
        {
            var length = ReadInt();
            var cavab = new T[length];
            for (var i = 0; i < length; i++)
            {
                cavab[i] = Read<T>();
            }
            return cavab;
        }
        public void WriteArrayOf<T>(T[] array) where T : IWSerializer
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var item in array)
            {
                item.WriteToParcel(this);
            }
        }
        public void Write<T>(T t) where T:IWSerializer
        {
            t.WriteToParcel(this);
        }
        #endregion

        #region Integer
        public int ReadInt()
        {
            var cari = ReadBytes(4);
            return WConverter.ToInt32(cari);
        }
        public void WriteInt(int n)
        {
            var cari = WConverter.GetBytes(n);
            WriteBytes(cari);
        }
        #endregion

        #region Byte
        public void WriteByte(byte b)
        {
            WriteBytes(b);
        }
        public byte ReadByte()
        {
            var b = ReadBytes(1);
            return b[0];
        }
        #endregion
       
        #region bool
        public void WriteBool(bool b)
        {
            WriteBytes((byte)(b ? 1 : 0));
        }
        public bool ReadBool()
        {
            var b = ReadBytes(1)[0];
            if (b==1)
            {
                return true;
            }
            if (b==0)
            {
                return false;
            }
            throw new Exception("Read Fault");
        }
        #endregion

        #region Char
        public void WriteChar(char ch)
        {
            WriteBytes(WConverter.GetBytes(ch));
        }
        public char ReadChar()
        {
            var cari = ReadBytes(2);
            return WConverter.ToChar(cari);
        }
        #endregion

        #region string
        public void WriteString(string s)
        {
            var length = s.Length;
            WriteInt(length);
            foreach (var ch in s)
            {
                WriteChar(ch);
            }
        }
        public string ReadString()
        {
            var length = ReadInt();
            var s = string.Empty;
            for (var i = 0; i < length; i++)
            {
                s += ReadChar();
            }
            return s;
        }
        #endregion

        #region double
        public double ReadDouble()
        {
            var cari = ReadBytes(8);
            return WConverter.ToDouble(cari);
        }
        public void WriteDouble(double d)
        {
            var cari = WConverter.GetBytes(d);
            WriteBytes(cari);
        }
        #endregion

        #region float
        public float ReadFloat()
        {
            var cari = ReadBytes(4);           
            return WConverter.ToFloat(cari);
        }
        public void WriteFloat(float f)
        {
            var cari = WConverter.GetBytes(f);
            WriteBytes(cari);
        }
        #endregion

        #region int array
        public void WriteIntegerArray(int[] array)
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var ch in array)
            {
                WriteInt(ch);
            }
        }
        public int[] ReadIntegerArray()
        {
            var length = ReadInt();
            var array = new int[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ReadInt();
            }
            return array;
        }
        #endregion

        #region double array
        public void WriteDoubleArray(double[] array)
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var ch in array)
            {
                WriteDouble(ch);
            }
        }
        public double[] ReadDoubleArray()
        {
            var length = ReadInt();
            var array = new double[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ReadDouble();
            }
            return array;
        }
        #endregion

        #region float array
        public void WriteFloatArray(float[] array)
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var ch in array)
            {
                WriteFloat(ch);
            }
        }
        public float[] ReadFloatArray()
        {
            var length = ReadInt();
            var array = new float[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ReadFloat();
            }
            return array;
        }
        #endregion

        #region string array
        public void WriteStringArray(string[] array)
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var ch in array)
            {
                WriteString(ch);
            }
        }
        public string[] ReadStringArray()
        {
            var length = ReadInt();
            var array = new string[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ReadString();
            }
            return array;
        }
        #endregion

        #region byte array
        public void WriteByteArray(byte[] array)
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var ch in array)
            {
                WriteByte(ch);
            }
        }
        public byte[] ReadByteArray()
        {
            var length = ReadInt();
            var array = new byte[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ReadByte();
            }
            return array;
        }
        #endregion

        #region short array
        public void WriteShortArray(byte[] array)
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var ch in array)
            {
                WriteShort(ch);
            }
        }
        public short[] ReadShortArray()
        {
            var length = ReadInt();
            var array = new short[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ReadByte();
            }
            return array;
        }
        #endregion

        #region long array
        public void WriteLongArray(long[] array)
        {
            var length = array.Length;
            WriteInt(length);
            foreach (var ch in array)
            {
                WriteLong(ch);
            }
        }
        public long[] ReadLongArray()
        {
            var length = ReadInt();
            var array = new long[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ReadLong();
            }
            return array;
        }
        #endregion

        #region long
        public long ReadLong()
        {
            var cari = ReadBytes(8);           
            return WConverter.ToLong(cari);
        }
        public void WriteLong(long l)
        {
            var cari = WConverter.GetBytes(l);
            WriteBytes(cari);
        }
        #endregion

        #region short
        public short ReadShort()
        {
            var cari = ReadBytes(2);           
            return WConverter.ToInt16(cari);
        }
        public void WriteShort(short s)
        {
            var cari = WConverter.GetBytes(s);
            WriteBytes(cari);
        }
        #endregion
    }
