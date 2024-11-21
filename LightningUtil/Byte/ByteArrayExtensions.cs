
namespace LightningUtil
{
    /// <summary>
    /// ByteArrayExtensions
    /// 
    /// July 26, 2022
    /// 
    /// Provides extremely fast solutions for byte array equality.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Extremely fast byte array comparison algorithm using unsafe code, 
        /// not using P/Invoke, and not using CompilerServices.Unsafe for one method.
        /// https://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net
        /// 
        /// Even faster than .NET Core spans!
        /// </summary>
        /// <param name="data1">The first byte array to compare.</param>
        /// <param name="data2">The second byte array to compare.</param>
        /// <returns>A boolean value determining if the byte arrays are equal.</returns>
        public static unsafe bool FastEqual(this byte[] data1, byte[] data2)
        {
            if (data1 == data2) return true;

            if (data1.Length != data2.Length) return false;

            fixed (byte* bytes1 = data1, bytes2 = data2)
            {
                int len = data1.Length;
                int rem = len % (sizeof(long) * 16);
                long* b1 = (long*)bytes1;
                long* b2 = (long*)bytes2;
                long* e1 = (long*)(bytes1 + len - rem);

                // this is witchcraft that i don't understand (entirely unrolled...)
                while (b1 < e1)
                {
                    if (*(b1) != *(b2) || *(b1 + 1) != *(b2 + 1) ||
                        *(b1 + 2) != *(b2 + 2) || *(b1 + 3) != *(b2 + 3) ||
                        *(b1 + 4) != *(b2 + 4) || *(b1 + 5) != *(b2 + 5) ||
                        *(b1 + 6) != *(b2 + 6) || *(b1 + 7) != *(b2 + 7) ||
                        *(b1 + 8) != *(b2 + 8) || *(b1 + 9) != *(b2 + 9) ||
                        *(b1 + 10) != *(b2 + 10) || *(b1 + 11) != *(b2 + 11) ||
                        *(b1 + 12) != *(b2 + 12) || *(b1 + 13) != *(b2 + 13) ||
                        *(b1 + 14) != *(b2 + 14) || *(b1 + 15) != *(b2 + 15))
                        return false;

                    b1 += 16;
                    b2 += 16;
                }

                for (int curByte = 0; curByte < rem; curByte++)
                {
                    if (data1[len - 1 - curByte] != data2[len - 1 - curByte]) return false;
                }

                return true;
            }
        }
    }
}