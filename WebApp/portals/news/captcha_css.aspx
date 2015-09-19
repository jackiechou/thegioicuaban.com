<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="captcha_css.aspx.cs" Inherits="WebApp.portals.news.captcha_css" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
   <%--<div id="recaptcha_widget">

   <div id="recaptcha_image"></div>
   <div class="recaptcha_only_if_incorrect_sol" style="color:red">Incorrect please try again</div>

   <span class="recaptcha_only_if_image">Enter the words above:</span>
   <span class="recaptcha_only_if_audio">Enter the numbers you hear:</span>

   <input type="text" id="recaptcha_response_field" name="recaptcha_response_field" />

   <div><a href="javascript:Recaptcha.reload()">Get another CAPTCHA</a></div>
   <div class="recaptcha_only_if_image"><a href="javascript:Recaptcha.switch_type('audio')">Get an audio CAPTCHA</a></div>
   <div class="recaptcha_only_if_audio"><a href="javascript:Recaptcha.switch_type('image')">Get an image CAPTCHA</a></div>

   <div><a href="javascript:Recaptcha.showhelp()">Help</a></div>

 </div>--%>

 <style type="text/css">
     .CaptchaDiv
     {
       width: 245px; height: 50px; 
       background:#CCC; 
     }
     .CaptchaImageDiv
     {
         float:left;
       width: 215px; height: 50px;
        background:#000;
     }
     .CaptchaIconsDiv
     {
        float:left;
        width: 24px;
     }
  </style>
    <div id="CaptchaDiv" class="CaptchaDiv">
        <div class="CaptchaImageDiv">
             <img id="imgLoading" src="skins/VBA_skin/images/loading.gif" alt='' style="display: none" />
              <input id="hiddenCaptcha" type="hidden" runat="server" />
             <asp:TextBox ID="txtCaptcha" TabIndex="6" runat="server"></asp:TextBox>
            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
        </div>
        <div id="CaptchaIconsDiv" class="CaptchaIconsDiv">
            <input id="btnGetImage" class="button small gray"  type="button" value="Refresh" />         
        </div>
    </div>
    </form>
</body>
</html>
