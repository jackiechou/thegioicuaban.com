<%@ Page Language="C#" ClientIDMode="Static" AutoEventWireup="true" CodeBehind="admin_video_edit.aspx.cs"
    Inherits="WebApp.modules.admin.media.admin_video_edit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/jquery/jquery.js"></script>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/plugins/ajaxtabs/jquery.tools.min.js"></script>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/plugins/jwplayer/jwplayer.js"></script>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/plugins/jwplayer/wmv/silverlight.js"></script>
    <script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/scripts/plugins/jwplayer/wmv/wmvplayer.js"></script>
    <script type="text/javascript">
        //====================================================================================
        function getbacktostepone() {
            window.location = "admin_video_edit.aspx";
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
    <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                File Manager: Upload Image/File</div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">
            <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                    <ajaxToolkit:TabContainer ID="VideoEdit" runat="server" ActiveTabIndex="0">
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Video File Information" ID="VideoFileInformation">
                            <ContentTemplate>
                                <div id="divVideoFileId" style="display: none;">
                                    <asp:HiddenField ID="hdFileId" runat="server" />
                                </div>
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
                                            * Album
                                        </td>
                                        <td align="left" width="300">
                                            <asp:DropDownList ID="ddlAlbumList" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" width="300">
                                            * PlayList
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
                                            <asp:TextBox ID="txtFileTitle" Width="250px" runat="server"></asp:TextBox>
                                        </td>
                                        <td align="left" width="300">
                                            Source
                                        </td>
                                        <td align="left" width="300">
                                            <asp:TextBox ID="txtSource" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            File Upload
                                        </td>
                                        <td align="left">
                                            <asp:RadioButtonList ID="rdlFileUpload" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Selected="True" Text="File Name" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="File URL" Value="1"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <div id="divVideoFileName" style="display: block; width: 250px; height: 29px;">
                                                <input id="File1" runat="server" type="file">
                                            </div>
                                            <div id="divVideoFileUrl" style="display: none; width: 250px; height: 20px;">
                                                <asp:TextBox ID="txtFileUrl" runat="server" Width="215px"></asp:TextBox>
                                            </div>
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
                                                                document.getElementById('divVideoFileName').style.display = 'none';
                                                                document.getElementById('divVideoFileUrl').style.display = 'block';
                                                            } else {
                                                                if (document.layers) {
                                                                    document.divVideoFileName.display = 'none';
                                                                    document.divVideoFileUrl.display = 'block';
                                                                }
                                                                else {
                                                                    document.all.divVideoFileName.style.display = 'none';
                                                                    document.all.divVideoFileUrl.style.display = 'block';
                                                                }
                                                            }
                                                        } else {
                                                            if (document.getElementById) {
                                                                document.getElementById('divVideoFileName').style.display = 'block';
                                                                document.getElementById('divVideoFileUrl').style.display = 'none';
                                                            } else {
                                                                if (document.layers) {
                                                                    document.divVideoFileName.display = 'block';
                                                                    document.divVideoFileUrl.display = 'none';
                                                                }
                                                                else {
                                                                    document.divVideoFileName.display = 'block';
                                                                    document.divVideoFileUrl.display = 'none';
                                                                }
                                                            }
                                                        }
                                                        return false;
                                                    }
                                                });
                                            </script>
                                        </td>
                                        <td align="left">
                                            Photo
                                        </td>
                                        <td align="left">
                                            <asp:Image ID="imgPhoto" runat="server" Height="30px" ImageAlign="Left" 
                                                Width="30px" />
                                            <asp:FileUpload ID="FileUpload1" runat="server" /><br />
                                            <asp:CheckBox ID="chkAutoCreateThumbnail" runat="server" Text="Auto-Create Thumbnail ?" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            Dimension
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtDimension" runat="server"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkAutoStart" runat="server" Text="AutoStart" />
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkMedialoop" runat="server" Text="Medialoop" />
                                            <asp:CheckBox ID="chkIsFilePublished" runat="server" Text="File is published ?" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Video File View" ID="VideoFileView">
                            <ContentTemplate>
                                <div id="mediaplayer">
                                </div>
                                <script type="text/javascript">
                                    //====JWPlayer========================================================================================
                                    jQuery(document).ready(function ($) {
                                        GetMediaFile();
                                    });
                                    function GetMediaFile() {
                                        var FileId = parseFloat($("#hdFileId").val().replace(/,/g, ''));
                                        var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
                                        var _flashplayer = base_url + "/scripts/plugins/jwplayer/player.swf";
                                        var _skin_player = base_url + "/scripts/plugins/jwplayer/skinplayer/stormtrooper.zip";
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
                                                var _previewimage = myArray[2].toString();
                                                var char = _filepath.substr(_filepath.lastIndexOf("."));
                                                if (char != ".wmv") {
                                                    jwplayer("mediaplayer").setup({
                                                        flashplayer: "" + _flashplayer + "",
                                                        file: "" + _filepath + "",
                                                        image: "" + _previewimage + "",
                                                        skin: "" + _skin_player + "",
                                                        width: "720",
                                                        height: "370",
                                                        controlbar: "bottom"
                                                    });
                                                }
                                                else {
                                                    var elm = document.getElementById("mediaplayer");
                                                    var src = base_url + '/scripts/plugins/jwplayer/wmv/wmvplayer.xaml';
                                                    var cfg = {
                                                        file: "" + _filepath + "",
                                                        image: "" + _previewimage + "",
                                                        skin: "" + _skin_player + "",
                                                        width: "720",
                                                        height: "370",
                                                        controlbar: "bottom"
                                                    };
                                                    var ply = new jeroenwijering.Player(elm, src, cfg);
                                                }
                                            }, error: function (e) {
                                                e.toString();
                                            }
                                        });
                                    }
                                </script>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Description" ID="Description">
                            <ContentTemplate>
                                <FCKeditorV2:FCKeditor ID="txtDescription" runat="server" Width="850px" Height="350px">
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
