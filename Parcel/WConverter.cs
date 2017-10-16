public static class WConverter
    {
        public static byte[] GetBytes(int n)
        {
            return BitConverter.GetBytes(n);
        }
        public static byte[] GetBytes(short s)
        {
            return BitConverter.GetBytes(s);
        }
        public static byte[] GetBytes(double d)
        {
            return BitConverter.GetBytes(d);
        }
        public static byte[] GetBytes(long l)
        {
            return BitConverter.GetBytes(l);
        }
        public static byte[] GetBytes(float f)
        {
            return BitConverter.GetBytes(f);
        }
        public static byte[] GetBytes(char ch)
        {
            return BitConverter.GetBytes(ch);
        }
        public static int ToInt32(byte[] array)
        {
            return BitConverter.ToInt32(array, 0);
        }
        public static short ToInt16(byte[] array)
        {
            return BitConverter.ToInt16(array, 0);
        }
        public static double ToDouble(byte[] array)
        {
            return BitConverter.ToDouble(array, 0);
        }
        public static long ToLong(byte[] array)
        {
            return BitConverter.ToInt64(array, 0);
        }
        public static float ToFloat(byte[] array)
        {
            return BitConverter.ToSingle(array, 0);
        }
        public static char ToChar(byte[] array)
        {
            return BitConverter.ToChar(array, 0);
        }
    }
