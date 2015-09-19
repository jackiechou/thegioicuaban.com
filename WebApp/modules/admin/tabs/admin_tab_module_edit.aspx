<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_tab_module_edit.aspx.cs" Inherits="WebApp.modules.admin.tabs.admin_tab_module_edit" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>add</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "admin_tab_module_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 1000);
       }
       function onError() {
           setTimeout(getbacktostepone, 1000);
       }
       function okay() {
           var mode = $get('hdnWindowUIMODE').value;
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
                Edit Module to Tab Name - &nbsp;<asp:Literal ID="Literal_TabName" runat="server"></asp:Literal>
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
          
                    <table cellpadding="0" cellspacing="5px">
                            <tr>
                                <td >Chọn Module</td>               
                                <td class="style1" >
                                     <asp:DropDownList ID="ddModuleList" runat="server" Width="128px"></asp:DropDownList>
                                     <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="ValidationCheck"
                                 ControlToValidate="ddModuleList" ErrorMessage="Please select a value" 
                                 Operator="NotEqual" ValueToCompare="0"></asp:CompareValidator>
                                </td>                    
                            </tr>
                            <tr>
                                <td >Chọn Pane</td>
                                <td class="style1" >
                                    <asp:DropDownList ID="ddlPaneName" runat="server" Width="128px">
                                        <asp:ListItem Text="ContentPane" Value="ContentPane"></asp:ListItem>
                                        <asp:ListItem Text="Banner" Value="Banner"></asp:ListItem>
                                        <asp:ListItem Text="DivContainer" Value="DivContainer"></asp:ListItem>
                                        <asp:ListItem Text="TopPane" Value="TopPane"></asp:ListItem>                        
                                        <asp:ListItem Text="LeftPane" Value="LeftPane"></asp:ListItem>
                                        <asp:ListItem Text="RightPane" Value="RightPane"></asp:ListItem>
                                        <asp:ListItem Text="BottomPane" Value="BottomPane"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>                    
                            </tr>
                           
                        </table>
  
                </asp:View>
                <asp:View ID="ViewSuccess" runat="server">
                    <p>
                        You have added a new expanse successfully<br /><br />
                        Dialog will close automatically in two second</p>                   
                </asp:View>
                <asp:View ID="ViewError" runat="server">
                    <p>
                        Error occured adding expanse<br />
                        Please wait<br />
                        Redirecting to step one</p>
                </asp:View>
            </asp:MultiView>
        </div>
        <div class="popup_Buttons">
            <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="ValidationCheck"
                OnClientClick="return false;" OnClick="btnOkay_Click" />
            <input id="btnCancel" value="Cancel" type="button" onclick="cancel();" />
        </div>
    </div>


    </form>
</body>
</html>