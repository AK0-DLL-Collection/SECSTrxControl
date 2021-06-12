using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.License
{
    public interface ILicenseKeyProtect
    {
        void invalidLicense(string msg);
        void stopSystemByLicenseCheck();
    }
}
