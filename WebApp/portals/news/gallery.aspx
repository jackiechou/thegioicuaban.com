<%@ Page Title="" Language="C#" MasterPageFile="~/portals/news/skins/VBA_skin/VBA_skin.Master" AutoEventWireup="true" CodeBehind="gallery.aspx.cs" Inherits="WebApp.portals.news.gallery" %>
<%@ Register Src="~/portals/news/controls/uc_ads.ascx" TagName="ads" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        jQuery(document).ready(function ($) {
            ajax_gallery('1', '15', '5', '20', '2');           
        });

        function ajax_gallery(pageIndex, pageSize, pageShowing, totalItemCount, topicId) {
            var ajaxFunction = 'ajax_gallery';
            var divContainer = 'divGallery';
            var baseUrl = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
            var service_url = baseUrl + "/portals/news/services/news.asmx/PopulateGalleryCollectionListWithPagination";
            $.ajax({
                type: "POST",
                url: service_url,
                data: "{ajaxFunction:'" + ajaxFunction + "',pageIndex:'" + pageIndex + "',pageSize:'" + pageSize + "',pageShowing:'" + pageShowing + "',totalItemCount:'" + totalItemCount + "'}",
                beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d != '')
                        $("#" + divContainer).html(msg.d);
                    else
                        $("#" + divContainer).html("Khong co du lieu");
                }
                    , error: function (request, status, error) {
                        $("#" + divContainer).html(request.responseText.toString());
                    }
            });
        }
        
    </script>
    <div class="fLeft clorB h3gallery">
            <h3 class="fLeft textTrans">
                Bộ Sưu Tập
            </h3>
    </div>
   <div id="divGallery"></div>   
</asp:Content>
