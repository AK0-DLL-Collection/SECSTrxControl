using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.License
{
    public class TrialLicenseKey : ILicenseKey
    {
        private string localhostname;
        private IPHostEntry ipHostInfo;

        private DateTime startDateTime = DateTime.Now;
        private double totalTrialSec = 3600;
        private long intervalSec = 120;

        protected bool isStarted = false;
        public bool IsStarted { get { return isStarted; } }
        private ILicenseKeyProtect agent;

        public TrialLicenseKey(ILicenseKeyProtect agent) 
        {
            this.agent = agent;
            localhostname = Environment.UserDomainName;
            ipHostInfo = Dns.GetHostEntry(localhostname);
            start();
        }

        public DateTime getExpirationDate() 
        {
            return DateTime.Now;
        }

        private DateTime now = DateTime.Now;
        private Boolean isValidationCheck() 
        {
            now = DateTime.Now;
            if (now.Subtract(startDateTime).TotalSeconds >= totalTrialSec)
            {
                agent.invalidLicense("License Key is Invalidation !");
                stop();
                return false;
            }
            return true;
        }

        public Boolean isValidation() 
        {
            return isValidationCheck();
        }

        public void checkValidation() 
        {
            if (! isValidation()) 
            {
                //throw new LicenseKeyException("License Key is Invalidation !");
                //agent.invalidLicense("License Key is Invalidation !");
            }
        }

        public String getHostname() 
        {
            return localhostname;
        }

        private Timer timer;
        private async void start()
        {
            await Task.Delay(10);
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
            isStarted = true;
            timer = new Timer(new TimerCallback(doCheck), null, 0, (intervalSec * 1000));
        }

        private async void stop()
        {
            await Task.Delay(10);
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
            isStarted = false;
        }

        private void doCheck(object obj)
        {
            checkValidation();
        }

    }
}
