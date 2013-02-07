using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using dotObjects.Core.Configuration;
using dotObjects.Core.Processing;
using dotObjects.Security.Configuration;

namespace dotObjects.Security
{
    public abstract class SecurityProvider
    {
        private static SecurityProvider instance;
        private Dictionary<string, string> settings;

        public Dictionary<string, string> Settings
        {
            get { return settings ?? (settings = new Dictionary<string, string>()); }
        }

        public bool IsAuthenticated
        {
            get { return AuthenticationToken != null; }
        }

        public abstract ProcessURI LoginURI { get; }
        public abstract ProcessURI LogoutURI { get; }
        public abstract object AuthenticationToken { get; protected set; }
        public abstract string[] SettingsKeys { get; }

        public static SecurityProvider GetInstance()
        {
            if (instance == null)
            {
                Provider configuration = SecuritySection.Current.Provider;
                string providerNotLoadedMsg = string.Format("Security Provider {0} cannot be loaded!",
                                                            configuration.Type);
                try
                {
                    instance = CreateInstance(configuration);
                }
                catch (Exception ex)
                {
                    throw new Exception(providerNotLoadedMsg, ex);
                }
                if (instance == null)
                    throw new Exception(providerNotLoadedMsg);
            }
            return instance;
        }

        private static SecurityProvider CreateInstance(Provider configuration)
        {
            Type providerType = Type.GetType(configuration.Type);
            SecurityProvider provider = (SecurityProvider) providerType.Assembly.CreateInstance(providerType.FullName);
            if (provider != null)
            {
                foreach (Setting setting in configuration.Settings)
                {
                    if (Array.Find(provider.SettingsKeys, key => key.Equals(setting.Name)) == null)
                        throw new SecurityProviderSettingException(setting.Name, provider);
                    provider.Settings.Add(setting.Name, setting.Value);
                }
            }
            return provider;
        }


        public abstract bool IsInRoles(StringCollection roles);
    }
}