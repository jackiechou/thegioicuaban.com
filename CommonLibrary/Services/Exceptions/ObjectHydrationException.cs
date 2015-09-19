using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Data;

namespace CommonLibrary.Services.Exceptions
{
    [Serializable()]
    public class ObjectHydrationException : BasePortalException
    {
        private System.Type _Type;
        private List<string> _Columns;
        public ObjectHydrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ObjectHydrationException(string message, Exception innerException, Type type, IDataReader dr)
            : base(message, innerException)
        {
            _Type = type;
            _Columns = new List<string>();
            foreach (DataRow row in dr.GetSchemaTable().Rows)
            {
                _Columns.Add(row["ColumnName"].ToString());
            }
        }
        protected ObjectHydrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        public List<string> Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }
        public System.Type Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public override string Message
        {
            get
            {
                string _Message = base.Message;
                _Message += " Expecting - " + Type.ToString() + ".";
                _Message += " Returned - ";
                foreach (string columnName in Columns)
                {
                    _Message += columnName + ", ";
                }
                return _Message;
            }
        }
    }
}
