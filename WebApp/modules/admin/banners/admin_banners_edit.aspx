<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_banners_edit.aspx.cs" Inherits="WebApp.modules.admin.banners.admin_banners_edit" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function validateOrFields(source, args) {
           var s1 = document.getElementById('<%= FileInput.ClientID %>').value;
           var s2 = document.getElementById('<%= txtUrl.ClientID %>').value;

           if (s1 == "" && s2 == "") {
               args.IsValid = false;
           }
           else {
               args.IsValid = true;
           }
           return;
       }
         
       function getbacktostepone() {
           window.location = "/modules/admin/banners/admin_banners_edit.aspx";
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

       //============================================================================================================
       function validateDate(sender, args) {
           if (sender._selectedDate < new Date()) {
               alert("Bạn không thể chọn ngày trước ngày hiện tại!");
               sender._selectedDate = new Date();
               // set the date back to the current date
               sender._textbox.set_Value(sender._selectedDate.format(sender._format))
           }
       }

       function checkDate(sender, args) {
           var start_date = document.getElementById('<%#txtStartDate.ClientID %>').value;
           var end_date = document.getElementById("<%#txtEndDate.ClientID %>").value;
           var s1 = new Date(start_date);
           var e1 = new Date(end_date);
           if (e1 < s1) {
               args.IsValid = false
           }
           else {
               args.IsValid = true
           }
       }


       function setToday(sender, args) {
           if (sender._textbox.get_element().value == "") {
               sender._selectedDate = new Date();
               // set the date back to the current date
               sender._textbox.set_Value(sender._selectedDate.format(sender._format))
           }
       }

       function setStartDate(sender, args) {
           var currentDate = new Date()
           currentDate.setDate(currentDate.getDate() + 2)
           sender._startDate = currentDate;
       }

       function setEndDate(sender, args) {
           var start_date = document.getElementById('<%#txtStartDate.ClientID %>').value;
           var end_date = document.getElementById("<%#txtEndDate.ClientID %>").value;

           var arr = start_date.split('/');                                 // Split the string on the /            
           var formatted_start_date = arr[2] + "," + arr[1] + "," + arr[0]; // Change the array so the format is yyyy,mm,dd

           var s1 = new Date(formatted_start_date)
           var day = s1.getDate();
           var month = ((s1.getMonth().length + 1) === 1) ? (s1.getMonth() + 1) : '0' + (s1.getMonth() + 1);
           var year = s1.getFullYear();
           var _daysInMonth = daysInMonth(month, year);

           var expiredDate;
           if (day <= (_daysInMonth - 2)) {
               day = ((day < 10) ? "0" : "") + day;
               expiredDate = day + "/" + month + "/" + (year + 1);
           } else {
               expiredDate = "02" + "/" + month + "/" + (year + 1);
           }
           $("#<%#txtEndDate.ClientID%>").val(expiredDate);

           ////          end_date.CalendarBehavior.initialize;
           ////          end_date.CalendarBehavior.set_endDate(s1);      
           ////          $find("CalendarExtender_ExpiredDate").set_selectedDate(sender.get_selectedDate());
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
                Quản lý Banner: Cập nhật banner</div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                    <table border="0" cellspacing="0" cellpadding="4"> 
                         <tr>
                             <td align="left" style="width:150px">
                                 * Ngôn ngữ</td>
                             <td align="left">
                                <asp:DropDownList ID="ddlCultureList" runat="server"></asp:DropDownList>
                             </td>
                             <td align="left">
                                 * Vị trí Banner:</td>
                             <td align="left">
                               <asp:DropDownList ID="ddlPosition" runat="server">
                                 </asp:DropDownList>
                                 &nbsp;&nbsp;<asp:RequiredFieldValidator ID="rfvPosition" runat="server" 
                                     ControlToValidate="ddlPosition" ErrorMessage="*" SetFocusOnError="True" 
                                     ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                             </td>
                        </tr>
                                              
                         <tr>
                            <td align="left" style="width:150px">
                                * Tiêu đề</td>
                            <td align="left">                                
                                 <asp:TextBox ID="txtTitle" runat="server" Width="300px" ></asp:TextBox>
                                &nbsp;&nbsp;<asp:RequiredFieldValidator ID="rfvNewTitle" runat="server" ControlToValidate="txtTitle"
                                    ErrorMessage="*" SetFocusOnError="True" ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>                                                          
                            </td>
                             <td align="left">
                                 &nbsp;Mô tả</td>
                             <td align="left">
                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>
                             </td>
                        </tr>       
                        
                        <tr>
                            <td align="left" style="width:150px">
                                * Hình ảnh</td>
                            <td align="left">                                
                                 <asp:Image ID="imgPhoto" runat="server" ImageAlign="Left" Height="30px" Width="30px" /><br />                                 
                                 <input id="FileInput" type="file" runat="server"/>
                               <asp:RegularExpressionValidator ID="rervInputFile" runat="server" 
                                                    ControlToValidate="FileInput" ErrorMessage="Loại file k hợp lệ" 
                                                    SetFocusOnError="True" 
                                                    ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$" 
                                                    ValidationGroup="ValidationCheck"></asp:RegularExpressionValidator>
                               
                               </td>
                            <td align="left">
                                &nbsp;Url</td>
                            <td align="left">
                                <asp:TextBox ID="txtUrl" runat="server" Width="300px" ></asp:TextBox>
                                &nbsp;&nbsp;<asp:CustomValidator ID="OrFieldValidator"  runat="server" Display="Dynamic" 
                                                        ValidationGroup="ValidationCheck" Text="Nhập Hình ảnh hoặc Url"
                                                            ClientValidationFunction="validateOrFields" 
                                                        OnServerValidate="OrFieldValidator_ServerValidate" />
                            </td>
                        </tr>     
                        
                        <tr>
                            <td align="left" style="width:150px">
                                Chiều rộng:</td>
                            <td align="left">                                
                                 <asp:TextBox ID="txtWidth" runat="server"></asp:TextBox>
                            </td>
                            <td align="left">
                                Ngày bắt đầu</td>
                            <td align="left">
                                 <asp:TextBox ID="txtStartDate" runat="server" Width="200px"></asp:TextBox>
                                <img src="../../../../images/icons/calendar.gif" alt="Click" width="24" height="17" id="btnShowDate" />
                                 <ajaxtoolkit:CalendarExtender ID="CalendarExtender_StartDate" runat="server"                                                   
                                    OnClientShowing="setStartDate" OnClientShown="setToday" OnClientDateSelectionChanged="setEndDate" CssClass="ajax__calendar"
                                    Animated="true" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="btnShowDate" 
                                    PopupPosition="BottomRight" TargetControlID="txtStartDate"></ajaxtoolkit:CalendarExtender> 
                            </td>
                        </tr>      
                        
                         <tr>
                            <td align="left" style="width:150px">
                                Chiều cao:</td>
                            <td align="left">                                
                                <asp:TextBox ID="txtHeight" runat="server"></asp:TextBox>
                            </td>
                             <td align="left">
                             Ngày kết thúc
                             </td>
                             <td align="left">
                                <asp:TextBox ID="txtEndDate" runat="server" Width="200px"></asp:TextBox>
                                <img src="../../../../images/icons/calendar.gif" alt="Click" width="24" height="17" id="btnShow" />
                                    <ajaxtoolkit:CalendarExtender ID="CalendarExtender_IssuedDate" runat="server"                                                   
                                    OnClientShowing="setStartDate" OnClientShown="setToday" OnClientDateSelectionChanged="setEndDate" CssClass="ajax__calendar"
                                    Animated="true" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="btnShow" 
                                    PopupPosition="BottomRight" TargetControlID="txtEndDate"></ajaxtoolkit:CalendarExtender>
                             </td>
                        </tr>                       
                       
                        <tr>
                            <td align="left">
                            Tình trạng</td>
                            <td align="left">
                            <asp:CheckBox ID="ChkBoxStatus" runat="server" Text="Active" Width="124px" />
                        </td>
                            <td align="left">
                                Tags</td>
                            <td align="left">
                                <asp:TextBox ID="txtTags" runat="server" Width="300px" ></asp:TextBox>
                                &nbsp;&nbsp;</td>
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