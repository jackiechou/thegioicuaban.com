using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;
using CommonLibrary.Common.Utilities;
using System.Collections;
using System.Data;
using System.Web;
using CommonLibrary.Common;
using CommonLibrary.Entities.Tabs;
using System.Data.SqlClient;
using CommonLibrary.Modules;

namespace CommonLibrary.Services.Banner
{
    public class BannerController
    {
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public BannerController() { }

        #region Code ===============================================================================================
        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_GetAll]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailById(int id)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_GetDetailById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByStatus(string status)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_GetListByStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByPositionStatus(int position, string status)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_GetListByPositionStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@position", position);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByNumPosition(int num_rows, int position, string status)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_GetListByNumPositionStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@num_rows", num_rows);
            cmd.Parameters.AddWithValue("@position", position);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByNumStatus(int num_rows, string status)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_GetListByNumStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@num_rows", num_rows);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetImageById(int id)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_GetImageById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }


        public string[] GetFrontMainImages(int id)
        {
            string[] array_list = new string[2];             
            DataTable dt =  GetDetailById(id);
            if (dt.Rows.Count > 0)
            {
                array_list[0] = dt.Rows[0]["MainImage"].ToString();
                array_list[1] = dt.Rows[0]["ThumbImage"].ToString();
            }
            return array_list;
        }



        public int Insert(int vendorid, string culturecode, string title, string main_image, string thumb_image, string url,
            int position, string description, string tags, int width, int height, string start_date, string end_date, string status)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            cmd.Parameters.AddWithValue("@vendorid", vendorid);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@mainimage", main_image);
            cmd.Parameters.AddWithValue("@thumbimage", thumb_image);
            cmd.Parameters.AddWithValue("@url", url);
            cmd.Parameters.AddWithValue("@position", position);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@tags", tags);
            cmd.Parameters.AddWithValue("@width", width);
            cmd.Parameters.AddWithValue("@height", height);
            cmd.Parameters.AddWithValue("@start_date", start_date);
            cmd.Parameters.AddWithValue("@end_date", end_date);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int vendorid, int id, string culturecode, string title, string main_image, string thumb_image, string url,
            int position, string description, string tags, int width, int height, string start_date, string end_date, string status)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            cmd.Parameters.AddWithValue("@vendorid", vendorid);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@mainimage", main_image);
            cmd.Parameters.AddWithValue("@thumbimage", thumb_image);
            cmd.Parameters.AddWithValue("@url", url);
            cmd.Parameters.AddWithValue("@position", position);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@tags", tags);
            cmd.Parameters.AddWithValue("@width", width);
            cmd.Parameters.AddWithValue("@height", height);
            cmd.Parameters.AddWithValue("@start_date", start_date);
            cmd.Parameters.AddWithValue("@end_date", end_date);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(int id, string status)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateSortKey(int option, int id, int previous_id)
        {
            SqlCommand cmd = new SqlCommand("[Banners].[Banner_UpdateSortKey]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@option", option);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@previous_id", previous_id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int id, string front_dir_path, string main_dir_path)
        {
            string MainImage = string.Empty, ThumbImage = string.Empty;
            string[] array_list = new string[2];
            array_list = GetFrontMainImages(id);
            MainImage = array_list[0];
            ThumbImage = array_list[1];
            FileHandleClass file_obj = new FileHandleClass();
            if (ThumbImage != string.Empty)
                file_obj.deleteFile(ThumbImage, front_dir_path);
            if (MainImage != string.Empty)
                file_obj.deleteFile(MainImage, main_dir_path);

            SqlCommand cmd = new SqlCommand("[Banners].[Banner_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }  
        #endregion ============================================================================================

        private string BannerClickThroughPage = "/DesktopModules/Admin/Banners/BannerClickThrough.aspx";
        private string FormatImage(string File, int Width, int Height, string BannerName, string Description)
        {
            System.Text.StringBuilder Image = new System.Text.StringBuilder();
            Image.Append("<img src=\"" + File + "\" border=\"0\"");
            if (!String.IsNullOrEmpty(Description))
            {
                Image.Append(" alt=\"" + Description + "\"");
            }
            else
            {
                Image.Append(" alt=\"" + BannerName + "\"");
            }
            if (Width > 0)
            {
                Image.Append(" width=\"" + Width.ToString() + "\"");
            }
            if (Height > 0)
            {
                Image.Append(" height=\"" + Height.ToString() + "\"");
            }
            Image.Append("/>");
            return Image.ToString();
        }
        private string FormatFlash(string File, int Width, int Height)
        {
            string Flash = "";
            Flash += "<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=4,0,2,0\" width=\"" + Width.ToString() + "\" height=\"" + Height.ToString() + "\">";
            Flash += "<param name=movie value=\"" + File + "\">";
            Flash += "<param name=quality value=high>";
            Flash += "<embed src=\"" + File + "\" quality=high pluginspage=\"http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash\" type=\"application/x-shockwave-flash\" width=\"" + Width.ToString() + "\" height=\"" + Height.ToString() + "\">";
            Flash += "</embed>";
            Flash += "</object>";
            return Flash;
        }
        public bool IsBannerActive(BannerInfo objBanner)
        {
            bool blnValid = true;
            if (Null.IsNull(objBanner.StartDate) == false && DateTime.Now < objBanner.StartDate)
            {
                blnValid = false;
            }
            if (blnValid)
            {
                switch (objBanner.Criteria)
                {
                    case 0:
                        if (objBanner.Impressions < objBanner.Views && objBanner.Impressions != 0)
                        {
                            blnValid = false;
                        }
                        break;
                    case 1:
                        if ((objBanner.Impressions < objBanner.Views && objBanner.Impressions != 0) || (DateTime.Now > objBanner.EndDate && Null.IsNull(objBanner.EndDate) == false))
                        {
                            blnValid = false;
                        }
                        break;
                }
            }
            return blnValid;
        }
        private object LoadBannersCallback(CacheItemArgs cacheItemArgs)
        {
            int PortalId = (int)cacheItemArgs.ParamList[0];
            int BannerTypeId = (int)cacheItemArgs.ParamList[1];
            string GroupName = (string)cacheItemArgs.ParamList[2];
            List<BannerInfo> FullBannerList = CBO.FillCollection<BannerInfo>(DataProvider.Instance().FindBanners(PortalId, BannerTypeId, GroupName));
            List<BannerInfo> ActiveBannerList = new List<BannerInfo>();
            foreach (BannerInfo objBanner in FullBannerList)
            {
                if (IsBannerActive(objBanner))
                {
                    ActiveBannerList.Add(objBanner);
                }
            }
            return ActiveBannerList;
        }
        public void AddBanner(BannerInfo objBannerInfo)
        {
            DataProvider.Instance().AddBanner(objBannerInfo.BannerName, objBannerInfo.VendorId, objBannerInfo.ImageFile, objBannerInfo.URL, objBannerInfo.Impressions, objBannerInfo.CPM, objBannerInfo.StartDate, objBannerInfo.EndDate, objBannerInfo.CreatedByUser, objBannerInfo.BannerTypeId,
            objBannerInfo.Description, objBannerInfo.GroupName, objBannerInfo.Criteria, objBannerInfo.Width, objBannerInfo.Height);
            ClearBannerCache();
        }
        public void ClearBannerCache()
        {
            DataCache.ClearCache("Banners:");
        }
        public void DeleteBanner(int BannerId)
        {
            DataProvider.Instance().DeleteBanner(BannerId);
            ClearBannerCache();
        }
        public string FormatBanner(int VendorId, int BannerId, int BannerTypeId, string BannerName, string ImageFile, string Description, string URL, int Width, int Height, string BannerSource,
        string HomeDirectory, string BannerClickthroughUrl)
        {
            string strBanner = "";
            string strWindow = "_new";
            if (Common.Globals.GetURLType(URL) == TabType.Tab)
            {
                strWindow = "_self";
            }
            string strURL = "";
            if (BannerId != -1)
            {
                if (string.IsNullOrEmpty(BannerClickthroughUrl))
                {
                    strURL = Common.Globals.ApplicationPath + BannerClickThroughPage + "?BannerId=" + BannerId.ToString() + "&VendorId=" + VendorId.ToString() + "&PortalId=" + Globals.GetPortalSettings().PortalId;
                }
                else
                {
                    strURL = BannerClickthroughUrl.ToString() + "?BannerId=" + BannerId.ToString() + "&VendorId=" + VendorId.ToString() + "&PortalId=" + Globals.GetPortalSettings().PortalId;
                }
            }
            else
            {
                strURL = URL;
            }
            strURL = HttpUtility.HtmlEncode(strURL);
            switch (BannerTypeId)
            {
                case (int)BannerType.Text:
                    strBanner += "<a href=\"" + strURL + "\" class=\"NormalBold\" target=\"" + strWindow + "\" rel=\"nofollow\"><u>" + BannerName + "</u></a><br>";
                    strBanner += "<span class=\"Normal\">" + Description + "</span><br>";
                    if (!String.IsNullOrEmpty(ImageFile))
                    {
                        URL = ImageFile;
                    }
                    if (URL.IndexOf("://") != -1)
                    {
                        URL = URL.Substring(URL.IndexOf("://") + 3);
                    }
                    strBanner += "<a href=\"" + strURL + "\" class=\"NormalRed\" target=\"" + strWindow + "\" rel=\"nofollow\">" + URL + "</a>";
                    break;
                case (int)BannerType.Script:
                    strBanner += Description;
                    break;
                default:
                    if (ImageFile.IndexOf("://") == -1 && ImageFile.StartsWith("/") == false)
                    {
                        if (ImageFile.ToLowerInvariant().IndexOf(".swf") == -1)
                        {
                            strBanner += "<a href=\"" + strURL + "\" target=\"" + strWindow + "\" rel=\"nofollow\">";
                            switch (BannerSource)
                            {
                                case "L":
                                    strBanner += FormatImage(HomeDirectory + ImageFile, Width, Height, BannerName, Description);
                                    break;
                                case "G":
                                    strBanner += FormatImage(Common.Globals.HostPath + ImageFile, Width, Height, BannerName, Description);
                                    break;
                            }
                            strBanner += "</a>";
                        }
                        else
                        {
                            switch (BannerSource)
                            {
                                case "L":
                                    strBanner += FormatFlash(HomeDirectory + ImageFile, Width, Height);
                                    break;
                                case "G":
                                    strBanner += FormatFlash(Common.Globals.HostPath + ImageFile, Width, Height);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (ImageFile.ToLowerInvariant().IndexOf(".swf") == -1)
                        {
                            strBanner += "<a href=\"" + strURL + "\" target=\"" + strWindow + "\" rel=\"nofollow\">";
                            strBanner += FormatImage(ImageFile, Width, Height, BannerName, Description);
                            strBanner += "</a>";
                        }
                        else
                        {
                            strBanner += FormatFlash(ImageFile, Width, Height);
                        }
                    }
                    break;
            }
            return strBanner;
        }
        public string FormatBanner(int VendorId, int BannerId, int BannerTypeId, string BannerName, string ImageFile, string Description, string URL, int Width, int Height, string BannerSource,
        string HomeDirectory)
        {
            return FormatBanner(VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, BannerSource,
            HomeDirectory, string.Empty);
        }
        public BannerInfo GetBanner(int BannerId, int VendorId, int PortalId)
        {
            return (BannerInfo)CBO.FillObject(DataProvider.Instance().GetBanner(BannerId, VendorId, PortalId), typeof(BannerInfo));
        }
        public DataTable GetBannerGroups(int PortalId)
        {
            return DataProvider.Instance().GetBannerGroups(PortalId);
        }
        public ArrayList GetBanners(int VendorId)
        {
            return CBO.FillCollection(DataProvider.Instance().GetBanners(VendorId), typeof(BannerInfo));
        }
        public ArrayList LoadBanners(int PortalId, int ModuleId, int BannerTypeId, string GroupName, int Banners)
        {
            if (GroupName == null)
            {
                GroupName = Null.NullString;
            }
            string cacheKey = string.Format(DataCache.BannersCacheKey, PortalId, BannerTypeId, GroupName);
            List<BannerInfo> bannersList = CBO.GetCachedObject<List<BannerInfo>>(new CacheItemArgs(cacheKey, DataCache.BannersCacheTimeOut, DataCache.BannersCachePriority, PortalId, BannerTypeId, GroupName), LoadBannersCallback);
            ArrayList arReturnBanners = new ArrayList(Banners);
            if (bannersList.Count > 0)
            {
                if (Banners > bannersList.Count)
                {
                    Banners = bannersList.Count;
                }
                int intIndex = new Random().Next(0, bannersList.Count);
                int intCounter = 1;
                while (intCounter <= bannersList.Count && arReturnBanners.Count != Banners)
                {
                    intIndex += 1;
                    if (intIndex > (bannersList.Count - 1))
                    {
                        intIndex = 0;
                    }
                    BannerInfo objBanner = bannersList[intIndex];
                    arReturnBanners.Add(objBanner);
                    objBanner.Views += 1;
                    if (Null.IsNull(objBanner.StartDate))
                    {
                        objBanner.StartDate = DateTime.Now;
                    }
                    if (Null.IsNull(objBanner.EndDate) && objBanner.Views >= objBanner.Impressions && objBanner.Impressions != 0)
                    {
                        objBanner.EndDate = DateTime.Now;
                    }
                    DataProvider.Instance().UpdateBannerViews(objBanner.BannerId, objBanner.StartDate, objBanner.EndDate);
                    if (!IsBannerActive(objBanner))
                    {
                        DataCache.RemoveCache(cacheKey);
                    }
                    intCounter += 1;
                }
            }
            return arReturnBanners;
        }
        public void UpdateBanner(BannerInfo objBannerInfo)
        {
            DataProvider.Instance().UpdateBanner(objBannerInfo.BannerId, objBannerInfo.BannerName, objBannerInfo.ImageFile, objBannerInfo.URL, objBannerInfo.Impressions, objBannerInfo.CPM, objBannerInfo.StartDate, objBannerInfo.EndDate, objBannerInfo.CreatedByUser, objBannerInfo.BannerTypeId,
            objBannerInfo.Description, objBannerInfo.GroupName, objBannerInfo.Criteria, objBannerInfo.Width, objBannerInfo.Height);
            ClearBannerCache();
        }
        public void UpdateBannerClickThrough(int BannerId, int VendorId)
        {
            DataProvider.Instance().UpdateBannerClickThrough(BannerId, VendorId);
        }
    }
}
