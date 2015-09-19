<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_tabs_edit.aspx.cs" Inherits="WebApp.modules.admin.tabs.admin_tabs_edit" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
        <link rel="stylesheet" href="../../../scripts/plugins/jquery-ui/css/ui-lightness/jquery-ui.custom.min.css" />
    <script src="../../../scripts/jquery/jquery.min.js"></script>
    <script src="../../../scripts/plugins/jquery-ui/js/jquery-ui.custom.min.js"></script>

   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "admin_tabs_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 2000);
       }
       function onError() {
           setTimeout(getbacktostepone, 2000);
       }
       function okay() 
       {          
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

       //==============================================================================================
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
           var start_date = document.getElementById('<%#txt_StartDate.ClientID %>').value;
           var end_date = document.getElementById("<%#txt_EndDate.ClientID %>").value;
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
           var start_date = document.getElementById('<%#txt_StartDate.ClientID %>').value;
           var end_date = document.getElementById("<%#txt_EndDate.ClientID %>").value;

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
           $("#<%#txt_EndDate.ClientID%>").val(expiredDate);

           ////          end_date.CalendarBehavior.initialize;
           ////          end_date.CalendarBehavior.set_endDate(s1);      
           ////          $find("CalendarExtender_ExpiredDate").set_selectedDate(sender.get_selectedDate());
       }
       //SiteMapPriority =======================================================================
       $(function () {
           $("#slider-range-max").slider({
               range: "max",
               min: 0,
               max: 10,
               value: 0,
               slide: function (event, ui) {
                   $("#txtSiteMapPriority").val(ui.value);
               }
           });
           $("#slider-range-max").width(200);
           $("#<%#txtSiteMapPriority.ClientID%>").val($("#slider-range-max").slider("value"));
       });
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
                     <table cellpadding="0" cellspacing="5px">       
                            <tr>
                                <td style=" width: 161px;">
                                    Chọn Portal (*)</td>
                                <td >
                                    <asp:DropDownList ID="ddlPortalList" runat="server" CssClass="combobox" 
                                        Width="200px">
                                    </asp:DropDownList>
                                </td>
                                <td >
                                    File Icon</td>
                                <td >
                                    <asp:Image ID="FileIcon_Img" runat="server" />
                                    <input id="FileIconInput" runat="server" type="file" style="width:100px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Content Item (*)</td>
                                <td >
                                    <asp:DropDownList ID="ddlContentItem" runat="server" CssClass="combobox" 
                                        Width="200px">
                                    </asp:DropDownList>
                                </td>
                                <td >
                                    File Icon Large</td>
                                <td >
                                     <asp:Image ID="FileIconLarge_Image" runat="server" style="width:100px" />
                                    <input id="FileIconLargeInput" runat="server" type="file" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    CultureCode (*)</td>
                                <td >
                                    <asp:DropDownList ID="ddlCultureCode" runat="server" CssClass="combobox" 
                                        Width="200px">
                                    </asp:DropDownList>
                                </td>
            
                                <td >
                                    Description</td>            
                                <td >
                                <asp:TextBox ID="txt_Desc" TextMode="MultiLine" runat="server"  Width="200px"></asp:TextBox>
                                </td>
            
                            </tr>
                            <tr>
                                <td>
                                    Tab Name (*)</td>
                                <td>
                                    <asp:TextBox ID="txt_TabName" runat="server" Width="200px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorTabName" runat="server" 
                                        ControlToValidate="txt_TabName" ErrorMessage="(*)" 
                                        ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                </td>
                                <td>
                                    Keywords</td>
                                <td>
                                    <asp:TextBox ID="txt_Keywords" runat="server" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Title</td>
                                <td >
                                    <asp:TextBox ID="txt_Title" runat="server" Width="200px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorTitle" ValidationGroup="ValidationCheck" ControlToValidate="txt_Title" runat="server" ErrorMessage="(*)"></asp:RequiredFieldValidator>
                                </td>            
                                <td >
                                    PageHeadText</td>
                                <td >
                                    <asp:TextBox ID="txt_PageHeadText" runat="server" Width="200px"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    Parent Tab (*)</td>
                                <td >
                                    <asp:DropDownList ID="ddlParentTab" runat="server" CssClass="combobox" 
                                        Width="200px">
                                    </asp:DropDownList>
                                </td>
                                <td >
                                    PageFooterText</td>
                                <td >
                                      <asp:TextBox ID="txt_PageFooterText" runat="server" Width="200px"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    Tab Path</td>
                                <td>
                                    <asp:TextBox ID="txt_Path" runat="server" Width="200px"></asp:TextBox></td>
                                <td>
                                    Control Bar</td>
                                <td>
                                    <asp:TextBox ID="txt_PageControlBar" runat="server" Width="200px"></asp:TextBox></td>
                            </tr>

                            <tr>
                                <td>
                                    Url </td>
                                <td >
                                    <asp:TextBox ID="txt_Url" runat="server" Width="200px"></asp:TextBox></td>
                                <td >
                                    Start Date</td>
                                <td >
                                    <asp:TextBox ID="txt_StartDate" runat="server" style="text-align:right"></asp:TextBox>
                                     &nbsp;
                                    <img id="btnShowDate" alt="Click" height="17" src="../../../images/icons/calendar.gif" width="24" />
                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender_StartDate" runat="server" 
                                        Animated="true" CssClass="ajax__calendar" Enabled="True" Format="dd/MM/yyyy" 
                                        OnClientDateSelectionChanged="setEndDate" OnClientShowing="setStartDate" 
                                        OnClientShown="setToday" PopupButtonID="btnShowDate" 
                                        PopupPosition="BottomRight" TargetControlID="txt_StartDate">
                                    </ajaxToolkit:CalendarExtender>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    Tab Route</td>
                                <td>
                                   <asp:DropDownList ID="ddlRouterList" runat="server" CssClass="combobox" Width="200px">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    End Date</td>
                                <td>
                                    <asp:TextBox ID="txt_EndDate" runat="server" style="text-align:right"></asp:TextBox>
                                    &nbsp;
                                    <img id="btnShowEndDate" alt="Click" height="17" src="../../../images/icons/calendar.gif" width="24" />
                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender_EndDate" runat="server" 
                                        Animated="true" CssClass="ajax__calendar" Enabled="True" Format="dd/MM/yyyy" 
                                        OnClientDateSelectionChanged="setEndDate" OnClientShowing="setStartDate" 
                                        OnClientShown="setToday" PopupButtonID="btnShowEndDate" 
                                        PopupPosition="BottomRight" TargetControlID="txt_EndDate">
                                    </ajaxToolkit:CalendarExtender>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    SiteMapPriority</td>
                                <td >
                                     <label for="amount"></label>
                                    <asp:TextBox ID="txtSiteMapPriority" Visible="false" runat="server" Text="0"></asp:TextBox>
                                    <div id="slider-range-max"></div>
                                </td>
                                <td >
                                    CssClass</td>
                                <td >
                                     <asp:TextBox ID="txt_CssClass" runat="server" style="text-align:right" 
                                        Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Features (*)</td>
                                <td >
                                   <asp:CheckBox ID="chkIsDelete" runat="server" Checked="false" 
                                        Text="Is Deleted" />
                                    <asp:CheckBox ID="chkDisableLink" runat="server" Checked="false" 
                                        Text="Disable Link" />
                                    <asp:CheckBox ID="chkDisplayTitle" runat="server" Checked="true" 
                                        Text="Display Title" />
                                </td>
                                <td >
                                    <asp:CheckBox ID="chkIsSecure" AutoPostBack="true" runat="server" Checked="true" 
                                        Text="Is Secure" oncheckedchanged="chkIsSecure_CheckedChanged" />
                                </td>
                                <td>                                                                        
                                <asp:CheckBox ID="chkIsVisible" runat="server" Checked="true" 
                                        Text="Is Visible" />
                                    <asp:CheckBox ID="chkPermanentRedirect" runat="server" Checked="true" 
                                        Text="Permanent Redirect" />
                                </td>
                            </tr>                            
                        </table> 
                        <div class="popup_Buttons">
                            <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="ValidationCheck" onclick="btnOkay_Click"/>
                            <input id="btnCancel" value="Cancel" onclick="cancel()" type="button"/>
                       </div>                 
                </asp:View>
                <asp:View ID="ViewSuccess" runat="server">
                     <p>
                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                    </p>                    
                     <div class="popup_Buttons">
                        <input id="Button1" value="Đóng" type="button" onclick="cancel();" />
                     </div>        
               </asp:View>               
                <asp:View ID="ViewError" runat="server">
                     <p>
                        <asp:Label ID="lblErrorMsg" runat="server"></asp:Label>
                    </p>                
                     <div class="popup_Buttons">
                        <input id="btnQuit" value="Đóng" type="button" onclick="cancel();" />
                     </div>
                </asp:View>
            </asp:MultiView>
        </div>
       
    </div>
    </form>
</body>
</html>