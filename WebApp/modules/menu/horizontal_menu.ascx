<%@ Control Language="C#" AutoEventWireup="true" Inherits="user_controls_admin_controls_menu_horizontal_menu_horizontal_menu" Codebehind="horizontal_menu.ascx.cs" %>
	<script type="text/javascript" src="../../Scripts/menu_sooperfish/lib/jquery-1.4.2.min.js"></script>
	<script type="text/javascript" src="../../Scripts/menu_sooperfish/lib/jquery.easing-sooper.js"></script>
	<script type="text/javascript" src="../../Scripts/menu_sooperfish/lib/jquery.sooperfish.js"></script>
   <script type="text/javascript">
       jQuery(document).ready(function ($) {
           $('ul.sf-menu').sooperfish({
               dualColumn: 6, //if a submenu has at least this many items it will be divided in 2 columns
               tripleColumn: 8, //if a submenu has at least this many items it will be divided in 3 columns
               hoverClass: 'sfHover',
               delay: 500, //make sure menus only disappear when intended, 500ms is advised by Jacob Nielsen
               animationShow: { width: 'show', height: 'show', opacity: 'show' },
               speedShow: 750,
               easingShow: 'easeOutBounce',
               animationHide: { width: 'hide', height: 'hide', opacity: 'hide' },
               speedHide: 300,
               easingHide: 'easeInOvershoot',
               autoArrows: true
           });
       });
    </script>
 
    <asp:Literal ID="Literal_Menu" runat="server"></asp:Literal> 