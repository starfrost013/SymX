namespace LightningUtil
{
    /// <summary>
    /// NoiseBasic
    /// 
    /// A very basic noise algorithm.
    /// This is probably versatile enough to generate a biome map later on. 
    /// </summary>
    public static class NoiseBasic
    {
        /// <summary>
        /// Total factor - The amplitude of the terrain you wish to generate.
        /// </summary>
        public static double Amplitude { get; set; }

        /// <summary>
        /// e-scale - How jagged the terrain you wish to generate is
        /// </summary>
        public static double Jaggedness { get; set; }

        /// <summary>
        /// e-factor - how high the hill heights are
        /// </summary>
        public static double HillHeight { get; set; }

        /// <summary>
        /// Pi-factor - how much amplitude the hill is
        /// </summary>
        public static double HillAmplitude { get; set; }

        /// <summary>
        /// 1-scale
        /// </summary>
        public static double Scale { get; set; }

        /// <summary>
        /// 1-factor - how much the hill heights cycle
        /// </summary>
        public static double CycleHillHeight { get; set; }

        /// <summary>
        /// pi-scale - the number of hills generated
        /// </summary>
        public static double NumberOfHills { get; set; }

        public static double Generate(double x)
        {
            return Amplitude *
                (CycleHillHeight * Math.Sin(Scale * x))
                - (HillHeight * Math.Sin(Jaggedness * Math.E * x)
                - (HillAmplitude * Math.Sin(NumberOfHills * Math.PI * x)));
        }
    }
}
