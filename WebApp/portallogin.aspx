<%@ Page Title="" Language="C#" MasterPageFile="~/templates/admin_templates/default_temp/PortalLogin.Master" AutoEventWireup="true" CodeBehind="portallogin.aspx.cs" Inherits="WebApp.portallogin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="nousername">
				<div class="loginmsg">The username you entered is incorrect.</div>
            </div><!--nousername-->
            
            <div class="nopassword">
				<div class="loginmsg">The password you entered is incorrect.</div>
                <div class="loginf">
                    <div class="thumb"><img alt="" src="templates/admin_templates/default_temp/images/portals/avatar1.png"/></div>
                    <div class="userlogged">
                        <h4></h4>
                        <a href="#">Not <span></span>?</a> 
                    </div>
                </div><!--loginf-->
            </div><!--nopassword-->
            
        
                <div class="username">
                	<div class="usernameinner">                    	                         
                         <input placeholder="Username" name="username" id="username" type="text" runat="server"/>                         
                    </div>
                </div>
                
                <div class="password">
                	<div class="passwordinner">
                    	<input placeholder="Password" name="password" id="password" type="password" runat="server"/>
                    </div>
                </div>
               
                <asp:Button ID="btnSubmit" runat="server" UseSubmitBehavior="true" 
                Text="Sign In" onclick="btnSubmit_Click" />
                
                <div class="keep">
                    <div id="uniform-undefined" class="checker">
                        <span><input id="chkRemmberMe" checked="checked" name="chkRemmberMe" type="checkbox" runat="server"/></span>
                     </div> Keep me logged in
                </div>       
</asp:Content>
