using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security;
using dotObjects.Core.Configuration;
using dotObjects.Core.Persistence;
using dotObjects.Core.Processing;

namespace dotObjects.Security
{
    public abstract class ModelSecurityProvider : SecurityProvider
    {
        private const string IdentityType = "identityType";
        private const string UsernameAttribute = "usernameAttribute";
        private const string PasswordAttribute = "passwordAttribute";
        private const string RolesAttribute = "rolesAttribute";

        private string IdentityTypeName
        {
            get { return Settings[IdentityType]; }
        }

        private string UsernameFieldName
        {
            get { return Settings[UsernameAttribute]; }
        }

        private string PasswordFieldName
        {
            get { return Settings[PasswordAttribute]; }
        }

        public override ProcessURI LoginURI
        {
            get { return new ProcessURI(GetType().Name, ProcessBehavior.Exec, "Login", "0"); }
        }

        public override ProcessURI LogoutURI
        {
            get { return new ProcessURI(GetType().Name, ProcessBehavior.Exec, "Logout", "0"); }
        }

        public override string[] SettingsKeys
        {
            get { return new[] {IdentityType, UsernameAttribute, PasswordAttribute, RolesAttribute}; }
        }


        public override bool IsInRoles(StringCollection roles)
        {
            if (!IsAuthenticated) return false;
            if (roles.Count == 0) return true;

            var rolesValue = GetRolesProperty().GetValue(AuthenticationToken, null) as string ?? "";
            return rolesValue.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Any(roles.Contains);
        }

        private PropertyInfo GetRolesProperty()
        {
            if (AuthenticationToken == null) return null;

            PropertyInfo rolesProperty;
            var tokenType = AuthenticationToken.GetType();
            do
            {
                rolesProperty = tokenType.GetProperty(Settings[RolesAttribute]);
                tokenType = tokenType.BaseType;
            } while (rolesProperty == null && tokenType != typeof (object));
            return rolesProperty;
        }

        public static void Login(string username, string password)
        {
            var provider = (ModelSecurityProvider) GetInstance();
            Type type = CoreSection.Current.FindType(provider.IdentityTypeName);

            if (type == null)
                throw new SecurityException(Localization.AccessForbidden);

            Query query = CreateAuthenticationQuery(username, password, provider, type);

            object token = query.ExecuteSingleResult();
            if (token == null)
                throw new SecurityException(Localization.AccessForbidden);

            provider.AuthenticationToken = token;
        }

        public static void Logout()
        {
            var provider = (ModelSecurityProvider) GetInstance();
            provider.AuthenticationToken = null;
        }

        protected static Query CreateAuthenticationQuery(string username, string password,
                                                         ModelSecurityProvider provider, Type type)
        {
            Query query = ContextFactory.GetContext(type).CreateQuery(type);
            query.Where = new Equals(provider.UsernameFieldName, username).And(new Equals(provider.PasswordFieldName, password));
            return query;
        }
    }
}