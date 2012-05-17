using System;
using System.Xml.Serialization;

namespace MsBuilderific
{
    /// <summary>
    /// Possible values for projects output type
    /// </summary>
    [Serializable]
    public enum ProjectOutputType
    {
        /// <summary>
        /// Represents a windows form executable
        /// </summary>
        [XmlEnum("WinExe")]
        WinExe = 0,
        /// <summary>
        /// Represents as console executable
        /// </summary>
        [XmlEnum("Exe")]
        Exe = 1,
        /// <summary>
        /// Represents a library
        /// </summary>
        [XmlEnum("Library")]
        Library = 2
    }
}
