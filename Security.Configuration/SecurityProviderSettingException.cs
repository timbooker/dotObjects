using System;

namespace dotObjects.Security
{
    internal class SecurityProviderSettingException : Exception
    {
        public SecurityProviderSettingException(string setting, SecurityProvider provider)
            : base(string.Format("The setting {0} doesn't exist! Available settings: {1}",
                                 setting, provider.SettingsKeys))
        {
        }
    }
}