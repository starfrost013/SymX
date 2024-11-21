namespace LightningUtil
{
    public static class ArrayUtils
    {
        public static byte[] Combine(byte[] array1, byte[] array2)
        {
            byte[] finalArray = new byte[array1.Length + array2.Length];
            Buffer.BlockCopy(array1, 0, finalArray, 0, array2.Length);
            Buffer.BlockCopy(array2, 0, finalArray, array1.Length, array2.Length);

            return finalArray;
        }

        public static byte[] Combine(byte[] array1, byte[] array2, byte[] array3)
        {
            byte[] finalArray = new byte[array1.Length + array2.Length + array3.Length];
            Buffer.BlockCopy(array1, 0, finalArray, 0, array2.Length);
            Buffer.BlockCopy(array2, 0, finalArray, array1.Length, array2.Length);
            Buffer.BlockCopy(array3, 0, finalArray, array2.Length, array3.Length);
            return finalArray;
        }
    }
}
