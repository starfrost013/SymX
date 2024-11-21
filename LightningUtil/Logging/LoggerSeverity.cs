namespace LightningUtil
{
    /// <summary>
    /// NCExceptionSeverity
    /// 
    /// February 3, 2022
    /// 
    /// Defines a list of exception severities for NC2.
    /// </summary>
    public enum LoggerSeverity
    {
        /// <summary>
        /// Defines a message
        /// </summary>
        Message = 0,

        /// <summary>
        /// Defines a warning, an issue that requires the user's attention but does not prevent execution of the current operation.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Defines an error, an issue that requires the user's attention and prevents execution of the current operation.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Defines a fatal error, an issue that requires the termination of program execution.
        /// </summary>
        FatalError = 3
    }
}
