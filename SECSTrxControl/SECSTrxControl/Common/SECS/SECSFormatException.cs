using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.Common.SECS
{
    public class SECSFormatException : Exception
    {
        public SECSFormatException() : base()
        {
            
        }

        public SECSFormatException(string message) : base(message) { }


    }
}
