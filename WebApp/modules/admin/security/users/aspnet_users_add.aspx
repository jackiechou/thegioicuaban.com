<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aspnet_users_add.aspx.cs" Inherits="WebApp.modules.admin.security.users.aspnet_users_add" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "aspnet_users_add.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 1000);
       }
       function onError() {
           setTimeout(getbacktostepone, 1000);
       }
       function okay() {          
            parent.location.href = parent.location.href;
            window.parent.document.getElementById('btnOkay').click();          
       }
       function cancel() {          
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
    <asp:ScriptManager ID="ScriptManager1" EnablePartialRendering="true" runat="server">
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
                    <table border="0" cellspacing="0" cellpadding="4" class="form">
                           <tr>
                              <td align="left" width="135" >
                                  * Application Id</td>
                                <td align="left">
                                   <asp:DropDownList ID="ddlApplicationList" runat="server" CssClass="combobox"  Width="149px" ></asp:DropDownList>             
                                   <ajaxToolkit:CascadingDropDown ID="CascadingDropDown1" runat="server" TargetControlID="ddlApplicationList"
                                        Category="ApplicationId"  PromptText="- Select -"  LoadingText="[Loading ...]"
                                        ServicePath="~/services/WebServices.asmx" ServiceMethod="GetApplicationList" />                
                                </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   FullName:
                               </td>
                               <td align="left">
                                   <asp:TextBox ID="txtFullName" runat="server" Width="180px"></asp:TextBox>
                                   <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ErrorMessage="*" Display="Dynamic"
                                       ForeColor="Red" SetFocusOnError="True" ControlToValidate="txtFullName" 
                                       ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                               </td>
                           </tr>      
                           <tr>
                               <td align="left">
                                   * PortalId</td>
                               <td align="left">
                                   <asp:DropDownList ID="ddlPortalList" CssClass="combobox" Width="147px" runat="server">
                                   </asp:DropDownList>
                                   <ajaxToolkit:CascadingDropDown ID="CascadingDropDown3"
                                           runat="server"
                                           Category="PortalId"
                                           TargetControlID="ddlPortalList"
                                           ParentControlID="ddlApplicationList"
                                           PromptText="- Select -"
                                           LoadingText="Loading ..."
                                           ServicePath="~/services/WebServices.asmx"
                                           ServiceMethod="GetPortalList"></ajaxToolkit:CascadingDropDown>
                               </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   Address:</td>
                               <td align="left">
                               <asp:TextBox ID="txtAddress" runat="server" Width="180px"></asp:TextBox>
                               </td>
                           </tr>
                           <tr>
                              <td align="left" >
                                  * Role</td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlRoleList"  runat="server"   
                                        CssClass="combobox" Width="147px" ></asp:DropDownList>             
                                     <ajaxToolkit:CascadingDropDown ID="CascadingDropDown2"
                                           runat="server"
                                           Category="RoleId"
                                           TargetControlID="ddlRoleList"
                                           ParentControlID="ddlApplicationList"
                                           PromptText="- Select -"
                                           LoadingText="Loading ..."
                                           ServicePath="~/services/WebServices.asmx"
                                           ServiceMethod="GetRoleList">
                                    </ajaxToolkit:CascadingDropDown>                             
                                </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   MobilePIN</td>
                               <td align="left">
                                   <asp:TextBox ID="txtMobilePIN" runat="server" Width="180px"></asp:TextBox>                                   
                               </td>
                           </tr>
                           <tr>
                               <td align="left">
                                   * Vendor
                               </td>
                               <td align="left">
                                   <asp:DropDownList ID="ddlVendorList" CssClass="combobox" Width="147px" runat="server">
                                   </asp:DropDownList>
                                    <ajaxToolkit:CascadingDropDown ID="CascadingDropDownVendor"
                                           runat="server"
                                           Category="VendorId"
                                           TargetControlID="ddlVendorList"
                                           ParentControlID="ddlPortalList"
                                           PromptText="- Select -"
                                           LoadingText="Loading ..."
                                           ServicePath="~/services/WebServices.asmx"
                                           ServiceMethod="GetVendorList">
                                    </ajaxToolkit:CascadingDropDown>    
                               </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   &nbsp;</td>
                           </tr>
                           <tr>
                               <td align="left">
                                   DisplayName</td>
                               <td align="left">
                                   <asp:TextBox ID="txtDisplayName" runat="server" Width="180px"></asp:TextBox>
                                   <asp:RequiredFieldValidator ID="rfvDisplayName" runat="server" 
                                       ControlToValidate="txtDisplayName" Display="Dynamic" ErrorMessage="*" 
                                       ForeColor="Red" SetFocusOnError="True" ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                               </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   Phone:
                               </td>
                               <td align="left">
                                   <asp:TextBox ID="txtPhone" runat="server" Width="180px"></asp:TextBox>
                                   <asp:RequiredFieldValidator ID="rfvPhone" runat="server" 
                                       ControlToValidate="txtPhone" Display="Dynamic" ErrorMessage="*" ForeColor="Red" 
                                       SetFocusOnError="True" ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                               </td>
                           </tr>
                           <tr>
                              <td align="left">
                                  UserName: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtUserName" runat="server" Width="180px"></asp:TextBox>
                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" 
                                       ForeColor="Red" SetFocusOnError="True" ControlToValidate="txtDisplayName" Display="Dynamic"
                                       ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   Email:</td>
                               <td align="left">
                                   <asp:TextBox ID="txtEmail" runat="server" Width="180px"></asp:TextBox>
                                   <asp:CustomValidator ID="CustomValidator13" runat="server" ValidationGroup="checkValidation"
                                       ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Invalid, please select one" ForeColor="red" 
                                       SetFocusOnError="True" Text="*" />
                                   <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                       ControlToValidate="txtEmail" Display="Dynamic" ValidationGroup="checkValidation" 
                                       ErrorMessage="Invalid email format" ForeColor="#FF3300" SetFocusOnError="True" 
                                       ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                               </td>
                           </tr>  
                           <tr>
                              <td align="left">
                                  Password: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtPassword" runat="server" Width="180px" TextMode="Password"
                                        ToolTip="Must have at least 1 number, 1 special character, and more than 6 characters"></asp:TextBox>
                                    <asp:CustomValidator ID="CustomValidator3"  ControlToValidate="txtPassword" ValidationGroup="checkValidation"
                                    Text="*" ForeColor="red" runat="server" ErrorMessage="Invalid, please select one" SetFocusOnError="True" Display="Dynamic"/>   
                
                                    <asp:RegularExpressionValidator ID="revCurrentPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="checkValidation"
                                        ErrorMessage="Must have at least 1 number, 1 special character, and more than 6 characters" 
                                        ValidationExpression="(?=^.{6,}$)(?=.*\d)(?=.*\W+)(?![.\n]).*$" Display="Dynamic" SetFocusOnError="True" ForeColor="#CC9900" 
                                        ></asp:RegularExpressionValidator>
                                </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   &nbsp;Password Question : </td>
                               <td align="left">
                                    <asp:DropDownList ID="ddlPassQuestion" runat="server" Width="180px">
                                    </asp:DropDownList>
                               </td>
                           </tr>
                           <tr>
                              <td align="left">
                                  Password Confirm: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtPasswordConfirm" runat="server" Width="180px" TextMode="Password" 
                                        ToolTip="Must have at least 1 number, 1 special character, and more than 6 characters" ></asp:TextBox>                             
                                  <asp:RegularExpressionValidator ID="revConfirmPassword" runat="server" 
                                    ControlToValidate="txtPasswordConfirm" ValidationGroup="checkValidation"
                                    ErrorMessage="Must have at least 1 number, 1 special character, and more than 6 characters" 
                                    ValidationExpression="(?=^.{6,}$)(?=.*\d)(?=.*\W+)(?![.\n]).*$" 
                                      Display="Dynamic" SetFocusOnError="True" ForeColor="#CC9900"></asp:RegularExpressionValidator>
                                  <asp:CompareValidator ID="CompareValidator_Password" runat="server"
                                      ValidationGroup="checkValidation"  ControlToValidate="txtPasswordConfirm" 
                                      ErrorMessage="Password Mismatch!Please don't copy" SetFocusOnError="True"  
                                      Operator="Equal" ControlToCompare="txtPassword" Display="Dynamic"                                       
                                    ForeColor="#FF3300"></asp:CompareValidator>
                                </td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   Password Answer :</td>
                               <td align="left">
                                    <asp:TextBox ID="txtPassAnswer" runat="server" Width="180px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPassAnswer" runat="server" ErrorMessage="*" 
                                       ForeColor="Red" SetFocusOnError="True" ControlToValidate="txtPassAnswer" Display="Dynamic"
                                       ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                               </td>
                           </tr> 
                           <tr>
                              <td align="left" colspan="2">
                                   <asp:CheckBox ID="chkIsSuperUser" runat="server" Checked="False" 
                                       Text="IsSuperUser" />
                                   &nbsp;
                                   <asp:CheckBox ID="chkUpdatePassword" runat="server" Checked="True" 
                                       Text="UpdatePassword" />
                                   &nbsp;
                                   <asp:CheckBox ID="chkIsDeleted" runat="server" Checked="True" 
                                       Text="IsDeleted" />
                                   &nbsp;
                                   <asp:CheckBox ID="chkIsApproved" runat="server" Checked="True" 
                                       Text="IsApproved" /></td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   Comment</td>
                               <td align="left">
                                   <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine"></asp:TextBox>
                               </td>
                           </tr>          
                                                                                                                                                                                                
                       </table>
                       <div class="popup_Buttons">
                            <asp:Button ID="btnOkay" Text="Done" UseSubmitBehavior="false" runat="server" ValidationGroup="checkValidation" OnClientClick="return false;" OnClick="btnOkay_Click" />
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