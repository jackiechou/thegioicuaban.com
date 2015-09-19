using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Web;

namespace CommonLibrary.Framework
{
    public class SecurityPolicy
    {
        private static bool m_Initialized = false;
        private static bool m_ReflectionPermission;
        private static bool m_WebPermission;
        private static bool m_AspNetHostingPermission;
        public const string ReflectionPermission = "ReflectionPermission";
        public const string WebPermission = "WebPermission";
        public const string AspNetHostingPermission = "AspNetHostingPermission";
        private static void GetPermissions()
        {
            if (!m_Initialized)
            {
                System.Security.CodeAccessPermission securityTest;
                try
                {
                    securityTest = new ReflectionPermission(PermissionState.Unrestricted);
                    securityTest.Demand();
                    m_ReflectionPermission = true;
                }
                catch
                {
                    m_ReflectionPermission = false;
                }
                try
                {
                    securityTest = new System.Net.WebPermission(PermissionState.Unrestricted);
                    securityTest.Demand();
                    m_WebPermission = true;
                }
                catch
                {
                    m_WebPermission = false;
                }
                try
                {
                    securityTest = new AspNetHostingPermission(AspNetHostingPermissionLevel.Unrestricted);
                    securityTest.Demand();
                    m_AspNetHostingPermission = true;
                }
                catch
                {
                    m_AspNetHostingPermission = false;
                }
                m_Initialized = true;
            }
        }
        public static bool HasAspNetHostingPermission()
        {
            GetPermissions();
            return m_AspNetHostingPermission;
        }
        public static bool HasReflectionPermission()
        {
            GetPermissions();
            return m_ReflectionPermission;
        }
        public static bool HasWebPermission()
        {
            GetPermissions();
            return m_WebPermission;
        }
        public static bool HasPermissions(string permissions, ref string permission)
        {
            bool _HasPermission = true;
            if (!String.IsNullOrEmpty(permissions))
            {
                foreach (string per in (permissions + ";").Split(Convert.ToChar(";")))
                {
                    permission = per;
                    if (!String.IsNullOrEmpty(permission.Trim()))
                    {
                        switch (permission)
                        {
                            case AspNetHostingPermission:
                                if (HasAspNetHostingPermission() == false)
                                {
                                    _HasPermission = false;
                                }
                                break;
                            case ReflectionPermission:
                                if (HasReflectionPermission() == false)
                                {
                                    _HasPermission = false;
                                }
                                break;
                            case WebPermission:
                                if (HasWebPermission() == false)
                                {
                                    _HasPermission = false;
                                }
                                break;
                        }
                    }
                }
            }
            return _HasPermission;
        }
        public static string Permissions
        {
            get
            {
                string strPermissions = "";
                if (Framework.SecurityPolicy.HasReflectionPermission())
                {
                    strPermissions += ", " + Framework.SecurityPolicy.ReflectionPermission;
                }
                if (Framework.SecurityPolicy.HasWebPermission())
                {
                    strPermissions += ", " + Framework.SecurityPolicy.WebPermission;
                }
                if (Framework.SecurityPolicy.HasAspNetHostingPermission())
                {
                    strPermissions += ", " + Framework.SecurityPolicy.AspNetHostingPermission;
                }
                if (!String.IsNullOrEmpty(strPermissions))
                {
                    strPermissions = strPermissions.Substring(2);
                }
                return strPermissions;
            }
        }
    }
}
