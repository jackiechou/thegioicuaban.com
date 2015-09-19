<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_modules_add.aspx.cs" Inherits="WebApp.modules.admin.dashboard.modules.admin_modules_add" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>add</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "admin_modules_add.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 2000);
       }
       function onError() {
           setTimeout(getbacktostepone, 2000);
       }
       function okay() {
           parent.location.href = parent.location.href;
           window.parent.document.getElementById('btnOkay').click();
           getbacktostepone();
       }
       function cancel() {
           window.parent.document.getElementById('btnCancel').click();
       }


       // disables the button specified and sets its style to a disabled "look".
       function disableButtonOnClick(oButton, sButtonText, sCssClass) {
           oButton.disabled = true;      // set button to disabled so you can't click on it.
           oButton.value = sButtonText;   // change the text of the button.
           oButton.setAttribute('className', sCssClass); // IE uses className for the css property.
           oButton.setAttribute('class', sCssClass); // Firefox, Safari use class for the css property.  (doesn't hurt to do both).
       }

       //create a function that accepts an input variable (which key was key pressed)
       function disableEnterKey(e) {
           var key;

           //if the users browser is internet explorer
           if (window.event) {
               key = window.event.keyCode; //store the key code (Key number) of the pressed key               
           } else { //otherwise, it is firefox store the key code (Key number) of the pressed key
               key = e.which;
           }

           //if key 13 is pressed (the enter key)
           if (key == 13) {
               if (e.preventDefault)
                   e.preventDefault();
               return false; //do nothing               
           } else {
               return true; //continue as normal (allow the key press for keys other than "enter")
           }
           //<input type="text" name="mytext" onKeyPress="return disableEnterKey(event)">  
       } 
    </script>
</head>
<body>
    <form id="form1" runat="server">  
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Thêm Module Chức Năng
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
     
                  <table cellpadding="0" cellspacing="5px">
                    <tr>
                        <td class="style2">Chọn Portal</td>
                        <td class="style3" >
                            <asp:DropDownList ID="ddlPortalList" runat="server" Width="200px">
                            </asp:DropDownList>
                        </td>
                    </tr>     

                    <tr>
                        <td class="style2">Tên Module</td>
                        <td class="style3" >
                            <asp:TextBox ID="txtName" runat="server" Width="200px"></asp:TextBox></td>
                    </tr>          
                  
                      <tr>
                          <td class="style2">
                              Control Src</td>
                          <td class="style3">
                          <input id="ControlSrc_File" type="file" runat="server"/> 
                          </td>
                      </tr>
                  
                      <tr>
                          <td class="style2">
                              Tab</td>
                          <td class="style3">
                                <asp:DropDownList ID="ddlTabList" runat="server">
                                </asp:DropDownList>
                          </td>
                      </tr>
                  
                    <tr>
                        <td class="style2" >
                            Control Key</td>
                        <td class="style3">
                            <asp:TextBox ID="txtControlKey" runat="server" Width="200px"></asp:TextBox>
                        </td>
                    </tr> 
                      <tr>
                          <td class="style3" colspan="2">
                              <asp:CheckBox ID="ckbAllTab" runat="server" Text="View All Tabs" />
                              <asp:CheckBox ID="ckbIsAdmin" runat="server" Text="Is Admin" />
                              <asp:CheckBox ID="ckbIsDeleted" runat="server" Text="Allow Delete" />
                              <asp:CheckBox ID="ckbckbInheritViewPermissions" runat="server" 
                                  Text="Allow View Permission" />
                          </td>
                      </tr>
                    </table>
                      <div class="popup_Buttons">
                            <asp:Button ID="btnOkay" Text="Done" runat="server" 
                                OnClientClick="return false;" OnClick="btnOkay_Click" />
                            <input id="btnCancel" value="Cancel" type="button" onclick="cancel();" />
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
        
    </div>


    </form>
</body>
</html>
