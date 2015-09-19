<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rights.aspx.cs" Inherits="WebApp.modules.admin.security.roles.rights" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        //Function checkBoxClicked checks the checkboxes based on the user input

        function checkBoxClicked(cbxView, cbxAdd, cbxEdit, cbxAll, ctl) {
            var cbkView = document.getElementById(cbxView);
            var cbkAdd = document.getElementById(cbxAdd);
            var cbkEdit = document.getElementById(cbxEdit);
            var cbkDelete = document.getElementById(cbxDelete);
            var cbkAll = document.getElementById(cbxAll);

            var itemChecked = getCheckedItem(cbxView, cbxAdd, cbxEdit,cbxDelete, cbxAll, ctl);

            if (itemChecked == "false") {
                if (ctl == "VIEW") {
                    if (cbkView.checked == true) {
                        cbkView.checked = true;
                        cbkAdd.checked = false;
                        cbkEdit.checked = false;
                        cbkDelete.checked = false;
                        cbkAll.checked = false;
                    }
                    else {
                        cbkView.checked = false;
                        cbkAdd.checked = false;
                        cbkEdit.checked = false;
                        cbkDelete.checked = false;
                        cbkAll.checked = false;
                    }
                }
                else if (ctl == "ADD") {
                    if (cbkAdd.checked == true) {
                        cbkView.checked = true;
                        cbkAdd.checked = true;
                        cbkEdit.checked = false;
                        cbkDelete.checked = false;
                        cbkAll.checked = false;
                    }

                    else {
                        cbkView.checked = false;
                        cbkAdd.checked = false;
                        cbkEdit.checked = false;
                        cbkDelete.checked = false;
                        cbkAll.checked = false;
                    }
                }
                else if (ctl == "EDIT") {
                    if (cbkEdit.checked == true) {
                        cbkView.checked = true;
                        cbkAdd.checked = true;
                        cbkEdit.checked = true;
                        cbkDelete.checked = true;
                        cbkAll.checked = false;
                    }
                    else {
                        cbkView.checked = false;
                        cbkAdd.checked = false;
                        cbkEdit.checked = false;
                        cbkDelete.checked = false;
                        cbkAll.checked = false;
                    }
                }

                else if (ctl == "ALL") {
                    if (cbkAll.checked == true) {
                        cbkView.checked = true;
                        cbkAdd.checked = true;
                        cbkEdit.checked = true;
                        cbkDelete.checked = true;
                        cbkAll.checked = true;
                    }
                    else {
                        cbkView.checked = false;
                        cbkAdd.checked = false;
                        cbkEdit.checked = false;
                        cbkDelete.checked = false;
                        cbkAll.checked = false;
                    }
                }
            }
            else {
                if (ctl == "VIEW") {
                    cbkView.checked = true;
                }
                else if (ctl == "ADD") {
                    cbkAdd.checked = true;
                }
                else if (ctl == "EDIT") {
                    cbkEdit.checked = true;
                }
                else if (ctl == "DELETE") {
                    cbkDelete.checked = true;
                }
                else if (ctl == "ALL") {
                    cbkAll.checked = true;
                }
            }
        }



        //Function getCheckedItem returns the previously selected checkboxes.
        function getCheckedItem(cbxView, cbxAdd, cbxEdit, cbxAll, ctl) {
            var cbxView = document.getElementById(cbxView);
            var cbkAdd = document.getElementById(cbxAdd);
            var cbkEdit = document.getElementById(cbxEdit);
            var cbkDelete = document.getElementById(cbxDelete);
            var cbkAll = document.getElementById(cbxAll);
            var retVal = "false";



            if (ctl == "VIEW") {
                if (cbkAdd.checked == true || cbkEdit.checked == true || cbkDelete.checked == true || cbkAll.checked == true) {
                    retVal = "true";
                }
                else {
                    retVal = "false";
                }
            }

            else if (ctl == "ADD") {
                if (cbkEdit.checked == true || cbkAll.checked == true) {
                    retVal = "true";
                }
                else {
                    retVal = "false";
                }
            }

            else if (ctl == "EDIT") {
                if (cbkAll.checked == true) {
                    retVal = "true";
                }
                else {
                    retVal = "false";
                }
            }

            else if (ctl == "DELETE") {
                if (cbkAll.checked == true) {
                    retVal = "true";
                }
                else {
                    retVal = "false";
                }
            }

            else if (ctl == "ALL") {
                retVal = "false";
            }

            return retVal;



        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    Application<asp:DropDownList ID="ddlApplicationList" runat="server" CssClass="combobox"  
                                     Width="149px" onselectedindexchanged="ddlApplicationList_SelectedIndexChanged" ></asp:DropDownList> 
    <br />
     Role: <asp:DropDownList ID="ddlRoleList"  runat="server"   
                                        CssClass="combobox" Width="147px" ></asp:DropDownList>  
    <br />
    <asp:GridView ID="GridView1" runat="server"  AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" CellPadding="4" ForeColor="#333333" GridLines="None">
        <Columns>

                    <asp:BoundField DataField="TabId" HeaderText="Tab Id" />
                    <asp:BoundField DataField="TabName" HeaderText="Tab Name" />
                    <asp:TemplateField HeaderText="View">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkView" runat="server"  />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Add">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkAdd" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Edit">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkEdit" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Delete">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkDelete" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="All">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkAll" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <EditRowStyle BackColor="#999999" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:GridView> 
    </form>
</body>
</html>
