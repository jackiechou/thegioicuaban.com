using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Portal;
using System.ComponentModel;
using System.Xml.Serialization;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Entities.Profile
{
    [XmlRoot("profiledefinition", IsNullable = false)]
    [Serializable()]
    public class ProfilePropertyDefinition : BaseEntityInfo
    {
        private int _DataType = Null.NullInteger;
        private string _DefaultValue;
        private bool _IsDirty;
        private int _Length;
        private int _ModuleDefId = Null.NullInteger;
        private int _PortalId;
        private string _PropertyCategory;
        private int _PropertyDefinitionId = Null.NullInteger;
        private string _PropertyName;
        private string _PropertyValue;
        private bool _Required;
        private string _ValidationExpression;
        private int _ViewOrder;
        private bool _Visible;
        private UserVisibilityMode _Visibility = UserVisibilityMode.AdminOnly;
        public ProfilePropertyDefinition()
        {
            PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
            this.PortalId = _Settings.PortalId;
            Initialize();
        }
        public ProfilePropertyDefinition(int portalId)
        {
            this.PortalId = portalId;
            Initialize();
        }
      //  [Editor("DotNetNuke.UI.WebControls.DNNListEditControl, DotNetNuke", typeof(DotNetNuke.UI.WebControls.EditControl)), List("DataType", "", ListBoundField.Id, ListBoundField.Value), IsReadOnly(true), Required(true), SortOrder(1)]
        [XmlIgnore()]
        public int DataType
        {
            get { return _DataType; }
            set
            {
                if (_DataType != value)
                    _IsDirty = true;
                _DataType = value;
            }
        }
        //[SortOrder(4)]
        [XmlIgnore()]
        public string DefaultValue
        {
            get { return _DefaultValue; }
            set
            {
                if (_DefaultValue != value)
                    _IsDirty = true;
                _DefaultValue = value;
            }
        }
        [Browsable(false)]
        [XmlIgnore()]
        public bool IsDirty
        {
            get { return _IsDirty; }
        }
        //[SortOrder(3)]
        [XmlElement("length")]
        public int Length
        {
            get { return _Length; }
            set
            {
                if (_Length != value)
                    _IsDirty = true;
                _Length = value;
            }
        }
        [Browsable(false)]
        [XmlIgnore()]
        public int ModuleDefId
        {
            get { return _ModuleDefId; }
            set { _ModuleDefId = value; }
        }
        [Browsable(false)]
        [XmlIgnore()]
        public int PortalId
        {
            get { return _PortalId; }
            set { _PortalId = value; }
        }
        //[Required(true), SortOrder(2)]
        [XmlElement("propertycategory")]
        public string PropertyCategory
        {
            get { return _PropertyCategory; }
            set
            {
                if (_PropertyCategory != value)
                    _IsDirty = true;
                _PropertyCategory = value;
            }
        }
        [Browsable(false)]
        [XmlIgnore()]
        public int PropertyDefinitionId
        {
            get { return _PropertyDefinitionId; }
            set { _PropertyDefinitionId = value; }
        }
      //  [Required(true), IsReadOnly(true), SortOrder(0), RegularExpressionValidator("^[a-zA-Z0-9._%\\-+']+$")]
        [XmlElement("propertyname")]
        public string PropertyName
        {
            get { return _PropertyName; }
            set
            {
                if (_PropertyName != value)
                    _IsDirty = true;
                _PropertyName = value;
            }
        }
        [Browsable(false)]
        [XmlIgnore()]
        public string PropertyValue
        {
            get { return _PropertyValue; }
            set
            {
                if (_PropertyValue != value)
                    _IsDirty = true;
                _PropertyValue = value;
            }
        }
      //  [SortOrder(6)]
        [XmlIgnore()]
        public bool Required
        {
            get { return _Required; }
            set
            {
                if (_Required != value)
                    _IsDirty = true;
                _Required = value;
            }
        }
      //  [SortOrder(5)]
        [XmlIgnore()]
        public string ValidationExpression
        {
            get { return _ValidationExpression; }
            set
            {
                if (_ValidationExpression != value)
                    _IsDirty = true;
                _ValidationExpression = value;
            }
        }
       // [IsReadOnly(true), SortOrder(8)]
        [XmlIgnore()]
        public int ViewOrder
        {
            get { return _ViewOrder; }
            set
            {
                if (_ViewOrder != value)
                    _IsDirty = true;
                _ViewOrder = value;
            }
        }
    //    [SortOrder(7)]
        [XmlIgnore()]
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible != value)
                    _IsDirty = true;
                _Visible = value;
            }
        }
        [Browsable(false)]
        [XmlIgnore()]
        public UserVisibilityMode Visibility
        {
            get { return _Visibility; }
            set
            {
                if (_Visibility != value)
                    _IsDirty = true;
                _Visibility = value;
            }
        }
        private void Initialize()
        {
            int defaultVisible = Null.SetNullInteger(UserModuleBase.GetSetting(PortalId, "Profile_DefaultVisibility"));
            if (defaultVisible == Null.NullInteger)
            {
                _Visibility = UserVisibilityMode.AdminOnly;
            }
            else
            {
                switch (defaultVisible)
                {
                    case 0:
                        _Visibility = UserVisibilityMode.AllUsers;
                        break;
                    case 1:
                        _Visibility = UserVisibilityMode.MembersOnly;
                        break;
                    case 2:
                        _Visibility = UserVisibilityMode.AdminOnly;
                        break;
                }
            }
        }
        public void ClearIsDirty()
        {
            _IsDirty = false;
        }
        public ProfilePropertyDefinition Clone()
        {
            ProfilePropertyDefinition objClone = new ProfilePropertyDefinition(this.PortalId);
            objClone.DataType = this.DataType;
            objClone.DefaultValue = this.DefaultValue;
            objClone.Length = this.Length;
            objClone.ModuleDefId = this.ModuleDefId;
            objClone.PropertyCategory = this.PropertyCategory;
            objClone.PropertyDefinitionId = this.PropertyDefinitionId;
            objClone.PropertyName = this.PropertyName;
            objClone.PropertyValue = this.PropertyValue;
            objClone.Required = this.Required;
            objClone.ValidationExpression = this.ValidationExpression;
            objClone.ViewOrder = this.ViewOrder;
            objClone.Visibility = this.Visibility;
            objClone.Visible = this.Visible;
            objClone.ClearIsDirty();
            return objClone;
        }
    }
}
