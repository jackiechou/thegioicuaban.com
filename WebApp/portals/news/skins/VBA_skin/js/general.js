function include_jQueryFilesToPage() {
    var siteAddress = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
    var head = document.getElementsByTagName('head')[0];

//    //CSS jnclude
//    var cssID = 'cssID';  // you could encode the css path itself to generate id.
//    var cssFilePath = siteAddress + '/portals/news/skins/VBA_skin/css/style.css';
//    if (!document.getElementById(cssID)) {
//        var link = document.createElement('link');
//        link.id = cssID;
//        link.rel = 'stylesheet';
//        link.type = 'text/css';
//        link.href = cssFilePath;
//        link.media = 'all';
//        head.appendChild(link);
//    }

//    //jQueryCSS include    
//    var jqCSS = 'cssIDJQ';  // you could encode the css path itself to generate id.
//    var jqCSSFilePath = '/scripts/jquery/jquery-ui-1.8.23.custom/css/ui-lightness/jquery-ui-1.8.23.custom.css';
//    if (!document.getElementById(jqCSS)) {
//        var link = document.createElement('link');
//        link.id = jqCSS;
//        link.rel = 'stylesheet';
//        link.type = 'text/css';
//        link.href = jqCSSFilePath;
//        link.media = 'all';
//        head.appendChild(link);
//    } 

    ////jQueryUI include    
    //var jqUIFilePath = '/scripts/jquery/jquery-ui-1.8.23.custom/js/jquery-ui-1.8.2.custom.min.js';
    //    var jqUI = "uiFileRefIDJQ";
    //    if (!document.getElementById(jqUI))
    //        document.write('<scr' + 'ipt type="text/javascript" id="' + jqUI + '" src="' + jqUIFilePath + '"></scr' + 'ipt>');


    // Core jQuery include
    var jqc = "coreFileRefIDJQ";
    var jqCoreFilePath = '/scripts/jquery/jquery.min.js';
    if (!document.getElementById(jqc))
        document.write('<scr' + 'ipt type="text/javascript" id="' + jqc + '" src="' + jqCoreFilePath + '"></scr' + 'ipt>');

    // JS Cookie include
    var jsCookieID = "jsCookieID";
    var jsCookiePath = '/scripts/js/cookie.js';
    var jqCoreFilePath = '/scripts/jquery/jquery.min.js';
    if (!document.getElementById(jsCookieID))
        document.write('<scr' + 'ipt type="text/javascript" id="' + jsCookieID + '" src="' + jsCookiePath + '"></scr' + 'ipt>');

    // JS News Ticker include ===============================================================================================
//    var jsNewsTickerID = "jsNewsTickerID";
//    var jsNewsTickerPath = '/scripts/plugins/jquery_news_ticker/includes/jquery.ticker.js';
//    if (!document.getElementById(jsCookieID))
//        document.write('<scr' + 'ipt type="text/javascript" id="' + jsNewsTickerID + '" src="' + jsNewsTickerPath + '"></scr' + 'ipt>');
}
function getVirtualDirectory() {
    var vDir = document.location.pathname.split('/');
    return '/' + vDir[1] + '/';
}
include_jQueryFilesToPage();