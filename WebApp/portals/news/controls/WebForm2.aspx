<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WebApp.portals.news.controls.WebForm2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/jquery/jquery.min.js"></script>
    <link rel="stylesheet" href="../../../scripts/plugins/highslide/highslide.css" type="text/css" />
    <script type="text/javascript" src="../../../scripts/plugins/highslide/highslide-full.js"></script>
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

        jQuery(document).ready(function (e) {
            ajaxGalleryFileList('1');
        });


        function ajaxGalleryFileList(CollectionId) {
            var divContainer = "divPicture";
            var baseUrl = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
            var serviceUrl = baseUrl + "/portals/news/services/news.asmx/PopulateGalleryFileListByCollectionId";
            var params = "{CollectionId:'" + CollectionId + "'}";
            $.ajax({
                type: "POST",
                url: serviceUrl,
                data: params,
                beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d != '')
                        $("#" + divContainer).html(msg.d);
                }
            , error: function (request, status, error) {
                alert(request.responseText.toString());
            }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divPicture" runat="server"></div>
    </form>
</body>
</html>
