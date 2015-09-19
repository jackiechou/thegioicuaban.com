using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;
using System.Collections;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security.Roles;

namespace CommonLibrary.Modules.Dashboard.Components.Portals
{
    public class PortalInfo
    {
        private int _Pages = Null.NullInteger;
        private int _Roles = Null.NullInteger;
        private int _Users = Null.NullInteger;
        private System.Guid _GUID;
        public System.Guid GUID
        {
            get { return _GUID; }
            set { _GUID = value; }
        }
        public int Pages
        {
            get
            {
                if (_Pages < 0)
                {
                    TabController controller = new TabController();
                    _Pages = controller.GetTabCount(PortalID);
                }
                return _Pages;
            }
        }
        private int _PortalID;
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        private string _PortalName;
        public string PortalName
        {
            get { return _PortalName; }
            set { _PortalName = value; }
        }
        public int Roles
        {
            get
            {
                if (_Roles < 0)
                {
                    RoleController controller = new RoleController();
                    ArrayList portalRoles = controller.GetPortalRoles(PortalID);
                    _Roles = portalRoles.Count;
                }
                return _Roles;
            }
        }
        public int Users
        {
            get
            {
                if (_Users < 0)
                {
                    _Users = UserController.GetUserCountByPortal(PortalID);
                }
                return _Users;
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("portal");
            writer.WriteElementString("portalName", PortalName);
            writer.WriteElementString("GUID", GUID.ToString());
            writer.WriteElementString("pages", Pages.ToString());
            writer.WriteElementString("users", Users.ToString());
            writer.WriteElementString("roles", Roles.ToString());
            writer.WriteEndElement();
        }
    }
}
