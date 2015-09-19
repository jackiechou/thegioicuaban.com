using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace CommonLibrary.Services.Installer.Writers
{
    public class CleanupComponentWriter
    {
        private string _BasePath;
        private SortedList<string, InstallFile> _Files;
        public CleanupComponentWriter(string basePath, SortedList<string, InstallFile> files)
        {
            _Files = files;
            _BasePath = basePath;
        }
        public virtual void WriteManifest(XmlWriter writer)
        {
            foreach (KeyValuePair<string, InstallFile> kvp in _Files)
            {
                writer.WriteStartElement("component");
                writer.WriteAttributeString("type", "Cleanup");
                writer.WriteAttributeString("fileName", kvp.Value.Name);
                writer.WriteAttributeString("version", Path.GetFileNameWithoutExtension(kvp.Value.Name));
                writer.WriteEndElement();
            }
        }
    }
}
