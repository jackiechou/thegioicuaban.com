<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="slide.aspx.cs" Inherits="WebApp.portals.news.slide" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
       /* ================ Slideshow =============================        
        .widget
        {
            background: #26005d url(http://backend.ngoisao.vn/webskins/skins/widget/widget_bg.png) no-repeat center 0;
            height: 324px;
            margin: 0 0 10px;
            position: relative;
            overflow: hidden;
            border: #000 3px solid;
        }
       
        .widget-item
        {
            width: 270px;
            height: 210px;
        }
        .widget-item p
        {
            padding: 0 0 5px;
            margin: 0;
        }
        .widget-item p img
        {
            width: 270px;
            height: 162px;
        }
        .widget-item a.widget-title
        {
            text-align: left;
            color: #fff;
            font-weight: bold;
            text-decoration: none;
            display: block;
            padding: 0;
        }
        
        .widget-slider
        {
            width: 270px;
            height: 210px;
            margin: 0 auto;
            text-align: center;
            padding: 10px 0 0;
            border-bottom: #5b10c5 1px solid;
            z-index: 100;
        }       

        .widget .slides_container
        {
            width: 20000em;
            height: 210px;
            position: relative;
            display: none;
            overflow: hidden;
        }
        
       
      

        .widget .slides_container a
        {
            width: 570px;
            height: 270px;
            display: block;
        }
        .widget .slides_container a img
        {
            display: block;
        }
      
        .widget a.prev, .widget a.next
        {
            display: block;
            bottom: 5px;
            background: url(http://backend.ngoisao.vn/webskins/skins/widget/widget_prev.png) no-repeat;
            width: 13px;
            height: 14px;
            position: absolute;
            text-indent: -99999px;
        }
        .widget a.prev
        {
            right: 45px;
        }
        .widget a.next
        {
            right: 10px;
            background-image: url(http://backend.ngoisao.vn/webskins/skins/widget/widget_next.png);
        }
        .widget-header a
        {
            display: block;
            background: url(http://backend.ngoisao.vn/webskins/skins/widget/widget_logo.png) no-repeat;
            width: 147px;
            height: 71px;
            text-indent: -99999px;
        }
        
        
        .pagination
        {
            margin: 26px auto 0;
            width: 100px;
        }
        .pagination li
        {
            float: left;
            margin: 0 1px;
            list-style: none;
        }
        .pagination li a
        {
            display: block;
            width: 12px;
            height: 0;
            padding-top: 12px;
            background-image: url(../img/pagination.png);
            background-position: 0 0;
            float: left;
            overflow: hidden;
        }
        .pagination li.current a
        {
            background-position: 0 -12px;
        }
        
         .widget-copyright
        {
            text-align: left;
            padding: 10px 0 0;
            color: #04c9eb;
            font-weight: bold;
            font-family: Arial,Helvetica,sans-serif;
            font-size: 12px;
        }
        .widget-copyright a
        {
            color: #04c9eb;
            text-decoration: none;
        }*/
    </style>

</head>
<body>
    <form id="form1" runat="server"> 
     <div class="qc-r" id="divSlideContainer" runat="server"></div>
	<script src="http://gsgd.co.uk/sandbox/jquery/easing/jquery.easing.1.3.js"></script>
	<script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/portals/news/skins/default_skin/js/slides.min.jquery.js"></script>

     <script type="text/javascript">
         $(function () {
             $('#widget-slider').slides({
                 preload: true,
                 preloadImage: '/images/loading.gif',
                 play: 8000,
                 pause: 2500,
                 fadeSpeed: 750,
                 hoverPause: true,
                 generatePagination: false,
                 generateNextPrev: true
             });
         });
	   </script>	 
    </form>
</body>
</html>
