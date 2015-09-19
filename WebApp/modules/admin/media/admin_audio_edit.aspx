<%@ Page Language="C#" ClientIDMode="Static" AutoEventWireup="true" CodeBehind="admin_audio_edit.aspx.cs"
    Inherits="WebApp.modules.admin.media.admin_audio_edit" %>

<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../../../scripts/jquery/jquery.js"></script>
    <script type="text/javascript" src="../../../scripts/plugins/ajaxtabs/jquery.tools.min.js"></script>
    <script type="text/javascript" src="../../../scripts/plugins/jwplayer/jwplayer.js"></script>
    <script type="text/javascript" src="../../../scripts/plugins/jwplayer/wmv/silverlight.js"></script>
    <script type="text/javascript" src="../../../scripts/plugins/jwplayer/wmv/wmvplayer.js"></script>
    <script type="text/javascript">
        //====================================================================================
        function getbacktostepone() {
            window.location = "admin_audio_edit.aspx";
        }
        function onSuccess() {
            setTimeout(okay, 2000);
        }
        function onError() {
            setTimeout(getbacktostepone, 2000);
        }
        function okay() {
            window.parent.document.getElementById('ButtonEditDone').click();
            parent.location.href = parent.location.href;
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Audio: Cập Nhập Thông Tin
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">
            <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                    <ajaxToolkit:TabContainer ID="TabAudio" runat="server">
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Audio Information" ID="TabInformation">
                            <ContentTemplate>
                                <div id="divVideoFileId" style="display: none;">
                                    <input id="txtFileId" type="hidden" runat="server"></input>
                                    </input> &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                                    </input></input></input></div>
                                <table border="0" cellspacing="0" cellpadding="4">
                                    <tr>
                                        <td align="left" width="180">
                                            * Nhóm Loại
                                        </td>
                                        <td align="left" width="300">
                                            <asp:DropDownList ID="ddlMediaTypeList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" width="300">
                                            * Chủ đề
                                        </td>
                                        <td align="left" width="300">
                                            <asp:DropDownList ID="ddlMediaTopicList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" width="180">
                                            * Chọn Album
                                        </td>
                                        <td align="left" width="300">
                                            <asp:DropDownList ID="ddlAlbumList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" width="300">
                                            * Chọn Playlist
                                        </td>
                                        <td align="left" width="300">
                                            <asp:DropDownList ID="ddlPlayList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" width="180">
                                            * Composer
                                        </td>
                                        <td align="left" width="300">
                                            <asp:DropDownList ID="ddlComposerList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" width="300">
                                            * Artist
                                        </td>
                                        <td align="left" width="300">
                                            <asp:DropDownList ID="ddlArtistList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" width="180">
                                            File Title:
                                        </td>
                                        <td align="left" width="300">
                                            <asp:TextBox ID="txtFileTitle" runat="server"></asp:TextBox>
                                        </td>
                                        <td align="left" width="300">
                                            Source
                                        </td>
                                        <td align="left" width="300">
                                            <asp:TextBox ID="txtSource" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" width="180">
                                            File Upload
                                        </td>
                                        <td align="left" width="300">
                                            <asp:RadioButtonList ID="rdlFileUpload" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Selected="True" Text="File Name" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="File URL" Value="1"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <div id="divAudioFileName" style="display: block; width: 250px; height: 29px;">
                                                <input id="File1" runat="server" type="file">
                                            </div>
                                            <div id="divAudioFileUrl" style="display: none; width: 250px; height: 20px;">
                                                <asp:TextBox ID="txtFileUrl" runat="server" Width="215px"></asp:TextBox></div>
                                            <script type="text/javascript">
                                                jQuery(document).ready(function ($) {
                                                    $("#rdlFileUpload input:radio:checked").each(function () {
                                                        ShowHiveDivFileUpload();
                                                    });

                                                    $("#rdlFileUpload").change(function () {
                                                        ShowHiveDivFileUpload();
                                                    });

                                                    function ShowHiveDivFileUpload() {
                                                        var selected_value = $("#rdlFileUpload input[type=radio]:checked").val();
                                                        if (selected_value == "1") {
                                                            if (document.getElementById) {
                                                                document.getElementById('divAudioFileName').style.display = 'none';
                                                                document.getElementById('divAudioFileUrl').style.display = 'block';
                                                            } else {
                                                                if (document.layers) {
                                                                    document.divAudioFileName.display = 'none';
                                                                    document.divAudioFileUrl.display = 'block';
                                                                }
                                                                else {
                                                                    document.all.divAudioFileName.style.display = 'none';
                                                                    document.all.divAudioFileUrl.style.display = 'block';
                                                                }
                                                            }
                                                        } else {
                                                            if (document.getElementById) {
                                                                document.getElementById('divAudioFileName').style.display = 'block';
                                                                document.getElementById('divAudioFileUrl').style.display = 'none';
                                                            } else {
                                                                if (document.layers) {
                                                                    document.divAudioFileName.display = 'block';
                                                                    document.divAudioFileUrl.display = 'none';
                                                                }
                                                                else {
                                                                    document.divAudioFileName.display = 'block';
                                                                    document.divAudioFileUrl.display = 'none';
                                                                }
                                                            }
                                                        }
                                                        return false;
                                                    }
                                                });
                                            </script>
                                        </td>
                                        <td align="left" width="300">
                                            Photo
                                        </td>
                                        <td align="left">
                                            <asp:Image ID="imgPhoto" runat="server" Height="30px" ImageAlign="Left" Width="30px" />
                                            <asp:FileUpload ID="FileUpload1" runat="server" /><br />
                                            <asp:CheckBox ID="chkAutoCreateThumbnail" runat="server" Text="Auto-Create Thumbnail ?" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            &#160;
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkAutoStart" runat="server" Text="AutoStart" />
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkMedialoop" runat="server" Text="Medialoop" />
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkIsFilePublished" runat="server" Text="File is published ?" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            Listen
                                        </td>
                                        <td align="left" colspan="3">
                                            <div id='mediaplayer'>
                                            </div>
                                            <script type="text/javascript">
                                                //====JWPlayer========================================================================================
                                                window.onload = function () {
                                                    GetMediaFile();
                                                }
                                                function GetMediaFile() {
                                                    var FileId = parseFloat($("#txtFileId").val().replace(/,/g, ''));
                                                    var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
                                                    var _flashplayer = base_url + "/scripts/plugins/jwplayer/player.swf";

                                                    $.ajax({
                                                        type: "POST",
                                                        url: base_url + "/services/WebServices.asmx/GetMediaFile",
                                                        data: "{FileId:'" + FileId + "'}",
                                                        async: false,
                                                        global: false,
                                                        beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
                                                        contentType: "application/json; charset=utf-8",
                                                        dataType: "json",
                                                        success: function (data) {
                                                            var myStrings = new String();
                                                            myStrings = '' + data.d + '';
                                                            var myArray = myStrings.split(",");
                                                            var _filepath = myArray[1].toString();
                                                            jwplayer('mediaplayer').setup({
                                                                'flashplayer': _flashplayer,
                                                                'id': 'playerID',
                                                                'width': '450',
                                                                'height': '24',
                                                                'file': _filepath,
                                                                'controlbar': 'bottom'
                                                            });
                                                        }, error: function (e) {
                                                            e.toString();
                                                        }
                                                    });
                                                }
                                            </script>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Description" ID="TabDescription">
                            <ContentTemplate>
                                <FCKeditorV2:FCKeditor ID="txtDescription" runat="server">
                                </FCKeditorV2:FCKeditor>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                    <div class="popup_Buttons">
                        <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="ValidationCheck"
                            OnClientClick="return false;" OnClick="btnOkay_Click" />
                        <input id="btnCancel" value="Cancel" type="button" onclick="cancel();" />
                    </div>
                </asp:View>
                <asp:View ID="ViewSuccess" runat="server">
                    <p>
                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                    </p>
                    <input id="btnClose" value="Đóng" type="button" onclick="cancel();" />
                </asp:View>
                <asp:View ID="ViewError" runat="server">
                    <p>
                        <asp:Label ID="lblErrorMsg" runat="server"></asp:Label>
                    </p>
                    <input id="btnExit" value="Đóng" type="button" onclick="cancel();" />
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
    </form>
</body>
</html>
