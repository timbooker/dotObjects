using dotObjects.Core;
using dotObjects.Core.Persistence;
using System;
using dotObjects.Core.Processing;
using dotObjects.Security.Web.Helpers;

namespace dotObjects.History
{

    public class ChangeInterceptor : IInterceptor
    {
        #region IProcessInterceptor Members

        public void OnApplicationStart()
        {
        }

        public void OnProcessExecuting(ProcessURI uri, object obj)
        {
            switch (uri.Behavior)
            {
                case ProcessBehavior.Edit:
                case ProcessBehavior.Delete:
                case ProcessBehavior.Exec:
                    SaveChange(uri, obj);
                    break;
            }

        }

        public void OnProcessExecuted(ProcessURI uri, object obj)
        {
            switch (uri.Behavior)
            {
                case ProcessBehavior.New:
                    SaveChange(uri, obj);
                    break;
            }
        }

        #endregion

        private static void SaveChange(ProcessURI uri, object obj)
        {
            if (obj != null)
            {
                object userId = null;
                if (SecurityHelper.IsAuthenticated)
                {
                    var token = SecurityHelper.AuthenticationToken;
                    userId = ContextFactory.GetContext(token.GetType()).GetID(token);
                }
                var entityId = ContextFactory.GetContext(obj.GetType()).GetID(obj);

                var change = new EntityChange(DateTime.Now, uri.ToString(), entityId, obj, userId != null ? userId.ToString() : null);
                ContextFactory.GetContext(typeof(EntityChange)).Insert(change);
            }
        }
    }
}
