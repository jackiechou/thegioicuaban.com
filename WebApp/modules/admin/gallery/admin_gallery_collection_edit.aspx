<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_gallery_collection_edit.aspx.cs" Inherits="WebApp.modules.admin.gallery.admin_gallery_collection_edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
<script language="javascript" type="text/javascript">
    function getbacktostepone() {
        window.location = "admin_gallery_collection_edit.aspx";
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
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
  <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Quản lý Gallery Collection</div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                 <asp:View ID="ViewInput" runat="server">                   
                    <div class="popup_Buttons">
                        <table border="0" cellpadding="4" cellspacing="0">
                            <tr>
                                <td align="left">
                                    * Gallery Topics <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" InitialValue="0" ControlToValidate="ddlTreeNode_Topics"
                                    ErrorMessage="(*)" SetFocusOnError="true" ></asp:RequiredFieldValidator></td>
                                <td align="left" style="height: 15px" colspan="2">
                                    <asp:DropDownList ID="ddlTreeNode_Topics" runat="server" AutoPostBack="true" Width="288px">
                                    </asp:DropDownList>
                                    
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    * Title  <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                                        ControlToValidate="txtTitle" ErrorMessage="*"></asp:RequiredFieldValidator></td>
                                <td align="left" colspan="2" style="height: 15px">
                                    <asp:TextBox ID="txtTitle" runat="server" TextMode="SingleLine" Width="520px"></asp:TextBox>                                   
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    IconFile</td>
                                <td align="left" style="vertical-align:top;">
                                    <div id="divFileName" style="display:block;">
                                        <input id="File1" type="file" runat="server" />   
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator" runat="server" 
                                            ControlToValidate="File1" ErrorMessage="Loại file k hợp lệ" 
                                            SetFocusOnError="True" 
                                            ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$" 
                                            ValidationGroup="ValidationControls"></asp:RegularExpressionValidator><br /><br />
                                        <asp:Literal ID="ltrFileName" runat="server"></asp:Literal>
                                    </div>
                                </td>
                                <td align="left">
                                    <asp:Image ID="imgPhoto" runat="server" Height="94px" Width="103px" />
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    * Description <asp:RequiredFieldValidator ID="rfvDescription" runat="server" 
                                        ControlToValidate="txtDescription" Display="Dynamic" SetFocusOnError="true" ErrorMessage="*"></asp:RequiredFieldValidator></td>
                                <td align="left" style="height: 15px" colspan="2">
                                    <asp:TextBox ID="txtDescription" runat="server" Height="180px" TextMode="MultiLine" 
                                        Width="523px"></asp:TextBox>                                    
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    Tags</td>
                                <td align="left" style="height: 15px" colspan="2">
                                    <asp:TextBox ID="txtTags" runat="server" TextMode="SingleLine" Width="520px"></asp:TextBox>
                                </td>
                            </tr>
                             <tr>
                                <td align="left">
                                    URl</td>
                                <td align="left" style="height: 15px" colspan="2">
                                    <asp:TextBox ID="txtUrl" runat="server" TextMode="SingleLine" Width="520px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    Status</td>
                                <td align="left" colspan="2">
                                    <asp:RadioButtonList ID="rdlStatus" runat="server" DataTextField="Status" 
                                        DataValueField="Status" RepeatDirection="Horizontal" 
                                        SelectedValue='<%# Bind("Status") %>' Width="213px">
                                        <asp:ListItem Value="1" Selected="True">Active</asp:ListItem>
                                        <asp:ListItem Value="0">Discontinued</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <asp:Button ID="btnOkay" Text="Done" runat="server" OnClientClick="return false;" OnClick="btnOkay_Click" />
                        <input id="Button1" value="Cancel" type="button" onclick="cancel();" />
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

