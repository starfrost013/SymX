using static LightningUtil.MathUtil;

namespace LightningUtil
{
    /// <summary>
    /// 1D simplex noise code
    /// 
    /// Based on public domain code from https://github.com/Xpktro/simplexnoise/blob/master/SimplexNoise/Noise.cs
    /// </summary>
    public static class NoiseSimplex1D
    {
        public static float Generate(float x)
        {
            int i0 = FastFloor(x);
            int i1 = i0 + 1;
            float x0 = x - i0;
            float x1 = x0 - 1.0f;

            float n0, n1;

            float t0 = 1.0f - x0 * x0;
            t0 *= t0;
            n0 = t0 * t0 * Gradient(SimplexNoisePermutations[i0 & 0xff], x0);

            float t1 = 1.0f - x1 * x1;
            t1 *= t1;
            n1 = t1 * t1 * Gradient(SimplexNoisePermutations[i1 & 0xff], x1);
            // The maximum value of this noise is 8*(3/4)^4 = 2.53125
            // A factor of 0.395 scales to fit exactly within [-1,1]
            return 0.395f * (n0 + n1);
        }
    }
}
