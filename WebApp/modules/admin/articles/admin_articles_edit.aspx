<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_articles_edit.aspx.cs" Inherits="WebApp.modules.admin.articles.admin_articles_edit" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <link type="image/x-icon" rel="shortcut icon" href="../../../templates/admin_templates/default_temp/images/favicon.ico"/>

     <link rel="icon" href="http://www.5eagles.com.vn/5eagles_icon.ico" type="image/x-icon" />    
    <link rel="shortcut icon" href="http://www.5eagles.com.vn/5eagles_icon.ico"/>  
    <%--TAB--%>
    <link rel="stylesheet" type="text/css" href="../../../scripts/plugins/ajaxtabs/tabs-no-images.css"/>   
    <title>edit</title>
    <style type="text/css">
        table.adminform {
            background-color: #F9F9F9;
            border: 1px solid #D5D5D5;
            border-collapse: collapse;
            margin: 8px 0 15px;
            width: 100%;
        }
       
        table.adminform tr.row0 {
            background-color: #F9F9F9;
        }
         table.adminform tr.row1 {
            background-color: #EEEEEE;
        }
    </style>
    <script type="text/javascript" src="../../../scripts/jquery/jquery.js"></script>
    <script type="text/javascript" src="../../../scripts/plugins/ajaxtabs/jquery.tools.min.js"></script>
   <script type="text/javascript">
       function getbacktostepone() {
           window.location = "admin_articles_edit.aspx";
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

       jQuery(document).ready(function ($) {
           ///// SHOW/HIDE TABS WHEN USER CLICK ///// 
           jQuery("ul.css-tabs").tabs("div.css-panes > div", {
               effect: 'fade',
               onBeforeClick: function (event, i) {
                   // get the pane to be opened
                   var pane = this.getPanes().eq(i);

                   // only load once. remove the if ( ... ){ } clause if
                   // you want the page to be loaded every time
                   if (pane.is(":empty")) {
                       // load it with a page specified in the tab's href
                       // attribute
                       pane.load(this.getTabs().eq(i).attr("href"));
                   }
               }
           });
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
                Quản Lý Tin Tức: Sửa nội dung
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                <ul class="css-tabs">
                    <li><a class="current" href="#DivInformation">Information</a></li>
                    <li><a href="#DivDescription">Description</a></li>
                </ul>
                <div class="css-panes">
	                <div id="DivInformation" style="display: block;">
                        <table  border="0" cellspacing="0" cellpadding="4" class="form">
                                        <tr>
                                            <td align="left">
                                                * Portal</td>
                                            <td align="left" style="height: 15px">
                                                <asp:DropDownList ID="ddlPortalList" runat="server">
                                            </asp:DropDownList></td>
                                            <td align="left" style="height: 15px">
                                                * Culture: </td>
                                            <td align="left" style="height: 15px">
                                                <asp:DropDownList ID="ddlCultureList" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                       </tr>
                                        <tr>
                                            <td align="left">
                                                * Chủ đề</td>
                                            <td align="left" style="height: 15px">
                                                <asp:DropDownList ID="ddlTreeNode_Category" runat="server">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvCategory" runat="server" 
                                                    ControlToValidate="ddlTreeNode_Category" ErrorMessage="*" 
                                                    SetFocusOnError="True" ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                            </td>
                                            <td align="left" style="height: 15px">
                                                Nguồn :
                                            </td>
                                            <td align="left" style="height: 15px">
                                                <asp:TextBox ID="txtSource" runat="server" Width="145px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                                    ControlToValidate="txtSource" ErrorMessage="*"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                &nbsp;</td>
                                            <td align="left" colspan="3" style="height: 15px">
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                               * Headline</td>
                                            <td align="left" style="height: 15px" colspan="3">
                                                <asp:TextBox ID="txtHeadline" runat="server" Width="476px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvHeadline" runat="server" 
                                                    ControlToValidate="txtHeadline" ErrorMessage="*" SetFocusOnError="True" 
                                                    ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                                &nbsp;
                                            </td>
                                    </tr>
                                        <tr>
                                            <td align="left">
                                                * Title</td>
                                            <td align="left" style="height: 15px" colspan="3">
                                     

                                                    <asp:TextBox ID="txtTitle" runat="server" Width="476px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                                                    ControlToValidate="txtTitle" ErrorMessage="*" SetFocusOnError="True" 
                                                    ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                                    &nbsp;&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                * Upload Ảnh:</td>
                                            <td colspan="3" align="top">
                                                <input id="FileInput" runat="server" type="file" />
                                                <asp:Image ID="front_Img" runat="server" />
                                                <asp:RegularExpressionValidator ID="rervInputFile" runat="server" 
                                                    ControlToValidate="FileInput" ErrorMessage="Loại file k hợp lệ" 
                                                    SetFocusOnError="True" 
                                                    ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$" 
                                                    ValidationGroup="ValidationControls"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                       <tr>
                                          <td align="left">
                                              Nội dung ngắn</td>
                                            <td colspan="3" style="height: 15px" align="left">
                                                <asp:TextBox ID="txtAbstract" TextMode="MultiLine" runat="server" Width="481px"></asp:TextBox>
                                           </td>
                                       </tr>                                       
                                       <tr>
                                          <td align="left" >
                                               Link ngoài:</td>
                                            <td align="left" colspan="3">
                                    
                                                <asp:TextBox ID="txtNavigateUrl" runat="server" Width="145px"></asp:TextBox>
                                    
                                           </td>
                                       </tr>                                       
                                        <tr>
                                            <td align="left">
                                                Tình trạng</td>
                                            <td align="left" colspan="3">
                                                <asp:RadioButtonList ID="rdlStatus" runat="server" RepeatDirection="Horizontal" 
                                                    Width="328px">
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                    </table>
	                </div>

	                <div id="DivDescription" style="display: none;">
                        <div class="showeditor">
                            <FCKeditorV2:FCKeditor ID="FCKeditorContent" runat="server" BasePath="~/editors/FCKeditor/"                              
                                LinkBrowserURL="~/userfiles/files/" Width="688px" Height="300px">&nbsp;</FCKeditorV2:FCKeditor>
                        </div>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ForeColor="#FF0000" SetFocusOnError="true" 
                                Display="Dynamic" ValidationGroup="VaditionCheck" ControlToValidate="FCKeditorContent" ErrorMessage="*"></asp:RequiredFieldValidator> 
	                </div>
                </div>
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