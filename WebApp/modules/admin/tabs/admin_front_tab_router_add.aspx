<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_front_tab_router_add.aspx.cs" Inherits="WebApp.modules.admin.tabs.admin_front_tab_router_add" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>add</title>
    <script language="javascript" type="text/javascript">
        function getbacktostepone() {
            window.location = "admin_front_tabs_router_add.aspx";
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
       
    </script>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
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