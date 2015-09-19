<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_picturebox.ascx.cs" Inherits="WebApp.portals.news.controls.uc_picturebox" %>
<script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/portals/news/skins/VBA_skin/js/jquery.tools.min.js"></script>
<div class="box-widget box-widget-navbar">
    <div class="box-widget box-home-item style-3">
        <div class="box-widget-header">
            <h2><a rel="nofollow" href="#">Doanh nghiệp tiêu biểu</a></h2>
            <a class="rss" href="#"></a>
            <div id="actions" class="pages" style="float:right;">
                <span class="r_page"><a class="next" rel="nofollow"></a></a></span>
                <span class="l_page"><a class="prev" rel="nofollow"></span> 
            </div>
        </div>
            <!-- root element for scrollable -->
            <div class="scrollable vertical">
                <!-- root element for the items -->
                <div class="items">
                    <asp:Literal ID="LiteralContents" runat="server"></asp:Literal>
                </div>
            </div>
        </div>          
        <div style="clear:both;"></div>
    </div>
</div> 