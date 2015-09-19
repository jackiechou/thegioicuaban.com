<%@ Page Title="" Language="C#" MasterPageFile="~/templates/admin_templates/default_temp/SiteMaster.Master" EnableViewState="true" EnableSessionState="True" MaintainScrollPositionOnPostback="true"  AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="WebApp.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
$(function() {
    setInterval(KeepSessionAlive, 10000);
});
function KeepSessionAlive() {
    $.post("/handlers/KeepSessionAlive.ashx", null, function () {
        $("#result").append("<p>Session is alive!<p/>");
    });
}    
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%------------------------ Load User Controls -------------------------------------------------------%>
<asp:ScriptManager ID="ScriptManager1" runat="server"  EnableScriptGlobalization="true" EnableScriptLocalization="true" EnableHistory="true" />
<asp:UpdatePanel ID="UpdatePanel_Container" runat="server" UpdateMode="Conditional">
     <ContentTemplate>  
        <asp:UpdateProgress ID="UpdateProgress_Container" runat="server" DisplayAfter="10" AssociatedUpdatePanelID="UpdatePanel_Container">
            <ProgressTemplate>
                <div style="position:absolute; top:50%; left:50%; margin-left:-250px; width:300px; background-color:#FFFFFF; color:#000000; text-align:center;">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/icons/loading.gif" /> 
                    <asp:Label ID="LabelLoadingText" Text="Please wait..." runat="server"  />
                    </div>
            </ProgressTemplate>
        </asp:UpdateProgress>   
       <div class="breadCrumb">
            <asp:Literal ID="Literal_Title" runat="server"></asp:Literal>                                          
       </div>
       <asp:PlaceHolder ID="PlaceHolder1" EnableViewState="true" runat="server"></asp:PlaceHolder>     
       <iframe id="frame1" runat="server" scrolling="no" enableviewstate="true" frameborder="0" visible="false" style="min-height:600px;height:100%;width:100%"></iframe>         
    </ContentTemplate>
<%------------------------END Load User Controls -------------------------------------------------------%>  
</asp:UpdatePanel>
<div id="result"></div>               
 </asp:Content>
