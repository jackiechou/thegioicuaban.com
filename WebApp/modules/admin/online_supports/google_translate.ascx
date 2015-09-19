<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="google_translate.ascx.cs" Inherits="WebApp.modules.admin.online_supports.google_translate" %>
<%--<script type="text/javascript" src="http://www.google.com/jsapi"></script>
        <script type="text/javascript">
            google.load("language", "1");
            function translate(element, toLanguage) {
                if ((element.innerHTML != null) && (element.innerHTML != "")) {
                    google.language.detect(element.innerHTML, function (result) {
                        if (!result.error && result.language) {
                            google.language.translate(element.innerHTML, result.language, toLanguage, function (result) {
                                if (result.translation) {
                                    element.innerHTML = result.translation;
                                }
                            });
                        }
                    });
                }
            }

            function ChangeLanguage(toLanguage) {
                //var toLanguage = "en";             
                var elements = "div,span,a,h1,h2,h3,h4,p,li,td,p";
                var e = elements.split(",");
                for (var j = 0; j < e.length; j++) {
                    var doc = document.getElementsByTagName(e[j]);
                    for (var i = 0; i < doc.length; i++) {
                        translate(doc[i], toLanguage);
                    }
                }
            }

            function GetChange(val) {
                ChangeLanguage(val);
            }

            google.setOnLoadCallback(GetChange);

</script>

<select id="ddlLanguage" onchange="GetChange(this.value)" style="width: 130px" >
    <option value="vi">Vietnamese</option>
    <option value="en">English</option>
    <option value="zh-cn">Chinese</option>    
    
   <a onclick="javascript:GetChange('vi'); return false"><img src="../../images/flags/vn.png" alt="" /></a>&nbsp;
    <a onclick="javascript:GetChange('en'); return false"><img src="../../images/flags/en1.png" alt="" /></a>&nbsp;
    <a onclick="javascript:GetChange('zh-cn'); return false"><img src="../../images/flags/ch.png" alt="" /></a>&nbsp;
     
</select>--%>



<style type="text/css">iframe.goog-te-banner-frame { display:none !important; }</style>
<style type="text/css">body {position: static !important; top: 0 !important;}</style>
 <div class ="google_translate">
    <div id="google_translate_element"></div>
        <script type="text/javascript">
            function googleTranslateElementInit() {
                new google.translate.TranslateElement({
                    pageLanguage: 'vi'
                }, 'google_translate_element');
            }
        </script>
        <script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>


</div>
