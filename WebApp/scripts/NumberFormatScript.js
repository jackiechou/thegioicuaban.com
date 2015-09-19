function IsNumeric(val) {
    var flag = false;
    if (isNaN(parseFloat(val)))
        flag = false;
    else
        flag = true;
    return flag
}