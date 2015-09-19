using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Services.Authentication
{
    [Serializable()]
	public class AuthenticationInfo : BaseEntityInfo, IHydratable
	{
		private int _AuthenticationID = Null.NullInteger;
		private int _PackageID;
		private bool _IsEnabled = false;
		private string _AuthenticationType = Null.NullString;
		private string _SettingsControlSrc = Null.NullString;
		private string _LoginControlSrc = Null.NullString;
		private string _LogoffControlSrc = Null.NullString;
		public int AuthenticationID {
			get { return _AuthenticationID; }
			set { _AuthenticationID= value; }
		}
		public int PackageID {
			get { return _PackageID; }
			set { _PackageID= value; }
		}
		public bool IsEnabled {
			get { return _IsEnabled; }
			set { _IsEnabled= value; }
		}
		public string AuthenticationType {
			get { return _AuthenticationType; }
			set { _AuthenticationType= value; }
		}
		public string SettingsControlSrc {
			get { return _SettingsControlSrc; }
			set { _SettingsControlSrc= value; }
		}
		public string LoginControlSrc {
			get { return _LoginControlSrc; }
			set { _LoginControlSrc= value; }
		}
		public string LogoffControlSrc {
			get { return _LogoffControlSrc; }
			set { _LogoffControlSrc= value; }
		}
		public virtual void Fill(System.Data.IDataReader dr)
		{
			AuthenticationID = Null.SetNullInteger(dr["AuthenticationID"]);
			PackageID = Null.SetNullInteger(dr["PackageID"]);
			IsEnabled = Null.SetNullBoolean(dr["IsEnabled"]);
			AuthenticationType = Null.SetNullString(dr["AuthenticationType"]);
			SettingsControlSrc = Null.SetNullString(dr["SettingsControlSrc"]);
			LoginControlSrc = Null.SetNullString(dr["LoginControlSrc"]);
			LogoffControlSrc = Null.SetNullString(dr["LogoffControlSrc"]);
			FillInternal(dr);
		}
		public virtual int KeyID {
			get { return AuthenticationID; }
			set { AuthenticationID = value; }
		}
	}
}
