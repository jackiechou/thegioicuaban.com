using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MediaLibrary;
using ArticleLibrary;
using GalleryLibrary;
using System.Web.Script.Services;

namespace WebApp.portals.news.services
{
    [WebService(Namespace = "http://thegioiso360.com/portals/news/services/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]

    public class news : System.Web.Services.WebService
    {
        [WebMethod(Description = "HelloWorld")]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static string[] PopulateEmailSuggestions(string prefixText)
        {
            return new string[] {
                prefixText + "@gmail.com",
                prefixText + "@yahoo.com",
                prefixText + "@yahoo.com.vn",
                prefixText + "@hotmail.com"            
            };
        }

      
        #region Media===================================================================================================================================================
        [WebMethod(Description = "PopulateHotAlbumListWithPagination")]
        public string PopulateHotAlbumListWithPagination(string ajaxFunction, string tabContainer, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {
            string strHTML = string.Empty;
            var _lst = MediaAlbums.GetActiveListByTypeIdFixedNum(3, totalItemCount);
            if (_lst.Count > 0)
            {
                //pageSize = 5 // 5 items wil be showing 
                //pageShowing = 5; // 5 pages will be showing in the list (1|2|3|4|5 then show next button)

                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowPrevious = false;
                bool bShowNext = false;

                if (pageIndex > 1)
                    bShowPrevious = true;

                if (intEndPage < PageCount)
                    bShowNext = true;

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='prev' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='prev'>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='next' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();

                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();

                string contents = string.Empty;
                foreach (var x in lst)
                {
                    contents += "<a href=\"#\"><h4><span>" + x.AlbumName + "</span></h4></a>";
                }
                strHTML = "<div class='album_info'>" + contents + "</div><br/>" + pagination_link;
            }
            return strHTML;
        }

        [WebMethod(Description = "PopulateHotSongListWithPagination")]
        public string PopulateHotSongListWithPagination(string ajaxFunction, string tabContainer, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {
            string strHTML = string.Empty;
            var _lst = MediaFiles.GetActiveListByTypeIdFixedNum(3, totalItemCount);
            if (_lst.Count > 0)
            {
                //pageSize = 5 // 5 items wil be showing 
                //pageShowing = 5; // 5 pages will be showing in the list (1|2|3|4|5 then show next button)

                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowPrevious = false;
                bool bShowNext = false;

                if (pageIndex > 1)
                    bShowPrevious = true;

                if (intEndPage < PageCount)
                    bShowNext = true;

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='prev'>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='next' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();

                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();

                string contents = string.Empty;
                string url = string.Empty;
                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
                foreach (var x in lst)
                {
                    if (x.FileName != string.Empty)
                        url = baseUrl + "/user_files/media/audio/music/" + x.FileName;
                    if (x.FileUrl != string.Empty && x.FileName == string.Empty)
                        url = x.FileUrl;
                    contents += "<div style='float:left;'><h4><span>" + x.Title + "</span></h4></div>"
                                    + "<div style='float:right;'>"
                                        + "<object type='application/x-shockwave-flash' data='" + baseUrl + "/portals/news/skins/default_skin/player/player_mp3_maxi.swf' width='60' height='20'>"
                                            + "<param name='movie' value='" + baseUrl + "/portals/news/skins/default_skin/player/player_mp3_maxi.swf' />"
                                            + "<param name='bgcolor' value='#ffffff' />"
                                            + "<param name='FlashVars' value='configxml=" + baseUrl + "/portals/news/skins/default_skin/player/config_player.xml&amp;mp3=" + url + "' />"
                                        + "</object>"
                                    + "</div>";
                }
                strHTML = "<div class='hotsong_info'>" + contents + "</div><br/>" + pagination_link;
            }
            return strHTML;
        }

        [WebMethod(Description = "PopulateHotSingerListWithPagination")]
        public string PopulateHotSingerListWithPagination(string ajaxFunction, string tabContainer, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {
            string strHTML = string.Empty;
            var _lst = MediaArtists.GetActiveListByFixedNum(totalItemCount);
            if (_lst.Count > 0)
            {
                //pageSize = 5 // 5 items wil be showing 
                //pageShowing = 5; // 5 pages will be showing in the list (1|2|3|4|5 then show next button)

                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowPrevious = false;
                bool bShowNext = false;

                if (pageIndex > 1)
                    bShowPrevious = true;

                if (intEndPage < PageCount)
                    bShowNext = true;

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='prev' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='prev'>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();


                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();

                string contents = string.Empty;
                foreach (var x in lst)
                {
                    contents += "<a href=\"#\"><h4><span>" + x.ArtistName + "</span></h4></a>";
                }
                strHTML = "<div class='hotsinger_info'>" + contents + "</div><br/>" + pagination_link;
            }
            return strHTML;
        }

        [WebMethod(Description = "PopulateHotPlayListWithPagination")]
        public string PopulateHotPlayListWithPagination(string ajaxFunction, string tabContainer, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {
            string strHTML = string.Empty;
            var _lst = MediaPlayLists.GetActiveListByTypeIdFixedNum(3, totalItemCount);
            if (_lst.Count > 0)
            {
                //pageSize = 5 // 5 items wil be showing 
                //pageShowing = 5; // 5 pages will be showing in the list (1|2|3|4|5 then show next button)

                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowPrevious = false;
                bool bShowNext = false;

                if (pageIndex > 1)
                    bShowPrevious = true;

                if (intEndPage < PageCount)
                    bShowNext = true;

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='prev' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='prev'>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='next' onClick=\"javascript:" + ajaxFunction + "('" + tabContainer + "'," + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();

                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();

                string contents = string.Empty;
                foreach (var x in lst)
                {
                    contents += "<a href=\"#\"><h4><span>" + x.PlayListName + "</span></h4></a>";
                }
                strHTML = "<div class='playlist_info'>" + contents + "</div><br/>" + pagination_link;
            }
            return strHTML;
        }
        #endregion END Media=========================================================================================================================================
     
        #region NEWS =======================================================================================================================================
        [WebMethod]
        [ScriptMethod]
        public string[] PopulateSuggestedArticleKeywordList(string prefixText)
        {
            string[] strArrayList = ArticleController.PopulateArticleKeywordList(prefixText);
            return strArrayList;
        }

        [WebMethod]
        [ScriptMethod]
        public string SearchByKeywordsWithPagination(string keywords, string cultureCode, string ajaxFunction, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {
            string strHTML = string.Empty;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string path_image = "/user_files/images/article_images/front_images/";
            var _lst = ArticleController.SearchByKeywords(keywords, cultureCode, totalItemCount);
            if (_lst.Count > 0)
            {
                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowFirst = false;
                bool bShowPrevious = false;
                bool bShowNext = false;
                bool bShowLast = false;

                if (pageIndex > 1)
                {
                    bShowPrevious = true;
                    bShowFirst = true;
                }

                if (intEndPage < PageCount)
                {
                    bShowNext = true;
                    bShowLast = true;
                }

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowFirst)
                    sbHTML.Append("<li><a rel='first' onClick=\"javascript:" + ajaxFunction + "(1," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='first'>[<<]</a></li>");

                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='prev' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='prev'>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "(" + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='next' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[>]</a></li>");
                if (bShowLast)
                    sbHTML.Append("<li><a rel='last' onClick=\"javascript:" + ajaxFunction + "(" + PageCount + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[>>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();

                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();
                int num = 1;
                string contents = string.Empty;
                contents += "<ul>";
                foreach (var x in lst)
                {
                    string code = x.CategoryCode;
                    string img = string.Empty;
                    if (x.FrontImage != "")
                        img = path_image + x.FrontImage;
                    else
                        img = "/images/no_image.jpg";

                    contents += "<li class=\"" + num + "\"><a class=\"thumb\" title=\"" + x.Title + "\" href=\"/tin-chi-tiet/" + x.Alias + "/" + code + "\">"
                             + "<img alt=\"" + x.Alias + "\" src=\"" + img + "\"></a>"
                             + "<a  class=\"title\" title=\"" + x.Title + "\" href=\"/tin-chi-tiet/" + x.Alias + "/" + code + "\">"
                             + x.Headline + "</a><span class='date'>(" + x.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy") + ")<span></li>";
                }
                strHTML ="<div class=\"fLeft clorB\">"
                        + "<h3 class=\"fLeft textTrans\">"
                        + "Kết quả tìm kiếm"
                        + "</h3>"
                        + "</div>"
                        + "<div class=\"row_item\">"
                        + "<div class='entry'>" + contents + "</div><div class=\"clear\"></div>" + pagination_link + "<div class=\"clear\"></div>"
                        + "</div>";
            }
            else
            {
                strHTML = "<div class=\"fLeft clorB\">"
                        + "<h3 class=\"fLeft textTrans\">"
                        + "Kết quả tìm kiếm"
                        + "</h3>"
                        + "</div>"
                        + "<div class=\"contact_us\">"
                        + "<div class='entry'>không có dữ liệu</div><div class=\"clear\"></div>"
                        + "</div>";
            }
            return strHTML;
        }


        [WebMethod(Description = "PopulateTopNewsWithPagination")]
        public string PopulateTopNewsWithPagination(string ajaxFunction, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {
            string strHTML = string.Empty;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string path_image = "/user_files/images/article_images/front_images/";
            var _lst = ArticleController.GetActiveListByFixedRecords(totalItemCount);
            if (_lst.Count > 0)
            {
                //pageSize = 5 // 5 items wil be showing 
                //pageShowing = 5; // 5 pages will be showing in the list (1|2|3|4|5 then show next button)

                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowFirst = false;
                bool bShowPrevious = false;
                bool bShowNext = false;
                bool bShowLast = false;

                if (pageIndex > 1)
                {
                    bShowPrevious = true;
                    bShowFirst = true;
                }

                if (intEndPage < PageCount)
                {
                    bShowNext = true;
                    bShowLast = true;
                }

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowFirst)
                    sbHTML.Append("<li><a rel='first' onClick=\"javascript:" + ajaxFunction + "(1," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='first'>[<<]</a></li>");

                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='prev' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='prev'>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "(" + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='next' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount  + ");\">[>]</a></li>");
                if (bShowLast)
                    sbHTML.Append("<li><a rel='last' onClick=\"javascript:" + ajaxFunction + "(" + PageCount + "," + pageSize + "," + pageShowing + "," + totalItemCount + ");\">[>>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();

                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();
                int num = 1;
                string contents = string.Empty;
                contents += "<ul>";
                foreach (var x in lst)
                {
                    string code = x.CategoryCode;
                    string img = string.Empty;
                    if (x.FrontImage != "")
                        img = path_image + x.FrontImage;
                    else
                        img = "/images/no_image.jpg";

                    //DateTime? dateValue = DateTime.Parse(x.DateCreated.ToString());
                    //DateTime? dateValue = DateTime.Parse(x.DateCreated.ToString(), new System.Globalization.CultureInfo("vi-VN", false));
                    //string strDateCreated = x.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy");
                    //string strDateCreated = x.DateCreated.HasValue ? x.DateCreated.Value.ToString("dd/MM/yyyy") : null;

                    contents += "<li class=\"" + num + "\"><a class=\"thumb\" title=\"" + x.Title + "\" href=\"/tin-chi-tiet/" + x.Alias + "/" + code + "\">"
                             + "<img alt=\"" + x.Alias + "\" src=\"" + img + "\"></a>"
                             + "<a  class=\"title\" title=\"" + x.Title + "\" href=\"/tin-chi-tiet/" + x.Alias + "/" + code + "\">"
                             + x.Headline + "</a><span>" + x.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy") + "<span></li>";
                }
                strHTML = "<div class='newcode_info'>" + contents + "</div><div class=\"clear\"></div>" + pagination_link;
            }
            return strHTML;
        }        
       
        [WebMethod(Description = "PopulateTopNewsCodeWithPagination")]
        public string PopulateTopNewsCodeWithPagination(string code, string ajaxFunction, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {           
            string strHTML = string.Empty;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string path_image = "/user_files/images/article_images/front_images/";
            var _lst = ArticleController.GetActiveListByFixedNumCode(code, totalItemCount);
            if (_lst.Count > 0)
            {
                //pageSize = 5 // 5 items wil be showing 
                //pageShowing = 5; // 5 pages will be showing in the list (1|2|3|4|5 then show next button)

                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowFirst = false;
                bool bShowPrevious = false;
                bool bShowNext = false;
                bool bShowLast = false;

                if (pageIndex > 1)
                {
                    bShowPrevious = true;
                    bShowFirst = true;
                }

                if (intEndPage < PageCount)
                {
                    bShowNext = true;
                    bShowLast = true;
                }

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowFirst)
                    sbHTML.Append("<li><a rel='first' onClick=\"javascript:" + ajaxFunction + "(1," + pageSize + "," + pageShowing + "," + totalItemCount + "," + code + ");\">[<<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='first'>[<<]</a></li>");

                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='prev' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + "," + code + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a rel='prev'>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "(" + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + "," + code + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='next' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + "," + code + ");\">[>]</a></li>");
                if (bShowLast)
                    sbHTML.Append("<li><a rel='last' onClick=\"javascript:" + ajaxFunction + "(" + PageCount + "," + pageSize + "," + pageShowing + "," + totalItemCount + "," + code + ");\">[>>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();

                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();
                int num = 1;
                string contents = string.Empty;
                contents += "<ul>";
                foreach (var x in lst)
                {
                    string img = string.Empty;
                    if (x.FrontImage != "")
                        img = path_image + x.FrontImage;
                    else
                        img = "/images/no_image.jpg";

                    //DateTime? dateValue = DateTime.Parse(x.DateCreated.ToString());
                    //DateTime? dateValue = DateTime.Parse(x.DateCreated.ToString(), new System.Globalization.CultureInfo("vi-VN", false));
                    //string strDateCreated = x.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy");
                    //string strDateCreated = x.DateCreated.HasValue ? x.DateCreated.Value.ToString("dd/MM/yyyy") : null;

                    contents += "<li class=\"" + num + "\"><a class=\"thumb\" title=\"" + x.Title + "\" href=\"/tin-chi-tiet/" + x.Alias + "/" + code + "\">"
                             + "<img alt=\"" + x.Alias + "\" src=\""+img+ "\"></a>"
                             + "<a  class=\"title\" title=\"" + x.Title + "\" href=\"/tin-chi-tiet/" + x.Alias + "/" + code + "\">"
                             + x.Headline + "</a><span>" + x.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy") + "<span></li>";
                }
                strHTML = "<div class='newcode_info'>" + contents + "</div><div class=\"clear\"></div>" + pagination_link;
            }
            return strHTML;
        }
        #endregion END Top News By Category Code ===============================================================================================================

        #region GALLERY =====================================================================================================
        [WebMethod]
        public string PopulateGalleryCollectionListWithPagination(string ajaxFunction, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount)
        {
            string strHTML = string.Empty, topicName = string.Empty;
            var list = GalleryTopic.GetList('1');
            int topicId = 1;
            foreach (var item in list)
            {
                topicId = item.Gallery_TopicId;
                topicName = item.Gallery_TopicName;
                strHTML += "<div class='divGalleryCollection'>" + topicName + "</div>"
                            +PopulateGalleryCollectionListWithPaginationByTopicId(ajaxFunction, pageIndex, pageSize, pageShowing, totalItemCount, topicId);
            }
            return strHTML;
        }

        [WebMethod]
        public string PopulateGalleryCollectionListWithPaginationByTopicId(string ajaxFunction, int? pageIndex, int? pageSize, int pageShowing, int totalItemCount, int topicId)
        {
            string strHTML = string.Empty;
            var _lst = GalleryCollection.GetActiveListByFixedNum(totalItemCount, topicId);
            if (_lst.Count > 0)
            {
                //pageSize = 5 // 5 items wil be showing 
                //pageShowing = 5; // 5 pages will be showing in the list (1|2|3|4|5 then show next button)

                string pagination_link = string.Empty;
                int TotalItemCount = 0;
                if (totalItemCount > _lst.Count)
                    TotalItemCount = _lst.Count;
                else
                    TotalItemCount = totalItemCount;

                int PageCount = 0;
                if (TotalItemCount > 0)
                    PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

                int? intStartPage = 0;
                int? intEndPage = 0;
                intStartPage = (pageIndex <= pageShowing ? 1 : pageIndex);

                if (PageCount > pageShowing & PageCount > intStartPage)
                    intEndPage = ((intStartPage + pageShowing - 1) > PageCount ? PageCount : (intStartPage + pageShowing - 1));
                else
                    intEndPage = PageCount;

                bool bShowPrevious = false;
                bool bShowNext = false;

                if (pageIndex > 1)
                    bShowPrevious = true;

                if (intEndPage < PageCount)
                    bShowNext = true;

                System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();
                sbHTML.Append("<div class='pagination'>");
                sbHTML.Append("<ul class='paging_navigation'>");

                //************to add first and previous*************************
                if (bShowPrevious)
                    sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex - 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + "," + topicId + ");\">[<]</a></li>");
                else
                    sbHTML.Append("<li><a>[<]</a></li>");

                //**********to add the page numbers***************************
                for (int? iPage = intStartPage; iPage <= intEndPage; iPage++)
                {
                    if (pageIndex == iPage)
                        sbHTML.Append("<li class='active' rel='nofollow'><a>");
                    else
                        sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "(" + iPage + "," + pageSize + "," + pageShowing + "," + totalItemCount + "," + topicId + ");\">");

                    sbHTML.Append(iPage.ToString());
                    sbHTML.Append("</a></li>");
                }
                //******to add next and last**********************************
                if (bShowNext)
                    sbHTML.Append("<li><a rel='nofollow' onClick=\"javascript:" + ajaxFunction + "(" + (pageIndex + 1) + "," + pageSize + "," + pageShowing + "," + totalItemCount + "," + topicId + ");\">[>]</a></li>");
                //************************************************************
                sbHTML.Append("</ul></div>");
                //no paging is needed if only one page
                if (PageCount > 1)
                    pagination_link = sbHTML.ToString();


                //content info ----------------------------------------------------------------------------------------------------------------------------------
                int _pagesize = (pageSize.HasValue) ? pageSize.Value : _lst.Count;   //if no page size specified, set page size to total # of rows in data table
                int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;     //paging logic <-> default to page 1 if no page supplied
                var lst = _lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();

                string path_image = string.Empty, contents = string.Empty;
                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
                string dir_path = baseUrl + "/" + System.Configuration.ConfigurationManager.AppSettings["upload_gallery_collection_image_dir"];
                foreach (var x in lst)
                {
                    if (x.IconFile != string.Empty)
                        path_image = dir_path + "/" + x.IconFile;
                    else
                        path_image = baseUrl + "/images/no_image.jpg";

                    contents += "<a class='gallery_link' href=\"/bo-suu-tap-chi-tiet/" + x.CollectionId + "\" rel='nofollow' title='" + x.Title + "'>"
                                     + "<img src='" + path_image + "'  width=\"150px\" height=\"150px\" alt='" + x.Title + "' >"
                               + "</a>";
                }
                strHTML = "<div class='gallery'>" + contents + "</div><br/>" + pagination_link;
            }
            return strHTML;
        }

        [WebMethod]
        public string PopulateGalleryFileListByCollectionId(string CollectionId)
        {
            string strHTML = string.Empty;            
            List<CustomGalleryFiles> _lst = GalleryFile.GetList(Convert.ToInt32(CollectionId), '1');
            if (_lst.Count > 0)
            {
                string contents = string.Empty, file_path = string.Empty, file_name = string.Empty;
                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
                string path_image = baseUrl + "/" + System.Configuration.ConfigurationManager.AppSettings["upload_gallery_content_image_dir"];
                foreach (var x in _lst)
                {
                    if (!string.IsNullOrEmpty(x.FileName) && x.FileName.Length > 50)
                        file_name = x.FileName.Substring(0, 50) + "...";

                    if (string.IsNullOrEmpty(x.FileUrl))
                        file_path = path_image + "/" + x.FileName;
                    else
                        file_path = x.FileUrl;

                    contents += "<a class='highslide' onclick=\"return hs.expand(this)\" href='" + file_path + "' title='" + file_name + "'>"
                                     + "<img src='" + file_path + "'  width=\"150px\" height=\"150px\" alt='" + file_name + "' >"
                               + "</a>";
                }
                strHTML = "<div class=\"highslide-gallery\" style=\"width: 600px; margin: auto\">" + contents + "</div>";
            }
            else
                strHTML = "<div class=\"highslide-gallery\" style=\"width: 600px; margin: auto\">Không có dữ liệu</div>";

            return strHTML;
        }
        #endregion GALLERY ==================================================================================================

       
    }
}
