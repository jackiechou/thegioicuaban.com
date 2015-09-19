<%@ Control ClientIDMode="Static" Language="C#" AutoEventWireup="true" CodeBehind="uc_news_slide.ascx.cs" Inherits="WebApp.portals.news.controls.uc_news_slide" %>


<div class="qc-r" id="divSlideContainer" runat="server"></div> 
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