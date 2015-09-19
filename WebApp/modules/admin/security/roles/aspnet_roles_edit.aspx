<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aspnet_roles_edit.aspx.cs" Inherits="WebApp.modules.admin.security.roles.aspnet_roles_edit" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "aspnet_roles_edit.aspx";
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

       function data_change(field) {
           var check = true;
           var value = field.value; //get characters
           //check that all characters are digits, ., -, or ""
           for (var i = 0; i < field.value.length; ++i) {
               var new_key = value.charAt(i); //cycle through characters
               if (((new_key < "0") || (new_key > "9")) &&
                    !(new_key == "")) {
                   check = false;
                   break;
               }
           }
           //apply appropriate colour based on value
           if (!check) {
               field.style.backgroundColor = "red";
           }
           else {
               field.style.backgroundColor = "white";
           }
       }
    </script>

</head>
<body>
    <form id="form1" runat="server">
  <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Edit
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                    <table border="0" cellspacing="0" cellpadding="4">
                       <tr>
                          <td align="left" >
                              * Application Id</td>
                            <td align="left">
                                <asp:DropDownList ID="ddlApplicationList" AutoPostBack="True" runat="server"   CssClass="combobox" ></asp:DropDownList>             
                                <asp:CustomValidator ID="CusValApp"  ControlToValidate="ddlApplicationList" 
                                Text="*" ForeColor="red" ClientValidationFunction="ClientValidateSelection" 
                                runat="server" ErrorMessage="Invalid, please select one" SetFocusOnError="True" Display="Dynamic"/>                                     
                            </td>
                       </tr>      
                       <tr>
                          <td align="left">
                              Role Name: </td>
                            <td align="left">
                                <asp:TextBox ID="txtRoleName" runat="server" Width="280px"></asp:TextBox>
                            </td>
                       </tr> 
                       <tr>
                          <td align="left">
                              Role Description: </td>
                            <td align="left">
                                <asp:TextBox ID="txtDescription" runat="server" Width="280px"></asp:TextBox>
                            </td>
                       </tr>                                                                                                                                                                                                         
                   </table>
                </asp:View>
                <asp:View ID="ViewSuccess" runat="server">
                     <p>
                        You have added a new expanse successfully<br />
                        Dialog will close automatically in two second</p>                    
               </asp:View>               
                <asp:View ID="ViewError" runat="server">
                    <p>
                        Error occured adding expanse<br />
                        Please wait<br />
                        Redirecting to step one</p>
                </asp:View>
            </asp:MultiView>
        </div>
        <div class="popup_Buttons">
            <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="checkValidation" OnClientClick="return false;" OnClick="btnOkay_Click" />
            <input id="btnCancel" value="Cancel" type="button" onclick="cancel();" />
        </div>
    </div>
    </form>
</body>
</html>