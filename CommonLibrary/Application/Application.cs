using System;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Application
{
	public class Application
	{
		private static ReleaseMode _status = ReleaseMode.None;
		protected internal Application()
		{
		}
		public string Company
		{
			get { return "5EAGLES Corporation"; }
		}
		public virtual string Description
		{
			get { return "5EAGLES Community Edition"; }
		}
		public string HelpUrl
		{
			get { return "http://www.5EAGLES.com/default.aspx?tabid=787"; }
		}
		public string LegalCopyright
		{
			get { return "5EAGLES® is copyright 2012-" + DateTime.Today.ToString("yyyy") + " by 5EAGLES Corporation"; }
		}
		public virtual string Name
		{
            get { return "5EAGLES CORP.CE"; }
		}
		public virtual string SKU
		{
            get { return "5EAGLES"; }
		}
		public ReleaseMode Status
		{
			get
			{
				if (_status == ReleaseMode.None)
				{
					Assembly assy = System.Reflection.Assembly.GetExecutingAssembly();
					if (Attribute.IsDefined(assy, typeof(AssemblyStatusAttribute)))
					{
						Attribute attr = Attribute.GetCustomAttribute(assy, typeof(AssemblyStatusAttribute));
						if (attr != null)
						{
							_status = ((AssemblyStatusAttribute)attr).Status;
						}
					}
				}
				return _status;
			}
		}
		public string Title
		{
			get { return "5EAGLES"; }
		}
		public string Trademark
		{
			get { return "5EAGLES"; }
		}
		public string Type
		{
			get { return "Framework"; }
		}
		public string UpgradeUrl
		{
			get { return "http://thegioiso360.com/upgrade"; }
		}
		public string Url
		{
            get { return "http://www.thegioiso360.com"; }
		}
		public System.Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
        }

     
    }
}
