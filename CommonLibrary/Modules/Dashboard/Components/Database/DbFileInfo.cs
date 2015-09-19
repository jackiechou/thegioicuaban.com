using System;
namespace CommonLibrary.Modules.Dashboard.Components.Database
{
	public class DbFileInfo
	{
		private string _FileType;
		public string FileType {
			get { return _FileType; }
			set { _FileType = value; }
		}
		private string _Name;
		public string Name {
			get { return _Name; }
			set { _Name = value; }
		}
		private long _Size;
		public long Size {
			get { return _Size; }
			set { _Size = value; }
		}
		public decimal Megabytes {
			get { return Convert.ToDecimal((Size / 1024)); }
		}
		private string _FileName;
		public string FileName {
			get { return _FileName; }
			set { _FileName = value; }
		}
		public string ShortFileName {
			get {
				string sname = string.Format("{0}...{1}", FileName.Substring(0, FileName.IndexOf('\\') + 1), FileName.Substring(FileName.LastIndexOf('\\', FileName.LastIndexOf('\\') - 1)));
				return sname;
			}
		}
	}
}
