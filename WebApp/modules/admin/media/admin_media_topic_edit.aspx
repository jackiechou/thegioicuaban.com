<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_media_topic_edit.aspx.cs" Inherits="WebApp.modules.admin.media.admin_media_topic_edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "modules/admin/media/admin_media_topic_edit.aspx";
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
                Media 
                Topic: Cập Nhập Thông Tin</div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                    <table border="0" cellspacing="0" cellpadding="4"> 
                                              
                           <tr>
                            <td align="left">
                                TypeId</td>
                            <td align="left">  
                        <asp:DropDownList ID="ddlMediaTypeList" runat="server" CssClass="combobox" 
                            onselectedindexchanged="ddlMediaTypeList_SelectedIndexChanged">
                        </asp:DropDownList></td>
                        </tr>       
                                              
                           <tr>
                               <td align="left">
                                   Parent:</td>
                               <td align="left">
                                   <asp:DropDownList ID="ddlMediaTopicList" runat="server" Height="18px" 
                                       Width="174px">
                                   </asp:DropDownList>
                               </td>
                           </tr>
                                              
                           <tr>
                               <td align="left">
                                   * TopicName:</td>
                               <td align="left">
                                   <asp:TextBox ID="txtTopicName" runat="server" ValidationGroup="ValidationCheck" 
                                       Width="348px"></asp:TextBox>
                                   <asp:RequiredFieldValidator ID="rfvTopicName" runat="server" 
                                       ControlToValidate="txtTopicName" Display="Dynamic" ErrorMessage="Not null" 
                                       ForeColor="#FF3300" SetFocusOnError="True" 
                                       ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                               </td>
                           </tr>
                           <tr>
                               <td align="left">
                                   Photo:</td>
                               <td align="left">
                                   <input id="myFile" type="file" runat="server"/>
                                   <asp:Image ID="imgPhoto" runat="server" Height="16px" ImageAlign="Left" 
                                       Width="16px" />
                                   &nbsp;<asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                                       runat="server" ControlToValidate="myFile" ErrorMessage="Loại file k hợp lệ" 
                                       SetFocusOnError="True" 
                                       ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$" 
                                       ValidationGroup="checkValidation"></asp:RegularExpressionValidator>
                               </td>
                           </tr>
                           <tr>
                               <td align="left">
                                   Description:</td>
                               <td align="left">
                                   <asp:TextBox ID="txtDescription" runat="server" Height="103px" 
                                       TextMode="MultiLine" Width="348px"></asp:TextBox>
                               </td>
                           </tr>
                           <tr>
                               <td align="left">
                                   Status:</td>
                               <td align="left">
                                   <asp:RadioButtonList ID="rdlStatus" runat="server" RepeatDirection="Horizontal" 
                                       Width="300px">
                                   </asp:RadioButtonList>
                               </td>
                           </tr>
                                              
                    </table>
                     <div class="popup_Buttons">
                        <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="ValidationCheck" OnClientClick="return false;" OnClick="btnOkay_Click" />
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
