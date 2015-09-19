<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_front_tab_control.aspx.cs" Inherits="WebApp.modules.admin.tabs.admin_front_tab_control" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet"  type="text/css" media="screen" href="../../../templates/admin_templates/default_temp/styles/main.css"/> 
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
     <div class="breadcrumbs">    
            CONNECTION &gt <span class="ContentsNavi_Bold"><asp:Literal ID="Literal_Title" runat="server"></asp:Literal></span>                                              
       </div>
    <asp:PlaceHolder ID="PlaceHolder1" EnableViewState="true" runat="server"></asp:PlaceHolder> 
    </form>
</body>
</html>
