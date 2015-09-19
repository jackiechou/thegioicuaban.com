<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_modules_edit.aspx.cs" Inherits="WebApp.modules.admin.dashboard.modules.admin_modules_edit" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>

<script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "admin_modules_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 2000);
       }
       function onError() {
           setTimeout(getbacktostepone, 2000);
       }
       function okay() {
           var mode = $get('hdnWindowUIMODE').value;
           if (mode == "edit") {
               parent.location.href = parent.location.href;
               window.parent.document.getElementById('ButtonEditDone').click();
           } else {
               parent.location.href = parent.location.href;
               window.parent.document.getElementById('btnOkay').click();
               getbacktostepone();
           }
       }
       function cancel() {
           var mode = $get('hdnWindowUIMODE').value;
           if (mode == "edit")
               window.parent.document.getElementById('ButtonEditCancel').click();
           else
               window.parent.document.getElementById('btnCancel').click();
       }

       // disables the button specified and sets its style to a disabled "look".
       function disableButtonOnClick(oButton, sButtonText, sCssClass) {
           oButton.disabled = true;      // set button to disabled so you can't click on it.
           oButton.value = sButtonText;   // change the text of the button.
           oButton.setAttribute('className', sCssClass); // IE uses className for the css property.
           oButton.setAttribute('class', sCssClass); // Firefox, Safari use class for the css property.  (doesn't hurt to do both).
       }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
     

    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Edit
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">
        
            <ajaxToolkit:TabContainer runat="server" ID="Tabs" Height="450px" 
                ActiveTabIndex="0" Width="550px" CssClass="google">
                <ajaxToolkit:TabPanel runat="server" ID="Panel1" HeaderText="Module Setting">
                    <ContentTemplate>
                        <asp:UpdatePanel ID="updatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="tabBody">                             
                                          
                                       <asp:MultiView ID="MultiView1" runat="server">
                                            <asp:View ID="ViewInput" runat="server">
                                                <table cellpadding="0" cellspacing="5px">
                                                    <tr>
                                                        <td>Application</td>
                                                        <td >
                                                            <asp:DropDownList ID="ddlApplicationList" runat="server" CssClass="combobox"  
                                                                Width="200px" onselectedindexchanged="ddlApplicationList_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>  
                                             
                                                    <tr>
                                                        <td>
                                                           Chọn Portal</td>
                                                        <td>
                                                           <asp:DropDownList ID="ddlPortalList" runat="server" CssClass="combobox" Width="200px">
                                                            </asp:DropDownList></td>
                                                    </tr>
                                             
                                                    <tr>
                                                        <td>Module Name</td>
                                                        <td >
                                                            <asp:TextBox ID="txtModuleTitle" runat="server" Width="192px"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvModuleTitle" ControlToValidate="txtModuleTitle" ValidationGroup="ValidationCheck" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                         </td>

                                                    </tr>            
                        
                                                    <tr>
                                                        <td colspan="2" >
                                                            <asp:CheckBox ID="ckbAllTab" runat="server" Text="View All Tabs" />
                                                            <asp:CheckBox ID="ckbIsAdmin" runat="server" Text="Is Admin" />
                                                            <asp:CheckBox ID="ckbIsDeleted" runat="server" Text="Allow Delete" />
                                                            <asp:CheckBox ID="ckbInheritViewPermissions" runat="server" 
                                                                Text="Inherit View Permissions" />
                                                        </td>
                                                    </tr> 

                                                    <tr>
                                                        <td colspan="2" >
                                                            <asp:Button ID="btnOkay" Text="Done" runat="server" 
                                                                ValidationGroup="ValidationCheck" onclick="btnOkay_Click"/>
                                                              <input id="btnCancel" value="Cancel" type="button"/>
                                                        </td>
                                                    </tr> 
                                                </table>
                       
                                              <div class="popup_Buttons">
                                                <asp:Button ID="Button1" Text="Done" runat="server" ValidationGroup="ValidationCheck" OnClientClick="return false;" OnClick="btnOkay_Click" />
                                                <input id="Button2" value="Cancel" type="button" onclick="cancel();" />
                                            </div>
                                        </asp:View>
                                        <asp:View ID="ViewSuccess" runat="server">
                                                <p>
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                            </p>
                                                <input id="btnClose" value="Cancel" type="button" onclick="cancel();" />                  
                                        </asp:View>               
                                        <asp:View ID="ViewError" runat="server">
                                            <p>
                                                <asp:Label ID="lblErrorMsg" runat="server"></asp:Label>
                                            </p>
                                            <input id="btnExit" value="Cancel" type="button" onclick="cancel();" /> 
                                        </asp:View>
                                    </asp:MultiView>
                                   
                                </div>                        
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            
                <ajaxToolkit:TabPanel runat="server" ID="Panel2" HeaderText="Module Controls" >
                    <ContentTemplate>
                        <div class="tabBody">
                            <iframe id="IframeModuleController" runat="server" frameborder="0" height="300" width="600" scrolling="no">
                            </iframe>
                        </div>  
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
        
                <ajaxToolkit:TabPanel runat="server" ID="Panel3" HeaderText="Permission">
                    <ContentTemplate>
                        <div class="tabBody">                  
                            <iframe id="IframePermission" runat="server" frameborder="0" height="300" width="600" scrolling="no">
                            </iframe>
                        </div> 
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer> 

         </div>        
    </div>
 
          </form>
</body>
</html>