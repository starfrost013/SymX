
namespace SymX
{
    /// <summary>
    /// Verbosity
    /// 
    /// May 16, 2022
    /// 
    /// Defines a list of verbosity levels for debugging purposes.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// Quiet mode - no output will be produced
        /// </summary>
        Quiet = 0,

        /// <summary>
        /// Normal mode - a normal amount of output (completion %, # of successful urls, etc) will be produced.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Verbose mode - all URLs downloaded will be output, as well as various other things
        /// </summary>
        Verbose = 2
    }
}
