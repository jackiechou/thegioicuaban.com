function LoaiBoTiengViet(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");

    //str= str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_/g,"-");
    // tìm và thay thế các kí tự đặc biệt trong chuỗi sang kí tự - 
    //str= str.replace(/-+-/g,"-"); //thay thế 2- thành 1- 
    //str= str.replace(/^\-+|\-+$/g,"");  //cắt bỏ ký tự - ở đầu và cuối chuỗi    */

    str = str.replace(/[\s]+/g, " "); //Loai bo khoang trang thua
    str = str.replace(/^\s+|\s+$/g, "");  // Loai khoang trang dau va cuoi
    return str;
}

function IsEmail(strEmail) {
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$/;
    var valid = emailPattern.test(strEmail); //!(strEmail.indexOf("@") == -1 || strEmail.lastIndexOf(".") <= strEmail.indexOf("@"));
    return valid;
}

function IsUserName(strUserName) {
    var userNamePattern = /^[a-zA-Z0-9.-]+$/; ///^([a-zA-Z0-9._-]|[\s])+$/;
    return (userNamePattern.test(strUserName));
}

function trim(stringToTrim) {
    return stringToTrim.replace(/^\s+|\s+$/g, "");
}
//function trim(str) {
//    str = str.replace(/^\s+/, '');
//    for (var i = str.length; i >= 0; i--)
//        if (/\S/.test(str.charAt(i)))
//            return str.substring(0, ++i)
//    return str
//};


//function trim(str, chars) {
//    return ltrim(rtrim(str, chars), chars);
//}
//function ltrim(str, chars) {
//    chars = chars || "\\s";
//    return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
//}
//function rtrim(str, chars) {
//    chars = chars || "\\s";
//    return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
//}
function checkSearch() {
    var q = $("#q").val();
    q = trim(q, ' ');
    q = q.replace(/\s/g, '_');
    q = q.replace('/', '-');
    if (q == "Tìm kiếm ...") alert("Bạn chưa nhập từ khóa");
    window.location.href = root_url + 'search/' + q + '/';
    return false;
} 




function getKeywordById(id_textbox) {
    var keyword = $("#" + id_textbox).val();
    keyword = keyword.replace(/([\?*#<>!\$%^&\(\)\/\\]+)/g, "");
    keyword = keyword.replace(/([ ]+)/g, " ");
    keyword = keyword.replace(/\"/g, '');
    return keyword;
}
function searchFromOtherKey(e) {
    var key;
    if (window.event) {
        key = window.event.keyCode;
    } else {
        key = e.which;
    }
    if (key == 13) {
        searchFromOther();
    }
}