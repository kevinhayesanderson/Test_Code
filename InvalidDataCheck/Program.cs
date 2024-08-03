namespace InvalidDataCheck
{
    internal static class Program
    {
        private static void Main(string[] _)
        {
            Console.WriteLine("Value    IsInvalidData");
            double dbl = default;
            Console.WriteLine($"{dbl}  {IsInvalidData(dbl)}");
            double? ndbl1 = default;
            Console.WriteLine($"{ndbl1}  {IsInvalidData(ndbl1)}");
            double? ndbl2 = double.NaN;
            Console.WriteLine($"{ndbl2}  {IsInvalidData(ndbl2)}");

            Console.ReadKey();
        }

        private static bool IsInvalidData(object? data)
        {
            bool res = data == null || IsNaN(data);
            return res;

            static bool IsNaN(object data) => data switch
            {
                double dbl => double.IsNaN(dbl),
                float flt => float.IsNaN(flt),
                ulong => false,
                uint => false,
                ushort => false,
                long => false,
                int => false,
                short => false,
                byte => false,
                sbyte => false,
                _ => throw new NotImplementedException(),
            };
        }

        ////Output:
        ////Value   IsInvalidData
        ////0       False
        ////        True
        ////NaN     True
    }
}


