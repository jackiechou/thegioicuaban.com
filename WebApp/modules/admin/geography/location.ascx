<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="location.ascx.cs" Inherits="WebApp.modules.admin.geography.location" %>
  <style type="text/css">
        .style1
        {
            font-size: x-large;
            font-weight: bold;
            font-family: "Arial Black";
            text-transform: uppercase;
        }
        .style2
        {
            text-align: center;
        }
        .style3
        {
            text-align: center;
            color: #336600;
        }
    </style>

<div class="style1">
    
        <p class="style3">
            Find Ip Address and location Of Client In ASP.NET</p>
        <p class="style2">
            Your network Details</p>
        <p class="style2">
            Ip address :             <asp:Label ID="lblip" runat="server" Text="Label"></asp:Label>
        </p>
        <p class="style2">
            country Name :
            <asp:Label ID="lblcountry" runat="server" Text="Label"></asp:Label>
        </p>
        <p class="style2">
            Region Name :
            <asp:Label ID="lblregion" runat="server" Text="Label"></asp:Label>
        </p>
        <p class="style2">
            city Name :
            <asp:Label ID="lblcity" runat="server" Text="Label"></asp:Label>
        </p>
    
    </div>