using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using CommonLibrary.Common;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Framework
{
    public class DiskPageStatePersister : PageStatePersister
    {
        public DiskPageStatePersister(Page page)
            : base(page)
        {
        }
        public string CacheDirectory
        {
            get { return PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath + "Cache"; }
        }
        public string StateFileName
        {
            get
            {
                StringBuilder key = new StringBuilder();
                {
                    key.Append("VIEWSTATE_");
                    key.Append(Page.Session.SessionID);
                    key.Append("_");
                    key.Append(Page.Request.RawUrl);
                }
                return CacheDirectory + "\\" + Globals.CleanFileName(key.ToString()) + ".txt";
            }
        }
        public override void Load()
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(StateFileName);
                string serializedStatePair = reader.ReadToEnd();
                IStateFormatter formatter = this.StateFormatter;
                Pair statePair = (Pair)formatter.Deserialize(serializedStatePair);
                ViewState = statePair.First;
                ControlState = statePair.Second;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
        public override void Save()
        {
            if (ViewState == null && ControlState == null)
            {
                return;
            }
            if (Page.Session != null)
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    Directory.CreateDirectory(CacheDirectory);
                }
                StreamWriter writer = new StreamWriter(StateFileName, false);
                IStateFormatter formatter = this.StateFormatter;
                Pair statePair = new Pair(ViewState, ControlState);
                string serializedState = formatter.Serialize(statePair);
                writer.Write(serializedState);
                writer.Close();
            }
        }
    }
}
