<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_events.ascx.cs" Inherits="WebApp.portals.news.controls.uc_events" %>
<script type="text/javascript">
    jQuery(document).ready(function ($) {
        ajaxNews('EVENT',1, 2, 5, 20);
    });

    function ajaxNews(code, pageIndex, pageSize, pageShowing, totalItemCount) {
        var ajaxFunction = "ajaxNews";
        var divContainer = "box-widget-topviews";
        var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
        var service_url = base_url + "/portals/news/services/news.asmx/PopulateTopNewsCodeWithPagination";
        $.ajax({
            type: "POST",
            url: service_url,
            data: "{code:'" + code + "',ajaxFunction:'" + ajaxFunction + "',pageIndex:'" + pageIndex + "',pageSize:'" + pageSize + "',pageShowing:'" + pageShowing + "',totalItemCount:'" + totalItemCount + "'}",
            beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d != '') {                   
                    $("#" + divContainer).html(msg.d);
                }
            }, error: function (request, status, error) {
                //alert(request.responseText);
            }
        });
    }
</script>
  <div class="box-widget box-widget-navbar">
    <div class="box-widget-header">
		<h2><a href="#">SỰ KIỆN NỔI BẬT</a></h2>
	</div>
    <div class="box-widget-topviews" id="box-widget-topviews">   
    </div>
</div>
<div class="clear"></div>