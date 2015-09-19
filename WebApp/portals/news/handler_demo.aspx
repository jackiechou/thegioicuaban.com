<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="handler_demo.aspx.cs" Inherits="WebApp.portals.news.handler_demo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>hanler demo</title>    
    <script type="text/javascript" src="../../scripts/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="../../scripts/jquery/ww.jquery.min.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function ($) {
            var base_url = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
            $('#btnSubmit').click(function () {              
                //								 var name = $('#name').val();
                //                                var email = $('#email').val();
                //                                var address = $('#address').val();
                $('#container').append('<img id="loadining" src="/images/icons/loading.gif" alt="loading"/>');

                InsertArticleComment();
            });

            function InsertArticleComment() {
            }

            function CallHelloWord1() {
                var handler_url = base_url + "/handlers/RestService.ashx";
                var method = "HelloWorld";
                var param = "ong noi may";
                $.getJSON(handler_url, { Method: method, name: param },
                                        function (result) {                                            
                                            //$("#container").text(result).fadeIn(1000);
                                            $('#response').remove();
                                            $('#container').append('<p id="response">' + result + '</p>');
                                            $('#loadining').fadeOut(500, function () {
                                                $(this).remove();
                                            });
                                            return false;
                                        },
                                        function (error, xhr) {
                                            $("#container").text(error.message).fadeIn(1000);
                                        },
                // Force all page Form Variables to be posted
                                        {postbackMode: "Post" });
            }

            function CallHelloWord2() {
                //                var handler_url = base_url + "/handlers/RestService.ashx";
                //                var method = "HelloWorld";               
                //                ajaxCallMethod(handler_url, method,
                //                        ["ong noi may"],
                //                        function (result) {
                //                            //$("#container").text(result).fadeIn(1000);
                //                            $('#response').remove();
                //                            $('#container').append('<p id="response">' + result + '</p>');
                //                            $('#loadining').fadeOut(500, function () {
                //                                $(this).remove();
                //                            });
                //                            return false;
                //                            }, 
                //                    function (error, xhr) {
                //                        $("#container").text(error.message).fadeIn(1000);
                //                    },
                //                    // Force all page Form Variables to be posted
                //                    {postbackMode: "Post" });
            }

           
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
        <input type="text" name="txt_name" value="Họ tên" id="txt_name"
                                onblur="processBlue('Họ tên','txt_name')" onfocus="processFocus('Họ tên','txt_name')">

        <input id="btnSubmit" class="send_button" name="btnSubmit" type="button" value="Gửi">              
    </div>
    <br />
     <br />

    <ul>
            <li><b>Retrieve Rest Data</b><br />
                <div style="margin-left: 20px;margin-bottom: 10px;">
                <strong>Raw Urls:</strong><br />
                <a href='<%= ResolveUrl("~/handlers/RestService.ashx?method=HelloWorld&message=tiamay") %>'>HelloWorld(default json)</a><br />


                <a href='<%= ResolveUrl("~/handlers/RestService.ashx?method=GetData&message=tiamay") %>'>Call Ajax WebHandler(default json)</a><br />
                <a href='<%= ResolveUrl("~/handlers/RestService.ashx?method=GetData&message=tiamay&format=string") %>'>Call Ajax WebHandler(default string)</a><br />
                <a href='<%= ResolveUrl("~/handlers/RestService.ashx?method=GetData&message=tiamay&format=xml") %>'>Call Ajax WebHandler(default xml)</a><br />

                <strong>Clean Routed Urls:</strong><br />
                <a href='<%= ResolveUrl("~/rest-service/tia-may") %>'>Clean Routed Urls(default json)</a><br />
                <a href='<%= ResolveUrl("~/rest-service/tia-may?format=string") %>'>Clean Routed Urls(format=string)</a><br />                
                <a href='<%= ResolveUrl("~/rest-service/tia-may?format=xml") %>'>Clean Routed Urls(format=xml)</a><br />                
                </div>
            </li>
        </ul>         


    </form>
</body>
</html>
