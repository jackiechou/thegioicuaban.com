<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="portal_edit.aspx.cs" Inherits="WebApp.modules.admin.security.portals.portal_edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "portal_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 2000);
       }
       function onError() {
           setTimeout(getbacktostepone, 2000);
       }
       function okay() {
           parent.location.href = parent.location.href;
           window.parent.document.getElementById('ButtonEditDone').click();
       }
       function cancel() {
           window.parent.document.getElementById('ButtonEditCancel').click();
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

       function setToday(sender, args) {
           if (sender._textbox.get_element().value == "") {
               var today = new Date();
               var numberOfDaysToAdd = 0;
               today.setDate(today.getDate() + numberOfDaysToAdd)
               sender._selectedDate = today;
               //sender._selectedDate = new Date();
               // set the date back to the current date
               sender._textbox.set_Value(sender._selectedDate.format(sender._format))
           }
       }

       function setStartDate(sender, args) {
           var currentDate = new Date()
           var numberOfDaysToAdd = 0;
           currentDate.setDate(currentDate.getDate() + numberOfDaysToAdd)
           sender._startDate = currentDate;
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
                Quản Lý Portals: Cập nhật thông tin
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                    <table  border="0" cellspacing="0" cellpadding="4" class="form">
                        
                            <tr>
                                <td align="left">
                                    * Tên Portal</td>
                                <td align="left">
                                    <asp:TextBox ID="txtPortalName" runat="server" Width="300px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPortalName" runat="server" ErrorMessage="*" 
                                        SetFocusOnError="True" ControlToValidate="txtPortalName" 
                                        ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                </td>
                                <td align="left">
                                    HostFee</td>
                                <td align="left">
                                    <asp:TextBox ID="txtHostFee" runat="server"></asp:TextBox>
                                </td>
                           </tr>
                            <tr>
                                <td align="left">
                                   * Ứng Dụng</td>
                                <td align="left">
                                     <asp:DropDownList ID="ddlApplicationList" AutoPostBack="True" runat="server"   CssClass="combobox" ></asp:DropDownList>             
                                <asp:CustomValidator ID="CusValApp"  ControlToValidate="ddlApplicationList" 
                                Text="*" ForeColor="red" ClientValidationFunction="ClientValidateSelection" 
                                runat="server" ErrorMessage="Invalid, please select one" SetFocusOnError="True" Display="Dynamic"/>                                     
                                </td>
                                <td align="left">
                                    HostSpace</td>
                                <td align="left">
                                    <asp:TextBox ID="txtHostSpace" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    * Ngôn ngữ</td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlLanguageList"  runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td align="left">
                                    LogoFile</td>
                                <td align="left">
                                    <asp:Image ID="Logo_Img" runat="server" /><br />
                                    <input id="InputLogoFile" type="file" runat="server" />
                                     <input id="HiddenLogoFile" type="hidden" value="" runat="server"/>
                                     <asp:RegularExpressionValidator ID="revLogoFile" runat="server" 
                                        SetFocusOnError="True" ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$"    
                                        ErrorMessage="*" ValidationGroup="ValidationCheck" 
                                            ControlToValidate="InputLogoFile"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    * Loại tiền tệ</td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlCurrencyTypeList" runat="server">
                                    </asp:DropDownList>   
                                </td>
                                <td align="left">
                                    BackgroundFile</td>
                                <td align="left">
                                    <asp:Image ID="Background_Img" runat="server" /><br />
                                    <input id="InputBackgroundFile" type="file"  runat="server"/>
                                    <input id="HiddenBackgroundFile" type="hidden" value="" runat="server"/>
                                      <asp:RegularExpressionValidator ID="revBackgroundFile" runat="server" 
                                        SetFocusOnError="True" ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$"    
                                        ErrorMessage="*" ValidationGroup="ValidationCheck" 
                                            ControlToValidate="InputBackgroundFile"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    * HomeDirectory</td>
                                <td align="left">
                                    <asp:TextBox ID="txtHomeDirectory" Width="300px" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" 
                                        SetFocusOnError="True" ControlToValidate="txtHomeDirectory" 
                                        ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                </td>
                                <td align="left">
                                    Ngày hết hạn</td>
                                <td align="left">
                                     <asp:TextBox ID="txtExpiryDate" runat="server"></asp:TextBox>
                                     <img src="../../../../images/icons/calendar.gif" alt="Click" width="24" height="17" id="btnShowDate" />
                                    <ajaxtoolkit:CalendarExtender ID="CalendarExtender_ExpiryDate" runat="server"                                                   
                                    OnClientShowing="setStartDate" OnClientShown="setToday" CssClass="ajax__calendar"
                                    Animated="true" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="btnShowDate" 
                                    PopupPosition="Right" TargetControlID="txtExpiryDate">
                                </ajaxtoolkit:CalendarExtender>     
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    * Url</td>
                                <td align="left">
                                    <asp:TextBox ID="txtUrl" runat="server" Width="300px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" 
                                        SetFocusOnError="True" ControlToValidate="txtUrl" 
                                        ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                </td>
                                <td align="left">
                                    FooterText</td>
                                <td align="left">
                                    <asp:TextBox ID="txtFooterText" Width="300px" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    KeyWords</td>
                                <td align="left">
                                     <asp:TextBox ID="txtKeyWords" Width="300px" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </td>
                                <td align="left">
                                   Description </td>
                                <td align="left">
                                    <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" Width="300px"></asp:TextBox>
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