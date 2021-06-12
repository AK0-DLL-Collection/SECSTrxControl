using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.License
{
    public class SimpleLicenseKey : ILicenseKey
    {
        private string localhostname;
        private IPHostEntry ipHostInfo;

        private ILicenseKeyProtect agent;
        public SimpleLicenseKey(ILicenseKeyProtect agent)
        {
            this.agent = agent;
            localhostname = Environment.UserDomainName;
            ipHostInfo = Dns.GetHostEntry(localhostname);
        }

        public DateTime getExpirationDate()
        {
            return DateTime.Now;
        }

        private Boolean isValidationCheck()
        {
            return true;
        }

        public bool isValidation()
        {
            return isValidationCheck();
        }

        public void checkValidation()
        {
            if (!isValidationCheck()) 
            {
                //
            }
        }

        public string getHostname()
        {
            return localhostname;
        }
    }
}
