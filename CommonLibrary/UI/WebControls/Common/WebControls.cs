using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.Common
{
    public enum eClickAction
    {
        PostBack,
        Expand,
        None,
        Navigate
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum Alignment
    {
        Top,
        Left,
        Bottom,
        Right
    }

    public class WebControls
    {

        public static bool IsDesignMode()
        {
            //If Not objControl.Site Is Nothing AndAlso objControl.Site.DesignMode = True Then
            if (System.Web.HttpContext.Current == null)
            {
                return true;
            }
            return false;
        }
    }

}
