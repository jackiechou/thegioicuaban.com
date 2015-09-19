<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="banner_rotator_demo.aspx.cs" Inherits="WebApp.portals.news.banner_rotator_demo" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">    
        <meta http-equiv="content-type" content="text/html; charset=UTF-8">
        <meta charset="UTF-8">
        <meta http-equiv="X-UA-Compatible" content="chrome=1">
        <title>jQuery Banner Rotator</title>      
        <link rel="stylesheet" type="text/css" href="../../scripts/plugins/banner_rotator/css/banner-rotator.css" />
        <!--jQuery-->
        <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
        <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script>
        <!--Banner-->
        <script type="text/javascript" src="../../scripts/plugins/banner_rotator/jquery.banner-rotator.js"></script>
        <!--Preview - Remove it-->
        <script type="text/javascript" src="../../scripts/plugins/banner_rotator/preview.js"></script>
        <!--Initialize-->
        <script type="text/javascript">
            $(document).ready(function () {
                //Banner Rotator Init <acronym title="JavaScript">JS</acronym>
                $(".banner").bannerRotator({
                    transition: "random"
                });
            });
        </script>       
    </head>
    <body>
     <form id="form1" runat="server">
        <div id="container">
            <div id="center">
                <div class="header">
                    <div class="logo"><img src="/images/logo/logo.png" width="400" height="56" alt="" /></div>
                    <div class="selector">
                        <div class="thumbbutton right" rel="outside"><div class="grain">Outside</div></div>
                        <div class="seperator"></div>
                        <div class="thumbbutton middle" rel="bullet"><div class="grain">Bullet</div></div>
                        <div class="seperator"></div>
                        <div class="thumbbutton middle" rel="thumbnail"><div class="grain">Thumbnail</div></div>
                        <div class="seperator"></div>
                        <div class="thumbbutton selected" rel="number"><div class="grain">Number</div></div>
                     </div>
                </div>
                <!--Banner Rotator - Start-->
                <div class="banner">
                    <div class="screen">
                        <noscript>
                            <!--Placeholder Image When Javascript is Off-->
                            <img src="../../scripts/plugins/banner_rotator/images/bg/background.jpg" alt="" />
                        </noscript>
                    </div>
                    <div class="cpanel">
                        <div class="items">
                            <asp:Literal ID="Literal_TopBanner" runat="server"></asp:Literal>
                        </div>
                        <div class="buttons">
                            <div class="previous-btn"></div>
                            <div class="play-btn"></div>   
                            <div class="next-btn"></div>              
                        </div>
                    </div>
                </div>
                <!--Banner Rotator - End-->
            </div>           
        </div> 
        </form>      
    </body>
</html>