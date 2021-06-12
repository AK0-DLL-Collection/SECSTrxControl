using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.License
{
    public class LicenseKeyException : Exception
    {
        public LicenseKeyException()
            : base("License Validation Message")
        {

        }

        public LicenseKeyException(string message)
            : base(message)
        {

        }

    }
}
