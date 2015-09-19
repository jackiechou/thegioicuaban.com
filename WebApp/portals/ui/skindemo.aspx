<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="skindemo.aspx.cs" Inherits="WebApp.portals.ui.skindemo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       protected System.Web.UI.WebControls.RadioButton optHost;
        protected System.Web.UI.WebControls.RadioButton optSite;
        protected System.Web.UI.WebControls.DropDownList cboSkin;
      

        <asp:RadioButton ID="optHost" runat="server" />
        <asp:RadioButton ID="optSite" runat="server" />
        <asp:DropDownList ID="cboSkin" runat="server">
        </asp:DropDownList>
      <asp:Literal ID="Literal_Skin" runat="server"></asp:Literal>
    </div>

    </form>
</body>
</html>
