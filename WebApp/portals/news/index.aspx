<%@ Page Title="" Language="C#" MasterPageFile="~/portals/news/skins/VBA_skin/VBA_skin.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="WebApp.portals.news.index" %>
<%@ Register Src="~/portals/news/controls/uc_ads.ascx" TagName="ads" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_banner.ascx" TagName="banner" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_partners.ascx" TagName="partners" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news.ascx" TagName="news" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news_bottom.ascx" TagName="news_bottom" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news_by_totalviews.ascx" TagName="news_by_totalviews" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_events.ascx" TagName="events" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_support_online.ascx" TagName="support_online" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_topics.ascx" TagName="topics" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="fl col1">
        <uc:banner ID="banner_control" runat="server" />
        
        <%--------------------------NEWS--------------------------------%>  
        <div id="box-home-items">              
            <div class="box-style">
                <asp:Label ID="lblNews" runat="server"></asp:Label>  
                <div class="clear"></div>
            </div>           
        </div> 
        <%-------------------------- END NEWS--------------------------------%> 
    </div>
    <div class="fr col2">
        <div class="col2_inner">
            <div class="firstleft"><uc:events ID="control_event" runat="server" /></div>                          
            <uc:news_by_totalviews ID="news_by_totalviews_control" runat="server"  />
            <uc:ads ID="ads_control" runat="server" />            
            <uc:partners ID="partners_control" runat="server" />   
            <uc:support_online ID="support_online_control" runat="server" />
        </div>      
    </div>
    <div class="clear"></div>    
    <%--<uc:news_bottom id="news_bottom_control" runat="server"/>--%>

</asp:Content>
