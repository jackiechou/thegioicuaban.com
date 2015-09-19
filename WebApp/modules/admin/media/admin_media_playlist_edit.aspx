<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_media_playlist_edit.aspx.cs" Inherits="WebApp.modules.admin.media.admin_media_playlist_edit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script type="text/javascript">
       function getbacktostepone() {
           window.location = "admin_media_playlist_edit.aspx";
       }
       var onSuccess = function () {
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
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" runat="server" id="hdnWindowUIMODE" />
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>
    <script type="text/javascript" src="../../../scripts/js/PostbackPrecedence.js"></script>
     <script type="text/javascript">
         preventPostBack('btnOkay', 'AlertDiv', 'AlertMessage');
    </script>
       <asp:UpdatePanel  ID="UpdatePanel1"  UpdateMode="Conditional" runat="server" >
            <Triggers>
                <asp:PostBackTrigger ControlID="btnOkay" />
            </Triggers>
            <ContentTemplate>
                <div class="popup_Container">
                    <div class="popup_Titlebar" id="PopupHeader">
                        <div class="TitlebarLeft">
                            Media Artist: Cập Nhập Thông Tin                            
                         </div>
                        <div class="TitlebarRight" onclick="cancel();">                            
                        </div>r
                    </div>
                    <div class="popup_Body">  
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                ...Processing...
                            </ProgressTemplate>
                        </asp:UpdateProgress>                               
                       <asp:MultiView ID="MultiView1" runat="server">
                            <asp:View ID="ViewInput" runat="server">
                                <table border="0" cellspacing="0" cellpadding="4"> 
                                              
                                       <tr>
                                        <td align="left" style="width:150px">
                                            Nhóm/ Loại</td>
                                        <td align="left">                                
                                        <asp:DropDownList ID="ddlMediaTypeList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>       
                                              
                                       <tr>
                                           <td align="left" style="width:150px">
                                               * PlayList Name</td>
                                           <td align="left">
                                               <asp:TextBox ID="txtPlayListName" runat="server" 
                                                   ValidationGroup="ValidationCheck" Width="300px"></asp:TextBox>
                                               <asp:RequiredFieldValidator ID="rfvPlayListName" runat="server" 
                                                   ControlToValidate="txtPlayListName" Display="Dynamic" ErrorMessage="Not null" 
                                                   ForeColor="#FF3300" SetFocusOnError="True" ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
                                           </td>
                                       </tr>
                                              
                                       <tr>
                                           <td align="left" style="width:150px">
                                               Chọn Image</td>
                                           <td align="left">
                                               <asp:Image ID="imgPhoto" runat="server" ImageAlign="Left" Height="30px" Width="30px" /><br />
                                                <asp:FileUpload ID="FileUpload1" onchange="File_OnChange(this)" runat="server" />         
                                                &nbsp;<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                SetFocusOnError="True" ValidationExpression="^.*\.(jpg|JPG|jpeg|JPEG|gif|GIF|png|PNG|bmp|BMP|tiff|TIFF)$"    
                                                ErrorMessage="Loại file k hợp lệ" ValidationGroup="checkValidation" 
                                                    ControlToValidate="FileUpload1"></asp:RegularExpressionValidator>
                                               <asp:TextBox ID="txtFile" runat="server" ReadOnly="true"></asp:TextBox>
                                               <script type="text/javascript">
                                                   function File_OnChange(sender) {
                                                       val = sender.value.split('\\');
                                                       document.getElementById('<%= txtFile.ClientID %>').value = val[val.length - 1];
                                                   }
                                                </script>
                                           </td>
                                       </tr>
                                              
                                       <tr>
                                           <td align="left" style="width:150px">
                                               Description</td>
                                           <td align="left">
                                               <asp:TextBox ID="txtDescription" Width="300px" TextMode="MultiLine" runat="server"></asp:TextBox>
                                           </td>
                                       </tr>
                                              
                                       <tr>
                                           <td align="left" style="width:150px">
                                               Status</td>
                                           <td align="left">
                                               <asp:RadioButtonList ID="rdlStatus" runat="server" RepeatDirection="Horizontal">
                                               </asp:RadioButtonList>
                                           </td>
                                       </tr>
                                       <tr>
                                        <td colspan="2">
                                             <div id="AlertDiv" class="MessageStyle">
                                                <span id="AlertMessage"></span>
                                            </div>
                                        </td>
                                       </tr>       
                                </table>
                  
                                 <div class="popup_Buttons">
                                    <asp:Button ID="btnOkay" Text="Done" runat="server" CausesValidation="false" ValidationGroup="ValidationCheck" OnClick="btnOkay_Click" />
                                    <input id="btnQuit" value="Cancel" type="button" onclick="cancel();" />
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
