using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Exception_Stream : Exception 
    {
        public Exception_Stream() : base() { }
        public Exception_Stream(string message) : base(message) { }
        public Exception_Stream(string message, System.Exception inner) : base (message, inner) { }
    }

    public class Exception_XML : Exception
    {
        public Exception_XML() : base() { }
        public Exception_XML(string message) : base(message) { }
        public Exception_XML(string message, System.Exception inner) : base (message, inner) { }
    }

    public class Exception_STOP : Exception
    {
        public Exception_STOP()  : base(){ }
        public Exception_STOP(string message) : base(message) { }
        public Exception_STOP(string message, System.Exception inner) : base(message,inner) { }
    }

}

