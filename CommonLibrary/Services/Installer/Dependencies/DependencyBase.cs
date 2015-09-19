using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public abstract class DependencyBase : IDependency
    {
        public virtual string ErrorMessage
        {
            get { return Null.NullString; }
        }
        public virtual bool IsValid
        {
            get { return true; }
        }
        public virtual void ReadManifest(XPathNavigator dependencyNav)
        {
        }
    }
}
