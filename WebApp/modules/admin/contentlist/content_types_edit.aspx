<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="content_types_edit.aspx.cs" Inherits="WebApp.modules.admin.contentlist.content_types_edit" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "content_types_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 1000);
       }
       function onError() {
           setTimeout(getbacktostepone, 1000);
       }
       function okay() {
           var mode = $get('hdnWindowUIMODE').value;
           if (mode == "edit")
               window.parent.document.getElementById('ButtonEditDone').click();
           else {
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
                    <table >
                        <tr>
                            <td>
                                ContentType:
                            </td>
                            <td>
                                <asp:TextBox ID="txtContentType" Width="245px" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvContentType" runat="server" 
                                    ErrorMessage="*" ControlToValidate="txtContentType"></asp:RequiredFieldValidator>
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
            <asp:Button ID="btnOkay" Text="Done" runat="server" OnClick="btnOkay_Click" />
            <input id="btnCancel" value="Cancel" type="button" onclick="cancel();" />
        </div>
    </div>
    </form>
</body>
</html>
