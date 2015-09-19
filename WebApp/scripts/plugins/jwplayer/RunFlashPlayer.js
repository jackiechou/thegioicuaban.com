function RunFlashPlayer(divContainer, fileUrl, width, height) {
    var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');         
    var src = base_url + "/scripts/plugins/dewplayer/player_mp3_maxi.swf?mp3=" + escape(fileUrl);

    // Media embed methods -------------------------------------------------
    function addParams(obj, params) {
        for (var p = 0; p < params.length; p++) {
            var prm = document.createElement("param");
            prm.setAttribute("name", params[p][0]);
            prm.setAttribute("value", params[p][1]);
            obj.appendChild(prm);
        }
    }
    function addAttributes(obj, params) {
        for (var p = 0; p < params.length; p++) {
            obj.setAttribute(params[p][0], params[p][1]);
        }
    }

    var attrs = [
            ["width", width],
            ["height", height]
          ];
    var params = [
            ["quality", "best"],
            ["allowScriptAccess", "always"],
            ["wmode", "window"]
          ];

    var obj = document.createElement("object");
    addAttributes(obj, attrs);
    addAttributes(obj, [
            ["align", "middle"],
            ["codebase", "http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0"],
            ["classid", "clsid:d27cdb6e-ae6d-11cf-96b8-444553540000"]
          ]);
    addParams(obj, params);
    addParams(obj, [
            ["movie", src]
          ]);
    var embed = document.createElement("embed");
    addAttributes(embed, attrs);
    addAttributes(embed, params);
    addAttributes(embed, [
            ["type", "application/x-shockwave-flash"],
            ["classname", "audio-player-embed"],
            ["pluginspage", "http://www.macromedia.com/go/getflashplayer"],
            ["flashvars", "playerMode=embedded"],
            ["bgcolor", "#ffffff"],
            ["src", src]
          ]);
    obj.appendChild(embed);
    document.getElementById(divContainer).appendChild(obj);
}