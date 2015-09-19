using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Modules
{
    public class ProfileUserControlBase : UserModuleBase
    {
        public event EventHandler ProfileUpdated;
        public event EventHandler ProfileUpdateCompleted;
        public void OnProfileUpdateCompleted(EventArgs e)
        {
            if (ProfileUpdateCompleted != null)
            {
                ProfileUpdateCompleted(this, e);
            }
        }
        public void OnProfileUpdated(EventArgs e)
        {
            if (ProfileUpdated != null)
            {
                ProfileUpdated(this, e);
            }
        }
    }
}
