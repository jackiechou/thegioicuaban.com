//******************************************************************************************
// To add cookie information to the HTTP header need to use the following Syntax:
//
// document.cookie = "name=value; expires=date; path=path;domain=domain; secure";
//
// This function sets a client-side cookie as above.  Only first 2 parameters are required
// Rest of the parameters are optional. If no CookieExp value is set, cookie is a session cookie.
//******************************************************************************************

function setCookie(CookieName, CookieVal, CookieExp, CookiePath, CookieDomain, CookieSecure) {
    var CookieText = escape(CookieName) + '=' + escape(CookieVal); //escape() : Encodes the String
    CookieText += (CookieExp ? '; EXPIRES=' + CookieExp.toGMTString() : '');
    CookieText += (CookiePath ? '; PATH=' + CookiePath : '');
    CookieText += (CookieDomain ? '; DOMAIN=' + CookieDomain : '');
    CookieText += (CookieSecure ? '; SECURE' : '');

    document.cookie = CookieText;
}

// This functions reads & returns the cookie value of the specified cookie (by cookie name)
function getCookie(CookieName) {
    var CookieVal = null;
    if (document.cookie)       //only if exists
    {
        var arr = document.cookie.split((escape(CookieName) + '='));
        if (arr.length >= 2) {
            var arr2 = arr[1].split(';');
            CookieVal = unescape(arr2[0]); //unescape() : Decodes the String
        }
    }
    return CookieVal;
}

// To delete a cookie, pass name of the cookie to be deleted
function deleteCookie(CookieName) {
    var tmp = getCookie(CookieName);
    if (tmp) {
        setCookie(CookieName, tmp, (new Date(1))); //Used for Expire
    }
}