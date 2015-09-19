using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.Template
{
    public class TemplateViewManager : System.Web.UI.Page
    {
        public static ContentsResponse RenderView(string path)
        {
            return RenderView(path, null);
        }

        public static ContentsResponse RenderView(string path, object data)
        {
            TemplatePage pageHolder = new TemplatePage();
            TemplateUserControl viewControl = 
                (TemplateUserControl)pageHolder.LoadControl(path);

            if (viewControl == null)
                return ContentsResponse.Empty;

            if (data != null)
            {
                viewControl.Data = data;
            }

            pageHolder.Controls.Add(viewControl);

            string result = "";
            using (StringWriter output = new StringWriter())
            {
                HttpContext.Current.Server.Execute(pageHolder, output, false);
                result = output.ToString();
            }

            return new ContentsResponse(
                    result, 
                    viewControl.StartupScript, 
                    viewControl.CustomStyleSheet
                    );
        }
    }
}