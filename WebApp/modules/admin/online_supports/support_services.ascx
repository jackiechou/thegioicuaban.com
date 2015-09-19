<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="support_services.ascx.cs" Inherits="WebApp.modules.admin.online_supports.support_services" %>
<%--<script type="text/javascript" src="../../../scripts/jquery/jquery.min.js"></script> --%> 
     
<%--<script type="text/javascript">
    jQuery(document).ready(function ($) {
        //var YahooId = 'kengamtrang,phuonganh_ticon,minhrhett';            
        var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
        $.ajax({
            type: "POST",
            url: base_url + "/services/vassws.asmx/CheckYahoo",
            //data: "{strYahooIDs:'" + YahooId + "'}",
            data: "{}",
            beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $("#<%=lblYahooStatus.ClientID%>").html(msg.d);
            }, error: function (e) {
                $("#<%=lblYahooStatus.ClientID%>").html("Ko truy van duoc du lieu");
            }
        });
    });
</script>--%>
<script type="text/javascript" src="http://download.skype.com/share/skypebuttons/js/skypeCheck.js"></script>
<asp:Label ID="lblSupportOnline" runat="server" Text="Label"></asp:Label>