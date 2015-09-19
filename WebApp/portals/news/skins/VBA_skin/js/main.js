var paging = 0; var check = 0; function gotopage(url) { window.location.href = url; }
function joc_home_video(val) {
    var boxvideo_1 = $('#boxvideo-1'); var boxvideo_2 = $('#boxvideo-2'); var boxvideo_3 = $('#boxvideo-3'); $('#switch-1').removeClass('current current-end'); $('#switch-2').removeClass('current current-end'); $('#switch-3').removeClass('current-top current-end'); if (val == 1) { boxvideo_1.css('display', 'block'); $('#switch-1').addClass('current'); }
    else
        boxvideo_1.css('display', 'none'); if (val == 2) { boxvideo_2.css('display', 'block'); $('#switch-2').addClass('current'); $('#switch-1').addClass('current-end'); }
    else
        boxvideo_2.css('display', 'none'); if (val == 3) { boxvideo_3.css('display', 'block'); $('#switch-3').addClass('current-top'); $('#switch-2').addClass('current-end'); }
    else
        boxvideo_3.css('display', 'none');
}
$(document).ready(function () {
    $(".option").change(function () {
        if ($(this).val() != "1")
            window.location.href = root_url + $(this).val();
    });
    $(".tab").click(function () {
        for (var i = 1; i < 4; i++)
        { $("#tab" + i).removeClass('current'); $(".tab" + i).css('display', 'none'); }
        $(this).addClass('current'); $("." + $(this).attr('rel')).css('display', 'block');
    });
    $("#weather").change(function () {
        for (var i = 0; i < 9; i++)
            $("#location_" + i).css('display', 'none'); $("#" + $(this).val()).css('display', 'block');
    });
    $("#bxh").change(function () {
        for (var i = 0; i < 6; i++)
            $("#bxh_" + i).css('display', 'none'); $("#bxh_" + $(this).val()).css('display', '');
    });
    $("#next").click(function () {
        if (check == 0) {
            $.ajax({ url: 'ajax.php?path=news&fnc=girl&page_no=' + (paging + 1), type: 'GET', dataType: 'json', success: function (data)
            { paging++; check = data.check; $("#cates").html(data.html); }, timeout: function () { }
            });
        }
    }); $("#prev").click(function () {
        if (paging > 0) {
            $.ajax({ url: 'ajax.php?path=news&fnc=girl&page_no=' + (paging - 1), type: 'GET', dataType: 'json', success: function (data)
            { check = data.check; paging--; $("#cates").html(data.html); }, timeout: function () { }
            });
        }
    }); $(".slide_show").mouseenter(function () {
        if ($("#one_link").attr("href") != $(this).attr("href"))
        { $("#one_link").attr("href", $(this).attr("href")); $("#one_image").attr("src", $(this).attr("rel")); $("#one_image").css("display", "inline"); } 
    });
}); function career_search(rewrite) {
    var keyword = $("#keyword").val(); var city = $("#city").val(); if ($.trim(keyword) == "" && city < 1)
    { alert('Nháº­p tá»« khÃ³a tÃ¬m kiáº¿m'); $('#keyword').focus(); }
    else {
        if (rewrite)
            window.location.href = root_url + 'job-search/' + keyword + "-" + city + ".html"; else
            window.location.href = root_url + '?app=career&page=job_search&keyword=' + keyword + "&city=" + city;
    }
    return false;
}
function opent(obj) {
    if ($('#' + obj).hasClass('fb-league-current'))
        $('#' + obj).removeClass('fb-league-current'); else
        $('#' + obj).addClass('fb-league-current');
}
function ajaxs(obj, cate_id, page_no, page) {
    var show = $("#shows").val();
    if (typeof show == 'undefined')
        show = 0;
    $.ajax({
        url: 'ajax.php?path=news&fnc=' + page + '&show=' + show + '&cateid=' + cate_id + '&page_no=' + page_no + '&box=' + obj,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data.code == 1) {
                if (data.html != "")
                    $("#" + obj).html(data.html);
            }
        },
        timeout: function () { }
    });
}
function search_date(cateId, type) {
    if (type == 0) {
        var date = $('.date').val();
        var month = $('.month').val();
        var year = $('.year').val();
    }
    else {
        var date = $('#date').val();
        var month = $('#month').val();
        var year = $('#year').val();
    }
    var date_full = date + '-' + month + '-' + year;
    window.location.href = root_url + 'date/' + cateId + '/' + date_full + '/';
}