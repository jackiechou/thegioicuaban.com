using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Security.Permissions;

namespace CommonLibrary.Entities.Modules
{
    [Serializable()]
    public class PortalDesktopModuleInfo : BaseEntityInfo
    {
        private int _PortalDesktopModuleID;
        private int _DesktopModuleID;
        private string _FriendlyName;
        private DesktopModulePermissionCollection _Permissions;
        private int _PortalID;
        private string _PortalName;
        public PortalDesktopModuleInfo()
        {
        }
        [XmlIgnore()]
        public int PortalDesktopModuleID
        {
            get { return _PortalDesktopModuleID; }
            set { _PortalDesktopModuleID = value; }
        }
        [XmlIgnore()]
        public int DesktopModuleID
        {
            get { return _DesktopModuleID; }
            set { _DesktopModuleID = value; }
        }
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set { _FriendlyName = value; }
        }
        public DesktopModulePermissionCollection Permissions
        {
            get
            {
                if (_Permissions == null)
                {
                    _Permissions = new DesktopModulePermissionCollection(DesktopModulePermissionController.GetDesktopModulePermissions(PortalDesktopModuleID));
                }
                return _Permissions;
            }
        }
        [XmlIgnore()]
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        [XmlIgnore()]
        public string PortalName
        {
            get { return _PortalName; }
            set { _PortalName = value; }
        }
    }
}
