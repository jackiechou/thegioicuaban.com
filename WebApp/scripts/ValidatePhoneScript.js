//<asp:TextBox id="txtPhone1" runat="server" ></asp:TextBox>
//<asp:CustomValidator ID="CusValidtxtPhone1" runat="server" ControlToValidate="txtPhone1" 
//ClientValidationFunction="CheckPhone" ErrorMessage="Enter Valid phone1 number" Display="None"></asp:CustomValidator>
//here is the textbox for Phone, below is the javascript funtion to validate the textbox..
//************************************
 function trimString (str){
  str = this != window? this : str;
  return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}
function CheckPhone(source, args)
  {
    var ControlId = source.id;
    var ControlId = ControlId.replace("CusValid","");
    //alert(ControlId);
    var TextObj = document.getElementById(ControlId);
    //var iPhone = document.getElementById("<%= txtPhone1.ClientID %>");
    var flag = check_phonenumber(TextObj);
   
      if (flag==true)
      {   
       args.IsValid = true;       
      }else
      {
      args.IsValid = false;     
      }
  }
 
  function check_phonenumber(phoneBox){
    var phoneVal = trimString(phoneBox.value.toString());
    phoneBox.value=trimString(phoneBox.value.toString());
   
    var valid;
    valid=true;
    var mstr=phoneVal.toString();
    var count=0;
    for (var i = 0;i < mstr.length;i++){
        var oc = mstr.charAt(i);
        if ((oc < "0" ||oc > "9")){
            count++;
        }
    }
   
    if(mstr.length>0){
        var oc = mstr.charAt(0);
        if ((oc < "0" ||oc > "9")&& oc!="+"){
            valid=false;
        }
    }
    for (var i = 1;i < mstr.length;i++){
        var oc = mstr.charAt(i);
        if ((oc < "0" ||oc > "9")&& oc!=" "&& oc!=","&& oc!="-"&& oc!="+" && oc!="(" && oc!=")"){
            valid=false;
        }
    }
    if (valid == false){       
        return(false);
    }
    return(true);
}
//*****************************************