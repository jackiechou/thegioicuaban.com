function checkDateNumber(source, args) {
    var minDate = 1900;
    var maxDate = 2100;
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


function chechDateWithMonthText(source, args) {
    var minDate = 1900;
    var maxDate = 2100;
    var dteDate;
    var dteDate2;
    var Months = new Array('JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC');
    var MonthsFull = new Array('JANUARY', 'FEBRUARY', 'MARCH', 'APRIL', 'MAY', 'JUNE', 'JULY', 'AUGUST', 'SEPTEMBER', 'OCTOBER', 'NOVEMBER', 'DECEMBER');


    var enteredDate = document.getElementById(source.controltovalidate).value + '';
    var obj1;
    if (enteredDate.indexOf("-", 0) >= 0) {
        obj1 = enteredDate.split("-");
    }
    else if (enteredDate.indexOf("/", 0) >= 0) {
        obj1 = enteredDate.split("/");
    }


    if (obj1.length != 3) {
        args.IsValid = false;
        return;
    }
    var obj2 = new Array();
    obj2[0] = parseInt(obj1[0], 10);


    var indexOfM = -1;
    if (isNaN(parseInt(obj1[1], 10)) == false) {
        obj2[1] = parseInt(obj1[1], 10);
        indexOfM = parseInt(obj1[1], 10);
    }
    else {
        obj2[1] = obj1[1];
        for (var i = 0; i < Months.length; i++) {
            if (Months[i] == obj1[1].toUpperCase()) {
                indexOfM = i + 1;
                break;
            }
        }
        if (indexOfM < 0) {
            for (var i = 0; i < MonthsFull.length; i++) {
                if (MonthsFull[i] == obj1[1].toUpperCase()) {
                    indexOfM = i + 1;
                    break;
                }
            }
        }
    }
    obj2[2] = parseInt(obj1[2], 10);


    if (indexOfM < 0) {
        args.IsValid = false;
        return;
    }
    dteDate3 = Date.parse(obj2[0] + '/' + indexOfM.toString() + '/' + obj2[2]);
    dteDate = new Date(obj2[2], indexOfM - 1, obj2[0]);
    if (isNaN(dteDate) == true) {
        args.IsValid = false;
        return;
    }


    if (obj2[0] == dteDate.getDate() &&
    indexOfM - 1 == dteDate.getMonth() &&
     (dteDate.getFullYear() > minDate && dteDate.getFullYear() < maxDate))
        args.IsValid = true;
    else
        args.IsValid = false;
}
////<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
////<asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="CustomValidator"  ClientValidationFunction="checkDateNumber" ControlToValidate="TextBox1"></asp:CustomValidator>

function isDate(txtDate, separator) {
    var aoDate,           // needed for creating array and object
        ms,               // date in milliseconds
        month, day, year; // (integer) month, day and year
    // if separator is not defined then set '/'
    if (separator === undefined) {
        separator = '/';
    }
    // split input date to month, day and year
    aoDate = txtDate.split(separator);
    // array length should be exactly 3 (no more no less)
    if (aoDate.length !== 3) {
        return false;
    }
    // define month, day and year from array (expected format is m/d/yyyy)
    // subtraction will cast variables to integer implicitly
    month = aoDate[0] - 1; // because months in JS start from 0
    day = aoDate[1] - 0;
    year = aoDate[2] - 0;
    // test year range
    if (year < 1000 || year > 3000) {
        return false;
    }
    // convert input date to milliseconds
    ms = (new Date(year, month, day)).getTime();
    // initialize Date() object from milliseconds (reuse aoDate variable)
    aoDate = new Date();
    aoDate.setTime(ms);
    // compare input date and parts from Date() object
    // if difference exists then input date is not valid
    if (aoDate.getFullYear() !== year ||
        aoDate.getMonth() !== month ||
        aoDate.getDate() !== day) {
        return false;
    }
    // date is OK, return true
    return true;
}

//function checkDate() {
//    // define date string to test
//    var txtDate = document.getElementById('txtDate').value;
//    // check date and print message
//    if (isDate(txtDate)) {
//        alert('OK');
//    }
//    else {
//        alert('Invalid date format!');
//    }
//}

//Function Load DatTime
function date_time(obj) {
    date = new Date;
    year = date.getFullYear();
    month = date.getMonth();
    //months = new Array('January', 'February', 'March', 'April', 'May', 'June', 'Jully', 'August', 'September', 'October', 'November', 'December');
    months = new Array('01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12');
    d = date.getDate();
    if (d < 10) { d = "0" + d }
    day = date.getDay();
    //days = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
    days = new Array('Chủ nhật', 'Thứ hai', 'Thứ ba', 'Thứ tư', 'Thứ năm', 'Thứ sáu', 'Thứ bảy');
    h = date.getHours();
    if (h < 10) {
        h = "0" + h;
    }
    m = date.getMinutes();
    if (m < 10) {
        m = "0" + m;
    }
    s = date.getSeconds();
    if (s < 10) {
        s = "0" + s;
    }
    //result = '' + days[day] + ' ' + months[month] + ' ' + d + ' ' + year + ' ' + h + ':' + m + ':' + s;
    result = '' + days[day] + ' ' + d + '/' + months[month] + '/' + year + ' ' + h + ':' + m + ':' + s;
    document.getElementById(obj).innerHTML = result;
    setTimeout('date_time("' + obj + '");', '1000');
    return true;
}