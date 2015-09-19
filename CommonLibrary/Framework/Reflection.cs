using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Exceptions;
using System.Web.Compilation;
using CommonLibrary.Framework.Providers;

namespace CommonLibrary.Framework
{
    public class Reflection
    {
        public static object CreateObject(string ObjectProviderType)
        {
            return CreateObject(ObjectProviderType, true);
        }
        public static object CreateObject(string ObjectProviderType, bool UseCache)
        {
            return CreateObject(ObjectProviderType, "", "", "", UseCache);
        }
        public static object CreateObject(string ObjectProviderType, string ObjectNamespace, string ObjectAssemblyName)
        {
            return CreateObject(ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, true);
        }
        public static object CreateObject(string ObjectProviderType, string ObjectNamespace, string ObjectAssemblyName, bool UseCache)
        {
            return CreateObject(ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, UseCache);
        }
        public static object CreateObject(string ObjectProviderType, string ObjectProviderName, string ObjectNamespace, string ObjectAssemblyName)
        {
            return CreateObject(ObjectProviderType, ObjectProviderName, ObjectNamespace, ObjectAssemblyName, true);
        }
        public static object CreateObject(string ObjectProviderType, string ObjectProviderName, string ObjectNamespace, string ObjectAssemblyName, bool UseCache)
        {
            string TypeName = "";
            ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ObjectProviderType);
            if (!String.IsNullOrEmpty(ObjectNamespace) && !String.IsNullOrEmpty(ObjectAssemblyName))
            {
                if (String.IsNullOrEmpty(ObjectProviderName))
                {
                    TypeName = ObjectNamespace + "." + objProviderConfiguration.DefaultProvider + ", " + ObjectAssemblyName + "." + objProviderConfiguration.DefaultProvider;
                }
                else
                {
                    TypeName = ObjectNamespace + "." + ObjectProviderName + ", " + ObjectAssemblyName + "." + ObjectProviderName;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(ObjectNamespace))
                {
                    if (String.IsNullOrEmpty(ObjectProviderName))
                    {
                        TypeName = ObjectNamespace + "." + objProviderConfiguration.DefaultProvider;
                    }
                    else
                    {
                        TypeName = ObjectNamespace + "." + ObjectProviderName;
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(ObjectProviderName))
                    {
                        TypeName = ((Provider)objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider]).Type;
                    }
                    else
                    {
                        TypeName = ((Provider)objProviderConfiguration.Providers[ObjectProviderName]).Type;
                    }
                }
            }
            return CreateObject(TypeName, TypeName, UseCache);
        }
        public static object CreateObject(string TypeName, string CacheKey)
        {
            return CreateObject(TypeName, CacheKey, true);
        }
        public static object CreateObject(string TypeName, string CacheKey, bool UseCache)
        {
            return Activator.CreateInstance(CreateType(TypeName, CacheKey, UseCache));
        }
        public static T CreateObject<T>()
        {
            return Activator.CreateInstance<T>();
        }
        public static Type CreateType(string TypeName)
        {
            return CreateType(TypeName, "", true, false);
        }
        public static Type CreateType(string TypeName, bool IgnoreErrors)
        {
            return CreateType(TypeName, "", true, IgnoreErrors);
        }
        public static Type CreateType(string TypeName, string CacheKey, bool UseCache)
        {
            return CreateType(TypeName, CacheKey, UseCache, false);
        }
        public static Type CreateType(string TypeName, string CacheKey, bool UseCache, bool IgnoreErrors)
        {
            if (String.IsNullOrEmpty(CacheKey))
            {
                CacheKey = TypeName;
            }
            Type objType = null;
            if (UseCache)
            {
                objType = (Type)DataCache.GetCache(CacheKey);
            }
            if (objType == null)
            {
                try
                {
                    objType = BuildManager.GetType(TypeName, true, true);
                    if (UseCache)
                    {
                        DataCache.SetCache(CacheKey, objType);
                    }
                }
                catch (Exception exc)
                {
                    if (!IgnoreErrors)
                    {
                        Exceptions.LogException(exc);
                    }
                }
            }
            return objType;
        }
        public static object CreateInstance(Type Type)
        {
            if (Type != null)
            {
                return Type.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, null, null, null);
            }
            else
            {
                return null;
            }
        }
        public static object GetProperty(Type Type, string PropertyName, object Target)
        {
            if (Type != null)
            {
                return Type.InvokeMember(PropertyName, System.Reflection.BindingFlags.GetProperty, null, Target, null);
            }
            else
            {
                return null;
            }
        }
        public static void SetProperty(Type Type, string PropertyName, object Target, object[] Args)
        {
            if (Type != null)
            {
                Type.InvokeMember(PropertyName, System.Reflection.BindingFlags.SetProperty, null, Target, Args);
            }
        }
        public static void InvokeMethod(Type Type, string PropertyName, object Target, object[] Args)
        {
            if (Type != null)
            {
                Type.InvokeMember(PropertyName, System.Reflection.BindingFlags.InvokeMethod, null, Target, Args);
            }
        }
    }
}
