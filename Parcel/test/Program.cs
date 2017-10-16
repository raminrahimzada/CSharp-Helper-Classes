public class Program
    {
        public static void Main(string[] args)
        {
            //creating test objects 
            var r = new TestClassOne
            {
                X = int.MaxValue + int.MinValue,
                Y = 145,
                Z = 123332,
                T = "a bb",
                B = new double[] {1, 2, 3},
                C = new[] {"qweqwe ", "A", "B", "qweqwe"},
                A = 3.14159265358979323846264338
            };
            //creating test objects 
            var r1 = new TestClassOne
            {
                X = int.MinValue,
                Y = 255,
                Z = 2555,
                T = "first second",
                B = new double[] {1, 2, 3},
                C = new[] {"test ", "A", "B", "vvv"},
                A = 3.14159265358979323846264338
            };
            //creating bigger test object
            var m = new TestClassTwo
            {
                X1 = r,
                X2 = r1,
            };
            var xxx = new List<TestClassOne>();
            for (var i = 0; i < 5; i++)
            {
                var re = new TestClassOne
                {
                    X = int.MaxValue,
                    Y = (byte) (25 + i),
                    Z = 2555 * i + 5,
                    T = new string((char) i, i),
                    B = Enumerable.Range(0, i).Select(ii => (double) (ii + i)).ToArray(),
                    C = new[] {"test", "A", "B", "visual", "studio"},
                    A = 3.14159265358979323846264338
                };
                xxx.Add(re);
            }
            m.X3 = xxx.ToArray();
            m.Dd = 11111;

            //creating Parcel from Stream
            var stream = File.Open("serialized_data.dat", FileMode.Create);
            var parcel = new Parcel(stream);
            //writing object to parcel
            parcel.Write(m);
            //flushing changes
            parcel.Flush();
            //Reset Seek Position To Zero To Read All Data in Parcel
            parcel.ResetToOrigin();
            //reading full object from Parcel
            var mm = parcel.Read<TestClassTwo>();

            // All Is Well -> m object written to file and read from file to mm
            //if so then m == mm 

        }
    }
