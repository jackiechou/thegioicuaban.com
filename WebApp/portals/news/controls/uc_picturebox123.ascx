<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_picturebox123.ascx.cs" Inherits="WebApp.portals.news.controls.uc_picturebox123" %>
<script type="text/javascript" src="../../../scripts/plugins/vticker/jquery.vticker-min.js"></script>
<script type="text/javascript" src="../../../scripts/plugins/jcarousel/jquery.jcarousel.min.js"></script>
<script type="text/javascript">
    jQuery(document).ready(function ($) {
        $("#ulCarousel").jcarousel({
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
     padding:5px 3px 5px 0px !important;
}
</style>

<div class="box-widget box-widget-navbar">
    <div class="box-widget box-home-item style-3">
        <div class="box-widget-header">
            <h2><a href="#">Doanh nghiệp tiêu biểu</a></h2>
            <a class="rss" href="#"></a>
            <div class="pages" style="float:right;">
                <span class="r_page"><a title="Sau" id="jcarousel_next" href="#"></a></span>
                <span class="l_page"><a title="Trước" id="jcarousel_back" href="#"></a></span>                        
            </div>
        </div>
        <div class="entry-content">
             <div class="data-list"> 
                <div class="boxCarousel">
                    <div id="divPictuerBox" runat="server"></div>
                </div>
		    </div>
           <div style="clear:both;"></div>
        </div>
    </div>
</div>