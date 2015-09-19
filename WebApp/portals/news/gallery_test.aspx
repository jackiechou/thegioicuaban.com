<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gallery_test.aspx.cs" Inherits="WebApp.portals.news.gallery_test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="skins/VBA_skin/css/style.css" />
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/jquery/jquery.min.js"></script>
    <link rel="stylesheet" href="skins/VBA_skin/css/lightbox.css" type="text/css" />    
    <script type="text/javascript" src="../../scripts/plugins/colorbox/jquery.colorbox.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            ajax_picture('1', '2', '5', '20', '2');

            //Examples of how to assign the ColorBox event to elements
            $(".group4").colorbox({ rel: 'group4', slideshow: true});
            $("#divPicture").appendTo('form');
            //Example of preserving a JavaScript event for inline calls.
            $("#click").click(function () {
                $('#click').css({ "background-color": "#f00", "color": "#fff", "cursor": "inherit" }).text("Open this window again and this message will still be here.");
                return false;
            });
        });
        function pageLoad(sender, args) {
            if (args.get_isPartialLoad()) {
                //Colorbox JQuery codes go here
                $(document).ready(function () {
                    $(".group4").colorbox({ rel: 'group4', slideshow: true });
                });
            }
        }
        function ajax_picture(pageIndex, pageSize, pageShowing, totalItemCount, topicId) {
            var ajaxFunction = 'ajax_picture';
            var baseUrl = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
            $.ajax({
                type: "POST",
                url: baseUrl + "/portals/news/services/news.asmx/PopulatePictureListWithPagination",
                data: "{ajaxFunction:'" + ajaxFunction + "',pageIndex:'" + pageIndex + "',pageSize:'" + pageSize + "',pageShowing:'" + pageShowing + "',totalItemCount:'" + totalItemCount + "',topicId:'" + topicId + "'}",
                beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d != '')
                        $("#divPicture").html(msg.d);
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
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
   <ContentTemplate>
    <div id="divPicture"></div>
    </ContentTemplate>
    </asp:UpdatePanel>
    

   <%-- <ul class="gallery clearfix">
            <li><a class="group4" href="../../user_files/images/gallery_images/collection/1.jpg" title="Hình 1">
                <img src="../../user_files/images/gallery_images/collection/1.jpg" width="60" height="60" alt="" /></a></li>
            <li><a class="group4" href="../../user_files/images/gallery_images/collection/2.jpg" title="Hình 2">
                <img src="../../user_files/images/gallery_images/collection/2.jpg" width="60" height="60" alt="" /></a></li>
            <li><a class="group4" href="../../user_files/images/gallery_images/collection/3.jpg" title="Hình 3">
                <img src="../../user_files/images/gallery_images/collection/3.jpg" width="60" height="60" alt="" /></a></li>
            <li><a class="group4" href="../../user_files/images/gallery_images/collection/4.jpg" title="Hình 4">
                <img src="../../user_files/images/gallery_images/collection/4.jpg" width="60" height="60" alt="" /></a></li>
            <li><a class="group4" href="../../user_files/images/gallery_images/collection/5.jpg" title="Hình 5">
                <img src="../../user_files/images/gallery_images/collection/5.jpg" width="60" height="60" alt="" /></a></li>
            <li><a class="group4" href="../../user_files/images/gallery_images/collection/6.jpg" title="Hình 6">
                <img src="../../user_files/images/gallery_images/collection/6.jpg" width="60" height="60" alt="" /></a></li>
        </ul>--%>
       
    </form>
</body>
</html>
