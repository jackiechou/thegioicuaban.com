<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_menu.ascx.cs" Inherits="WebApp.portals.news.controls.uc_menu" %>
<script type="text/javascript" src="<%# Request.Url.Host.ToString() %>/portals/news/skins/default_skin/js/ddmenu.js"></script>
<nav id="ddmenu"> 
    <ul>
        <li><a href='#'>Trang Chủ</a></li> 
        <li> <a href='#'>Giới thiệu</a></li>
        <li>
            <a href='#'>Gian hàng</a> 
              <%--  <div class="column">
                <b>Lorem Ipsum</b>
                <a href="#">Dolor sit amet</a>
                <a href="#">Consectetur elit</a>
                <a href="#">Etiam massa</a>
                <a href="#">Suscipit sapien</a>
                <a href="#">Quis turpis</a>
                <a href="#">Web Menu Builder</a>
                <a href="#">Quos torpusior</a>
                <a href="#">Velit a dapibus</a>
            </div>--%>
        </li> 
        <li><a href='#'>Shopping</a></li>
        <li>
            <a href='#'>Báo giá</a>
              <%--  <div class="dropToLeft2">
                    <div class="column">
                        <b>MENU BUILDER</b>
                        <div>
                            <a href="#">Fermentum ut nulla</a>
                            <a href="#">Duis ut mauris</a>
                            <a href="#">Quisque tempor</a>
                        </div>
                        <b>MENU MAKER</b>
                        <div>
                            <a href="#">Quisque dictum</a>
                            <a href="#">Nulla scelerisque</a>
                            <a href="#">hendrerit tincidunt</a>
                        </div>
                    </div>
                    <div class="column">
                        <b>JQUERY MENU</b>
                        <div>
                            <a href="#">Suspendisse potenti</a>
                            <a href="#">Curabitur in mauris</a>
                            <a href="#">Phasellus ultrices</a>
                            <a href="#">Quisque ornare</a>
                            <a href="#">Vestibulum</a>
                            <a href="#">Best JavaScript menu</a>
                            <a href="#">Proin sed magna</a>
                            <a href="#">Etiam aliquet</a>
                        </div>
                    </div>
                </div>--%>
        </li> 
                            
        <li><a href='#'>Hình thức thanh toán</a></li> 
    </ul>    
</nav>