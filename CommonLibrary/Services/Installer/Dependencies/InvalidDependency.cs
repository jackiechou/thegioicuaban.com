using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public class InvalidDependency : DependencyBase
    {
        private string _ErrorMessage;
        public InvalidDependency(string ErrorMessage)
        {
            this._ErrorMessage = ErrorMessage;
        }
        public override string ErrorMessage
        {
            get { return this._ErrorMessage; }
        }
        public override bool IsValid
        {
            get { return false; }
        }
    }
}
