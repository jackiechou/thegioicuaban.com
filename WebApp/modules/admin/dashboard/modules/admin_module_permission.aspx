<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_module_permission.aspx.cs" Inherits="WebApp.modules.admin.dashboard.modules.admin_module_permission" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>module_permission</title>

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

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Add Module Control to Module
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
          <%-- <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">--%>
              <table cellpadding="0" cellspacing="5px">
                    <tr>
                        <td>Permission</td>
                        <td >
                            <asp:DropDownList ID="ddlPermissionList" runat="server" Width="198px">
                            </asp:DropDownList>
                        </td>
                    </tr>                                              
                    <tr>
                        <td>Role</td>
                        <td >
                            <asp:DropDownList ID="ddlRoleList" runat="server" Width="198px">
                            </asp:DropDownList>
                            </td>
                    </tr>  		
                    <tr>
                        <td colspan="2" >
                            <asp:CheckBox ID="ckbAllowAccess" runat="server" Text="Allow Access" />                                                       
                        </td>
                    </tr> 

                    <tr>
                        <td colspan="2" >
                            <asp:Button ID="btnSubmit" Text="Done" runat="server" ValidationGroup="ValidationCheck" OnClientClick="return false;"/>
                                <input id="btnReset" value="Cancel" type="button"/>
                        </td>
                    </tr> 
                </table>   
                <%--</asp:View>
                <asp:View ID="ViewSuccess" runat="server">
                    <p>
                        You have added a new expanse successfully<br /><br />
                        Dialog will close automatically in two second</p>                   
                </asp:View>
                <asp:View ID="ViewError" runat="server">
                    <p>
                        Error occured adding expanse<br />
                        Please wait<br />
                        Redirecting to step one</p>
                </asp:View>
            </asp:MultiView>--%>
        </div>
        <div class="popup_Buttons">
            <asp:Button ID="btnOkay" Text="Done" runat="server" 
                OnClientClick="return false;" OnClick="btnOkay_Click" />
            <input id="btnCancel" value="Cancel" type="button" onclick="cancel();" />
        </div>
    </div>

    </form>
</body>
</html>
