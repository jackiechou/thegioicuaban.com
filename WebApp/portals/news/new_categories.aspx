<%@ Page Title="" Language="C#" MasterPageFile="~/portals/news/skins/VBA_skin/VBA_skin.Master" AutoEventWireup="true" CodeBehind="new_categories.aspx.cs" Inherits="WebApp.portals.news.new_categories" %>
<%@ Register Src="~/portals/news/controls/uc_ads.ascx" TagName="ads" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_partners.ascx" TagName="partners" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news.ascx" TagName="news" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news_bottom.ascx" TagName="news_bottom" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news_by_totalviews.ascx" TagName="news_by_totalviews" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_events.ascx" TagName="events" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_support_online.ascx" TagName="support_online" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_topics.ascx" TagName="topics" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="fl col1">
        <div class="fLeft clorB">
            <h3 class="fLeft textTrans">
                <asp:Literal ID="ltrCateName" runat="server"></asp:Literal>
            </h3>
        </div>
        <div class="clear"></div>       

        <asp:Literal ID="ltrNewCode" runat="server"></asp:Literal>
        <div id="divNewCode"></div>
        <script type="text/javascript">
            // get param=============================================================================================
            var code = getURLParameter("code");
            function getURLParameter(param) {
                return decodeURI((RegExp(param + '=' + '(.+?)(&|$)').exec(location.search) || [, null])[1]);
            }
            // end ===================================================================================================

            jQuery(document).ready(function ($) {
                ajaxNewCode('divNewCode', code, 1, 2, 200);
            });

            function ajaxNewCode(obj, code, pageIndex, pageSize, iTotalItemCount) {
                var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
                var service_url = base_url + "/portals/news/services/news.asmx/PopulateNewsCodeWithPagination";
                $.ajax({
                    type: "POST",
                    url: service_url,
                    data: "{code:'" + code + "',pageIndex:'" + pageIndex + "',pageSize:'" + pageSize + "',iTotalItemCount:'" + iTotalItemCount + "'}",
                    beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        if (msg.d != '')
                            $("#" + obj).html(msg.d);
                    },
                    timeout: function () { }
                });
            }
        </script>

    </div>
     <div class="fr col2">
        <div class="col2_inner">
            <uc:news_by_totalviews ID="news_by_totalviews_control" runat="server" />                
            <uc:events ID="control_event" runat="server" />                  
            <uc:ads ID="ads_control" runat="server" />
            <uc:support_online ID="support_online_control" runat="server" />   
            <uc:partners ID="partners_control" runat="server" /> 
        </div>
    </div>
    <div class="clear"></div>
</asp:Content>
