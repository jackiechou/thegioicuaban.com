<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_banner.ascx.cs" Inherits="WebApp.portals.news.controls.uc_banner" %>
<script type="text/javascript" src="/scripts/CheckDateJScript.js"></script>
<script type="text/javascript" src="/portals/news/skins/VBA_skin/js/js-image-slider.js"></script>
 <div id="sliderFrame">
    <div class="heading_banner">
        <div class="local_timer">
            <span id="spanCurrentDateTime" class="txtgrey"></span> 
                <script type="text/javascript">
                    jQuery(document).ready(function () { date_time('spanCurrentDateTime'); });    
                </script>
        </div>
        <div class="titleBanner">
            Tin nổi bật
        </div>
    </div>
    <div id="divBanner" class="content_banner" runat="server"></div>        
    <div style="clear:both;height:0;"></div>    
</div>