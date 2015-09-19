<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit_module_permission.ascx.cs" Inherits="WebApp.modules.admin.dashboard.modules.edit_module_permission" %>
<table cellpadding="0" cellspacing="5px">
    <tr>
        <td>Permission</td>
        <td></td>
        <td >
            <table>
                <tr>
                    <td class="style3">List Role</td>
                    <td class="style3">List Permition</td>
                </tr>
                <tr>
                    <td class="style3">RoleId1</td>
                    <td class="style3"><asp:CheckBox ID="PermissionId1" runat="server" />
                        <asp:CheckBox ID="PermissionId2" runat="server" />
                        <asp:CheckBox ID="PermissionId3" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="style3">RoleId2</td>
                    <td class="style3"><asp:CheckBox ID="CheckBox1" runat="server" />
                        <asp:CheckBox ID="CheckBox2" runat="server" />
                        <asp:CheckBox ID="CheckBox3" runat="server" /></td>
                </tr>
            </table>

            <asp:GridView ID="GridView1" runat="server" DataKeyNames="ModuleID" 
                        CssClass="table_list" AutoGenerateColumns="False" EmptyDataText="No Data" 
                        AllowPaging="True" ShowFooter="True" 
                        onrowcancelingedit="GridView1_RowCancelingEdit"                     
                        onrowcreated="GridView1_RowCreated" 
                        onrowdatabound="GridView1_RowDataBound"                 
                        onrowdeleting="GridView1_RowDeleting"        
                        onrowediting="GridView1_RowEditing" 
                        onpageindexchanging="GridView1_PageIndexChanging" 
                        onselectedindexchanged="GridView1_SelectedIndexChanged" 
                        onselectedindexchanging="GridView1_SelectedIndexChanging" 
                        onsorting="GridView1_Sorting" ondatabound="GridView1_DataBound" 
                        onrowcommand="GridView1_RowCommand"  >
                         <Columns>      
                              <asp:TemplateField HeaderText="x">
                                <EditItemTemplate>                                         
                                    <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("ModuleID") %>' Width="30px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkBxSelect" runat="server" />
                                    <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("ModuleID") %>' />
                                </ItemTemplate>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkBxHeader" onclick="javascript:HeaderClick(this);" runat="server" />
                                </HeaderTemplate>
                            </asp:TemplateField>                
                    
                            <asp:BoundField DataField="ModuleTitle" HeaderText="ModuleTitle" />  
                            <asp:BoundField DataField="ControlSrc" HeaderText="ControlSrc" />                                 
                            <asp:BoundField DataField="IsAdmin" HeaderText="IsAdmin" />                             
                            <asp:BoundField DataField="ViewOrder" HeaderText="ViewOrder" />   
                            <asp:BoundField DataField="IsAdmin" HeaderText="IsAdmin" />   
                        </Columns>     
                         <EmptyDataTemplate>No Data</EmptyDataTemplate>
                       <PagerTemplate>
                           <div class="pagination">
                            <asp:ImageButton ID="imgBtnFirst" runat="server" BorderWidth="0" BorderStyle="None"
                                ImageUrl="~/images/icons/arrow_first.png"
                                CommandArgument="First" CommandName="Page" Height="22px" Width="26px" />
                                <asp:ImageButton ID="imgBtnPrev" runat="server" BorderWidth="0" BorderStyle="None"
                                ImageUrl="~/images/icons/arrow_previous.png"
                                CommandArgument="Prev" CommandName="Page" Height="23px" Width="29px" />
                                     Page
                            <asp:DropDownList ID="ddlPages" runat="server" BorderWidth="0" BorderStyle="None"
                                AutoPostBack="True" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged">
                            </asp:DropDownList> of <asp:Label ID="lblPageCount"
                                runat="server"></asp:Label>
                            <asp:ImageButton ID="imgBtnNext" runat="server" BorderWidth="0" BorderStyle="None"
                                ImageUrl="~/images/icons/arrow_next.png"
                                CommandArgument="Next" CommandName="Page" Height="21px" Width="27px" />
                            <asp:ImageButton ID="imgBtnLast" runat="server" BorderWidth="0" BorderStyle="None"
                                ImageUrl="~/images/Icons/arrow_last.png"
                                CommandArgument="Last" CommandName="Page" Height="21px" Width="31px" />
                         </div>
                        </PagerTemplate> 
                    </asp:GridView>

        </td>
    </tr>
</table>