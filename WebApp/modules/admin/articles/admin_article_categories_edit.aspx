<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_article_categories_edit.aspx.cs" Inherits="WebApp.modules.admin.articles.admin_article_categories_edit" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
       <script language="javascript" type="text/javascript">
           function getbacktostepone() {
               window.location = "admin_article_categories_edit.aspx";
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

           //===========================================================
           //Prevent users to initiate multiple asynchronous postbacks
           var Page;
           var postBackElement;
           var pbQueue = new Array();
           var argsQueue = new Array();

           function pageLoad() {
               Page = Sys.WebForms.PageRequestManager.getInstance();
               Page.add_beginRequest(OnBeginRequest);
               Page.add_endRequest(endRequest);
           }

           function OnBeginRequest(sender, args) {
               postBackElement = args.get_postBackElement();
               if (Page.get_isInAsyncPostBack()) {
                   alert('One request is already in progress.');
                   //Method1: CancelPostBack 
                   args.set_cancel(true);
                   postBackElement.disabled = true;

                   //Method 2: Queuing up the Postbacks
                   //pbQueue.push(args.get_postBackElement().id);
                   //argsQue.push(document.forms[0].__EVENTARGUMENT.value);

                   //Method 3: User div HideForm
                   //$get('divBG').className = 'divBG';
               }
           }

           function endRequest(sender, args) {
               postBackElement.disabled = false;
               //$get('divBG').className = 'divBG';

               //if (pbQueue.length > 0) {
               ////pulls the first item out of the array and removes it.
               //   __doPostBack(pbQueue.shift(), argsQueue.shift());
               //}
           }

           function CancelPostBack() {
                if (Page.get_isInAsyncPostBack())
                   Page.abortPostBack();
           }


           function HideDiv(sender, args) {
               postBackElement = args.get_postBackElement();
               if (postBackElement.id == 'btnClear') {
                   $get('UpdateProgress1').style.display = "block";
               }
           }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <script type="text/javascript" src="../../../scripts/js/PostbackPrecedence.js"></script>
         <script type="text/javascript">
             preventPostBack('btnOkay', 'AlertDiv', 'AlertMessage');
        </script>
        <asp:UpdatePanel  ID="UpdatePanel1" UpdateMode="Conditional" runat="server" >
            <Triggers>
                <asp:PostBackTrigger ControlID="btnOkay" />
            </Triggers>
            <ContentTemplate>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        ...Processing...
                    </ProgressTemplate> 
                </asp:UpdateProgress>
                <div class="popup_Container">
                    <div class="popup_Titlebar" id="PopupHeader">
                        <div class="TitlebarLeft">
                            Quản Lý Nhóm Tin Tức: Sửa thông tin nhóm
                        </div>
                        <div class="TitlebarRight" onclick="cancel();">
                        </div>
                    </div>
                    <div class="popup_Body">                     
                        <asp:MultiView ID="MultiView1" runat="server">
                            <asp:View ID="ViewInput" runat="server">                    
                                    <table  border="0" cellspacing="0" cellpadding="4" class="form">
                                        <tr>
                                            <td width="127" >
                                                * Portal</td>
                                            <td align="left" style="height: 15px; width: 26px;">
                                                <asp:DropDownList ID="ddlPortalList" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>                                                                        
                                      
                                        <tr>
                                            <td width="127">
                                                * Culture</td>
                                            <td align="left" style="height: 15px; width: 26px;">
                                                <asp:DropDownList ID="ddlCultureList" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                      
                                        <tr>
                                            <td width="127">
                                                * Mã Code</td>
                                            <td align="left" style="height: 15px; width: 26px;">
                                                <asp:TextBox ID="txtCode" runat="server" Width="485px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorCode" runat="server" 
                                                    ControlToValidate="txtCode" ErrorMessage="*" SetFocusOnError="True" 
                                                    ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                      
                                        <tr>
                                            <td align="left" >
                                                * Nhóm tin</td>
                                            <td align="left" style="height: 15px">                      
                                                <asp:TextBox ID="txtName" runat="server" Width="485px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                                                    ControlToValidate="txtName" ErrorMessage="*" SetFocusOnError="True" 
                                                    ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" >
                                                * ParentID</td>
                                            <td style="height: 15px" align="left">
                                                <asp:ListBox ID="lstTreeNode" runat="server" Height="200px" Rows="5" 
                                                    Width="485px" />

                                            </td>
                                        </tr>                                       
                                        <tr>
                                            <td align="left" >
                                                * Photo</td>
                                            <td style="height: 15px" align="left">
                                                <a class="MagicZoom" href='<%# Eval("CategoryImage") %>' 
                                                    rel="zoom-position: outer; zoom-width:420px; zoom-height:240px" 
                                                    style="cursor: crosshair">
                                                <asp:Image ID="imgPhoto" runat="server" Height="30px" ImageAlign="Left" 
                                                    Width="30px" />
                                                </a>
                                                <br />
                                                <br />
                                                <input id="FileInput" type="file" runat="server" />
                                                <asp:RegularExpressionValidator ID="rervInputFile" runat="server" 
                                                    ControlToValidate="FileInput" ErrorMessage="Loại file k hợp lệ" 
                                                    SetFocusOnError="True" 
                                                    ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$" 
                                                    ValidationGroup="ValidationControls"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>                                       
                                        <tr>
                                            <td align="left" >
                                                Description</td>
                                            <td align="left" style="height: 15px">
                                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" 
                                                    Width="485px"></asp:TextBox>
                                            </td>
                                        </tr>
  
                                        <tr>
                                            <td align="left">
                                                Tình trạng</td>
                                            <td align="left">
                                                <asp:RadioButtonList ID="rdlStatus" runat="server" RepeatDirection="Horizontal" 
                                                    Width="328px">
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
            </ContentTemplate>
        </asp:UpdatePanel>  
    </form>
</body>
</html>
