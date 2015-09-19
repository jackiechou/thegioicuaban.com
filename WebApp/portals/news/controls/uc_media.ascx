<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_media.ascx.cs" Inherits="WebApp.portals.news.controls.uc_media" %>
  
<script src="<%# Request.Url.Host.ToString() %>/portals/news/skins/default_skin/js/tabcontent.js" type="text/javascript"></script>
<script type="text/javascript">
    jQuery(document).ready(function ($) {
        ajax_ws('tab1', '1', '2', '5', '20');
        ajax_ws('tab2', '1', '2', '5', '20');
        ajax_ws('tab3', '1', '2', '5', '20');
        ajax_ws('tab4', '1', '2', '5', '20');
    });
    
     function ajax_ws(tabContainer, pageIndex, pageSize, pageShowing, totalItemCount) {
        var ajaxFunction = 'ajax_ws';
        var baseUrl = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');        
        var methodName = "";
        var divContainer = "";
        switch (tabContainer) {
            case 'tab1':
                methodName = "PopulateHotSongListWithPagination";
                divContainer = "divHitSongList";
                break;
            case 'tab2':
                methodName = "PopulateHotAlbumListWithPagination";
                divContainer = "divAlbumList";
                break;
            case 'tab3':
                methodName = "PopulateHotSingerListWithPagination";
                divContainer = "divHotSingerList";
                break;
            case 'tab4':
                methodName = "PopulateHotPlayListWithPagination";
                divContainer = "divHotPlayList";
                break;
            default:
                methodName = "PopulateHotSongListWithPagination";
                divContainer = "divHitSongList";
                break;
        }


        $.ajax({
            type: "POST",
            url: baseUrl + "/portals/news/services/news.asmx/" + methodName,
            data: "{ajaxFunction:'" + ajaxFunction + "',tabContainer:'" + tabContainer + "',pageIndex:'" + pageIndex + "',pageSize:'" + pageSize + "',pageShowing:'" + pageShowing + "',totalItemCount:'" + totalItemCount + "'}",
            beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d != '')
                    $("#" + divContainer).html(msg.d);
            }
            ,error: function (request, status, error) {
                alert(request.responseText.toString());
            }
        });
    }  
</script>
<div class="box-widget box-widget-navbar">
    <div class="box-widget-header">
        <h2><a href="#">Hot Media</a></h2>
    </div>
    <div class="box-widget-bottom fx">		
        <ul class="tabs">   
            <li class="selected"><a href="javascript:void(0);" rel="tab1" onclick="ajax_ws('tab1','1','2','5','20');">Ca khúc mới</a></li>    
            <li><a href="javascript:void(0);" rel="tab2" onclick="ajax_ws('tab2','1','2','5','20');">Album</a></li>
            <li><a href="javascript:void(0);" rel="tab3" onclick="ajax_ws('tab3','1','2','5','20');">Ca sĩ</a></li>
            <li><a href="javascript:void(0);" rel="tab4" onclick="ajax_ws('tab4','1','2','5','20');">Playlist</a></li>  
        </ul>
        <div class="tabcontents">
            <div id="tab1" class="tabcontent">
                <div id="divHitSongList"></div>
            </div>
            <div id="tab2" class="tabcontent">
                <div id="divAlbumList"></div>
            </div> 
            <div id="tab3" class="tabcontent">
                <div id="divHotSingerList"></div>
            </div>  
            <div id="tab4" class="tabcontent">
                <div id="divHotPlayList"></div>
            </div> 
        </div>
                              
    </div>
    <div class="clear"></div>
</div>