$(function () {
    var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
    //HEADER NEWS ============================================================
    //    $('#news_ticker').ticker({
    //        speed: 0.10,           // The speed of the reveal
    //        //ajaxFeed: false,       // Populate jQuery News Ticker via a feed
    //        //feedUrl: false,        // The URL of the feed
    //        // MUST BE ON THE SAME DOMAIN AS THE TICKER
    //        //feedType: 'xml',       // Currently only XML
    //        htmlFeed: true,        // Populate jQuery News Ticker via HTML
    //        debugMode: true,       // Show some helpful errors in the console or as alerts
    //        // SHOULD BE SET TO FALSE FOR PRODUCTION SITES!
    //        controls: true,        // Whether or not to show the jQuery News Ticker controls
    //        titleText: 'TIN NÓNG - SƯ KIỆN NỔI BẬT',   // To remove the title set this to an empty String
    //        displayType: 'reveal', // Animation type - current options are 'reveal' or 'fade'
    //        direction: 'ltr',       // Ticker direction - current options are 'ltr' or 'rtl'
    //        pauseOnItems: 2000,    // The pause on a news item before being replaced
    //        fadeInSpeed: 600,      // Speed of fade in animation
    //        fadeOutSpeed: 300      // Speed of fade out animation
    //    });

    //HEADER NEWS VERTICAL TICKER - ADS
    function tick() {
        $('#news_ticker li:first').slideUp(function () { $(this).appendTo($('#news_ticker')).slideDown(); });
        $('#adv_left_inner li:first').slideUp(function () { $(this).appendTo($('#adv_left_inner')).slideDown(); });
        $('#adv_right_inner li:first').slideUp(function () { $(this).appendTo($('#adv_right_inner')).slideDown(); });
    }
    setInterval(function () { tick() }, 5000);

    //BANNER ROTATOR =================================================================================================
    $(".banner").bannerRotator({
        width: "980",
        height: "300",
        transition: "random"
    });

    //PARTNER VERTICAL SCROLL =========================================================================================
    function vertical_ticker() {
        $('#scroller_vticker li:first').slideUp(function () { $(this).appendTo($('#scroller_vticker')).slideDown(); });
    }
    setInterval(function () { vertical_ticker() }, 4000);

    //FOOTER SLIDE =======================================================================================================
    $('#carouselSider').jsCarousel({ onthumbnailclick: function (src) { alert(src); }, autoscroll: true, masked: false, itemstodisplay: 7, orientation: 'h' });


    //    $('.vticker').easyTicker({
    //        direction: 'up',
    //        visible: 6,
    //        easing: 'easeOutBounce',
    //	    interval: 2500,
    //        controls: {
    //            up: '.btnUp',
    //            down: '.btnDown',
    //            toggle: '.btnToggle'
    //        }
    //    });

    //JQUERY TOOL SCROLLABLE
    //    $(".scrollable").scrollable({ circular: true, vertical: true, mousewheel: true, interval: 7000, speed: 3000 }).navigator({ history: true }).autoscroll({ autoplay: true });
    //    $(".item").hover(function () {
    //        $(this).toggleClass("active");
    //    });  

    //MENU ===========================================================================================
    LoadMenuList();
    function LoadMenuList() 
    {
        $('ul.sf-menu').sooperfish({
            dualColumn: 6, //if a submenu has at least this many items it will be divided in 2 columns
            tripleColumn: 8, //if a submenu has at least this many items it will be divided in 3 columns
            hoverClass: 'sfHover',
            delay: 500, //make sure menus only disappear when intended, 500ms is advised by Jacob Nielsen
            animationShow: { width: 'show', height: 'show', opacity: 'show' },
            speedShow: 750,
            easingShow: 'easeOutBounce',
            animationHide: { width: 'hide', height: 'hide', opacity: 'hide' },
            speedHide: 300,
            easingHide: 'easeInOvershoot',
            autoArrows: true
        });

        $('#nav').each(function () {
            var currentURL = "";
            currentURL = window.location.toString().split("/");
            var currentURLdetail = currentURL[currentURL.length - 1];
            var n = $(this).find('li').length - 1;
            $(this).find('li').each(function (i) {
                var flag = 0;
                $(this).find('a').each(function (i) {
                    var linka = $(this).attr("href");
                    if ($(this).attr("href").indexOf(currentURLdetail) > -1) {
                        flag = 1;
                    }
                });
                if (flag == 1) $(this).addClass("current");
            });
        });


        /* Add Magic Line markup via JavaScript, because it ain't gonna work without */
        $("#nav").append("<li id='magic-line'></li>");

        /* Cache it */
        var magicLine = $("#magic-line");

        //        magicLine.width($(".current_page_item").width())
        //        .css("left", $(".current_page_item a").position().left)
        //        .data("origLeft", magicLine.position().left)
        //        .data("origWidth", magicLine.width());

        $("#nav li").find("a").hover(function () {
            leftPos = $(this).position().left;
            newWidth = $(this).parent().width();

            magicLine.stop().animate({
                left: leftPos,
                width: newWidth
            });
        }, function () {
            magicLine.stop().animate({
                left: magicLine.data("origLeft"),
                width: magicLine.data("origWidth")
            });
        });
    }

    //SEARCH =========================================================================================
    $('.searchInput').keypress(function (event) {
        // track enter key
        var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));
        if (keycode == 13) { // keycode for enter key
            // force the 'Enter Key' to implicitly click the Update button
            //document.getElementById('btnSubmit').click();
            $('.searchButtom').click();
            return false;
        } else {
            return true;
        }
    });

    $('.searchButtom').click(function (e) {
        e.preventDefault();
        var keywords = $('.searchInput').val();
        if (keywords != '') {
            ajaxGetSearcnResult(keywords, "vi-VN", 1, 15, 3, 45);
        }
        else
            $(".main_content").html("Vui lòng nhập thông tin cần tìm");
    });

    function ajaxGetSearcnResult(keywords, cultureCode, pageIndex, pageSize, pageShowing, totalItemCount) {
        var ajaxFunction = "ajaxGetSearcnResult";
        var divContainer = "main_content";
        var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
        var service_url = base_url + "/portals/news/services/news.asmx/SearchByKeywordsWithPagination";
        $.ajax({
            type: "POST",
            url: service_url,
            data: "{keywords:'" + keywords + "',cultureCode:'" + cultureCode + "',ajaxFunction:'" + ajaxFunction + "',pageIndex:'" + pageIndex + "',pageSize:'" + pageSize + "',pageShowing:'" + pageShowing + "',totalItemCount:'" + totalItemCount + "'}",
            beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg.d != '')
                    $("." + divContainer).html(msg.d);
                else
                    $("." + divContainer).html("Không tìm thấy dữ liệu");
            }, error: function (request, status, error) {
                $("." + divContainer).html(request.responseText);
            }
        });
    }
});


