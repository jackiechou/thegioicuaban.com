using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web;
using System.Reflection;
using System.Web.UI.HtmlControls;

namespace CommonLibrary.UI.WebControls
{
    public static class ControlExtenders 
    {
        public static string RenderControl(string path)
        {
            return RenderControl(path, null);
        }

        public static string RenderControl(string path, object data)
        {
            Page pageHolder = new Page();
            pageHolder.EnableEventValidation = false;

            UserControl viewControl = (UserControl)pageHolder.LoadControl(path);
            if (data != null)
            {
                Type viewControlType = viewControl.GetType();
                FieldInfo field = viewControlType.GetField("Data");

                if (field != null)
                {
                    field.SetValue(viewControl, data);
                }
                else
                {
                    throw new Exception("View file: " + path + " does not have a public Data property");
                }
            }
            
            HtmlForm _form = new HtmlForm();
            pageHolder.Controls.Add(_form);
            _form.Controls.Add(viewControl);

            StringWriter output = new StringWriter();
            HttpContext.Current.Server.Execute(pageHolder, output, false);           
            return output.ToString();
        }

        public static string RenderControl(string path, string propertyName, object propertyValue)
        {
            Page pageHolder = new Page();
            pageHolder.EnableEventValidation = false;

            UserControl viewControl =
                (UserControl)pageHolder.LoadControl(path);

            if (propertyValue != null)
            {
                Type viewControlType = viewControl.GetType();
                PropertyInfo property =
                    viewControlType.GetProperty(propertyName);

                if (property != null)
                {
                    property.SetValue(viewControl, propertyValue, null);
                }
                else
                {
                    throw new Exception(string.Format(
                        "UserControl: {0} does not have a public {1} property.", path, propertyName));
                }
            }

            HtmlForm _form = new HtmlForm();
            pageHolder.Controls.Add(_form);
            _form.Controls.Add(viewControl);

            StringWriter output = new StringWriter();
            HttpContext.Current.Server.Execute(pageHolder, output, false);                          
            return output.ToString();
        }
    }
}
