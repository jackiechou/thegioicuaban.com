function dateFromStringWithTime(str) {
    var match;
    if (!(match = str.match(/\d+/))) {
        return false;
    }
    var date = new Date();
    date.setTime(match[0] - 0);
    return date;
}

function getTimeNowToString() {
    var newDate = new Date();
    var day = newDate.getDay();
    var result = formatDateGo(newDate.getHours()) + ':' + formatDateGo(newDate.getMinutes());
    //result += ' GMT ';

    result += ' - ';
    if (day == 0)
        result += 'Chủ nhật ';
    else
        result += 'Thứ ' + (day + 1);
    result += ' ngày ' + formatDateGo(newDate.getDate()) + " tháng " +
            formatDateGo(newDate.getMonth() + 1) + " năm " + newDate.getUTCFullYear();
    return result;
}

var formatDateGo = function (d) {
    if (d < 10)
        d = "0" + d;
    return d;
}

/// note: sẽ có 2 trường hợp a giá trị chuyền vào.
/// 1: là 1 chuỗi
/// 2: là 1 object Date -ví dụ: new Date()
/// vì vậy khi mà chuyền vào 1 đối tượng Date thì thêm 1 param initDate và chuyền vào là true.
function formatDateTime(a, initDate) {
    var e = [[11, 'sáng'], [14, 'trưa'], [19, 'chiều']];
    var f = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
    var g = new Date(); //new Date();
    var j = null;

    if (initDate) {
        j = a;
    }
    else {
        var _match = a.match(/^\/Date\((\d+)([-+]\d\d)(\d\d)\)\/$/);
        if (_match) {
            j = new Date(1 * _match[1] + 3600000 * (_match[2] - 7) + 60000 * _match[3]);
        } else {
            j = new Date(a);
        }
    }

    var d = Math.floor(g.getTime() / 1000) - Math.floor(j / 1000);
    if (d < 0) {
        return j.getHours() + ':' + formatValue(j.getMinutes()) + ' ' + formatValue(j.getDate()) + '/' + formatValue(j.getMonth() + 1) + '/' + j.getFullYear();
    }
    if (d < 60) {
        return (d == 0 ? 'vài' : d) + ' giây trước';
    }
    if (d < 3600) return Math.floor(d / 60) + ' phút trước';
    if (d < 43200) return Math.floor(d / 3600) + ' tiếng trước';
    var h = j.getHours();
    var m = formatValue(j.getMinutes());
    if (d < 518400) {
        var b = 'tối';
        for (i = 0; i < 3; i++) if (h < e[i][0]) {
            b = e[i][1];
            break
        }
        d = (g.getDay() + 7 - j.getDay()) % 7;
        var k = '';
        if (d == 0)
            k = 'hôm nay';
        /*else if (d == 1)
        k = 'hôm qua';*/
        else
            return h + ':' + m + ' ' + formatValue(j.getDate()) + '/' +
                                    formatValue(j.getMonth() + 1) + '/' + j.getFullYear(); //f[j.getDay()];
        return (h % 12).toString() + ':' + m + ' ' + b + ' ' + k
    }
    h = formatValue((h));
    return h + ':' + m + ' ' + formatValue(j.getDate()) + '/' +
                                    formatValue(j.getMonth() + 1) + '/' + j.getFullYear();
}
function formatValue(value) {
    return value < 10 ? ('0' + value) : value;
}

function formatJSONDate(jsonDate) {
    var newDate = dateFormat(jsonDate, "hh:mm dd/mm/yyyy");
    return newDate;
}
function DateDeserialize(dateStr) {
    return formatJSONDate(eval('new' + dateStr.replace(/\//g, ' ')));
}
function Substringtext(_title, _numberkeyword) {
    if (_title.length < _numberkeyword)
        return _title;
    else
        return _title.substring(0, _numberkeyword) + '...';
}
