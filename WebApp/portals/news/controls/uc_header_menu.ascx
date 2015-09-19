<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_header_menu.ascx.cs"
    Inherits="WebApp.portals.news.controls.uc_header_menu" %>

<div class="zone-wrapper zone-menu-wrapper clearfix hide-bg" id="zone-menu-wrapper">
    <div class="zone zone-menu clearfix container-12" id="zone-menu">
        <div id="region-menu" class="grid-12 region region-menu">
            <div class="region-inner region-menu-inner">
                <section id="block-system-main-menu" class="block block-system block-menu block-main-menu block-system-main-menu even">
                    <div class="block-inner clearfix">
                        <h2 class="block-title">Main menu</h2>            
                        <div class="clearfix" id="ddmenu">                       
                            <asp:Literal ID="Literal_Menu" runat="server"></asp:Literal> 
                        </div>
                    </div>
                </section>
            </div>
        </div>
    </div>
</div>
