using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CommonLibrary.Services.Exceptions
{
    [Serializable()]
    public class PageLoadException : BasePortalException
    {
        public PageLoadException()
            : base()
        {
        }
        public PageLoadException(string message)
            : base(message)
        {
        }
        public PageLoadException(string message, Exception inner)
            : base(message, inner)
        {
        }
        protected PageLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
