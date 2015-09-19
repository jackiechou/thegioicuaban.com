<%@ Page Title="" Language="C#" MasterPageFile="~/portals/news/skins/VBA_skin/VBA_skin.Master" AutoEventWireup="true" CodeBehind="gallery_details.aspx.cs" Inherits="WebApp.portals.news.gallery_details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<link rel="stylesheet" href="../../scripts/plugins/highslide/highslide.css" type="text/css" />
<script type="text/javascript" src="../../scripts/plugins/highslide/highslide-full.js"></script>
 <script type="text/javascript">
    var graphicDirPath = '/scripts/plugins/highslide/graphics/'
    hs.graphicsDir = graphicDirPath;
    hs.align = 'center';
    hs.transitions = ['expand', 'crossfade'];
    hs.fadeInOut = true;
    hs.dimmingOpacity = 0.8;
    hs.outlineType = 'rounded-white';
    hs.captionEval = 'this.thumb.alt';
    hs.marginBottom = 105; // make room for the thumbstrip and the controls
    hs.numberPosition = 'caption';

    // Add the slideshow providing the controlbar and the thumbstrip
    hs.addSlideshow({
        //slideshowGroup: 'group1',
        interval: 5000,
        repeat: false,
        useControls: true,
        overlayOptions: {
            className: 'text-controls',
            position: 'bottom center',
            relativeTo: 'viewport',
            offsetY: -60
        },
        thumbstrip: {
            position: 'bottom center',
            mode: 'horizontal',
            relativeTo: 'viewport'
        }
    });
</script>
<div class="fLeft clorB h3gallery">
    <h3 class="fLeft textTrans">
        Chi Tiết Bộ Sưu Tập 
    </h3>
    <span class="divBackButtonBox">
        <a onclick="javascript:window.history.back();">
            <span class="btnBack">Quay về</span>
        </a>
    </span>
</div>
<div id="divGalleryFileList" class="divGalleryFileList" runat="server"></div>  
</asp:Content>
