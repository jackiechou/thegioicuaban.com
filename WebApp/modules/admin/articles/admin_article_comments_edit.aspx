<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_article_comments_edit.aspx.cs"
    Inherits="WebApp.modules.admin.articles.admin_article_comments_edit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function getbacktostepone() {
            window.location = "admin_article_comments_edit.aspx";
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
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
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
                        Cập nhật bình luận</div>
                    <div class="TitlebarRight" onclick="cancel();">
                    </div>
                </div>
                <div class="popup_Body">
                    <asp:MultiView ID="MultiView1" runat="server">
                        <asp:View ID="ViewInput" runat="server">
                            <table border="0" cellspacing="0" cellpadding="4" class="form">
                                <tr>
                                    <td width="127">
                                        Tin tức</td>
                                    <td align="left" style="height: 15px; width: 26px;">
                                        <asp:TextBox ID="txtArticle" runat="server" Enabled="False" Width="479px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="127">
                                        Tên</td>
                                    <td align="left" style="height: 15px; width: 26px;">
                                        <asp:TextBox ID="txtName" runat="server" Width="480px" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="127">
                                        Email</td>
                                    <td align="left" style="height: 15px; width: 26px;">
                                        <asp:TextBox ID="txtEmail" runat="server" Width="481px" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        Nội dung</td>
                                    <td align="left" style="height: 15px">
                                        <asp:TextBox ID="txtCommentText" runat="server" TextMode="MultiLine" 
                                            Width="485px" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        Tình trạng</td>
                                    <td style="height: 15px" align="left">
                                        <asp:RadioButtonList ID="rdlStatus" runat="server" RepeatDirection="Horizontal" 
                                            Width="328px">
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                            <div class="popup_Buttons">
                                <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="ValidationCheck"
                                    OnClick="btnOkay_Click" />
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
