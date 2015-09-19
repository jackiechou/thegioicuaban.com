<%@ Page Title="" Language="C#" MasterPageFile="~/portals/news/skins/VBA_skin/VBA_skin.Master"
    AutoEventWireup="true" CodeBehind="contact.aspx.cs" Inherits="WebApp.portals.news.contact" %>

<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
<%@ Register Src="~/portals/news/controls/uc_contactinfo.ascx" TagName="contactinfo"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            //CAPTCHA ==================================================================================================

            getCaptchaImage(190, 90, 6);
            $("#btnGetImage").click(function () {
                getCaptchaImage(190, 90, 6);
            });

            function getCaptchaImage(width, height, num_char, hiddenStoreCaptchaValue) {
                var random_text = GetRandomText(num_char);
                $("#<%= hiddenCaptcha.ClientID %>").val(random_text);
                $('#imgLoading').show();
                $('#imgId').remove();
                $('#CaptchaImageDiv').append('<img id="imgId" alt="" style="display:none" src="/handlers/CaptchaHandler.ashx?captchatext=' + random_text + '&width=' + width + '&height=' + height + '" />');
                $('#imgId').load(function () {
                    $("#imgLoading").hide();
                    $('#imgId').fadeIn('slow');
                });
            }

            function GetRandomText(count) {
                var allowedChars = new String();
                allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
                allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
                allowedChars += "1,2,3,4,5,6,7,8,9,0";

                var arr = new Array();
                arr = allowedChars.split(",");

                var passwordString = new String();
                var temp = new String();
                passwordString = "";
                temp = "";

                for (i = 0; i < count; i++) {
                    var rand = Math.random();
                    temp = Math.ceil(rand * (arr.length - 1));
                    passwordString += arr[temp];
                }
                return passwordString;
            }
            //==========================================================================================================
        });
    </script>
    <div class="fl col1">
        <div class="fLeft clorB">
            <h3 class="fLeft textTrans">
                Liên hệ
            </h3>
        </div>
        <div class="contact_us">
            <div class="news_ykien_description">
                <center>
                    <table border="0" cellspacing="2">
                    <tr><td colspan="2" align="left">Nếu bạn có bất kỳ một yêu cầu hay ý kiến đóng góp nào, xin hãy sử dụng mẫu dưới đây</td></tr>
                        <tr>
                            <td>Họ và tên bạn:</td>
                            <td  align="left">
                                 <asp:TextBox ID="txtName" TabIndex="1" runat="server" Width="300px" ></asp:TextBox>
                                           <asp:RequiredFieldValidator ID="rfvName" runat="server" ForeColor="#FF0000" SetFocusOnError="true" Display="Dynamic" ValidationGroup="check" 
                                                ControlToValidate="txtName" ErrorMessage="*"></asp:RequiredFieldValidator>                       
                            </td>
                        </tr>
                        <tr>
                            <td >Email của bạn:</td>
                            <td   align="left">                                
                                 <asp:textbox id="txtFrom" TabIndex="2" runat="server" Width="300px"></asp:textbox>
                                <asp:RequiredFieldValidator ID="rfvFrom" runat="server" ForeColor="#FF0000" SetFocusOnError="true" Display="Dynamic" ValidationGroup="check"
                                    ControlToValidate="txtFrom" ErrorMessage="*"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revFrom" runat="server" ControlToValidate="txtFrom" Display="Dynamic" ValidationGroup="check" 
                                    ErrorMessage="Email không hợp lệ" SetFocusOnError="True" ForeColor="#FF0000"
                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                
                            </td>
                        </tr>
                        <tr>
                            <td >Tiêu đề:</td>
                            <td   align="left">                                
                                <asp:textbox id="txtSubject" TabIndex="3" runat="server" Font-Names="Verdana" 
                                    Width="450px"></asp:textbox>
                                <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ForeColor="#FF0000" SetFocusOnError="true" Display="Dynamic" ValidationGroup="check" 
                                                ControlToValidate="txtSubject" ErrorMessage="*"></asp:RequiredFieldValidator> 
                                
                            </td>
                        </tr>
                        <tr valign"top">
                            <td  valign="top">Nội dung:</td>
                            <td   align="left">
                                
                                <asp:textbox id="txtBody" TabIndex="4" runat="server" Font-Names="Verdana"
											        columns="20" rows="2" textmode="MultiLine" width="450px" Height="120px"></asp:textbox>
                                
                            </td>
                        </tr>
                        <tr>
                            <td > File đính kèm: </td>
                            <td   align="left">
                                
                                <input id="inpAttachment" TabIndex="5" type="file" size="15" name="filMyFile0" runat="server" /></td>
                        </tr>
                        <tr>
                            <td >Mã bảo mật:</td>
                            <td  align="left">  
                                <div class="CaptchaWrapper">
                                    <div class="CaptchaBox">
                                        <div id="CaptchaImageDiv" class="CaptchaImageDiv">
                                            <img id="imgLoading" src="skins/VBA_skin/images/loading.gif" alt='' style="display: none" />
                                            <input id="hiddenCaptcha" type="hidden" runat="server" />
                                         </div>                                         
                                    </div>
                                    <div class="CaptchaIconsDiv">                                        
                                        <div class="CaptchaHeadText">                                            
                                            <h4>Nhập Captcha theo đúng hình trên: </h4>
                                        </div>
                                        <div class="CaptchaControls">  
                                            <asp:TextBox ID="txtCaptcha" Width="140" TabIndex="6" runat="server"></asp:TextBox>
                                            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>                                        
                                            <input id="btnGetImage" class="BtnGetCapcha"  type="button" value="Refresh" />  
                                         </div>          
                                    </div>
                                </div>            
                            </td>
                        </tr>                                                                  
                        <tr>
                            <td>
                                
                                &nbsp;</td>
                            <td align="left">               
                                 
                                <asp:Button ID="btnSend" runat="server" Text="Gửi" Width="77px" 
                                    onclick="btnSend_Click" />
                                 
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <div id="response"></div>
                            </td>
                        </tr>
                    </table>
                    </center>
            </div>
        </div>
    </div>
    <div class="fr col2">
        <div class="col2_inner">
            <uc:contactinfo ID="contactInfo2" runat="server" />
        </div>
    </div>
    <div class="clear">
    </div>
</asp:Content>
