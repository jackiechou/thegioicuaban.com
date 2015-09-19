<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin_module_permission_edit.aspx.cs" Inherits="WebApp.modules.admin.dashboard.modules.admin_module_permission_edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>module_permission</title>

    <script language="javascript" type="text/javascript">
        function getbacktostepone() {
            window.location = "admin_modules_edit.aspx";
        }
        function onSuccess() {
            setTimeout(okay, 2000);
        }
        function onError() {
            setTimeout(getbacktostepone, 2000);
        }
        function okay() {
            var mode = $get('hdnWindowUIMODE').value;
            if (mode == "edit") {
                parent.location.href = parent.location.href;
                window.parent.document.getElementById('ButtonEditDone').click();
            } else {
                parent.location.href = parent.location.href;
                window.parent.document.getElementById('btnOkay').click();
                getbacktostepone();
            }
        }
        function cancel() {
            var mode = $get('hdnWindowUIMODE').value;
            if (mode == "edit")
                window.parent.document.getElementById('ButtonEditCancel').click();
            else
                window.parent.document.getElementById('btnCancel').click();
        }

        // disables the button specified and sets its style to a disabled "look".
        function disableButtonOnClick(oButton, sButtonText, sCssClass) {
            oButton.disabled = true;      // set button to disabled so you can't click on it.
            oButton.value = sButtonText;   // change the text of the button.
            oButton.setAttribute('className', sCssClass); // IE uses className for the css property.
            oButton.setAttribute('class', sCssClass); // Firefox, Safari use class for the css property.  (doesn't hurt to do both).
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
     <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
       
   <div>
       <asp:DropDownList ID="ddlApplicationList" runat="server">
       </asp:DropDownList>
   <asp:GridView ID="GridView1" runat="server" DataKeyNames="RoleId" 
                AutoGenerateColumns="False" EmptyDataText="No Data" 
                    AllowPaging="True" Width="100%" Height="100%"    
                AllowSorting="True"  BackColor="White" ShowFooter="True"  
                BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" 
                CellPadding="3"  ForeColor="Black" GridLines="Vertical" 
        ondatabound="GridView1_DataBound" 
        onpageindexchanging="GridView1_PageIndexChanging" 
        onrowcancelingedit="GridView1_RowCancelingEdit" 
        onrowcommand="GridView1_RowCommand" onrowcreated="GridView1_RowCreated" 
        onrowdatabound="GridView1_RowDataBound" onrowediting="GridView1_RowEditing" 
        onselectedindexchanged="GridView1_SelectedIndexChanged" 
        onselectedindexchanging="GridView1_SelectedIndexChanging" onsorting="GridView1_Sorting"                             
                >
                    <AlternatingRowStyle BackColor="#CCCCCC" />
                    <Columns>                                                                    
                    <asp:BoundField DataField="RoleName" HeaderText="RoleName" />                           
                        <asp:TemplateField HeaderText="View">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEditView" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkView" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Edit">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEditUpdate" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkEdit" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Delete">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEditDelete" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkDelete" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Full Controls">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEditFullControls" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkFullControls" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                           
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
                    <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            </asp:GridView>
   </div>
   <div>
       <asp:Button ID="btnUpdate" runat="server" Text="Update" 
           onclick="btnUpdate_Click" />
   </div>
       


    </form>
</body>
</html>
