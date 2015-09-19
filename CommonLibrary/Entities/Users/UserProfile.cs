using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Profile;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Entities.Users
{
    [Serializable()]
    public class UserProfile
    {
        private const string cPrefix = "Prefix";
        private const string cFirstName = "FirstName";
        private const string cMiddleName = "MiddleName";
        private const string cLastName = "LastName";
        private const string cSuffix = "Suffix";
        private const string cUnit = "Unit";
        private const string cStreet = "Street";
        private const string cCity = "City";
        private const string cRegion = "Region";
        private const string cCountry = "Country";
        private const string cPostalCode = "PostalCode";
        private const string cTelephone = "Telephone";
        private const string cCell = "Cell";
        private const string cFax = "Fax";
        private const string cWebsite = "Website";
        private const string cIM = "IM";
        private const string cPhoto = "Photo";
        private const string cTimeZone = "TimeZone";
        private const string cPreferredLocale = "PreferredLocale";
        private bool _IsDirty;
        private bool _ObjectHydrated;
        private int UserID;
        private ProfilePropertyDefinitionCollection _profileProperties;
        public UserProfile()
        {
        }
        public string Cell
        {
            get { return GetPropertyValue(cCell); }
            set { SetProfileProperty(cCell, value); }
        }
        public string City
        {
            get { return GetPropertyValue(cCity); }
            set { SetProfileProperty(cCity, value); }
        }
        public string Country
        {
            get { return GetPropertyValue(cCountry); }
            set { SetProfileProperty(cCountry, value); }
        }
        public string Fax
        {
            get { return GetPropertyValue(cFax); }
            set { SetProfileProperty(cFax, value); }
        }
        public string FirstName
        {
            get { return GetPropertyValue(cFirstName); }
            set { SetProfileProperty(cFirstName, value); }
        }
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public string IM
        {
            get { return GetPropertyValue(cIM); }
            set { SetProfileProperty(cIM, value); }
        }
        public bool IsDirty
        {
            get { return _IsDirty; }
        }
        public string LastName
        {
            get { return GetPropertyValue(cLastName); }
            set { SetProfileProperty(cLastName, value); }
        }
        public string Photo
        {
            get { return GetPropertyValue(cPhoto); }
            set { SetProfileProperty(cPhoto, value); }
        }
        public string PostalCode
        {
            get { return GetPropertyValue(cPostalCode); }
            set { SetProfileProperty(cPostalCode, value); }
        }
        public string PreferredLocale
        {
            get { return GetPropertyValue(cPreferredLocale); }
            set { SetProfileProperty(cPreferredLocale, value); }
        }
        public ProfilePropertyDefinitionCollection ProfileProperties
        {
            get
            {
                if (_profileProperties == null)
                {
                    _profileProperties = new ProfilePropertyDefinitionCollection();
                }
                return _profileProperties;
            }
        }
        public string Region
        {
            get { return GetPropertyValue(cRegion); }
            set { SetProfileProperty(cRegion, value); }
        }
        public string Street
        {
            get { return GetPropertyValue(cStreet); }
            set { SetProfileProperty(cStreet, value); }
        }
        public string Telephone
        {
            get { return GetPropertyValue(cTelephone); }
            set { SetProfileProperty(cTelephone, value); }
        }
        public int TimeZone
        {
            get
            {
                Int32 retValue = Null.NullInteger;
                string propValue = GetPropertyValue(cTimeZone);
                if (propValue != null)
                {
                    retValue = int.Parse(propValue);
                }
                return retValue;
            }
            set { SetProfileProperty(cTimeZone, value.ToString()); }
        }
        public string Unit
        {
            get { return GetPropertyValue(cUnit); }
            set { SetProfileProperty(cUnit, value); }
        }
        public string Website
        {
            get { return GetPropertyValue(cWebsite); }
            set { SetProfileProperty(cWebsite, value); }
        }
        public void ClearIsDirty()
        {
            _IsDirty = false;
            foreach (ProfilePropertyDefinition profProperty in ProfileProperties)
            {
                profProperty.ClearIsDirty();
            }
        }
        public ProfilePropertyDefinition GetProperty(string propName)
        {
            return ProfileProperties[propName];
        }
        public string GetPropertyValue(string propName)
        {
            string propValue = Null.NullString;
            ProfilePropertyDefinition profileProp = GetProperty(propName);
            if (profileProp != null)
            {
                propValue = profileProp.PropertyValue;
            }
            return propValue;
        }
        public void InitialiseProfile(int portalId)
        {
            InitialiseProfile(portalId, true);
        }
        public void InitialiseProfile(int portalId, bool useDefaults)
        {
            _profileProperties = ProfileController.GetPropertyDefinitionsByPortal(portalId, true);
            if (useDefaults)
            {
                foreach (ProfilePropertyDefinition ProfileProperty in _profileProperties)
                {
                    ProfileProperty.PropertyValue = ProfileProperty.DefaultValue;
                }
            }
        }
        public void SetProfileProperty(string propName, string propvalue)
        {
            ProfilePropertyDefinition profileProp = GetProperty(propName);
            if (profileProp != null)
            {
                profileProp.PropertyValue = propvalue;
                if (profileProp.IsDirty)
                    _IsDirty = true;
            }
        }
    }
}
