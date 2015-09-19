<%@ Page Title="" Language="C#" MasterPageFile="~/portals/news/skins/VBA_skin/VBA_skin.Master" AutoEventWireup="true" CodeBehind="details.aspx.cs" Inherits="WebApp.portals.news.details" %>
<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/portals/news/controls/uc_ads.ascx" TagName="ads" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_partners.ascx" TagName="partners" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news.ascx" TagName="news" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news_bottom.ascx" TagName="news_bottom" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_events.ascx" TagName="events" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_support_online.ascx" TagName="support_online" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_topics.ascx" TagName="topics" TagPrefix="uc" %>
<%@ Register Src="~/portals/news/controls/uc_news_by_totalviews.ascx" TagName="totalviews" TagPrefix="uc" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        getCaptchaImage(190, 90, 6);
        $("#btnGetImage").click(function () {            
            getCaptchaImage(190, 90, 6);
        });
    });

    function getCaptchaImage(width, height, num_char) {
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="fl col1">           
        <div id="detail"> 
	        <div id="entry-detail">
	    	    <div class="entry-detail-box">
                    <asp:Literal ID="LiteralEntry" runat="server"></asp:Literal>
                    <div id="ct-comment">
                        <div class="ct-heading">
                            <h4>Gửi bình luận</h4>
                            <span class="btnCommentSubmit">                            
                                <asp:Button ID="btnSend" CssClass="SendButton button small gray" runat="server" Text="Gửi" onclick="btnSend_Click"  ValidationGroup="ValidationCheck" />
                            </span>
                        </div>               
                        <div class="ct-body">
                            <table>
                                <tr>
                                    <td>
                                         <asp:TextBox ID="txtName" placeholder="Họ tên" runat="server" Height="26px" Width="320px"></asp:TextBox>
                                         <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" Display="Dynamic" 
                                            ErrorMessage="*" ForeColor="#FF0000" SetFocusOnError="true" 
                                            ValidationGroup="ValidationCheck" ></asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmail" placeholder="Email" runat="server" Height="26px" Width="330px"></asp:TextBox>
                                        <asp:RequiredFieldValidator 
                                            ID="rfvEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" 
                                            ErrorMessage="*" ForeColor="#FF0000" SetFocusOnError="true" 
                                            ValidationGroup="ValidationCheck" ></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                            ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Email không hợp lệ" 
                                            ForeColor="#FF0000" SetFocusOnError="True" 
                                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                            ValidationGroup="ValidationCheck" ></asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">                                        
                                        <asp:TextBox ID="txtComments"  placeholder="Nhập bình luận ở đây!</b>" TextMode="MultiLine"
                                            Columns="50" Rows="5" runat="server"></asp:TextBox>  
                                        <ajaxToolkit:HtmlEditorExtender TargetControlID="txtComments" runat="server">
                                            <Toolbar>
                                                <ajaxToolkit:Bold />
                                                <ajaxToolkit:Italic />
                                            </Toolbar>
                                        </ajaxToolkit:HtmlEditorExtender>
                                    </td>
                                </tr>
                                <tr>
                                   <td colspan="2">
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
                                                    <input id="btnGetImage" class="BtnGetCapcha" type="button" value="Refresh" />  
                                                 </div>          
                                            </div>
                                        </div>          
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>      
            </div>     
            <div class="clear"></div>
            <%--Main Content--%>
            <div class="normal">
                <asp:Literal ID="ltrMainText" runat="server"></asp:Literal>            
            </div>
            <div class="clear"></div>
            <%--Related News--%>
            <div class="boxnew">
                <h4 class="relateNews">Tin liên quan</h4>
            </div>
            <div class="boxnew-headerNews">                                 
                <asp:Literal ID="ltrRelationNews" runat="server"></asp:Literal>
            </div>                
        </div>
        <div class="clear"></div>            
    </div>
    <div class="fr col2">
        <div class="col2_inner">                  
            <uc:events ID="event_control" runat="server" />          
            <uc:totalviews ID="totalviews_control" runat="server" />  
            <uc:ads ID="ads_control" runat="server" />
            <uc:partners ID="partners_control" runat="server" /> 
            <uc:support_online ID="support_online_control" runat="server" />  
        </div>             
    </div>
   <div class="clear"></div>

</asp:Content>
