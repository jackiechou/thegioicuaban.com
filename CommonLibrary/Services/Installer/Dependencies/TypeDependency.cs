using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public class TypeDependency : DependencyBase
    {
        private string DependentTypes;
        private string DependentType;
        public override string ErrorMessage
        {
            get { return Util.INSTALL_Namespace + " - " + DependentType; }
        }
        public override bool IsValid
        {
            get
            {
                bool _IsValid = true;
                if (!String.IsNullOrEmpty(DependentTypes))
                {
                    foreach (string DependentType in (DependentTypes + ";").Split(';'))
                    {
                        if (!String.IsNullOrEmpty(DependentType.Trim()))
                        {
                            if (Framework.Reflection.CreateType(DependentType, true) == null)
                            {
                                _IsValid = false;
                                break;
                            }
                        }
                    }
                }
                return _IsValid;
            }
        }
        public override void ReadManifest(XPathNavigator dependencyNav)
        {
            DependentTypes = dependencyNav.Value;
        }
    }
}
