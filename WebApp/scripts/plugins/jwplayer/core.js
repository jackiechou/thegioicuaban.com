/***************************/
//@Author: Adrian "yEnS" Mato Gondelle
//@website: www.yensdesign.com
//@email: yensamg@gmail.com
//@license: Feel free to use it, but keep this credits please!					
/***************************/

function embed_clip(image_link, video_link, flash_width, flash_height) {
    var player_link = "Scripts/jwplayer/player.swf";
    var id_container = "myplayer2";
    var playerFlv = new SWFObject(player_link, id_container, flash_width, flash_height, "9.0.115");
    playerFlv.addVariable("image", image_link);
    playerFlv.addVariable("file", video_link);
    playerFlv.addVariable("screencolor", "white");
    playerFlv.write("player2");
    //create a javascript object to allow us send events to the flash player
    var player2 = document.getElementById(id_container);
    var mute2 = 0;

    //EVENTS for FLV files player
    $("#play2").click(function () {
        player2.sendEvent("PLAY", "true");
    });
    $("#pause2").click(function () {
        player2.sendEvent("PLAY", "false");
    });
    $("#mute2").click(function () {
        if (mute2 == 0) {
            player2.sendEvent("mute", "true");
            mute2 = 1;
        }
        else {
            player2.sendEvent("mute", "false");
            mute2 = 0;
        }
    });
}
