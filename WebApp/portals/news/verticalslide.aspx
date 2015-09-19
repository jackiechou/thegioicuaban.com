<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="verticalslide.aspx.cs" Inherits="WebApp.portals.news.verticalslide" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript" src="../../scripts/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="skins/VBA_skin/js/jquery.tools.min.js"></script>
    <link rel="stylesheet" type="text/css" href="skins/VBA_skin/css/scrollable-vertical.css">
    <script type="text/javascript">
        $(function () {
            $(".scrollable").scrollable({ circular: true, vertical: true, mousewheel: true, interval: 7000, speed: 3000 }).navigator({ history: true }).autoscroll({ autoplay: true });            
            $(".item").hover(function () {
                $(this).toggleClass("active");
            });
        });
</script>
        <div class="box-widget box-widget-navbar">
            <div class="box-widget box-home-item style-3">
                <div class="box-widget-header">
                    <h2><a rel="nofollow" href="#">Doanh nghiệp tiêu biểu</a></h2>
                    <a class="rss" href="#"></a>
                    <div id="actions" class="pages" style="float:right;">
                        <span class="r_page"><a class="prev" rel="nofollow">« Back</a></span>
                        <span class="l_page"><a class="next" rel="nofollow">Next »</a></span> 
                    </div>
                </div>
                <div class="entry-content">
                    <div class="data-list">
                        <div class="boxCarousel">
                            <!-- root element for scrollable -->
                            <div class="scrollable vertical">
                                <!-- root element for the items -->
                                <div class="items">
                                    <asp:Literal ID="LiteralContents" runat="server"></asp:Literal>
                                </div>
                            </div>
                         </div>
                    </div>
                </div>
                <div style="clear:both;"></div>
            </div>
         </div> 
    </form>
</body>
</html>
