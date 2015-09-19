using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common;

namespace CommonLibrary.Entities.Portal
{
    public class PortalTemplateValidator : XmlValidatorBase
    {
        public bool Validate(string xmlFilename, string schemaFileName)
        {
            SchemaSet.Add("", schemaFileName);
            return Validate(xmlFilename);
        }
    }
}
