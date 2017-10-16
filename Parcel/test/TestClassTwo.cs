public class TestClassTwo:IWSerializer
    {
        public TestClassOne X1;
        public TestClassOne X2;
        public TestClassOne[] X3;
        public long Dd;

        public void ReadFromParcel(Parcel p)
        {
            X1 = p.Read<TestClassOne>();
            X2 = p.Read<TestClassOne>();
            X3 = p.ReadArrayOf<TestClassOne>();
            Dd = p.ReadLong();
        }

        public void WriteToParcel(Parcel p)
        {
            p.Write(X1);
            p.Write(X2);
            p.WriteArrayOf(X3);
            p.WriteLong(Dd);
        }
    }
