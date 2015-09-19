using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common.Lists;
using System.Data;
using System.Collections;
using CommonLibrary.Data;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security.Profile;


namespace CommonLibrary.Entities.Profile
{
    public class ProfileController
    {
        private static DataProvider provider = DataProvider.Instance();
        private static ProfileProvider profileProvider = ProfileProvider.Instance();
        private static int _orderCounter;
        private static void AddDefaultDefinition(int PortalId, string category, string name, string strType, int length, ListEntryInfoCollection types)
        {
            _orderCounter += 2;
            AddDefaultDefinition(PortalId, category, name, strType, length, _orderCounter, types);
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a single default property definition
        /// </summary>
        /// <param name="PortalId">Id of the Portal</param>
        /// <param name="category">Category of the Property</param>
        /// <param name="name">Name of the Property</param>
        /// <history>
        ///     [cnurse]	02/22/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        internal static void AddDefaultDefinition(int PortalId, string category, string name, string strType, int length, int viewOrder, ListEntryInfoCollection types)
        {
            ListEntryInfo typeInfo = types["DataType:" + strType];
            if (typeInfo == null)
            {
                typeInfo = types["DataType:Unknown"];
            }
            ProfilePropertyDefinition propertyDefinition = new ProfilePropertyDefinition(PortalId);
            propertyDefinition.DataType = typeInfo.EntryID;
            propertyDefinition.DefaultValue = "";
            propertyDefinition.ModuleDefId = Null.NullInteger;
            propertyDefinition.PropertyCategory = category;
            propertyDefinition.PropertyName = name;
            propertyDefinition.Required = false;
            propertyDefinition.Visible = true;
            propertyDefinition.Length = length;
            propertyDefinition.ViewOrder = viewOrder;
            AddPropertyDefinition(propertyDefinition);
        }
        private static ProfilePropertyDefinitionCollection FillCollection(IDataReader dr)
        {
            ArrayList arrDefinitions = CBO.FillCollection(dr, typeof(ProfilePropertyDefinition));
            ProfilePropertyDefinitionCollection definitionsCollection = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition definition in arrDefinitions)
            {
                definition.ClearIsDirty();
                object setting = UserModuleBase.GetSetting(definition.PortalId, "Profile_DefaultVisibility");
                if (setting != null)
                {
                    definition.Visibility = (UserVisibilityMode)setting;
                }
                definitionsCollection.Add(definition);
            }
            return definitionsCollection;
        }
        private static ProfilePropertyDefinition FillPropertyDefinitionInfo(IDataReader dr)
        {
            ProfilePropertyDefinition definition = null;
            try
            {
                definition = FillPropertyDefinitionInfo(dr, false);
            }
            catch
            {
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return definition;
        }
        private static ProfilePropertyDefinition FillPropertyDefinitionInfo(IDataReader dr, bool checkForOpenDataReader)
        {
            ProfilePropertyDefinition definition = null;
            bool canContinue = true;
            if (checkForOpenDataReader)
            {
                canContinue = false;
                if (dr.Read())
                {
                    canContinue = true;
                }
            }
            if (canContinue)
            {
                int portalid = 0;
                portalid = Convert.ToInt32(Null.SetNull(dr["PortalId"], portalid));
                definition = new ProfilePropertyDefinition(portalid);
                definition.PropertyDefinitionId = Convert.ToInt32(Null.SetNull(dr["PropertyDefinitionId"], definition.PropertyDefinitionId));
                definition.ModuleDefId = Convert.ToInt32(Null.SetNull(dr["ModuleDefId"], definition.ModuleDefId));
                definition.DataType = Convert.ToInt32(Null.SetNull(dr["DataType"], definition.DataType));
                definition.DefaultValue = Convert.ToString(Null.SetNull(dr["DefaultValue"], definition.DefaultValue));
                definition.PropertyCategory = Convert.ToString(Null.SetNull(dr["PropertyCategory"], definition.PropertyCategory));
                definition.PropertyName = Convert.ToString(Null.SetNull(dr["PropertyName"], definition.PropertyName));
                definition.Length = Convert.ToInt32(Null.SetNull(dr["Length"], definition.Length));
                definition.Required = Convert.ToBoolean(Null.SetNull(dr["Required"], definition.Required));
                definition.ValidationExpression = Convert.ToString(Null.SetNull(dr["ValidationExpression"], definition.ValidationExpression));
                definition.ViewOrder = Convert.ToInt32(Null.SetNull(dr["ViewOrder"], definition.ViewOrder));
                definition.Visible = Convert.ToBoolean(Null.SetNull(dr["Visible"], definition.Visible));
            }
            return definition;
        }
        private static List<ProfilePropertyDefinition> FillPropertyDefinitionInfoCollection(IDataReader dr)
        {
            List<ProfilePropertyDefinition> arr = new List<ProfilePropertyDefinition>();
            try
            {
                ProfilePropertyDefinition obj;
                while (dr.Read())
                {
                    obj = FillPropertyDefinitionInfo(dr, false);
                    arr.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return arr;
        }
        private static List<ProfilePropertyDefinition> GetPropertyDefinitions(int portalId)
        {
            string key = string.Format(DataCache.ProfileDefinitionsCacheKey, portalId);
            List<ProfilePropertyDefinition> definitions = (List<ProfilePropertyDefinition>)DataCache.GetCache(key);
            if (definitions == null)
            {
                Int32 timeOut = DataCache.ProfileDefinitionsCacheTimeOut * Convert.ToInt32(Host.Host.PerformanceSetting);
                definitions = FillPropertyDefinitionInfoCollection(provider.GetPropertyDefinitionsByPortal(portalId));
                if (timeOut > 0)
                {
                    DataCache.SetCache(key, definitions, TimeSpan.FromMinutes(timeOut));
                }
            }
            return definitions;
        }
        public static void GetUserProfile(UserInfo objUser)
        {
            profileProvider.GetUserProfile(ref objUser);
        }
        public static void UpdateUserProfile(UserInfo objUser)
        {
            if (objUser.Profile.IsDirty)
            {
                profileProvider.UpdateUserProfile(objUser);
            }
            DataCache.ClearUserCache(objUser.PortalID, objUser.Username);
        }
        public static UserInfo UpdateUserProfile(UserInfo objUser, ProfilePropertyDefinitionCollection profileProperties)
        {
            bool updateUser = Null.NullBoolean;
            if (profileProperties != null)
            {
                foreach (ProfilePropertyDefinition propertyDefinition in profileProperties)
                {
                    string propertyName = propertyDefinition.PropertyName;
                    string propertyValue = propertyDefinition.PropertyValue;
                    if (propertyDefinition.IsDirty)
                    {
                        objUser.Profile.SetProfileProperty(propertyName, propertyValue);
                        if (propertyName.ToLower() == "firstname" || propertyName.ToLower() == "lastname")
                        {
                            updateUser = true;
                        }
                    }
                }
                UpdateUserProfile(objUser);
                if (updateUser)
                {
                    UserController.UpdateUser(objUser.PortalID, objUser);
                }
            }
            return objUser;
        }
        public static bool ValidateProfile(int portalId, UserProfile objProfile)
        {
            bool isValid = true;
            foreach (ProfilePropertyDefinition propertyDefinition in objProfile.ProfileProperties)
            {
                if (propertyDefinition.Required && propertyDefinition.PropertyValue == Null.NullString)
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }
        public static void AddDefaultDefinitions(int PortalId)
        {
            _orderCounter = 1;
            ListController objListController = new ListController();
            ListEntryInfoCollection dataTypes = objListController.GetListEntryInfoCollection("DataType");
            AddDefaultDefinition(PortalId, "Name", "Prefix", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "FirstName", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "MiddleName", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "LastName", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "Suffix", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Unit", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Street", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "City", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Region", "Region", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Country", "Country", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "PostalCode", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Telephone", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Cell", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Fax", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Website", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "IM", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "Photo", "Image", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "Biography", "RichText", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "TimeZone", "TimeZone", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "PreferredLocale", "Locale", 0, dataTypes);
        }
        public static int AddPropertyDefinition(ProfilePropertyDefinition definition)
        {
            if (definition.Required)
            {
                definition.Visible = true;
            }
            int intDefinition = provider.AddPropertyDefinition(definition.PortalId, definition.ModuleDefId, definition.DataType, definition.DefaultValue, definition.PropertyCategory, definition.PropertyName, definition.Required, definition.ValidationExpression, definition.ViewOrder, definition.Visible,
            definition.Length, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PROFILEPROPERTY_CREATED);
            ClearProfileDefinitionCache(definition.PortalId);
            return intDefinition;
        }
        public static void ClearProfileDefinitionCache(int PortalId)
        {
            DataCache.ClearDefinitionsCache(PortalId);
        }
        public static void DeletePropertyDefinition(ProfilePropertyDefinition definition)
        {
            provider.DeletePropertyDefinition(definition.PropertyDefinitionId);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PROFILEPROPERTY_DELETED);
            ClearProfileDefinitionCache(definition.PortalId);
        }
        public static ProfilePropertyDefinition GetPropertyDefinition(int definitionId, int portalId)
        {
            bool bFound = Null.NullBoolean;
            ProfilePropertyDefinition definition = null;
            foreach (ProfilePropertyDefinition def in GetPropertyDefinitions(portalId))
            {
                if (def.PropertyDefinitionId == definitionId)
                {
                    definition = def;
                    bFound = true;
                    break;
                }
            }
            if (!bFound)
            {

                definition = FillPropertyDefinitionInfo(provider.GetPropertyDefinition(definitionId));
            }
            return definition;
        }
        public static ProfilePropertyDefinition GetPropertyDefinitionByName(int portalId, string name)
        {
            bool bFound = Null.NullBoolean;
            ProfilePropertyDefinition definition = null;
            foreach (ProfilePropertyDefinition def in GetPropertyDefinitions(portalId))
            {
                if (def.PropertyName == name)
                {
                    definition = def;
                    bFound = true;
                    break;
                }
            }
            if (!bFound)
            {
                definition = FillPropertyDefinitionInfo(provider.GetPropertyDefinitionByName(portalId, name));
            }
            return definition;
        }
        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByCategory(int portalId, string category)
        {
            ProfilePropertyDefinitionCollection definitions = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition definition in GetPropertyDefinitions(portalId))
            {
                if (definition.PropertyCategory == category)
                {
                    definitions.Add(definition);
                }
            }
            return definitions;
        }
        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByPortal(int portalId)
        {
            return GetPropertyDefinitionsByPortal(portalId, true);
        }
        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByPortal(int portalId, bool clone)
        {
            ProfilePropertyDefinitionCollection definitions = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition definition in GetPropertyDefinitions(portalId))
            {
                if (clone)
                {
                    definitions.Add(definition.Clone());
                }
                else
                {
                    definitions.Add(definition);
                }
            }
            return definitions;
        }
        public static void UpdatePropertyDefinition(ProfilePropertyDefinition definition)
        {
            if (definition.Required)
            {
                definition.Visible = true;
            }
            provider.UpdatePropertyDefinition(definition.PropertyDefinitionId, definition.DataType, definition.DefaultValue, definition.PropertyCategory, definition.PropertyName, definition.Required, definition.ValidationExpression, definition.ViewOrder, definition.Visible, definition.Length,
            UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PROFILEPROPERTY_UPDATED);
            ClearProfileDefinitionCache(definition.PortalId);
        }
    }
}
