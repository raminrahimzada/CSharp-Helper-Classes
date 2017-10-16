public class TestClassOne:IWSerializer
    {
        public int X { get; set; }
        public byte Y { get; set; }
        public int Z { get; set; }
        public string T { get; set; }
        public double A { get; set; }
        public double[] B { get; set; }
        public string[] C { get; set; }
        public float D { get; set; }

        public void ReadFromParcel(Parcel p)
        {
            X = p.ReadInt();
            Y = p.ReadByte();
            Z = p.ReadInt();
            T = p.ReadString();
            A = p.ReadDouble();
            B = p.ReadDoubleArray();
            C = p.ReadStringArray();
            D = p.ReadFloat();
        }

        public void WriteToParcel(Parcel p)
        {
            p.WriteInt(X);
            p.WriteByte(Y);
            p.WriteInt(Z);
            p.WriteString(T);
            p.WriteDouble(A);
            p.WriteDoubleArray(B);
            p.WriteStringArray(C);
            p.WriteFloat(D);
        }
    }
