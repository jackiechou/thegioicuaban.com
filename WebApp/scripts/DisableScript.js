function DisableRightClick() {
    am = "This function is disabled!";
    bV = parseInt(navigator.appVersion)
    bNS = navigator.appName == "Netscape"
    bIE = navigator.appName == "Microsoft Internet Explorer"

    function nrc() {
        if (bIE && (event.button > 1)) {
            alert(am);
            return false;
        }
    }
    document.onmousedown = nrc;
    if (bNS && bV < 5) window.onmousedown = nrc;
}

