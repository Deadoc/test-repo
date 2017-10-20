using System;

namespace PoliceSoft
{
    public class VersionFormatter
    {
        public static String FormatVersion(Version version)
        {
            String result;

            if (version.Revision == 0)
                result = String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            else
                result = String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            return result;
        }
    }
}
