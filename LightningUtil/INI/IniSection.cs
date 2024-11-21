namespace LightningUtil
{
    /// <summary>
    /// NCIniFileSection
    /// 
    /// March 7, 2022
    /// 
    /// Defines an MC INI file section.
    /// </summary>
    public class IniSection
    {
        /// <summary>
        /// The values of this section.
        /// </summary>
        public Dictionary<string, string> Values { get; set; }

        /// <summary>
        /// The name of this section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor for <see cref="IniSection"/> with a parameter for the section name.
        /// </summary>
        public IniSection(string name)
        {
            Name = name;
            Values = new();
        }

        /// <summary>
        /// Gets the value with the key <see cref="key"/>
        /// </summary>
        /// <param name="valueName">The INI key contained within the section.</param>
        /// <returns>A string containing the value corresponding to the key <paramref name="valueName"/>.</returns>
        public string? GetValue(string valueName, bool caseSensitive = false)
        {
            valueName = valueName.ToLowerInvariant();

            foreach (var kvp in Values)
            {
                string currentValueName = kvp.Key;

                if (!caseSensitive)
                {
                    currentValueName = kvp.Key.ToLowerInvariant();
                    valueName = valueName.ToLowerInvariant();
                }

                if (currentValueName == valueName) return kvp.Value;
            }

            return null;
        }
    }
}
