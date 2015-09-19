using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.UI.WebControls
{
    public class LiteralTemplate : ITemplate
    {
        private string m_strHTML = "";
        private Control m_objControl;
        public LiteralTemplate(string html)
        {
            m_strHTML = html;
        }
        public LiteralTemplate(Control ctl)
        {
            m_objControl = ctl;
        }
        public void InstantiateIn(System.Web.UI.Control container)
        {
            if (m_objControl == null)
            {
                container.Controls.Add(new LiteralControl(m_strHTML));
            }
            else
            {
                container.Controls.Add(m_objControl);
            }
        }
    }
}
