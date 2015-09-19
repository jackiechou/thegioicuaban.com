<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="WebApp.login" EnableSessionState="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LOGIN</title>
    <script type="text/javascript" src="scripts/js/login_middle.js"></script>
     <script type="text/javascript" src="scripts/jquery/jquery.min.js"></script>    
     <script type="text/javascript" src="scripts/jquery/jquery.tools.min.js"></script>
     <script type="text/javascript" src="scripts/plugins/uniform/jquery.uniform.min.js"></script>
    <link rel="stylesheet"  type="text/css" href="templates/admin_templates/default_temp/styles/login.css" media="screen" />
    <link rel="stylesheet" href="scripts/plugins/uniform/css/uniform.default.css" type="text/css" media="screen" charset="utf-8" />
    <script type="text/javascript">
        function doClick(buttonName, e) {
            //the purpose of this function is to allow the enter key to 
            //point to the correct button to click.
            var key;

            if (window.event)
                key = window.event.keyCode;     //IE
            else
                key = e.which;     //firefox

            if (key == 13) {
                //Get the button the user wants to have clicked
                var btn = document.getElementById(buttonName);
                if (btn != null) { //If we find the button click it
                    btn.click();
                    event.keyCode = 0
                }
            }
        }

        //scripts for login page to run properly
        $(function () {
            //tab funciton
            $("ul.tabs").tabs("div.panes > div");

            // Closable function
            $('.closable').append('<span class="closelink" title="Close"></span>');

            $('.closelink').click(function () {
                $(this).parent().fadeOut('600', function () { $(this).remove(); });
            });

            //$("select, input, button, textarea").uniform();
            $(":checkbox").uniform({ checkboxClass: 'myCheckClass' });


            //SEND EMAIL ==============================================================================================
            function isValidEmailAddress(emailAddress) {
                var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
                return pattern.test(emailAddress);
            }
            $("#btnSend").click(function () {
                var Email = $("#txtEmail").val();
                if (isValidEmailAddress(Email)) {
                    GetUser(Email);
                } else
                    $('.forgotPasswordPane_fieldset').append('<span id="response">Email không đúng định dạng</span>');
            });
            function GetUser(Email) {               
                $('.forgotPasswordPane_fieldset').append('<img id="loadining" src="/images/icons/loading.gif" alt="loading"/>');
                var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
                $.ajax({
                    type: "POST",
                    url: base_url + "/services/WebServices.asmx/GetUserByEmail",
                    data: "{Email:'" + Email + "'}",
                    async: false,
                    global: false,
                    beforeSend: function (xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $('#response').remove();
                        $('.forgotPasswordPane_fieldset').append('<span id="response">' + data.d + '</span>');
                        $('#loadining').fadeOut(500, function () {
                            $(this).remove();
                        });
                        return false;
                    }, error: function (e) {
                        e.toString();
                    }
                });
            }
        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div id="box_container">	
	    <div id="login" class="login-container">     
             <h2 class="head-alt">Đăng Nhập</h2>
             <ul class="tabs">
		        <li><a href="" class="">Login</a></li>
		        <li><a href="" class="current">Lost Password ?</a></li>
	        </ul>
            <div id="" class="panes">               
                <div id="loginPane" class="pane">
                    <fieldset class="loginPane_fieldset">
				        <legend>Please enter user information!</legend>
                        <label>Username:</label><br />
                        <asp:TextBox ID="txtUsername" CssClass="loginUsername" runat="server"></asp:TextBox>                    
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorUsername" runat="server" Display="Dynamic" 
                        ErrorMessage="*" ControlToValidate="txtUsername" ValidationGroup="ValidateGroupCheck"></asp:RequiredFieldValidator>           
                                         
				        <label class="formLabel">Password</label><br /> 
                        <asp:TextBox ID="txtPassword" CssClass="loginPassword" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorPassword" runat="server" 
                            ErrorMessage="*" ControlToValidate="txtPassword" ValidationGroup="ValidateGroupCheck"></asp:RequiredFieldValidator>                       

                        <div class="logControl">
                            <div class="memory">
                                <div class="checker">
                                    <asp:CheckBox ID="chkRemmberMe" Checked="True" runat="server"/>
                                </div>                                
                                <label class="checkbox_text">Remember me</label>
                            </div>
                            <asp:Button ID="btnSubmit" runat="server" Text="Log in" CssClass="button fright" onclick="btnSubmit_Click" />   
                        </div>
				    </fieldset>
                </div>  
                <div id="forgotPasswordPane" class="pane">
                     <fieldset class="forgotPasswordPane_fieldset">
                        <legend>Vui lòng nhập Email xác nhận.</legend>
                        <br />
                        <asp:TextBox ID="txtEmail" CssClass="loginUsername" runat="server"></asp:TextBox> 
                        <br /><br />                         
                        <div class="logControl">
                            <input id="btnSend" type="button" value="Gửi xác nhận" class="button fright" />
                        </div>
                    </fieldset>                   
                </div>                       
            </div>	
        </div>
    </div>
    </form>
</body>
</html>
