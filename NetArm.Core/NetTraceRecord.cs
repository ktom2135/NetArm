using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace NetArm.Core
{
    public class NetTraceRecord
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Duration
        {
            get
            {
                return (EndTime - StartTime).TotalMilliseconds;
            }
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
}
