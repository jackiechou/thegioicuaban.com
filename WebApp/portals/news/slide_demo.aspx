<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="slide_demo.aspx.cs" Inherits="WebApp.portals.news.slide_demo" %>
<%@ Register Src="~/portals/news/controls/uc_picturebox.ascx" TagName="picturebox" TagPrefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <%--<link rel="stylesheet" type="text/css" href="skins/VBA_skin/css/style.css" />--%>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/plugins/vticker/jquery.vticker-min.js"></script>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/plugins/jcarousel/jquery.jcarousel.min.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            jQuery('#ulCarousel').jcarousel({
                vertical: true,
                scroll: 1,
                auto: 10,
                animation: 'slow',
                visible: 4,
                wrap: 'both',
                initCallback: initPortalBox_callback,
                buttonNextHTML: null,
                buttonPrevHTML: null,
                itemFirstInCallback: mycarousel_initCallback
            });
        });
        function mycarousel_initCallback(carousel) {
            carousel.clip.hover(function () {
                carousel.stopAuto();
            }, function () {
                carousel.startAuto();
            });
        };


        function init_mousewheel(elm, carousel) {
            $(elm).bind('mousewheel DOMMouseScroll', function (event) {
                event.preventDefault();
                var delta = event.wheelDelta || -event.detail;
                if (delta > 0)
                    carousel.prev();
                else
                    carousel.next();
            });
        }

        function initPortalBox_callback(carousel) {

            $('#jcarousel_next').bind('click', function () {
                carousel.next();
                return false;
            });
            $('#jcarousel_back').bind('click', function () {
                carousel.prev();
                return false;
            });

            init_mousewheel('#ulCarousel', carousel);
        }

        var dataLogHot = null;

        function mycarousel_itemLoadCallback(carousel, state) {
            if (carousel.has(carousel.first, carousel.last)) {
                return;
            }
            if (dataLogHot != null) {
                mycarousel_itemAddCallback(carousel, carousel.first, carousel.last, dataLogHot);
                return;
            }
        };

        function mycarousel_itemAddCallback(carousel, first, last, data) {
            carousel.size(data.length);
            for (var i = first; i < last; i++) {
                var r = data[i];
                carousel.add(i, mycarousel_getItemHTML(r));
            }
        };
    </script>
    <style type="text/css">
    .page2cols .wrap_content .col1 .picture_box li
    {
        padding: 5px 3px 5px 0px !important;
    }
    .jcarousel-skin-tango .jcarousel-container
        {
            border: 1px solid #346F97;
        }
        .jcarousel-skin-tango .jcarousel-direction-rtl
        {
            direction: rtl;
        }
        .jcarousel-skin-tango .jcarousel-container-vertical
        {
            width: 295px;
            height: auto;
        }
        
        .jcarousel-skin-tango .jcarousel-clip
        {
            overflow: hidden;
        }
        .jcarousel-skin-tango .jcarousel-clip-vertical
        {
            width: 295px;
            height: 618px;          
        } 
        
       .jcarousel-skin-tango .jcarousel-clip-vertical
        {
            text-align:center; padding:3px;
        }   
         #ulCarousel ul { list-style:none; }
        .jcarousel-skin-tango .jcarousel-clip-vertical #ulCarousel ul li
        {
            list-style-type:none;
            margin:padding:10px 0 !important;
            padding: padding:10px 0 !important;
            border:1px solid #eee;
        }
       
        .jcarousel-skin-tango .jcarousel-item-vertical
        {
            margin-bottom: 8px;
        }
        
        .jcarousel-skin-tango .jcarousel-item-placeholder
        {
            background: #fff;
            color: #000;
        }
        .jcarousel-skin-tango .jcarousel-next-vertical
        {
            position: absolute;
            bottom: 5px;
            left: 43px;
            width: 32px;
            height: 32px;
            cursor: pointer;
        }
        
        .jcarousel-skin-tango .jcarousel-next-vertical:hover, .jcarousel-skin-tango .jcarousel-next-vertical:focus
        {
            background-position: 0 -32px;
        }
        
        .jcarousel-skin-tango .jcarousel-next-vertical:active
        {
            background-position: 0 -64px;
        }
        
        .jcarousel-skin-tango .jcarousel-next-disabled-vertical, .jcarousel-skin-tango .jcarousel-next-disabled-vertical:hover, .jcarousel-skin-tango .jcarousel-next-disabled-vertical:focus, .jcarousel-skin-tango .jcarousel-next-disabled-vertical:active
        {
            cursor: default;
            background-position: 0 -96px;
        }
        
        .jcarousel-skin-tango .jcarousel-prev-vertical
        {
            position: absolute;
            top: 5px;
            left: 43px;
            width: 32px;
            height: 32px;
            cursor: pointer;
        }
        
        .jcarousel-skin-tango .jcarousel-prev-vertical:hover, .jcarousel-skin-tango .jcarousel-prev-vertical:focus
        {
            background-position: 0 -32px;
        }
        
        .jcarousel-skin-tango .jcarousel-prev-vertical:active
        {
            background-position: 0 -64px;
        }
        
        .jcarousel-skin-tango .jcarousel-prev-disabled-vertical, .jcarousel-skin-tango .jcarousel-prev-disabled-vertical:hover, .jcarousel-skin-tango .jcarousel-prev-disabled-vertical:focus, .jcarousel-skin-tango .jcarousel-prev-disabled-vertical:active
        {
            cursor: default;
            background-position: 0 -96px;
        }
        .jcarousel-item-vertical
        {
            float: left;
            height: auto;
            list-style: none outside none;
            width:auto;
        }
       
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="wrap">
        <div class="box-widget box-widget-navbar">
            <div class="box-widget box-home-item style-3">
                <div class="box-widget-header">
                    <h2>
                        <a href="#">Doanh nghiệp tiêu biểu</a></h2>
                    <a class="rss" href="#"></a>
                    <div class="pages" style="float: right;">
                        <span class="r_page"><a title="Sau" id="jcarousel_next" href="#">>></a></span><span
                            class="l_page"><a title="Trước" id="jcarousel_back" href="#"><<</a></span>
                    </div>
                </div>
                <div class="entry-content">
                    <div class="data-list">
                        <div class="boxCarousel">
                            <div class=" jcarousel-skin-tango">
                                <div class="jcarousel-container jcarousel-container-vertical">
                                    <div class="jcarousel-clip jcarousel-clip-vertical">
                                         <ul id="ulCarousel" class="jcarousel jcarousel-skin-tango">
                                            <li class="carousel-item">
                                                <div class="divitem">
                                                    <div class="col_odd" style="float:left">
                                                        <img src="http://static.flickr.com/66/199481236_dc98b5abb3_s.jpg" width="145px" height="150px"
                                                            alt="" />
                                                    </div>
                                                    <div class="col_even" style="float:left">
                                                       <img src="http://static.flickr.com/75/199481072_b4a0d09597_s.jpg" width="145px" height="150px"
                                                        alt="" />
                                                    </div>    
                                                </div>
                                            </li>
                                           
                                            <li>
                                                <div class="divitem">
                                                    <div class="col_odd" style="float:left">
                                                        <img src="http://static.flickr.com/57/199481087_33ae73a8de_s.jpg" width="145px" height="150px" alt="" />
                                                    </div>
                                                    <div class="col_even" style="float:left">
                                                        <img src="http://static.flickr.com/77/199481108_4359e6b971_s.jpg" width="145px" height="150px" alt="" />
                                                    </div> 
                                                </div> 
                                             </li>

                                            <li>
                                                <div class="divitem">
                                                    <div class="col_odd" style="float:left">
                                                         <img src="http://static.flickr.com/77/199481108_4359e6b971_s.jpg" width="145px" height="150px"
                                                        alt="" />
                                                    </div>
                                                    <div class="col_even" style="float:left">
                                                       <img src="http://static.flickr.com/58/199481143_3c148d9dd3_s.jpg" width="145px" height="150px"
                                                    alt="" />
                                                    </div>
                                                </div> 
                                            </li>
                                            <li>
                                                <div class="divitem">
                                                    <div class="col_odd" style="float:left">
                                                        <img src="http://static.flickr.com/72/199481203_ad4cdcf109_s.jpg" width="145px" height="150px"
                                                        alt="" />
                                                    </div>
                                                    <div class="col_even" style="float:left">
                                                       <img src="http://static.flickr.com/58/199481218_264ce20da0_s.jpg" width="145px" height="150px"
                                                    alt="" />
                                                    </div>
                                                </div>
                                            </li>
                                            
                                            <li>
                                                <div class="divitem">
                                                    <div class="col_odd" style="float:left">
                                                       <img src="http://static.flickr.com/69/199481255_fdfe885f87_s.jpg" width="145px" height="150px"
                                                        alt="" />
                                                    </div>
                                                    <div class="col_even" style="float:left">
                                                       <img src="http://static.flickr.com/60/199480111_87d4cb3e38_s.jpg" width="145px" height="150px"
                                                    alt="" />
                                                    </div>
                                                </div>
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both;">
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>