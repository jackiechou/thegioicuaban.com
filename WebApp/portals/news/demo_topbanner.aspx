<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="demo_topbanner.aspx.cs" Inherits="WebApp.portals.news.demo_topbanner" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
 #s_show {
    position:relative;
    height:350px;
    background-color:#2a2a2a;
    border:5px groove Fuchsia;
}
#s_show IMG {
    position:absolute;
    top:40px;
    left:50px;
    border-width:5px;
    border-color:Fuchsia;
    border-style:groove;
    z-index:8;
    opacity:0.0;
}
#s_show IMG.active {
    z-index:10;
    opacity:1.0;
}
#s_show IMG.last-active {
    z-index:9;
}
</style>
 <script type="text/javascript" src="../../scripts/jquery/jquery.min.js"></script>
<script type="text/javascript">
    function slideShow() {
        var $active = $('#s_show IMG.active');
        if ($active.length == 0) $active = $('#s_show IMG:last');
        var $next = $active.next().length ? $active.next()
        : $('#s_show IMG:first');
        $active.addClass('last-active');
        $next.css({ opacity: 0.0 })
        .addClass('active')
        .animate({ opacity: 1.0 }, 1000, function () {
            $active.removeClass('active last-active');
        });
    }

    $(function () {
        setInterval("slideShow()", 3000);
    });
</script>
</head>
<body>
    <form id="form1" runat="server">    
    <div id="s_show" >
        <asp:Literal ID="Literal_TopBanner" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
