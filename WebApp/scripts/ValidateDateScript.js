//The text input should be in dd-MMM-yyyy Format.
//In Code-behind :  textbox.Attributes.Add("onblur", "ValidateDate('" + textbox.ClientID + "')");

function IsValidDate(myDate) {
    var filter = /^([012]?\d|3[01])-([Jj][Aa][Nn]|[Ff][Ee][bB]|[Mm][Aa][Rr]|[Aa][Pp][Rr]|[Mm][Aa][Yy]|[Jj][Uu][Nn]|[Jj][u]l|[aA][Uu][gG]|[Ss][eE][pP]|[oO][Cc]|[Nn][oO][Vv]|[Dd][Ee][Cc])-(19|20)\d\d$/;
    return filter.test(myDate);
}

function ValidateDate(txt) {
    var txtTest = document.getElementById(txt);
    var isValid = IsValidDate(txtTest.value);
    if (isValid) {
        var currentDate = new Date();
        var txtValue = txtTest.value;
        var date = new Array();
        date = txtValue.split('-');
        txtValue = date[1] + ' ' + date[0] + ',' + date[2];
        txtValue = new Date(txtValue);
        txtTest.value = txtValue.format("dd-MMM-yyyy");
        if (txtValue <= currentDate) {
            alert('Past Date.');
        }
        else {
            txtTest.focus();
            alert('Future or Equal to Today Date.');
        }
    }
    else {
        if (txtTest.value == '') {
            alert('Please Enter Date !');
        } else {
            txtTest.value = '';
            txtTest.focus();
            alert('Incorrect Date format! Date format should be (dd-MMM-yyyy i.e 02-Jan-2012)');
        }
    }
    return isValid
}

function workingDaysBetweenDates(startDate, endDate) {

    // Validate input
    if (endDate < startDate)
        return 0;

    // Calculate days between dates
    var millisecondsPerDay = 86400 * 1000; // Day in milliseconds
    startDate.setHours(0, 0, 0, 1);  // Start just after midnight
    endDate.setHours(23, 59, 59, 999);  // End just before midnight
    var diff = endDate - startDate;  // Milliseconds between datetime objects    
    var days = Math.ceil(diff / millisecondsPerDay);

    // Subtract two weekend days for every week in between
    var weeks = Math.floor(days / 7);
    var days = days - (weeks * 2);

    // Handle special cases
    var startDay = startDate.getDay();
    var endDay = endDate.getDay();

    // Remove weekend not previously removed.   
    if (startDay - endDay > 1)
        days = days - 2;

    // Remove start day if span starts on Sunday but ends before Saturday
    if (startDay == 0 && endDay != 6)
        days = days - 1

    // Remove end day if span ends on Saturday but starts after Sunday
    if (endDay == 6 && startDay != 0)
        days = days - 1

    return days;
}


function GetMaxDay(month, year) {
    if (month == 2 && ((year % 400 == 0) || (year % 100 != 0 && year % 4 == 0))) return 29;
    if (month == 1 || month == 3 || month == 7 || month == 5 || month == 8 || month == 10 || month == 12) return 31;
    if (month == 4 || month == 6 || month == 9 || month == 11) return 30;
    return 28;
}


function IsValidDate(objTxtDay, objTxtMonth, objTxtYear, strDateLabel) {
    var valid = true;
    if (objTxtDay.value.length == 0 || isNaN(objTxtDay.value) || objTxtDay.value <= 0) {
        valid = false;
        alert(strDateLabel + " không hợp lệ." + " (Ngày phải là con số > 0)");
        objTxtDay.focus();
    }
    else if (objTxtMonth.value.length == 0 || isNaN(objTxtMonth.value) || objTxtMonth.value <= 0 || objTxtMonth.value > 12) {
        valid = false;
        alert(strDateLabel + " không hợp lệ." + " (Tháng phải là con số từ 1 -> 12)");
        objTxtMonth.focus();
    }
    else if (objTxtYear.value.length == 0 || isNaN(objTxtYear.value) || objTxtYear.value <= 0) {
        valid = false;
        alert(strDateLabel + " không hợp lệ." + " (Năm phải là con số > 0)");
        objTxtYear.focus();
    }
    else if (objTxtDay.value > GetMaxDay(objTxtMonth.value, objTxtYear.value)) {
        valid = false;
        alert(strDateLabel + " không hợp lệ." + " (Ngày phải là con số từ 1 -> " + GetMaxDay(objTxtMonth.value, objTxtYear.value) + ")");
        objTxtDay.focus();
    }

    return valid;
}

//1 : date1 greater than date2
//-1: ----- less than -------
//0 : ----- equal -------
function CompareDate(date1, date2) {
    var value = 0;
    var arr1 = date1.split("-");
    var arr2 = date2.split("-");
    value = CompareNumber(arr1[2], arr2[2]);
    if (value != 0)
        return value;
    value = CompareNumber(arr1[1], arr2[1]);
    if (value != 0)
        return value;
    value = CompareNumber(arr1[0], arr2[0]);
    return value;
}

//=========================================
function CheckDate(element, content) {
    var dateNow = new Date();
    if (content < dateNow) {
        alert("vui lòng nhập ngày lớn hơn ngày hiện tại");
        document.getElementById(element).value = "";
    }
}
function CheckDate1(firstelement, element, content) {
    var dateNow = new Date();
    var datestart = document.getElementById(firstelement).value;
    if (content < dateNow || content < firstelement) {
        alert("vui lòng nhập ngày lớn hơn ngày khởi hành");
        document.getElementById(element).value = "";
    }
}
function copyValue(element, content) {
    document.getElementById(element).innerHtml = content;
}

function parseDate(str) {
    var myDate = str.split('/')
    var s1 = new Date(myDate)
    var day = myDate[0];
    var month = myDate[1];
    var year = myDate[2];
    return new Date(year, month, day);
}

function caculateDayDiff(first, second) {
    return (second - first) / (1000 * 60 * 60 * 24)
}

function isDate(source, args) {
    var current_date = new Date();
    var minDate = 1900;
    var maxDate = parseInt(current_date.getFullYear()) - 18;
    var dteDate;
    var dteDate2;
    var Months = new Array('JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC');


    var obj1 = document.getElementById(source.controltovalidate).value.split("/");
    var obj2 = new Array();
    obj2[0] = parseInt(obj1[0], 10);
    obj2[1] = parseInt(obj1[1], 10) - 1;
    obj2[2] = parseInt(obj1[2], 10);

    dteDate = new Date(obj2[2], obj2[1], obj2[0]);

    var indexOfM = -1;
    for (var i = 0; i < Months.length; i++) {
        if (Months[i] == obj1[1].toUpperCase()) {
            indexOfM = i;
        }
    }

    dteDate2 = new Date(obj2[2], indexOfM, obj2[0]);
    if (
          (
               (obj2[0] == dteDate.getDate()) &&
               (obj2[1] == dteDate.getMonth()) &&
               (obj2[2] == dteDate.getFullYear()) &&
               (dteDate.getFullYear() > minDate) &&
               (dteDate.getFullYear() < maxDate)
           ) ||
          (
               (obj1[0] == dteDate2.getDate()) &&
               (indexOfM == dteDate2.getMonth()) &&
               (obj1[2] == dteDate2.getFullYear()) &&
               (dteDate2.getFullYear() > minDate) &&
               (dteDate2.getFullYear() < maxDate)
           )

       )
        args.IsValid = true;
    else
        args.IsValid = false;

}