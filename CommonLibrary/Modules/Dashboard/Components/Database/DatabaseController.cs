using System.Collections.Generic;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Modules.Dashboard.Components.Database
{
	public class DatabaseController : IDashboardData
	{
		public static DbInfo GetDbInfo()
		{
			return CBO.FillObject<DbInfo>(DataService.GetDbInfo());
		}
		public static List<BackupInfo> GetDbBackups()
		{
			return CBO.FillCollection<BackupInfo>(DataService.GetDbBackups());
		}
		public static List<DbFileInfo> GetDbFileInfo()
		{
			return CBO.FillCollection<DbFileInfo>(DataService.GetDbFileInfo());
		}
		public void ExportData(System.Xml.XmlWriter writer)
		{
			DbInfo database = GetDbInfo();
			writer.WriteStartElement("database");
			writer.WriteElementString("productVersion", database.ProductVersion);
			writer.WriteElementString("servicePack", database.ServicePack);
			writer.WriteElementString("productEdition", database.ProductEdition);
			writer.WriteElementString("softwarePlatform", database.SoftwarePlatform);
			writer.WriteStartElement("backups");
			foreach (BackupInfo backup in database.Backups) {
				writer.WriteStartElement("backup");
				writer.WriteElementString("name", backup.Name);
				writer.WriteElementString("backupType", backup.BackupType);
				writer.WriteElementString("size", backup.Size.ToString());
				writer.WriteElementString("startDate", backup.StartDate.ToString());
				writer.WriteElementString("finishDate", backup.FinishDate.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("files");
			foreach (DbFileInfo dbFile in database.Files) {
				writer.WriteStartElement("file");
				writer.WriteElementString("name", dbFile.Name);
				writer.WriteElementString("fileType", dbFile.FileType);
				writer.WriteElementString("shortFileName", dbFile.ShortFileName);
				writer.WriteElementString("fileName", dbFile.FileName);
				writer.WriteElementString("size", dbFile.Size.ToString());
				writer.WriteElementString("megabytes", dbFile.Megabytes.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
}
