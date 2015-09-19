<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="checkdomain.ascx.cs" Inherits="WebApp.modules.admin.domain.checkdomain" %>
<script type="text/javascript">  
    //============================================================================================================================================
    function AjaxRequest() { var a = new Object(); a.timeout = null; a.generateUniqueUrl = true; a.url = window.location.href; a.method = "GET"; a.async = true; a.username = null; a.password = null; a.parameters = new Object(); a.requestIndex = AjaxRequest.numAjaxRequests++; a.responseReceived = false; a.groupName = null; a.queryString = ""; a.responseText = null; a.responseXML = null; a.status = null; a.statusText = null; a.aborted = false; a.xmlHttpRequest = null; a.onTimeout = null; a.onLoading = null; a.onLoaded = null; a.onInteractive = null; a.onComplete = null; a.onSuccess = null; a.onError = null; a.onGroupBegin = null; a.onGroupEnd = null; a.xmlHttpRequest = AjaxRequest.getXmlHttpRequest(); if (a.xmlHttpRequest == null) { return null } a.xmlHttpRequest.onreadystatechange = function () { if (a == null || a.xmlHttpRequest == null) { return } if (a.xmlHttpRequest.readyState == 1) { a.onLoadingInternal(a) } if (a.xmlHttpRequest.readyState == 2) { a.onLoadedInternal(a) } if (a.xmlHttpRequest.readyState == 3) { a.onInteractiveInternal(a) } if (a.xmlHttpRequest.readyState == 4) { a.onCompleteInternal(a) } }; a.onLoadingInternalHandled = false; a.onLoadedInternalHandled = false; a.onInteractiveInternalHandled = false; a.onCompleteInternalHandled = false; a.onLoadingInternal = function () { if (a.onLoadingInternalHandled) { return } AjaxRequest.numActiveAjaxRequests++; if (AjaxRequest.numActiveAjaxRequests == 1 && typeof (window.AjaxRequestBegin) == "function") { AjaxRequestBegin() } if (a.groupName != null) { if (typeof (AjaxRequest.numActiveAjaxGroupRequests[a.groupName]) == "undefined") { AjaxRequest.numActiveAjaxGroupRequests[a.groupName] = 0 } AjaxRequest.numActiveAjaxGroupRequests[a.groupName]++; if (AjaxRequest.numActiveAjaxGroupRequests[a.groupName] == 1 && typeof (a.onGroupBegin) == "function") { a.onGroupBegin(a.groupName) } } if (typeof (a.onLoading) == "function") { a.onLoading(a) } a.onLoadingInternalHandled = true }; a.onLoadedInternal = function () { if (a.onLoadedInternalHandled) { return } if (typeof (a.onLoaded) == "function") { a.onLoaded(a) } a.onLoadedInternalHandled = true }; a.onInteractiveInternal = function () { if (a.onInteractiveInternalHandled) { return } if (typeof (a.onInteractive) == "function") { a.onInteractive(a) } a.onInteractiveInternalHandled = true }; a.onCompleteInternal = function () { if (a.onCompleteInternalHandled || a.aborted) { return } a.onCompleteInternalHandled = true; AjaxRequest.numActiveAjaxRequests--; if (AjaxRequest.numActiveAjaxRequests == 0 && typeof (window.AjaxRequestEnd) == "function") { AjaxRequestEnd(a.groupName) } if (a.groupName != null) { AjaxRequest.numActiveAjaxGroupRequests[a.groupName]--; if (AjaxRequest.numActiveAjaxGroupRequests[a.groupName] == 0 && typeof (a.onGroupEnd) == "function") { a.onGroupEnd(a.groupName) } } a.responseReceived = true; a.status = a.xmlHttpRequest.status; a.statusText = a.xmlHttpRequest.statusText; a.responseText = a.xmlHttpRequest.responseText; a.responseXML = a.xmlHttpRequest.responseXML; if (typeof (a.onComplete) == "function") { a.onComplete(a) } if (a.xmlHttpRequest.status == 200 && typeof (a.onSuccess) == "function") { a.onSuccess(a) } else { if (typeof (a.onError) == "function") { a.onError(a) } } delete a.xmlHttpRequest.onreadystatechange; a.xmlHttpRequest = null }; a.onTimeoutInternal = function () { if (a != null && a.xmlHttpRequest != null && !a.onCompleteInternalHandled) { a.aborted = true; a.xmlHttpRequest.abort(); AjaxRequest.numActiveAjaxRequests--; if (AjaxRequest.numActiveAjaxRequests == 0 && typeof (window.AjaxRequestEnd) == "function") { AjaxRequestEnd(a.groupName) } if (a.groupName != null) { AjaxRequest.numActiveAjaxGroupRequests[a.groupName]--; if (AjaxRequest.numActiveAjaxGroupRequests[a.groupName] == 0 && typeof (a.onGroupEnd) == "function") { a.onGroupEnd(a.groupName) } } if (typeof (a.onTimeout) == "function") { a.onTimeout(a) } delete a.xmlHttpRequest.onreadystatechange; a.xmlHttpRequest = null } }; a.process = function () { if (a.xmlHttpRequest != null) { if (a.generateUniqueUrl && a.method == "GET") { a.parameters.AjaxRequestUniqueId = new Date().getTime() + "" + a.requestIndex } var c = null; for (var b in a.parameters) { if (a.queryString.length > 0) { a.queryString += "&" } a.queryString += encodeURIComponent(b) + "=" + encodeURIComponent(a.parameters[b]) } if (a.method == "GET") { if (a.queryString.length > 0) { a.url += ((a.url.indexOf("?") > -1) ? "&" : "?") + a.queryString } } a.xmlHttpRequest.open(a.method, a.url, a.async, a.username, a.password); if (a.method == "POST") { if (typeof (a.xmlHttpRequest.setRequestHeader) != "undefined") { a.xmlHttpRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded") } c = a.queryString } if (a.timeout > 0) { setTimeout(a.onTimeoutInternal, a.timeout) } a.xmlHttpRequest.send(c) } }; a.handleArguments = function (c) { for (var b in c) { if (typeof (a[b]) == "undefined") { a.parameters[b] = c[b] } else { a[b] = c[b] } } }; a.getAllResponseHeaders = function () { if (a.xmlHttpRequest != null) { if (a.responseReceived) { return a.xmlHttpRequest.getAllResponseHeaders() } alert("Cannot getAllResponseHeaders because a response has not yet been received") } }; a.getResponseHeader = function (b) { if (a.xmlHttpRequest != null) { if (a.responseReceived) { return a.xmlHttpRequest.getResponseHeader(b) } alert("Cannot getResponseHeader because a response has not yet been received") } }; return a } AjaxRequest.getXmlHttpRequest = function () {
        if (window.XMLHttpRequest) { return new XMLHttpRequest() } else {
            if (window.ActiveXObject) {
                /*@cc_on@*/
                /*@if(@_jscript_version >=5)
                try { return new ActiveXObject("Msxml2.XMLHTTP"); } catch (e) { try { return new ActiveXObject("Microsoft.XMLHTTP"); } catch (E) { return null; } } @end@*/
            } else { return null }
        }
    }; AjaxRequest.isActive = function () { return (AjaxRequest.numActiveAjaxRequests > 0) }; AjaxRequest.get = function (a) { AjaxRequest.doRequest("GET", a) }; AjaxRequest.post = function (a) { AjaxRequest.doRequest("POST", a) }; AjaxRequest.doRequest = function (a, c) { if (typeof (c) != "undefined" && c != null) { var b = new AjaxRequest(); b.method = a; b.handleArguments(c); b.process() } }; AjaxRequest.submit = function (b, d) { var c = new AjaxRequest(); if (c == null) { return false } var a = AjaxRequest.serializeForm(b); c.method = b.method.toUpperCase(); c.url = b.action; c.handleArguments(d); c.queryString = a; c.process(); return true }; AjaxRequest.serializeForm = function (b) { var f = b.elements; var e = f.length; var d = ""; this.addField = function (h, i) { if (d.length > 0) { d += "&" } d += encodeURIComponent(h) + "=" + encodeURIComponent(i) }; for (var c = 0; c < e; c++) { var a = f[c]; if (!a.disabled) { switch (a.type) { case "text": case "password": case "hidden": case "textarea": this.addField(a.name, a.value); break; case "select-one": if (a.selectedIndex >= 0) { this.addField(a.name, a.options[a.selectedIndex].value) } break; case "select-multiple": for (var g = 0; g < a.options.length; g++) { if (a.options[g].selected) { this.addField(a.name, a.options[g].value) } } break; case "checkbox": case "radio": if (a.checked) { this.addField(a.name, a.value) } break } } } return d }; AjaxRequest.numActiveAjaxRequests = 0; AjaxRequest.numActiveAjaxGroupRequests = new Object(); AjaxRequest.numAjaxRequests = 0;
    //=======================================================================================================================================

    function gmobj(mtxt) { if (document.getElementById) { m = document.getElementById(mtxt) } else if (document.all) { m = document.all[mtxt] } else if (document.layers) { m = document[mtxt] } return m; }

    function CheckDomain(Domain, Ext, Command, divID) {
        AjaxRequest.get(
        {
            "url": "../../../modules/admin/domain/Whois.ashx?domain=" + Domain + "&ext=" + Ext + "&cmd=" + Command
            , 'onLoading': function () {
                gmobj(divID).innerHTML = "<img src=\"../../../images/icons/ajax-loader.gif\" border=\"0\" />"
            }
	        , 'onSuccess': function (req) {
	            var strResponse = req.responseText;
	          
	            if (Command != "") {
	                gmobj(divID).innerHTML = "<span style=\"color:red;\">" + strResponse + "</span>";
	            } else {
	                if (strResponse == "0") //Domain đã được đăng ký
	                {

	                    gmobj(divID).innerHTML = "<span style=\"color:red;\">Đã được đăng ký</span>";
	                } else //Domain chưa đăng ký
	                {
	                    gmobj(divID).innerHTML = "<span style=\"color:green;\">Chưa có ai đăng ký</span>";
	                }
	            }
	        }
	        , 'onError': function (req) {
	            gmobj(divID).innerHTML = req.statusText;
	        }
        });
    }

</script>



<h2>Check Domain Available</h2> 
<asp:RadioButtonList ID="rdlDomainGroup" runat="server" 
    onselectedindexchanged="rdlDomainGroup_SelectedIndexChanged">
</asp:RadioButtonList>       
<asp:TextBox ID="txtDomain" runat="server" Width="188px"></asp:TextBox>
&nbsp;
<asp:DropDownList ID="ddlDomainExtension" runat="server">
</asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvDomain" runat="server" 
    ControlToValidate="txtDomain" Display="Dynamic" 
    ErrorMessage="* Vui lòng nhập địa chỉ domain"></asp:RequiredFieldValidator>
<asp:Button ID="btnCheck" runat="server" Text="Check" OnClick="btnCheck_Click" />
<br />

            
<div id="divResult" runat="server"></div>

