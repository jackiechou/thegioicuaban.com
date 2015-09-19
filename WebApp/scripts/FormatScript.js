function FormatNumber(obj) {
    var strvalue;
    if (eval(obj))
        strvalue = eval(obj).value;
    else
        strvalue = obj;
    var num;
    num = strvalue.toString().replace(/\$|\,/g, '');

    if (isNaN(num))
        num = "";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    num = Math.floor(num / 100).toString();
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' +
            num.substring(num.length - (4 * i + 3));
    //return (((sign)?'':'-') + num);
    eval(obj).value = (((sign) ? '' : '-') + num);
}

function GetFormatNumber(strvalue) {
    var num;
    num = strvalue.toString().replace(/\$|\,/g, '');

    if (isNaN(num)) {
        num = "";
    }
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    num = Math.floor(num / 100).toString();
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++) {
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
    }
    return (((sign) ? '' : '-') + num);
}


var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
function DocSo3ChuSo(baso) {
    var tram;
    var chuc;
    var donvi;
    var KetQua = "";
    tram = parseInt(baso / 100);
    chuc = parseInt((baso % 100) / 10);
    donvi = baso % 10;
    if (tram == 0 && chuc == 0 && donvi == 0) return "";
    if (tram != 0) {
        KetQua += ChuSo[tram] + " trăm ";
        if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
    }
    if ((chuc != 0) && (chuc != 1)) {
        KetQua += ChuSo[chuc] + " mươi";
        if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
    }
    if (chuc == 1) KetQua += " mười ";
    switch (donvi) {
        case 1:
            if ((chuc != 0) && (chuc != 1)) {
                KetQua += " mốt ";
            }
            else {
                KetQua += ChuSo[donvi];
            }
            break;
        case 5:
            if (chuc == 0) {
                KetQua += ChuSo[donvi];
            }
            else {
                KetQua += " lăm ";
            }
            break;
        default:
            if (donvi != 0) {
                KetQua += ChuSo[donvi];
            }
            break;
    }
    return KetQua;
}

function DocTienBangChu(SoTien) {
    var lan = 0;
    var i = 0;
    var so = 0;
    var KetQua = "";
    var tmp = "";
    var ViTri = new Array();
    if (SoTien < 0) return "Số tiền âm !";
    if (SoTien == 0) return "Không đồng !";
    if (SoTien > 0) {
        so = SoTien;
    }
    else {
        so = -SoTien;
    }
    if (SoTien > 8999999999999999) {
        //SoTien = 0;
        return "Số quá lớn!";
    }

    ViTri[5] = Math.floor(so / 1000000000000000);
    if (isNaN(ViTri[5]))
        ViTri[5] = "0";
    so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
    ViTri[4] = Math.floor(so / 1000000000000);
    if (isNaN(ViTri[4]))
        ViTri[4] = "0";
    so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
    ViTri[3] = Math.floor(so / 1000000000);
    if (isNaN(ViTri[3]))
        ViTri[3] = "0";
    so = so - parseFloat(ViTri[3].toString()) * 1000000000;
    ViTri[2] = parseInt(so / 1000000);
    if (isNaN(ViTri[2]))
        ViTri[2] = "0";
    ViTri[1] = parseInt((so % 1000000) / 1000);
    if (isNaN(ViTri[1]))
        ViTri[1] = "0";
    ViTri[0] = parseInt(so % 1000);
    if (isNaN(ViTri[0]))
        ViTri[0] = "0";
    if (ViTri[5] > 0) {
        lan = 5;
    }
    else if (ViTri[4] > 0) {
        lan = 4;
    }
    else if (ViTri[3] > 0) {
        lan = 3;
    }
    else if (ViTri[2] > 0) {
        lan = 2;
    }
    else if (ViTri[1] > 0) {
        lan = 1;
    }
    else {
        lan = 0;
    }
    //        
    for (i = lan; i >= 0; i--) {
        tmp = DocSo3ChuSo(ViTri[i]);
        KetQua += tmp;
        if (ViTri[i] > 0) KetQua += Tien[i];
        if ((i > 0) && (tmp.length > 0)) KetQua += ','; //&& (!string.IsNullOrEmpty(tmp))
    }
    if (KetQua.substring(KetQua.length - 1) == ',') {
        KetQua = KetQua.substring(0, KetQua.length - 1);
    }

    KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);

    return KetQua; //.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
}

function formatCurrency(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    num = Math.floor(num / 100).toString();
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' +
    num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '-') + num);
}

function CommaFormatted(amount) {
    var delimiter = ","; // replace comma if desired
    var a = amount.split('.', 2)
    var d = a[1];
    var i = parseInt(a[0]);
    if (isNaN(i)) { return ''; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    var n = new String(i);
    var a = [];
    while (n.length > 3) {
        var nn = n.substr(n.length - 3);
        a.unshift(nn);
        n = n.substr(0, n.length - 3);
    }
    if (n.length > 0) { a.unshift(n); }
    n = a.join(delimiter);
    if (d.length < 1) { amount = n; }
    else { amount = n + '.' + d; }
    amount = minus + amount;
    return amount;
}


function CurrencyFormatted(amount) {
    var i = parseFloat(amount);
    if (isNaN(i)) { i = 0.00; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    i = parseInt((i + .005) * 100);
    i = i / 100;
    s = new String(i);
    if (s.indexOf('.') < 0) { s += '.00'; }
    if (s.indexOf('.') == (s.length - 2)) { s += '0'; }
    s = minus + s;
    return s;
}

function CheckDecimal(inputtxt) {
    var decimal = /^[0-9]+(\.[0-9]+)+$/
    if (inputtxt.value.match(decimal)) {
        alert('The number has a decimal part...')
        return true;
    }
    else {
        alert('The number has no decimal part...')
        return false;
    }
}

function addSeparatorsNF(nStr, inD, outD, sep) {
    nStr += '';
    var dpos = nStr.indexOf(inD);
    var nStrEnd = '';
    if (dpos != -1) {
        nStrEnd = outD + nStr.substring(dpos + 1, nStr.length);
        nStr = nStr.substring(0, dpos);
    }
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(nStr)) {
        nStr = nStr.replace(rgx, '$1' + sep + '$2');
    }
    return nStr + nStrEnd;
}

function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function formatNumberWithSeparator(n, separator) {
    separator = separator || ",";

    n = n.toString()
        .split("").reverse().join("")
        .replace(/(\d{3})/g, "$1" + separator)
        .split("").reverse().join("");

    // Strings that have a length that is a multiple of 3 will have a leading separator
    return n[0] == separator ? n.substr(1) : n;
}

function ValidateNumber(obj) {
    if (obj.length != 0) {
        var text = /^[-0-9]+$/;

        if ((document.getElementById(obj.id).value != "") && (!text.test(document.getElementById(obj.id).value))) {
            alert("Please enter numeric values only");
            obj.focus();
            obj.select();
        }
    }
}

//Validate the Text Box value is a Numeric Or Not then Round the Number into 2 Decimal Places Using Ja
function ValidateNumericAndRound2Decimal(obj) {
    if (isNumeric(obj.value) == false) {
        varNum = Math.round(obj.value * 100) / 100;
        strDecPt = varNum + "";
        if (strDecPt.indexOf(".") == -1) { strDecPt += ".00" }
        if (strDecPt.indexOf(".") == strDecPt.length - 2) { strDecPt += "0" }
        obj.value = strDecPt;
    }
    else {
        obj.value = "0.00"
    }
}

function isNumeric(strNum) {
    var isNumber = true;
    var VarDec = 0;
    var validChars = "0123456789.-";
    var thisChar;
    for (i = 0; i < strNum.length && isNumber == true; i++) {
        thisChar = strNum.charAt(i);
        if (thisChar == "-" && i > 0) isNumber = false;
        if (thisChar == ".") {
            VarDec = VarDec + 1;
            if ((i == 0 || i == strNum.length - 1) && strNum.length == 1) isNumber = false;
            if (VarDec > 1) isNumber = false;
        }
        if (validChars.indexOf(thisChar) == -1) isNumber = false;
    }
    return isNumber;
}

function isInteger(s) {
    return (s.toString().search(/^-?[0-9]+$/) == 0);
}

function isUnsignedInteger(s) {
    return (s.toString().search(/^[0-9]+$/) == 0);
}

function NumericValidation(page)
{
    //Validation for Numbers, Period and Percentage using javascript.
    var keyvalue=0;
    if(navigator.appName=='Microsoft Internet Explorer')
    {
        keyvalue=event.keyCode;
    }
    else
    {
        keyvalue=page.which;
    }
    if((keyvalue >= 48 && keyvalue <= 57 )||(keyvalue == 46 )||(keyvalue == 37 )|| (keyvalue == 13))
    {
    return true;
    }
    else
    {
        alert("Only Numbers, Period and Percentage Symbols are allowed");
        return false;
    }
    //event of text box. like: onKeyPress="return NumericValidation(event);"
}


function ValidateString(objInput) {
    var regStringExp = /^[A-z,0-9]*$/;
    if (objInput.value.search(regStringExp) == -1) {
        alert('The String Input Should Contain Characters and Numbers...')
        objInput.focus();
        return false;
    } 
}

function ValidateMobile(objInput) {
    //Validation for Mobile Number=> var regNumericExp = /\d{10}/;
//    var regNumericExp = "^[0-9]+$";
//    if (objInput.value.search(regNumericExp) == -1) {
//        alert('The Numeric Input Should Contain Numbers...')
//        objInput.focus();
//        return false;
//    }
    
    if (objInput == "") {
        alert('Please Enter Required fields')
        return false;
    }else if (objInput.length < 10) {
        alert('Mobile Number must be 10 digits');
        return false;
    }
    else {
        if (objInput.search(Numeric) == -1) {
            alert('Mobile Number must be Numeric');
            objInput.focus();
            return false;
        } 
    }
}

function validateEmail(email) {
    var emailpat = /^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/;
    var matchArray = email.match(emailpat);
    if (matchArray == null) {
        return false;
    }
    return true;
}

//function checkEmail (strEmail) 
//{
//    var checkTLD=1;
//    var knownDomsPat=/^(com|net|org|edu|int|mil|gov|arpa|biz|aero|name|coop|info|pro|museum)$/;
//    var emailPat=/^(.+)@(.+)$/;
//    var specialChars="\\(\\)><@,;:\\\\\\\"\\.\\[\\]";
//    var validChars="\[^\\s" + specialChars + "\]";
//    var quotedUser="(\"[^\"]*\")";
//    var ipDomainPat=/^\[(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})\]$/;
//    var atom=validChars + '+';
//    var word="(" + atom + "|" + quotedUser + ")";
//    var userPat=new RegExp("^" + word + "(\\." + word + ")*$");
//    var domainPat=new RegExp("^" + atom + "(\\." + atom +")*$");
//    var matchArray=strEmail.match(emailPat);
//    var user=matchArray[1];
//    var domain=matchArray[2];

//    if (user.match(validChars) == null) {
//        alert("Ths username contains invalid characters.");
//        return false;    
//    }

//    if (domain.match(validChars) == null) {
//        alert("Ths domain name contains invalid characters.");
//        return false;    
//    }

//    if (user.match(userPat)==null) {
//        alert("The username doesn't seem to be valid.");
//        return false;
//    }

//    var atomPatern=new RegExp("^" + atom + "$");
//    var domArray=domain.split(".");
//    var slen=domArray.length;

//    for(i=0;i<0;i++){
//        if (domArray[i].search(atomPatern)==-1) {
//        alert("The domain name does not seem to be valid.");
//        return false;
//        }
//    }

//    if (checkTLD && domArray[domArray.length-1].length!=2 && 
//        domArray[domArray.length-1].search(knownDomsPat)==-1) {
//        alert("The address must end in a well-known domain or two letter " + "country.");
//        return false;
//    }

//    if (slen<2) {
//        alert("This address is missing a hostname!");
//        return false;
//    }
//    return true;
//}

//function checkDomain(name)
//{
//    var array = new Array(
//    '.com','.net','.org','.biz','.coop','.info','.museum','.name',
//    '.pro','.edu','.gov','.int','.mil','.ac','.ad','.ae','.af','.ag',
//    '.ai','.al','.am','.an','.ao','.aq','.ar','.as','.at','.au','.aw',
//    '.az','.ba','.bb','.bd','.be','.bf','.bg','.bh','.bi','.bj','.bm',
//    '.bn','.bo','.br','.bs','.bt','.bv','.bw','.by','.bz','.ca','.cc',
//    '.cd','.cf','.cg','.ch','.ci','.ck','.cl','.cm','.cn','.co','.cr',
//    '.cu','.cv','.cx','.cy','.cz','.de','.dj','.dk','.dm','.do','.dz',
//    '.ec','.ee','.eg','.eh','.er','.es','.et','.fi','.fj','.fk','.fm',
//    '.fo','.fr','.ga','.gd','.ge','.gf','.gg','.gh','.gi','.gl','.gm',
//    '.gn','.gp','.gq','.gr','.gs','.gt','.gu','.gv','.gy','.hk','.hm',
//    '.hn','.hr','.ht','.hu','.id','.ie','.il','.im','.in','.io','.iq',
//    '.ir','.is','.it','.je','.jm','.jo','.jp','.ke','.kg','.kh','.ki',
//    '.km','.kn','.kp','.kr','.kw','.ky','.kz','.la','.lb','.lc','.li',
//    '.lk','.lr','.ls','.lt','.lu','.lv','.ly','.ma','.mc','.md','.mg',
//    '.mh','.mk','.ml','.mm','.mn','.mo','.mp','.mq','.mr','.ms','.mt',
//    '.mu','.mv','.mw','.mx','.my','.mz','.na','.nc','.ne','.nf','.ng',
//    '.ni','.nl','.no','.np','.nr','.nu','.nz','.om','.pa','.pe','.pf',
//    '.pg','.ph','.pk','.pl','.pm','.pn','.pr','.ps','.pt','.pw','.py',
//    '.qa','.re','.ro','.rw','.ru','.sa','.sb','.sc','.sd','.se','.sg',
//    '.sh','.si','.sj','.sk','.sl','.sm','.sn','.so','.sr','.st','.sv',
//    '.sy','.sz','.tc','.td','.tf','.tg','.th','.tj','.tk','.tm','.tn',
//    '.to','.tp','.tr','.tt','.tv','.tw','.tz','.ua','.ug','.uk','.um',
//    '.us','.uy','.uz','.va','.vc','.ve','.vg','.vi','.vn','.vu','.ws',
//    '.wf','.ye','.yt','.yu','.za','.zm','.zw');

//    var name = name;
//    var val = true;

//    var dot = name.lastIndexOf(".");
//    var dname = name.substring(0,dot);
//    var ext = name.substring(dot,name.length);
//    //alert(ext);

//    if(dot>2 && dot<57)
//    {
//        for(var i=0; i<=47;i++)
//        {
//            if(j==0 || j==dname.length-1)
//            {
//                alert("Domain name should not begin or end with '-'");
//                return false;
//            }            
//            else
//            {
//                alert("Your domain name should not have special characters");
//                return false;
//            }
//        }    
//    }
//    else
//    {
//        alert("Your Domain name is too short/long");
//        return false;
//    }
//    return true;
//}

function isDigit(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode

    if (charCode > 47 && charCode < 58) {
        return true;
    }
    else {
        return false;
    }
}


function CheckPositiveNumber(e) {
    var charCode = (e.which) ? e.which : event.keyCode

    if (charCode > 31 && (charCode < 46 || charCode > 57)) {
        return false;
    }
    return true;
}



function valIntegerwithMinus(obj)
{
    //Validate integer with special character '-' in Javascript
    var i, strVal, blnChange; blnChange = false; strVal = ""; 

    for(i=0;i<(obj.value).length;i++)
    {
        switch(obj.value.charAt(i))
        {
            case "0":

            case "1":

            case "2":

            case "3":

            case "4":

            case "5":

            case "6":

            case "7":

            case "8":

            case "-":

            case "9":
                strVal=strVal+obj.value.charAt(i); break; default :blnChange = true; break;
        }
    }
    if (blnChange)
    {
        obj.value=strVal;
    }

}

function valIntegerPostalCode(obj) {

    var i, strVal, blnChange;

    blnChange = false

    strVal = "";

    for (i = 0; i < (obj.value).length; i++) {

        switch (obj.value.charAt(i)) {

            case "a":

            case "b":

            case "c":

            case "d":

            case "e":

            case "f":

            case "g":

            case "h":

            case "i":

            case "j":

            case "k":

            case "l":

            case "m":

            case "n":

            case "o":

            case "p":

            case "q":

            case "r":

            case "s":

            case "t":

            case "u":

            case "v":

            case "w":

            case "x":

            case "y":

            case "z":

            case "A":

            case "B":

            case "C":

            case "D":

            case "E":

            case "F":

            case "G":

            case "H":

            case "I":

            case "J":

            case "K":

            case "L":

            case "M":

            case "N":

            case "O":

            case "P":

            case "Q":

            case "R":

            case "S":

            case "T":

            case "U":

            case "V":

            case "W":

            case "X":

            case "Y":

            case "Z":

            case " ":

            case "0":

            case "1":

            case "2":

            case "3":

            case "4":

            case "5":

            case "6":

            case "7":

            case "8":

            case "-":

            case "9":

                strVal = strVal + obj.value.charAt(i);

                break;

            default:

                blnChange = true;

                break;

        }

    }

    if (blnChange) {

        obj.value = strVal;

    }

}

function GetNumberPageInBrowserHistory() {
    //Get the Number of elements in the Current Browsers History
    var vHist = history.length;
    document.write("The number of pages visited in this window is" + vHist + " pages.");
}

function ValidatePassswordWithoutSpaces(txtNewpassword) {
    var regExp = /^[A-z,0-9,@,_,*,&,#,^,$,.]*$/;
    //document.getElementById("<%=txtNewpassword.ClientID %>
    if (txtNewpassword.value.search(regExp) == -1) {
        alert("Please Enter New Password and Spaces are Not Allowed");
        txtNewpassword.focus();
        return false;
    }
}

function trim(str) {
    return str.replace(/^\s+|\s+$/g, '');
}

function CompareNumber(num1, num2) {
    num1 = parseFloat(num1);
    num2 = parseFloat(num2);
    if (num1 > num2)
        return 1;
    if (num1 < num2)
        return -1;
    return 0;
}

function PriceFormat(Price) {
    var result = "";
    Price = Price + "";

    var StartIndex = Price.indexOf(".");
    if (StartIndex != -1) {
        result = "," + Price.substring(StartIndex + 1, Price.length);
    }
    else {
        StartIndex = Price.length;
    }
    for (var i = StartIndex - 1, Count = 0; i >= 0; i--, Count++) {
        result = Price.charAt(i) + ((Count % 3 == 0 && Count != 0) ? "." : "") + result;
    }
    return result;
}
String.prototype.Trim = function () {
    return this.replace(/^\s+|\s+$/g, "");
}
String.prototype.LTrim = function () {
    return this.replace(/^\s+/, "");
}
String.prototype.RTrim = function () {
    return this.replace(/\s+$/, "");
}