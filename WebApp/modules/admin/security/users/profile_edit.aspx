<%@ Page Title="" Language="C#" MasterPageFile="~/templates/portal_templates/default_template/PopUp.Master" AutoEventWireup="true" CodeBehind="profile_edit.aspx.cs" Inherits="WebApp.modules.admin.security.users.profile_edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <script language="javascript" type="text/javascript">
     function getbacktostepone() {
         window.location = "profile_edit.aspx";
     }
     function onSuccess() {
         setTimeout(okay, 2000);
     }
     function onError() {
         setTimeout(getbacktostepone, 2000);
     }
     function okay() {
         parent.location.href = parent.location.href;
         window.parent.document.getElementById('ButtonUserProfileDone').click();
     }
     function cancel() {
         window.parent.document.getElementById('ButtonUserProfileCancel').click();
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
      <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
   
                   <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                                      <table border="0" cellspacing="0" cellpadding="4" class="form">
                           <tr>
                               <td align="left">
                                   FullName: </td>
                               <td align="left">
                                   <asp:TextBox ID="txtFullName" runat="server" Width="180px"></asp:TextBox>
                                   <asp:CustomValidator ID="CustomValidator9" runat="server" 
                                       ValidationGroup="checkValidation" 
                                       ControlToValidate="txtFullName" Display="Dynamic" 
                                       ErrorMessage="Invalid, please select one" ForeColor="red" 
                                       SetFocusOnError="True" Text="*" /></td>
                               <td align="left">
                                   &nbsp;</td>
                               <td align="left">
                                   Address:</td>
                               <td align="left">
                                 <asp:TextBox ID="txtAddress" runat="server" Width="180px"></asp:TextBox>
                                   <asp:CustomValidator ID="CustomValidator16" runat="server" 
                                       ControlToValidate="txtAddress" Display="Dynamic" 
                                       ErrorMessage="Invalid, please select one" ForeColor="red" 
                                       SetFocusOnError="True" Text="*" ValidationGroup="checkValidation" /></td>
                           </tr>
                                          <tr>
                                              <td align="left">
                                                  DisplayName</td>
                                              <td align="left">
                                                  <asp:TextBox ID="txtDisplayName" runat="server" Width="180px"></asp:TextBox>
                                              </td>
                                              <td align="left">
                                                  &nbsp;</td>
                                              <td align="left">
                                                  Phone:
                                              </td>
                                              <td align="left">
                                                  <asp:TextBox ID="txtPhone" runat="server" Width="180px"></asp:TextBox>
                                                  <asp:CustomValidator ID="CustomValidator14" runat="server" 
                                                      ControlToValidate="txtPhone" Display="Dynamic" 
                                                      ErrorMessage="Invalid, please select one" ForeColor="red" 
                                                      SetFocusOnError="True" Text="*" />
                                              </td>
                                          </tr>
                           <tr>
                              <td align="left">
                                  UserName: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtUserName" runat="server" Width="180px"></asp:TextBox>
                                    <asp:CustomValidator ID="CustomValidatorUserName"  ControlToValidate="txtUserName" ValidationGroup="checkValidation"
                                    Text="*" ForeColor="red" runat="server" ErrorMessage="Invalid, please select one" SetFocusOnError="True" Display="Dynamic"/>   
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
                                   Password Question : </td>
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
                                   &nbsp; Password Answer :
                               </td>
                               <td align="left">
                                    <asp:TextBox ID="txtPassAnswer" runat="server" Width="180px"></asp:TextBox>
                               </td>
                           </tr> 
                                                                                                                                                                                                
                       </table>
                       <div class="popup_Buttons">
                            <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="checkValidation" OnClientClick="return false;" OnClick="btnOkay_Click" />
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
</asp:Content>
