<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="video.ascx.cs" Inherits="WebApp.portals.news.controls.video" %>
<script type="text/javascript">
    jQuery.noConflict();
    jQuery(document).ready(function ($) {
        //====JWPlayer========================================================================================
        GetMediaFile();
        function GetMediaFile() {
            var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
            var file_path = base_url + '/user_files/media/video';
            var image_path = base_url + '/user_files/images/media_images/photo';
            var player_path = "/scripts/plugins/jwplayer/player.swf";
            var skin_path = "/scripts/plugins/jwplayer/skinplayer/stormtrooper.zip";
            var skin_config_file = "/scripts/plugins/jwplayer/skinplayer/stormtrooper/stormtrooper.xml";

            $.ajax({
                type: "POST",
                url: base_url + "/services/WebServices.asmx/GetListMediaFile",
                data: "",
                async: false,
                global: false,
                beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var jsonList = '[' + data.d + ']';
                    var obj = jQuery.parseJSON(jsonList);
                    var list = '';
                    jQuery.each(obj, function () {
                        list += '{duration:"0", title: "' + this.Title + '", file: "' + file_path + '/' + this.FileName + '", image: "' + image_path + '/' + this.Photo + '" },';
                    });
                    var strplaylist = eval('[' + list.substring(0, list.length - 1) + ']');
                    jwplayer("mediaplayer").setup({
                        playlist: strplaylist,
                        skin: skin_path,
                        flashplayer: player_path,
                        stretching: 'fill',
                        "playlist.position": "bottom",
                        'controlbar': {
                            "position": "bottom"
                        },
                        "playlist.size": "150",
                        'width': '480',
                        'height': '420',
                        "repeat": "list",
                        "autostart": "true",
                        "volume": "50"
                    });
                }, error: function (e) {
                    e.toString();
                }
            });
        }
    });
    </script>
<div id="mediaplayer"></div>