function preventPostBack(exclusivePostBackElement, divElem, messageElem) {
    Sys.Application.add_load(ApplicationLoadHandler)
    function ApplicationLoadHandler(sender, args) {
        if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
        }
    }
    var lastPostBackElement;
    function InitializeRequest(sender, args) {
        disableButtonOnClick();
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm.get_isInAsyncPostBack() && args.get_postBackElement().id === exclusivePostBackElement) {
            if (lastPostBackElement === exclusivePostBackElement) {
                args.set_cancel(true);
                ActivateAlertDiv('visible', 'A previous postback is still executing. The new postback has been canceled.');
                setTimeout("ActivateAlertDiv('hidden','')", 1500);
            }
            else if (lastPostBackElement !== exclusivePostBackElement) {
                prm.abortPostBack();
            }
        }
        else if (prm.get_isInAsyncPostBack() && args.get_postBackElement().id !== exclusivePostBackElement) {
            if (lastPostBackElement === exclusivePostBackElement) {
                args.set_cancel(true);
                ActivateAlertDiv('visible', 'A previous postback is still executing. The new postback has been canceled.');
                setTimeout("ActivateAlertDiv('hidden','')", 1500);
            }
        }
        lastPostBackElement = args.get_postBackElement().id;
    }

    function ActivateAlertDiv(visString, msg) {
        var adiv = $get(divElem);
        var aspan = $get(messageElem);
        adiv.style.visibility = visString;
        aspan.innerHTML = msg;
    }

    //disables the button specified and sets its style to a disabled "look".
    function disableButtonOnClick() {
        var oButton = $get(exclusivePostBackElement);
        oButton.disabled = true;      // set button to disabled so you can't click on it.
        oButton.value = 'Please wait...';   // change the text of the button.
        oButton.setAttribute('className', 'disabled_button'); // IE uses className for the css property.
        oButton.setAttribute('class', 'disabled_button'); // Firefox, Safari use class for the css property.  (doesn't hurt to do both).
    }

    if (typeof (Sys) !== "undefined") Sys.Application.notifyScriptLoaded();
}