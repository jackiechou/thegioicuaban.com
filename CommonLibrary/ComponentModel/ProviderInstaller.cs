using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CommonLibrary.Framework.Providers;

namespace CommonLibrary.ComponentModel
{
    public class ProviderInstaller : IComponentInstaller
    {
        private ComponentLifeStyleType _ComponentLifeStyle;
        private Type _ProviderInterface;
        private string _ProviderType;
        public ProviderInstaller(string providerType, Type providerInterface)
        {
            this._ComponentLifeStyle = ComponentLifeStyleType.Singleton;
            this._ProviderType = providerType;
            this._ProviderInterface = providerInterface;
        }
        public ProviderInstaller(string providerType, Type providerInterface, ComponentLifeStyleType lifeStyle)
        {
            this._ComponentLifeStyle = lifeStyle;
            this._ProviderType = providerType;
            this._ProviderInterface = providerInterface;
        }
        public void InstallComponents(IContainer container)
        {
            ProviderConfiguration config = ProviderConfiguration.GetProviderConfiguration(_ProviderType);
            if (config != null)
            {
                InstallProvider(container, (Provider)config.Providers[config.DefaultProvider]);
                foreach (Provider provider in config.Providers.Values)
                {
                    if (!config.DefaultProvider.Equals(provider.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        InstallProvider(container, provider);
                    }
                }
            }
        }
        private void InstallProvider(IContainer container, Provider provider)
        {
            if (provider != null)
            {
                Type type = System.Web.Compilation.BuildManager.GetType(provider.Type, false, true);
                if (type == null)
                {
                    throw new ConfigurationErrorsException(string.Format("Could not load provider {0}", provider.Type));
                }
                container.RegisterComponent(provider.Name, _ProviderInterface, type, _ComponentLifeStyle);
                Dictionary<string, string> settingsDict = new Dictionary<string, string>();
                settingsDict.Add("providerName", provider.Name);
                foreach (string key in provider.Attributes.Keys)
                {
                    settingsDict.Add(key, provider.Attributes.Get(key));
                }
                container.RegisterComponentSettings(type.FullName, settingsDict);
            }
        }
    }
}
