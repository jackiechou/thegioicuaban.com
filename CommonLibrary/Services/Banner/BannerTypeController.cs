using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Services.Banner
{
    public class BannerTypeController
    {
        public ArrayList GetBannerTypes()
        {
            ArrayList arrBannerTypes = new ArrayList();
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Banner, Services.Localization.Localization.GetString("BannerType.Banner.String", Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.MicroButton, Services.Localization.Localization.GetString("BannerType.MicroButton.String", Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Button, Services.Localization.Localization.GetString("BannerType.Button.String", Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Block, Services.Localization.Localization.GetString("BannerType.Block.String", Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Skyscraper, Services.Localization.Localization.GetString("BannerType.Skyscraper.String", Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Text, Services.Localization.Localization.GetString("BannerType.Text.String", Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Script, Services.Localization.Localization.GetString("BannerType.Script.String", Services.Localization.Localization.GlobalResourceFile)));
            return arrBannerTypes;
        }
    }
}
