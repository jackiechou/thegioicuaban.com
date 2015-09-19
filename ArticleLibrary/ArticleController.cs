using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Library.Common;
using CommonLibrary;
using CommonLibrary.Common;
using CommonLibrary.Modules;
using System.Collections.Generic;
using System.Linq;

namespace ArticleLibrary
{
    public class ArticleController
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();

        public ArticleController() { }

        public static string[] PopulateArticleKeywordList(string prefixText)
        {
            using (ArticleEntities dbContext = new ArticleLibrary.ArticleEntities())
            {
                if (dbContext.Connection.State != ConnectionState.Closed)
                    dbContext.Connection.Open();
                string LowerKeywords = @prefixText.ToLower();

                var query = (from x in dbContext.Articles
                            where x.Headline.ToLower().Contains(LowerKeywords)
                            || x.Alias.Replace("-", " ").Contains(LowerKeywords)
                            || x.Authors.Contains(LowerKeywords)
                            || x.Abstract.Contains(LowerKeywords)                          
                            || x.Source.Contains(LowerKeywords)
                            || x.Tags.Contains(LowerKeywords)
                            && x.Status == "2"
                            orderby x.ArticleId descending
                             select x).ToList();

                string[] items = new string[query.Count];
                items = query.Select(o => o.ToString()).ToArray();
                return items;
            }
       
        }

        public static List<Article> SearchByKeywords(string Keywords, string CultureCode, int iTotalItemCount)
        {
            using (ArticleEntities dbContext = new ArticleLibrary.ArticleEntities())
            {
                if (dbContext.Connection.State != ConnectionState.Closed)
                    dbContext.Connection.Open();
                string LowerKeywords = @Keywords.ToLower();

                var query = from x in dbContext.Articles
                            where x.Headline.ToLower().Contains(LowerKeywords)
                            || x.Alias.Replace("-", " ").Contains(LowerKeywords)
                            || x.Authors.Contains(LowerKeywords)
                            || x.Abstract.Contains(LowerKeywords)
                            || x.MainText.Contains(LowerKeywords)
                            || x.Source.Contains(LowerKeywords)
                            || x.Tags.Contains(LowerKeywords)
                            && x.CultureCode.Trim() == CultureCode && x.Status == "2"
                            orderby x.ArticleId descending
                            select x;
                int TotalItemCount = 0;
                if (query.Count() > 0)
                {
                    if (iTotalItemCount <= query.Count())
                        TotalItemCount = iTotalItemCount;
                    else
                        TotalItemCount = query.Count();
                }
                return query.Take(TotalItemCount).ToList();
            }
        }


        public static List<Article> GetActiveListByTotalViewsFixedNum(string Code, string CultureCode, int iTotalItemCount)
        {
            using (ArticleEntities dbContext = new ArticleLibrary.ArticleEntities())
            {
                if (dbContext.Connection.State != ConnectionState.Closed)
                    dbContext.Connection.Open();

                var query = from x in dbContext.Articles
                            where x.CategoryCode == Code
                                && x.CultureCode.Trim() == CultureCode 
                                && x.Status == "2"
                                && x.TotalViews > 3
                            orderby x.TotalViews descending
                            select x;
                int TotalItemCount = 0;
                if (query.Count() > 0)
                {
                    if (iTotalItemCount <= query.Count())
                        TotalItemCount = iTotalItemCount;
                    else
                        TotalItemCount = query.Count();
                }
                return query.Take(TotalItemCount).ToList();
            }
        }

        public static List<Article> GetActiveListByFixedNumCode(string Code, string CultureCode, int iTotalItemCount)
        {
            using (ArticleEntities dbContext = new ArticleLibrary.ArticleEntities())
            {
                if (dbContext.Connection.State != ConnectionState.Closed)
                    dbContext.Connection.Open();

                var query = from x in dbContext.Articles
                            where x.CategoryCode.Trim() == Code && x.CultureCode.Trim() == CultureCode && x.Status == "2"
                            orderby x.ArticleId descending
                            select x;
                int TotalItemCount = 0;
                if (query.Count() > 0)
                {
                    if (iTotalItemCount <= query.Count())
                        TotalItemCount = iTotalItemCount;
                    else
                        TotalItemCount = query.Count();
                }
                return query.Take(TotalItemCount).ToList();
            }
        }

        public static List<Article> GetActiveListByFixedRecords(int iTotalItemCount)
        {
            using (ArticleEntities dbContext = new ArticleLibrary.ArticleEntities())
            {
                if (dbContext.Connection.State != ConnectionState.Closed)
                    dbContext.Connection.Open();
                List<Article> lst = (from x in dbContext.Articles
                                            where x.Status == "2"
                                            orderby x.ArticleId descending
                                            select x).ToList();

                int TotalItemCount = 0;
                if (lst.Count() > 0)
                {
                    if (iTotalItemCount <= lst.Count())
                        TotalItemCount = iTotalItemCount;
                    else
                        TotalItemCount = lst.Count();
                }
                return lst.Take(TotalItemCount).ToList();
            }
        }

        public static List<Article> GetActiveListByFixedNumCode(string Code, int iTotalItemCount)
        {
            using (ArticleEntities dbContext = new ArticleLibrary.ArticleEntities())
            {
                if (dbContext.Connection.State != ConnectionState.Closed)
                    dbContext.Connection.Open();
                IQueryable<Article> query = from x in dbContext.Articles
                                            where x.Status == "2"
                                            orderby x.ArticleId descending
                                            select x; 

                List<Article> lst = new List<Article>();
                if (string.IsNullOrEmpty(Code))
                    lst = query.ToList();
                else
                    lst = query.Where(x => x.CategoryCode.Trim() == Code).ToList();                            
                
                int TotalItemCount = 0;
                if (lst.Count() > 0)
                {
                    if (iTotalItemCount <= lst.Count())
                        TotalItemCount = iTotalItemCount;
                    else
                        TotalItemCount = lst.Count();
                }
                return lst.Take(TotalItemCount).ToList();
            }
        }

        public int GetNewspaper(string userid, int portalid, string culturecode, string code, string news, string category, int num_rows)
        {
            int result = -1;
            string link = string.Empty;

            if (news == "vnexpress")
            {
                news = "http://vnexpress.net/GL/";
                link = news + category;
                result = Get_VNEXPRESS(userid, portalid, culturecode, code, link, category, num_rows);
            }
            else if (news == "vietnamnet")
            {
                news = "http://vietnamnet.vn/vn/";
                link = news + category + "/index.html";
                result = Get_VIETNAMNET(userid, portalid, culturecode, code, link, category, num_rows);
            }
            return result;
        }

        public int Get_VNEXPRESS(string userid, int portalid, string culturecode, string code, string link, string category, int num_rows)
        {

            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(link);
            HtmlNodeCollection tables = doc.DocumentNode.SelectNodes("//div[@class='folder-news']");
            string img = "";
            int result = -1;
            int i = 1;
            foreach (HtmlNode table in tables)
            {
                if (i <= num_rows)
                {
                    img = "";
                    img = table.SelectNodes(".//a")[0].InnerHtml;
                    img = Regex.Match(img, "src=\"(?<img>.*?)\"").Groups["img"].ToString();
                    if (img != "")
                    {

                        img = "http://vnexpress.net" + img;
                        string linkdetail = table.SelectNodes(".//a")[0].OuterHtml;
                        linkdetail = Regex.Match(linkdetail, " href=\"(?<link>.*?)\"").Groups["link"].ToString();
                        linkdetail = "http://vnexpress.net" + linkdetail;
                        string headline = table.SelectNodes(".//a")[1].InnerText;
                        headline = headline.Replace("'", "");
                        string title = headline;
                        //string alias = Modules.ModuleClass.RemoveSign4VietnameseString(title.Replace(" ", "-"));
                        //alias = alias.Replace("\"", "");

                        string shorttext = table.SelectNodes(".//p")[1].InnerText;
                        string date = table.SelectNodes(".//label")[1].InnerText;
                        date = date.Replace("&nbsp;", "");
                        date = date.Replace("|", "");

                        HtmlWeb hw2 = new HtmlWeb();
                        HtmlDocument doc2 = hw2.Load(linkdetail);
                        HtmlNodeCollection tables2 = doc2.DocumentNode.SelectNodes("//div[@class='content']");
                        string contents = "";
                        foreach (HtmlNode table2 in tables2)
                        {
                            contents = table2.SelectNodes(".")[0].OuterHtml;
                            int start = contents.IndexOf("<p class=\"Normal\">", 0);
                            int stop = contents.LastIndexOf("</p>");
                            if (start >= 0)
                                contents = contents.Substring(start, stop + 4 - start);
                        }
                        contents = contents.Replace("src=\"", "src=\"http://vnexpress.net");

                        string status = "1";
                        string source = "vnexpress";
                        string main_img = "";
                        string abstract_info = "";
                        string navigateurl = "";
                        result = Insert(userid, portalid, culturecode, code, title, headline, abstract_info, img, main_img, contents, source,navigateurl, status);
                        i++;
                    }
                }
            }
            return result;
        }

        public int Get_VIETNAMNET(string userid, int portalid, string culturecode, string code, string link, string category, int num_rows)
        {

            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(link);
            HtmlNodeCollection tables = doc.DocumentNode.SelectNodes("//div[@class='item']");

            int result = -1;
            int i = 1;
            foreach (HtmlNode table in tables)
            {
                if (i < num_rows)
                {
                    string img = "";
                    img = table.InnerHtml;
                    img = Regex.Match(img, "src=\"(?<img>.*?)\"").Groups["img"].ToString();
                    string linkdetail = table.SelectNodes(".//a[@class='item_link']")[0].OuterHtml;
                    linkdetail = Regex.Match(linkdetail, " href=\"(?<link>.*?)\"").Groups["link"].ToString();
                    linkdetail = "http://vietnamnet.vn" + linkdetail;
                    string headline = table.SelectNodes(".//a[@class='item_link']")[0].InnerText;
                    headline = headline.Replace("'", "");
                    string title = headline;
                    string shorttext = table.SelectNodes(".//div[@class='lead']")[0].InnerText;

                    HtmlWeb hw2 = new HtmlWeb();
                    HtmlDocument doc2 = hw2.Load(linkdetail);
                    HtmlNode datenote = doc2.DocumentNode.SelectNodes("//div[@id='article']//div[@id='date']")[0];
                    string date = datenote.InnerText;
                    date = date.Trim();
                    date = ModuleClass.RemoveSign4VietnameseString(date);
                    date = date.Replace("\r\n", "");
                    date = date.Replace(" ", "");
                    //date = Regex.Match(date, "Capnhatluc(?<date>.*?)").Groups["date"].ToString();
                    date = date.Substring("Capnhatluc".Length, 20);

                    date = date.Substring(0, 10) + " " + date.Substring(10, 8) + " " + date.Substring(18, 2);

                    HtmlNode contentnode = doc2.DocumentNode.SelectNodes("//div[@class='article_content']")[0];
                    string contents = "";
                    contents = contentnode.InnerHtml;
                    contents = contents.Trim();
                    string status = "-1";
                    string source = "vietnamnet";
                    string main_img = string.Empty;
                    string abstract_info = "";
                    string navigateurl = "";
                    result = Insert(userid, portalid, culturecode, code, title, headline, abstract_info, img, main_img, contents, source,navigateurl, status);
                    i++;
                }
            }
            return result;

        }
            
        public DataTable GetDataByConditions(int portalid, string culturecode, string begindate, string enddate, string code, string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetDataByConditions]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            cmd.Parameters.AddWithValue("@begindate", begindate);
            cmd.Parameters.AddWithValue("@enddate", enddate);
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByAlias(string alias)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetDetailByAlias]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@alias", alias);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByCodeAlias(string alias, string code)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetDetailByCodeAlias]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@alias", alias);
            cmd.Parameters.AddWithValue("@code", code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByID(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetDetailById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetFrontImageByID(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetFrontImageById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public string GetMainImageByID(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetMainImageById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
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
            DataTable dt = GetDetailByID(id);
            array_list[0] = dt.Rows[0]["MainImage"].ToString();
            array_list[1] = dt.Rows[0]["ThumbImage"].ToString();
            return array_list;
        }


        public DataSet ShowPaging(int current_page, int record_per_page, int page_size)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_Paging]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@current_page", current_page);
            cmd.Parameters.AddWithValue("@record_per_page", record_per_page);
            cmd.Parameters.AddWithValue("@page_size", page_size);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            con.Close();
            return ds;
        }

        public DataTable GetListByCode(string code)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetListByCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@code", code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByNumCode(string code, int num_rows)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetListByNumCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@num_rows", num_rows);
            cmd.Parameters.AddWithValue("@code", code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetTopListByCodeSelectId(string code, int idx, int record)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetTopListByCodeSelectId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@record", record);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetTopNewListByCodeSelectedId(string code, int idx, int record)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetTopNewListByCodeSelectedId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@record", record);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetTopOldListByCodeSelectedId(string code, int idx, int record)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetTopOldListByCodeSelectedId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@record", record);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListTotalViewsByNumRecords(int num_rows)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetListTotalViewsByNumRecords]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@num_rows", num_rows);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetRandomList(int records)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_GetRandomList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@records", records);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string userid, int portalid, string culturecode, string code, string title, string headline, string abstract_info,
            string frontimage, string mainimage, string maintext, string source, string navigateurl, string status)
        {

            /*** PostedDate ******************************************************************************/
            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US");
            //customCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            //customCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
            //string PostedDate = System.DateTime.Now.ToString("G");
            /*********************************************************************************************/

            string alias = ModuleClass.convertTitle2Link(headline);
            maintext = module_obj.UTF8_Encode(maintext);
            string tags = module_obj.createTags(headline);

            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@headline", headline);
            cmd.Parameters.AddWithValue("@alias", alias);
            cmd.Parameters.AddWithValue("@abstract", abstract_info);
            cmd.Parameters.AddWithValue("@frontimage", frontimage);
            cmd.Parameters.AddWithValue("@mainimage", mainimage);
            cmd.Parameters.AddWithValue("@maintext", maintext);
            cmd.Parameters.AddWithValue("@tags", tags);
            cmd.Parameters.AddWithValue("@source", source);
            cmd.Parameters.AddWithValue("@navigateurl", navigateurl);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(string userid, int portalid, string culturecode, int idx, string code, string title, string headline, string abstract_info, string frontimage, string mainimage, string maintext, string source, string navigateurl, string status)
        {
            string alias = ModuleClass.convertTitle2Link(headline);
            maintext = module_obj.UTF8_Encode(maintext);
            string tags = module_obj.createTags(headline);

            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@headline", headline);
            cmd.Parameters.AddWithValue("@alias", alias);
            cmd.Parameters.AddWithValue("@abstract", abstract_info);
            cmd.Parameters.AddWithValue("@frontimage", frontimage);
            cmd.Parameters.AddWithValue("@mainimage", mainimage);
            cmd.Parameters.AddWithValue("@maintext", maintext);
            cmd.Parameters.AddWithValue("@tags", tags);
            cmd.Parameters.AddWithValue("@source", source);
            cmd.Parameters.AddWithValue("@navigateurl", navigateurl);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateSortKey(int option, int idx)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_UpdateSortKey]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@option", option);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update_Total_View(int idx, int totalview)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_UpdateTotalView]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@total", totalview);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(string userid, int idx, string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(string userid, int idx, string front_dir_path, string main_dir_path)
        {
            string MainImage = string.Empty, ThumbImage = string.Empty;
            string[] array_list = new string[2];
            array_list = GetFrontMainImages(idx);
            MainImage = array_list[0];
            ThumbImage = array_list[1];
            FileHandleClass file_obj = new FileHandleClass();
            if (ThumbImage != string.Empty)
                file_obj.deleteFile(ThumbImage, front_dir_path);
            if (MainImage != string.Empty)
                file_obj.deleteFile(MainImage, main_dir_path);


            SqlCommand cmd = new SqlCommand("[Articles].[sp_Articles_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
