using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using CommonLibrary.Common.Utilities;
using System.Web.UI;

namespace CommonLibrary.Framework
{
    public class CachePageStatePersister : PageStatePersister
    {
        private const string VIEW_STATE_CACHEKEY = "__VIEWSTATE_CACHEKEY";
        public CachePageStatePersister(Page page)
            : base(page)
        {
        }
        public override void Load()
        {
            string key = Page.Request.Params[VIEW_STATE_CACHEKEY] as string;
            if (string.IsNullOrEmpty(key) || !key.StartsWith("VS_"))
            {
                throw new ApplicationException("Missing valid " + VIEW_STATE_CACHEKEY);
            }
            Pair state = DataCache.GetCache<Pair>(key);
            if (state != null)
            {
                ViewState = state.First;
                ControlState = state.Second;
            }
            DataCache.RemoveCache(key);
        }
        public override void Save()
        {
            if (ViewState == null && ControlState == null)
            {
                return;
            }
            StringBuilder key = new StringBuilder();
            {
                key.Append("VS_");
                key.Append(Page.Session == null ? Guid.NewGuid().ToString() : Page.Session.SessionID);
                key.Append("_");
                key.Append(DateTime.Now.Ticks.ToString());
            }
            Pair state = new Pair(ViewState, ControlState);
            //DataCache.SetCache(key.ToString(), state, objDependency, DateTime.Now.AddMinutes(Page.Session.Timeout), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
            Page.ClientScript.RegisterHiddenField(VIEW_STATE_CACHEKEY, key.ToString());
        }
    }
}
