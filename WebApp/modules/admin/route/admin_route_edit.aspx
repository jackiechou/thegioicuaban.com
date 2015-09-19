<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_route_edit.aspx.cs" Inherits="WebApp.modules.admin.route.admin_route_edit" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <link type="image/x-icon" rel="shortcut icon" href="../../../templates/admin_templates/default_temp/images/favicon.ico"/>
    <link rel="icon" href="http://www.5eagles.com.vn/5eagles_icon.ico" type="image/x-icon" />    
    <link rel="shortcut icon" href="http://www.5eagles.com.vn/5eagles_icon.ico"/>  
    <title>edit</title>
   <script type="text/javascript">
       function getbacktostepone() {
           window.location = "admin_route_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 2000);
       }
       function onError() {
           setTimeout(cancel, 2000);
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
                Quản Lý Route</div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">                
                <table  border="0" cellspacing="0" cellpadding="4" class="form">
                            <tr>
                                <td align="left">
                                    * Portal</td>
                                <td align="left" style="height: 15px">
                                    <asp:DropDownList ID="ddlPortalList" runat="server">
                                </asp:DropDownList>
                                </td>
                                <td align="left" style="height: 15px">
                                    * Content Item </td>
                                <td align="left" style="height: 15px">
                                    <asp:DropDownList ID="ddlContentItem" runat="server" CssClass="combobox" Width="100px">
                                    </asp:DropDownList>
                                    </td>
                                <td align="left" style="height: 15px">
                                    * Culture: 
                                </td>
                                <td align="left" style="height: 15px">
                                        <asp:DropDownList ID="ddlCultureList" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                        ControlToValidate="ddlCultureList" ErrorMessage="*" SetFocusOnError="True" 
                                        ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td align="left">
                                    * RouteName</td>
                                <td align="left" style="height: 15px" colspan="5">
                                    <asp:TextBox ID="txtRouteName" runat="server" Width="460px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRouteName" runat="server" 
                                        ControlToValidate="txtRouteName" ErrorMessage="*" SetFocusOnError="True" 
                                        ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                </td>
                        </tr>
                            <tr>
                                <td align="left">
                                    * RouteUrl</td>
                                <td align="left" style="height: 15px" colspan="5">
                                    <asp:TextBox ID="txtRouteUrl" runat="server" Width="460px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRouteUrl" runat="server" 
                                        ControlToValidate="txtRouteUrl" ErrorMessage="*" SetFocusOnError="True" 
                                        ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    PhysicalFile</td>
                                <td colspan="5" style="height: 15px" align="left">
                                        <asp:FileUpload ID="FileUpload_PhysicalFile" runat="server" />                                    
                                    <asp:Label ID="lblPhysicalFile" Visible="false" runat="server" Text="Label"></asp:Label>
                                </td>
                            </tr>                                       
                            <tr>
                                <td align="left">
                                    RouteValueDictionary</td>
                                <td align="left" colspan="5" style="height: 15px">
                                    <asp:TextBox ID="txtRouteValueDictionary" runat="server" Width="460px"></asp:TextBox>                                                
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    Description</td>
                                <td align="left" colspan="5" style="height: 15px">
                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" 
                                        Width="481px" TabIndex="7"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" >
                                    CheckPhysicalUrlAccess</td>
                                <td align="left" colspan="5">                                    
                                    <asp:CheckBox ID="chkCheckPhysicalUrlAccess" runat="server" />
                                </td>
                            </tr>                                       
                            <tr>
                                <td align="left">
                                    Discontinued</td>
                                <td align="left" colspan="5">
                                    <asp:RadioButtonList ID="rdlDiscountinue" runat="server" RepeatDirection="Horizontal" 
                                        Width="242px" Height="23px">
                                        <asp:ListItem Value="1" Selected="True">Active</asp:ListItem>
                                        <asp:ListItem Value="0">InActive</asp:ListItem>
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