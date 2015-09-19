<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_background_audio_select.aspx.cs" Inherits="WebApp.modules.admin.media.admin_background_audio_select" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "modules/admin/media/admin_background_audio_select.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 2000);
       }
       function onError() {
           setTimeout(getbacktostepone, 2000);
       }
       function okay() {         
            parent.location.href = parent.location.href;
            window.parent.document.getElementById('ButtonSelectDone').click();         
       }
       function cancel() {          
            window.parent.document.getElementById('ButtonSelectCancel').click();         
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
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Quản lý nhạc nền</div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">
            <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">  
                <div>
                    Portal :<asp:DropDownList ID="ddlPortalList" runat="server" onselectedindexchanged="ddlPortalList_SelectedIndexChanged"></asp:DropDownList>  
                     &nbsp;
                     Media Type: <asp:DropDownList ID="ddlMediaTypeList" runat="server" onselectedindexchanged="ddlMediaTypeList_SelectedIndexChanged"></asp:DropDownList>  
                </div>               
                <div class="content_used">							
	                <div class="bor_img">
                        <asp:Literal ID="Ltr_SeletedItem" runat="server"></asp:Literal>		
	                </div>
	                <p class="text_used">Nhạc nền đang sử dụng</p>												
                </div> 
                <div runat="server" id="lstThemes" class="content_view">
                    <p style="text-align:center; font-size:1.3em; padding-top:10px;"><b>Chọn nhạc nền mới</b></p>
                    <asp:ListView ID="ListView1" GroupItemCount="1" DataKeyNames="Id" 
                        runat="server" onitemcommand="ListView1_ItemCommand"  >
                        <LayoutTemplate>       
                                <ul class="list_theme" id="tbl1" runat="server">
                                    <li runat="server" id="groupPlaceholder"></li>
                                </ul>
      
                        </LayoutTemplate>
                        <GroupTemplate>
                            <li id="Li1" runat="server">
                                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                            </li>
                        </GroupTemplate>
                        <ItemTemplate>      
                            <div class="theme_item">
                                <object type="application/x-shockwave-flash" data='<%# Eval("FileName", "../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=../../../user_files/media/audio/background_audio/{0}") %>' width="240" height="20" id="dewplayer-vol">
                                    <param name="wmode" value="transparent" />
                                    <param name="movie" value='<%# Eval("FileName", "../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=../../../user_files/media/audio/background_audio/{0}") %>'/>
                                </object>
                                <br />
                                <asp:Label runat="server" ID="Title" Text='<%#Eval("Title") %>' ></asp:Label><br /><br />
                                <asp:LinkButton ID="btnSelect" Text="Chọn Theme" CommandName="UpdateStatus" 
                                CommandArgument='<%#Eval("Id")%>' runat="server" />                               
                                                                           
                                </div> 
                        </ItemTemplate>
                    </asp:ListView>  
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