using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.UI
{
    public class ControlUtilities
    {
        public static T FindParentControl<T>(Control control) where T : Control
        {
            T parent = default(T);
            if (control.Parent == null)
            {
                parent = null;
            }
            else
            {
                T parentT = control.Parent as T;
                if (parentT != null)
                {
                    parent = parentT;
                }
                else
                {
                    parent = FindParentControl<T>(control.Parent);
                }
            }
            return parent;
        }

        public static T LoadControl<T>(TemplateControl containerControl, string ControlSrc) where T : Control
        {
            T ctrl;
            if (ControlSrc.ToLower().EndsWith(".ascx"))
            {
                ctrl = (T)containerControl.LoadControl("~/" + ControlSrc);
            }
            else
            {
                System.Type objType = Framework.Reflection.CreateType(ControlSrc);
                ctrl = (T)containerControl.LoadControl(objType, null);
            }
            return ctrl;
        }
    }
}
