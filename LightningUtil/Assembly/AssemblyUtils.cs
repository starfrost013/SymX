namespace LightningUtil
{
    /// <summary>
    /// NCAssembly
    /// 
    /// NuCore Assembly utilities.
    /// </summary>
    public static class AssemblyUtils
    {
        #region class and namespace names for NCException
        internal static string LIGHTNING_UTILITIES_NAME = "LightningUtil.Lightning";

        internal static string LIGHTNING_UTILITIES_PRESET_NAME = $"LightningUtil.NCMessageBoxPresets";

        /// <summary>
        /// LightningUtil.Lightning assembly.
        /// </summary>
        internal static Assembly? NCLightningAssembly { get; private set; }

        internal static bool NCLightningExists => (NCLightningAssembly != null);

        static AssemblyUtils()
        {
            try
            {
                // try and load LightningUtil.Lightning.
                // this is all kludge until we get a better msgbox api
                NCLightningAssembly = Assembly.Load(LIGHTNING_UTILITIES_NAME);
            }
            catch
            {
                // dont load
                NCLightningAssembly = null;
            }
        }

        #endregion
    }
}
