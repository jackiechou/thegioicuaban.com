<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vietnam_map.ascx.cs" Inherits="WebApp.modules.admin.map.vietnam_map" %>

<script src="http://maps.google.com/maps?file=api&amp;v=2.44&amp;key=ABQIAAAASITAD3HrjcjYa0eX8ziX8BT9qabG24fVUpi8IpUmffGm9Ol0IRQtsqy30zW2XtzWFPUdgBpY-QOFQQ" type="text/javascript"></script><!-- <script src="http://maps.google.com/maps?file=api&amp;v=2.44&amp;key=ABQIAAAAdt-Rn4GDqjcKzbKhgHTmCBTY7rO2OYVjt7p88NiaKHGzvi0ajxSGYQygukf79rrthgwSMh1II-6c8g" type="text/javascript"></script> --><script type="text/javascript"><!--    //function load() {	
    //	if (GBrowserIsCompatible()) {
    //        
    //		var map = new GMap(document.getElementById("map"));
    //		map.setMapType(G_NORMAL_MAP);
    //		var type1_array = new Array();
    //		var type1_base = new GTileLayer(new GCopyrightCollection("Hochiminh 2009"), 13, 15);
    //		type1_base.getCopyright = function(a, b) {
    //			return "(c)2009 Hochiminh / (c)2009 CPM";
    //		}
    //		type1_base.getTileUrl = function(a, b) {
    //		var oldZoom = 17 - b;
    //			return  "http://anan-vietnam.com/maps/hcmc/index.php?x=" + a.x + "&y=" + a.y + "&zoom=" + oldZoom;
    //		}
    //		
    //		type1_array[0] = type1_base;
    //		var type1 = new GMapType(type1_array, G_NORMAL_MAP.getProjection(), "Hochiminh 2009", {shortName:"Hochiminh 2009",maxResolution:15,minResolution:13,errorMessage:''});
    //		map.addMapType(type1);
    //		map.setMapType(type1);
    //		
    //		var point = new GLatLng(10.769462,106.669607);
    //		
    //		map.centerAndZoom(point,3);
    //		//map.addControl(new GSmallMapControl());
    //		map.addControl(new GSmallMapControl());
    //		map.enableDragging();
    //		
    //		var ananicon = new GIcon();
    //		ananicon.image = "webmedia/images/marker.png";
    //		ananicon.shadow = "webmedia/images/shadow.png";
    //		ananicon.iconSize = new GSize(42, 31);
    //		ananicon.shadowSize = new GSize(64, 33);
    //		ananicon.iconAnchor = new GPoint(20, 30);
    //		ananicon.infoWindowAnchor = new GPoint(20, 1);
    //     	map.addOverlay(new GMarker(point,ananicon));
    //		
    //	} else {
    //     	document.getElementById("map").innerHTML = 'This browswer does not support Google Map API<br>'
    //		+'Supporting browser as follow.<br>'
    //		+'<ul><li>IE 5.5 or later (Windows)</li>'
    //		+'<li>Firefox 0.8 or later (Windows, Mac, Linux)</li>'
    //		+'<li>Safari 1.2.4 or later (Mac)</li>'
    //		+'<li>Netscape 7.1 or later (Windows, Mac, Linux)</li>'
    //		+'<li>Mozilla 1.4 or later (Windows, Mac, Linux)</li>'
    //		+'<li>Opera 7.5 or later (Windows, Mac, Linux)</li>'
    //		+'</ul>';
    //    }
    //}
    var WINDOW_HTML = '<div style="width: 210px;padding-right: 10px"><img src="webmedia/images/logo.jpg" alt="logo"/><a href="index.aspx">CPM</a> 436A/101 3 Tháng 2 P.12 Q.10 TP.HCM</div>';

    function load() {
        if (GBrowserIsCompatible()) {
            var map = new GMap2(document.getElementById("map"));
            map.addControl(new GSmallMapControl());
            map.addControl(new GMapTypeControl());
            map.setCenter(new GLatLng(10.769462, 106.669607), 16);

            var marker = new GMarker(new GLatLng(10.769462, 106.669607));
            map.addOverlay(marker);
            GEvent.addListener(marker, "click", function () {
                marker.openInfoWindowHtml(WINDOW_HTML);
            });
            marker.openInfoWindowHtml(WINDOW_HTML);
        }
    }
  --></script> 
<%--<div class="ourservice-detail">   
    <div class="ourservice-detail-header"><a href="index.aspx?s=RESOURCES"><%= (LanguageID == 1 ? "Our Resource" : "Tài nguyên") %></a> <img src="webmedia/images/bullet.jpg" alt="" />VN Map</div>
    <div class="ourservice-detail-container">
        <div class="ourservice-detail-maintit">VN Map</div>
        <div class="ourservice-detail-article">
            <div id="map" style="width: 498px; height: 498px;margin:20px 100px;"></div>      
        </div>
    </div>
</div>--%>
<script type="text/javascript">
    load();
</script>