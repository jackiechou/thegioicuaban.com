<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vticker_demo.aspx.cs" Inherits="WebApp.portals.news.vticker_demo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <title>vticker test</title>
   <style type="text/css">  
.scroller .ticker {
	width: 275px;
	height: 563px;
	overflow: hidden;
	border: 1px solid #DDD;
	margin: 0;
	padding: 0;
	list-style: none;
	border-radius: 5px;
	box-shadow: 0px 0px 5px #DDD;
}

.scroller .ticker li {
    border-bottom: 1px dotted #DDDDDD;
    height: 128px;
    margin: 0;
    overflow: hidden;
    padding: 6px;
}
.scroller .ticker img {
	float: left;
	height: 120px;
	width: 120px;	
} 
.scroller .ticker .col_odd
{
    float:left; padding: 3px; border:1px solid #CCCCCC;
    margin-right:7px;
}
.scroller .ticker .col_even
{
    float:left; padding: 3px; border:1px solid #CCCCCC;
}


</style>
<script src="../../scripts/jquery/jquery.min.js"></script>
<script>
    function vertical_ticker() {
        $('#scroller_vticker li:first').slideUp(function () { $(this).appendTo($('#scroller_vticker')).slideDown(); });
    }
    setInterval(function () { vertical_ticker() }, 4000);	
</script>
</head>
<body>
    <form id="form1" runat="server">
<div id="scroller" class="scroller" runat="server"></div>
    </form>
</body>
</html>
