<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_gallery.ascx.cs" Inherits="WebApp.portals.news.controls.uc_gallery" %>
<script type="text/javascript">
    jQuery(document).ready(function ($) {        
        ajax_gallery('1', '2', '5', '20', '2');
    });

    function ajax_gallery(pageIndex, pageSize, pageShowing, totalItemCount, topicId) {
        var ajaxFunction = 'ajax_gallery';
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
                    $("#divGallery").html(msg.d);
            }
            , error: function (request, status, error) {
                alert(request.responseText.toString());
            }
        });
    }  
</script>

 <div id="divGallery"></div>