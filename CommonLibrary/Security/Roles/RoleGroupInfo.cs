using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security.Roles;
using System.Xml;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;
using System.Xml.Serialization;
using CommonLibrary.Entities;

namespace CommonLibrary.Security.Roles
{
    [Serializable()]
    public class RoleGroupInfo : BaseEntityInfo, IHydratable, IXmlSerializable
    {
        private int _RoleGroupID = Null.NullInteger;
        private int _PortalID = Null.NullInteger;
        private string _RoleGroupName;
        private string _Description;
        private Dictionary<string, RoleInfo> _Roles;
        public RoleGroupInfo()
        {
        }
        public RoleGroupInfo(int roleGroupID, int portalID, bool loadRoles)
        {
            _PortalID = portalID;
            _RoleGroupID = roleGroupID;
            if (loadRoles)
            {
                GetRoles();
            }
        }
        public int RoleGroupID
        {
            get { return _RoleGroupID; }
            set { _RoleGroupID = value; }
        }
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        public string RoleGroupName
        {
            get { return _RoleGroupName; }
            set { _RoleGroupName = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public Dictionary<string, RoleInfo> Roles
        {
            get
            {
                if (_Roles == null && RoleGroupID > Null.NullInteger)
                {
                    GetRoles();
                }
                return _Roles;
            }
        }
        private void GetRoles()
        {
            _Roles = new Dictionary<string, RoleInfo>();
            foreach (RoleInfo role in new RoleController().GetRolesByGroup(PortalID, RoleGroupID))
            {
                _Roles[role.RoleName] = role;
            }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            RoleGroupID = Null.SetNullInteger(dr["RoleGroupId"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            RoleGroupName = Null.SetNullString(dr["RoleGroupName"]);
            Description = Null.SetNullString(dr["Description"]);
            FillInternal(dr);
        }
        public int KeyID
        {
            get { return RoleGroupID; }
            set { RoleGroupID = value; }
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        private void ReadRoles(XmlReader reader)
        {
            reader.ReadStartElement("roles");
            _Roles = new Dictionary<string, RoleInfo>();
            do
            {
                reader.ReadStartElement("role");
                RoleInfo role = new RoleInfo();
                role.ReadXml(reader);
                _Roles.Add(role.RoleName, role);
            } while (reader.ReadToNextSibling("role"));
        }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (reader.NodeType == XmlNodeType.Whitespace)
                {
                    continue;
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLowerInvariant())
                    {
                        case "roles":
                            if (!reader.IsEmptyElement)
                            {
                                ReadRoles(reader);
                            }
                            break;
                        case "rolegroupname":
                            RoleGroupName = reader.ReadElementContentAsString();
                            break;
                        case "description":
                            Description = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("rolegroup");
            writer.WriteElementString("rolegroupname", RoleGroupName);
            writer.WriteElementString("description", Description);
            writer.WriteStartElement("roles");
            if (Roles != null)
            {
                foreach (RoleInfo role in Roles.Values)
                {
                    role.WriteXml(writer);
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
