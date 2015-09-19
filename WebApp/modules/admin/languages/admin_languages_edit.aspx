<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_languages_edit.aspx.cs" Inherits="WebApp.modules.admin.languages.admin_languages_edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "modules/admin/languages/admin_languages_edit.aspx";
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

       function FormatNumber(obj) {
           var strvalue;
           if (eval(obj))
               strvalue = eval(obj).value;
           else
               strvalue = obj;
           var num;
           num = strvalue.toString().replace(/\$|\,/g, '');

           if (isNaN(num))
               num = "";
           sign = (num == (num = Math.abs(num)));
           num = Math.floor(num * 100 + 0.50000000001);
           num = Math.floor(num / 100).toString();
           for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
               num = num.substring(0, num.length - (4 * i + 3)) + ',' +
            num.substring(num.length - (4 * i + 3));
           //return (((sign)?'':'-') + num);
           eval(obj).value = (((sign) ? '' : '-') + num);
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
                Quản lý ngôn ngữ
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                    <table border="0" cellspacing="0" cellpadding="4"> 
                         <tr>
                             <td align="left" style="width:150px">
                                 * Tên ngôn ngữ:</td>
                             <td align="left">
                                 <asp:TextBox ID="txtCultureName" runat="server"></asp:TextBox>
                                 <asp:RequiredFieldValidator ID="rfvCultureName" runat="server" 
                                     ControlToValidate="txtCultureName" ErrorMessage="*" SetFocusOnError="True" 
                                     ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                             </td>
                        </tr>
                                              
                         <tr>
                            <td align="left" style="width:150px">
                                * Mã ngôn ngữ</td>
                            <td align="left">                                
                                <asp:TextBox ID="txtCultureCode" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvCultureCode" runat="server" ErrorMessage="*" 
                                    SetFocusOnError="True" ControlToValidate="txtCultureCode" 
                                    ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>                                                               
                            </td>
                        </tr>                           
                       
                         <tr>
                             <td align="left" style="width:150px">
                                 * Đặc tả</td>
                             <td align="left">
                                 <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
                                 <asp:RequiredFieldValidator ID="rfvescription" runat="server" ErrorMessage="*" 
                                    SetFocusOnError="True" ControlToValidate="txtDescription" 
                                    ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                             </td>
                         </tr>
                       
                        <tr>
                            <td align="left">
                            Tình trạng</td>
                            <td align="left">
                            <asp:RadioButtonList ID="rdlDiscontinued" runat="server" RepeatDirection="Horizontal" Width="300px">                                                                                                 
                            </asp:RadioButtonList>
                        </td>
                        </tr>     
                    </table>
                     <div class="popup_Buttons">
                        <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="ValidationCheck" OnClick="btnOkay_Click" />
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