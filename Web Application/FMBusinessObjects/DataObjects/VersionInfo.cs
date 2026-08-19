// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionInfo.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Serializable version of the System.Version class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Serializable version of the System.Version class.
    /// </summary>
    [Serializable]
    public class VersionInfo : ICloneable, IComparable
    {
        #region Attributes

        /// <summary>
        /// The major version.
        /// </summary>
        private readonly int _MajorVersion;

        /// <summary>
        /// The minor version.
        /// </summary>
        private readonly int _MinorVersion;

        /// <summary>
        /// The build number.
        /// </summary>
        private readonly int _Build;

        /// <summary>
        /// The revision number.
        /// </summary>
        private readonly int _Revision;

        /// <summary>
        /// Any added string suffix such as Alpha, RC1, Beta, etc.
        /// </summary>
        private readonly string _Suffix;

        #endregion Attributes

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class. 
        /// </summary>
        public VersionInfo()
        {
            this._MajorVersion = 0;
            this._MinorVersion = 0;
            this._Build = -1;
            this._Revision = -1;
            this._Suffix = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class. 
        /// </summary>
        /// <param name="major">
        /// Major version number.
        /// </param>
        /// <param name="minor">
        /// Minor version number.
        /// </param>
        public VersionInfo(int major, int minor)
        {
            this._Build = -1;
            this._Revision = -1;
            this._Suffix = string.Empty;

            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", major, @"Invalid major version number value.");
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", minor, @"Invalid minor version number value.");
            }

            this._MajorVersion = major;
            this._MinorVersion = minor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class. 
        /// </summary>
        /// <param name="major">
        /// Major version number.
        /// </param>
        /// <param name="minor">
        /// Minor version number.
        /// </param>
        /// <param name="build">
        /// Build number.
        /// </param>
        public VersionInfo(int major, int minor, int build)
        {
            this._Revision = -1;
            this._Suffix = string.Empty;

            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", major, @"Invalid major version number value.");
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", minor, @"Invalid minor version number value.");
            }

            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", build, @"Invalid build number value.");
            }

            this._MajorVersion = major;
            this._MinorVersion = minor;
            this._Build = build;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class. 
        /// </summary>
        /// <param name="major">
        /// Major version number.
        /// </param>
        /// <param name="minor">
        /// Minor version number.
        /// </param>
        /// <param name="build">
        /// Build number.
        /// </param>
        /// <param name="revision">
        /// Revision number.
        /// </param>
        public VersionInfo(int major, int minor, int build, int revision)
        {
            this._Suffix = string.Empty;

            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", major, @"Invalid major version number value.");
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", minor, @"Invalid minor version number value.");
            }

            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", build, @"Invalid build number value.");
            }

            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException("revision", revision, @"Invalid revision number value.");
            }

            this._MajorVersion = major;
            this._MinorVersion = minor;
            this._Build = build;
            this._Revision = revision;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class. 
        /// </summary>
        /// <param name="major">
        /// Major version number.
        /// </param>
        /// <param name="minor">
        /// Minor version number.
        /// </param>
        /// <param name="build">
        /// Build number.
        /// </param>
        /// <param name="revision">
        /// Revision number.
        /// </param>
        /// <param name="suffix">
        /// Suffix value.
        /// </param>
        public VersionInfo(int major, int minor, int build, int revision, string suffix)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", major, @"Invalid major version number value.");
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", minor, @"Invalid minor version number value.");
            }

            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", build, @"Invalid build number value.");
            }

            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException("revision", revision, @"Invalid revision number value.");
            }

            this._MajorVersion = major;
            this._MinorVersion = minor;
            this._Build = build;
            this._Revision = revision;
            this._Suffix = suffix;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the major version number
        /// </summary>
        /// <value></value>
        public int MajorVersion
        {
            get
            {
                return this._MajorVersion;
            }
        }

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        /// <value></value>
        public int MinorVersion
        {
            get
            {
                return this._MinorVersion;
            }
        }

        /// <summary>
        /// Gets the build number
        /// </summary>
        /// <value></value>
        public int Build
        {
            get
            {
                return this._Build;
            }
        }

        /// <summary>
        /// Gets the revision number
        /// </summary>
        /// <value></value>
        public int Revision
        {
            get
            {
                return this._Revision;
            }
        }

        /// <summary>
        /// Gets the suffix number
        /// </summary>
        /// <value></value>
        public string Suffix
        {
            get
            {
                return this._Suffix;
            }
        }
        #endregion Properties

        #region Static Methods
        /// <summary>
        /// The from string.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// A null argument exception will be thrown if the passed in version number is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if an error is encountered with the format of the string version.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if any of single version parts contains a negative value.
        /// </exception>
        public static VersionInfo FromString(string version)
        {
            int majorVersion = -1;
            int minorVersion = -1;
            int build = -1;
            int revision = -1;
            string suffix = string.Empty;
            
            bool hasSuffix = false;
            string localVersion = (string)version.Clone();

            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            // Look for a suffix first and extract it.  It may have a hyphen separator or a decimal separator.  We'll handle the hyphen first.
            char[] alternateSuffixDelimeter = new char[1] { '-' };
            string[] suffixParts = version.Split(alternateSuffixDelimeter);
            int num1 = suffixParts.Length;

            if (num1 > 2)
            {
                throw new ArgumentException(@"Invalid version format. [major.minor.build.revision-optionalsuffix]", "version");
            }

            // Save off the suffix if it exists and move the version part to the localVersion.
            if (num1 == 2)
            {
                hasSuffix = true;
                localVersion = suffixParts[0];
                suffix = suffixParts[1];
            }

            // Split the version number using a decimal delimeter.  We may or may not encounter a suffix.  If we already have one and we find another one, we'll concat them
            // back together.
            char[] delimeter = new char[1] { '.' };
            string[] versionParts = localVersion.Split(delimeter);

            num1 = versionParts.Length;
            if (num1 < 2)
            {
                if (hasSuffix && num1 > 4)
                {
                    throw new ArgumentException(@"Invalid version format. [major.minor.build.revision]", "version");
                }
                else if (!hasSuffix && num1 > 5)
                {
                    throw new ArgumentException(@"Invalid version format. [major.minor.build.revision.optionalsuffix]", "version");
                }
            }

            majorVersion = int.Parse(versionParts[0], CultureInfo.InvariantCulture);
            if (majorVersion < 0)
            {
                throw new ArgumentOutOfRangeException("version", majorVersion, @"Invalid major version number value.");
            }

            minorVersion = int.Parse(versionParts[1], CultureInfo.InvariantCulture);
            if (minorVersion < 0)
            {
                throw new ArgumentOutOfRangeException("version", minorVersion, @"Invalid minor version number value.");
            }

            num1 -= 2;
            if (num1 > 0)
            {
                build = int.Parse(versionParts[2], CultureInfo.InvariantCulture);
                if (build < 0)
                {
                    throw new ArgumentOutOfRangeException("version", build, @"Invalid build number value.");
                }

                num1--;
                if (num1 > 0)
                {
                    revision = int.Parse(versionParts[3], CultureInfo.InvariantCulture);
                    if (revision < 0)
                    {
                        throw new ArgumentOutOfRangeException("version", revision, @"Invalid revision number value.");
                    }

                    num1--;
                    if (num1 > 0)
                    {
                        suffix = !string.IsNullOrEmpty(suffix) ? string.Format("{0}-{1}", versionParts[4], suffix) : versionParts[4];
                    }
                }
            }

            return new VersionInfo(majorVersion, minorVersion, build, revision, suffix);
        }

        #endregion Static Methods

        #region ICloneable Members
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A new instance of a <see cref="VersionInfo"/> object, populated with the same values as the current one.
        /// </returns>
        public object Clone()
        {
            var version1 = new VersionInfo(
                this._MajorVersion, this._MinorVersion, this._Build, this._Revision, this._Suffix);

            return version1;
        }
        #endregion

        #region IComparable Members
        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="version">
        /// VersionInfo instance to compare to.
        /// </param>
        /// <returns>
        /// -1 if this <see cref="VersionInfo"/> is less than the passed in instance.
        /// 0 if this <see cref="VersionInfo"/> represents the same version as the passed in instance.
        /// 1 if this <see cref="VersionInfo"/> is greater than the passed in instance.
        /// </returns>
        public int CompareTo(object version)
        {
            if (version == null)
            {
                return 1;
            }

            if (!(version is VersionInfo))
            {
                throw new ArgumentException("Argument is not of type VersionInfo.");
            }

            VersionInfo version1 = (VersionInfo)version;
            if (this._MajorVersion != version1._MajorVersion)
            {
                if (this._MajorVersion > version1._MajorVersion)
                {
                    return 1;
                }

                return -1;
            }

            if (this._MinorVersion != version1._MinorVersion)
            {
                if (this._MinorVersion > version1._MinorVersion)
                {
                    return 1;
                }

                return -1;
            }

            if (this._Build != version1._Build)
            {
                if (this._Build > version1._Build)
                {
                    return 1;
                }

                return -1;
            }

            if (this._Revision == version1._Revision)
            {
                return 0;
            }

            if (this._Revision > version1._Revision)
            {
                return 1;
            }

            return -1;
        }
        #endregion

        #region Equality Operators
        /// <summary>
        /// Determines if the passed in <see cref="VersionInfo"/> instance is equal to the current instance.
        /// </summary>
        /// <param name="version">
        /// An instance of <see cref="VersionInfo"/> to compare this instance to.
        /// </param>
        /// <returns>
        /// True if the two instances of <see cref="VersionInfo"/> are the same, otherwise; False.
        /// </returns>
        public override bool Equals(object version)
        {
            if ((version == null) || !(version is VersionInfo))
            {
                return false;
            }

            VersionInfo version1 = (VersionInfo)version;

            if (((this._MajorVersion == version1._MajorVersion) && (this._MinorVersion == version1._MinorVersion)) && (this._Build == version1._Build) && (this._Revision == version1._Revision))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int num1 = 0;
            num1 |= (this._MajorVersion & 15) << 0x1c;
            num1 |= (this._MinorVersion & 0xff) << 20;
            num1 |= (this._Build & 0xff) << 12;
            num1 |= (this._Revision & 0xfff) << 7;
            return num1 | this._Suffix.GetHashCode();
        }

        /// <summary>
        /// Operator ==s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator ==(VersionInfo v1, VersionInfo v2)
        {
            return typeof(VersionInfo).IsInstanceOfType(v1) && v1.Equals(v2);
        }

        /// <summary>
        /// Operator &gt;s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator >(VersionInfo v1, VersionInfo v2)
        {
            return typeof(VersionInfo).IsInstanceOfType(v2) && v2 < v1;
        }

        /// <summary>
        /// Operator &gt;=s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator >=(VersionInfo v1, VersionInfo v2)
        {
            return typeof(VersionInfo).IsInstanceOfType(v2) && v2 <= v1;
        }

        /// <summary>
        /// Operator !=s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator !=(VersionInfo v1, VersionInfo v2)
        {
            return typeof(VersionInfo).IsInstanceOfType(v1) && !v1.Equals(v2);
        }

        /// <summary>
        /// Operator &lt;s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator <(VersionInfo v1, VersionInfo v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }

            return v1.CompareTo(v2) < 0;
        }

        /// <summary>
        /// Operator &lt;=s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator <=(VersionInfo v1, VersionInfo v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }

            return v1.CompareTo(v2) <= 0;
        }
        #endregion Equality Operators

        #region String Operators
        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this._Build == -1)
            {
                return this.ToString(2);
            }

            if (this._Revision == -1)
            {
                return this.ToString(3);
            }

            if (string.IsNullOrEmpty(this._Suffix))
            {
                return this.ToString(4);
            }

            return this.ToString(5);
        }

        /// <summary>
        /// Converts the VersionInfo information into a formatted string.
        /// </summary>
        /// <param name="fieldCount">
        /// The number of version elements that should be included in the returned string.
        /// </param>
        /// <returns>
        /// A string version of the version information.
        /// </returns>
        /// <remarks>
        /// If the specified field count is 1, then only the Major number is returned.  Example: "1"
        /// If the specified field count is 2, then the Major and Minor numbers are returned.  Example "1.2"
        /// If the specified field count is 3, then the Major, Minor and Build numbers are returned.  Example "1.2"
        /// </remarks>
        public string ToString(int fieldCount)
        {
            switch (fieldCount)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return this._MajorVersion.ToString(CultureInfo.InvariantCulture);
                case 2:
                    return string.Format(
                        "{0}.{1}",
                        this._MajorVersion.ToString(CultureInfo.InvariantCulture),
                        this._MinorVersion.ToString(CultureInfo.InvariantCulture));
            }

            if (this._Build == -1)
            {
                throw new ArgumentException("Build number not available");
            }

            if (fieldCount == 3)
            {
                return string.Format(
                    "{0}.{1}.{2}",
                    this._MajorVersion.ToString(CultureInfo.InvariantCulture),
                    this._MinorVersion.ToString(CultureInfo.InvariantCulture),
                    this._Build.ToString(CultureInfo.InvariantCulture));
            }

            if (this._Revision == -1)
            {
                throw new ArgumentException("Revision number not available");
            }

            if (fieldCount == 4)
            {
                return string.Format(
                    "{0}.{1}.{2}.{3}",
                    this._MajorVersion.ToString(CultureInfo.InvariantCulture),
                    this._MinorVersion.ToString(CultureInfo.InvariantCulture),
                    this._Build.ToString(CultureInfo.InvariantCulture),
                    this._Revision.ToString(CultureInfo.InvariantCulture));
            }

            // if the Suffix already has a hyphen in it, then we'll use a decimal as a delimeter, otherwise; we'll use a hyphen.
            if (fieldCount == 5)
            {
                return string.Format(
                    "{0}.{1}.{2},{3}{5}{4}",
                    this._MajorVersion.ToString(CultureInfo.InvariantCulture),
                    this._MinorVersion.ToString(CultureInfo.InvariantCulture),
                    this._Build.ToString(CultureInfo.InvariantCulture),
                    this._Revision.ToString(CultureInfo.InvariantCulture),
                    this._Suffix,
                    this._Suffix.Contains("-") ? "." : "-");
            }

            throw new ArgumentException("Invalid argument.  Field count must be between 0 and 5.");
        }
        #endregion String Operators
    }
}

