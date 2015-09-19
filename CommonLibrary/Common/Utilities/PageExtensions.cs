using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.Common.Utilities
{
    public static class PageExtensions
    {

        public static bool IsAsyncPostBack(Page page)
        {

            var result = false;

            var scriptManager = ScriptManager.GetCurrent(page);

            if (scriptManager != null)
            {

                result = scriptManager.IsInAsyncPostBack;

            }

            return result;

        }

    }
}
