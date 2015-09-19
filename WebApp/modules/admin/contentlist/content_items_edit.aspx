<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="content_items_edit.aspx.cs" Inherits="WebApp.modules.admin.contentlist.content_items_edit" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "content_items_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 1000);
       }
       function onError() {
           setTimeout(getbacktostepone, 1000);
       }
       function okay() {
           var mode = $get('hdnWindowUIMODE').value;
           if (mode == "edit"){
               parent.location.href = parent.location.href;
               window.parent.document.getElementById('ButtonEditDone').click();
           }else {
               parent.location.href = parent.location.href;
               window.parent.document.getElementById('btnOkay').click();
              // getbacktostepone();
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
                    <table style="width: 411px" >
                        <tr>
                            <td>
                                ContentTypeID:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlContentTypes" runat="server">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvContentTypes" runat="server" ValidationGroup="ValidationCheck" 
                                    ErrorMessage="*" ControlToValidate="ddlContentTypes"></asp:RequiredFieldValidator>
                            </td>
                        </tr>       
                        <tr>
                            <td>
                                Contents:
                            </td>
                            <td>
                                <asp:TextBox ID="txtContents" Width="245px" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvContents" runat="server" ValidationGroup="ValidationCheck"  
                                    ErrorMessage="*" ControlToValidate="txtContents"></asp:RequiredFieldValidator>
                            </td>
                        </tr>  
                              
                        <tr>
                            <td>
                                ContentKey:
                            </td>
                            <td>
                                <asp:TextBox ID="txtContentKey" Width="245px" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvContentKey" runat="server" ValidationGroup="ValidationCheck" 
                                    ErrorMessage="*" ControlToValidate="txtContentKey"></asp:RequiredFieldValidator>
                            </td>
                            </tr>
                             <tr>
                                <td>
                                    Indexed:
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="rdlIndexed" runat="server" DataTextField="Status" 
                                        DataValueField="Status" SelectedValue='<%# Bind("Indexed") %>' 
                                        RepeatDirection="Horizontal" Width="166px">                                                                                                                
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                     </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator ID="rfvIndexed" runat="server" ValidationGroup="ValidationCheck" 
                                        ErrorMessage="*" ControlToValidate="rdlIndexed"></asp:RequiredFieldValidator>
                                </td>
                            </tr>                                   
                    </table>
                    <div class="popup_Buttons">
                        <asp:Button ID="btnOkay" Text="Done" runat="server" OnClick="btnOkay_Click" />
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
