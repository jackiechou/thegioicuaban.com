using System.Collections.Generic;
namespace CommonLibrary.Modules.Dashboard.Components.Database
{
	public class DbInfo
	{
		private string _ProductVersion;
		public string ProductVersion {
			get { return _ProductVersion; }
			set { _ProductVersion = value; }
		}
		private string _ServicePack;
		public string ServicePack {
			get { return _ServicePack; }
			set { _ServicePack = value; }
		}
		private string _ProductEdition;
		public string ProductEdition {
			get { return _ProductEdition; }
			set { _ProductEdition = value; }
		}
		private string _SoftwarePlatform;
		public string SoftwarePlatform {
			get { return _SoftwarePlatform; }
			set { _SoftwarePlatform = value; }
		}
		public List<BackupInfo> Backups {
			get { return DatabaseController.GetDbBackups(); }
		}
		public List<DbFileInfo> Files {
			get { return DatabaseController.GetDbFileInfo(); }
		}
	}
}
