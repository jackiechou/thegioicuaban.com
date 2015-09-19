<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="paths.ascx.cs" Inherits="WebApp.modules.admin.security.paths" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script type="text/javascript">
    //variable that will store the id of the last clicked row
    var previousRow;

    function ChangeRowColor(row) {

        //If last clicked row and the current clicked row are same
        if (previousRow == row)
            return; //do nothing

        //If there is row clicked earlier
        else if (previousRow != null)

        //change the color of the previous row back to white
        //document.getElementById(previousRow).style.backgroundColor = "#ffffff";
            document.getElementById(previousRow).style.backgroundColor = "";

        //change the color of the current row to light yellow
        document.getElementById(row).style.backgroundColor = "#FF885F";

        //assign the current row id to the previous row id for next row to be clicked
        previousRow = row;

    }
    //---------------------------------------------------------------------------------
    function DeleteConfirmation() {
        if (confirm("Are you sure you want to delete selected records ?") == true)
            return true;
        else
            return false;
    }

    function UpdateMultipleSortKeyConfirmation() {
        if (confirm("Are you sure you want to update selected records ?") == true)
            return true;
        else
            return false;
    }
    //================================================================================
    //================================================================================
    //Reference of the GridView. 
    var TargetBaseControl = null;
    //Total no of checkboxes in a particular column inside the GridView.
    var CheckBoxes;
    //Total no of checked checkboxes in a particular column inside the GridView.
    var CheckedCheckBoxes;
    //Array of selected item's Ids.
    var SelectedItems;
    //Hidden field that wil contain string of selected item's Ids separated by '|'.
    var SelectedValues;

    window.onload = function () {
        //Get reference of the GridView. 
        try {
            TargetBaseControl = document.getElementById('<%= this.GridView1.ClientID %>');
        }
        catch (err) {
            TargetBaseControl = null;
        }

        //Get total no of checkboxes in a particular column inside the GridView.
        try {
            CheckBoxes = parseInt('<%= this.GridView1.Rows.Count %>');
        }
        catch (err) {
            CheckBoxes = 0;
        }

        //Get total no of checked checkboxes in a particular column inside the GridView.
        CheckedCheckBoxes = 0;

        //Get hidden field that wil contain string of selected item's Ids separated by '|'.
        SelectedValues = document.getElementById('<%= this.hdnFldSelectedValues.ClientID %>');

        //Get an array of selected item's Ids.
        if (SelectedValues.value == '')
            SelectedItems = new Array();
        else
            SelectedItems = SelectedValues.value.split('|');

        //Restore selected CheckBoxes' states.
        if (TargetBaseControl != null)
            RestoreState();
    }

    function HeaderClick(CheckBox) {
        //Get all the control of the type INPUT in the base control.
        var Inputs = TargetBaseControl.getElementsByTagName('input');

        //Checked/Unchecked all the checkBoxes in side the GridView & modify selected items array.
        for (var n = 0; n < Inputs.length; ++n)
            if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf('chkBxSelect', 0) >= 0 && Inputs[n].checked) {
                Inputs[n].checked = CheckBox.checked;
                if (CheckBox.checked)
                    SelectedItems.push(document.getElementById(Inputs[n].id.replace('chkBxSelect', 'hdnFldId')).value);
                else {
                    DeleteItem(document.getElementById(Inputs[n].id.replace('chkBxSelect', 'hdnFldId')).value);
                    //alert('Select at least one checkbox!');
                }
            }

        //Update Selected Values. 
        SelectedValues.value = SelectedItems.join('|');

        //Reset Counter
        CheckedCheckBoxes = CheckBox.checked ? CheckBoxes : 0;
    }

    function ChildClick(CheckBox, HCheckBox, Id) {
        //Modifiy Counter;            
        if (CheckBox.checked && CheckedCheckBoxes < CheckBoxes)
            CheckedCheckBoxes++;
        else if (CheckedCheckBoxes > 0)
            CheckedCheckBoxes--;

        //Change state of the header CheckBox.
        if (CheckedCheckBoxes < CheckBoxes)
            HCheckBox.checked = false;
        else if (CheckedCheckBoxes == CheckBoxes)
            HCheckBox.checked = true;

        //Modify selected items array.
        if (CheckBox.checked)
            SelectedItems.push(Id);
        else
            DeleteItem(Id);

        //Update Selected Values. 
        SelectedValues.value = SelectedItems.join('|');
    }

    function RestoreState() {
        //Get all the control of the type INPUT in the base control.
        var Inputs = TargetBaseControl.getElementsByTagName('input');

        //Header CheckBox
        var HCheckBox = null;

        //Restore previous state of the all checkBoxes in side the GridView.
        for (var n = 0; n < Inputs.length; ++n)
            if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf('chkBxSelect', 0) >= 0)
                if (IsItemExists(document.getElementById(Inputs[n].id.replace('chkBxSelect', 'hdnFldId')).value) > -1) {
                    Inputs[n].checked = true;
                    CheckedCheckBoxes++;
                }
                else
                    Inputs[n].checked = false;
            else if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf('chkBxHeader', 0) >= 0)
                HCheckBox = Inputs[n];

        //Change state of the header CheckBox.
        if (CheckedCheckBoxes < CheckBoxes)
            HCheckBox.checked = false;
        else if (CheckedCheckBoxes == CheckBoxes)
            HCheckBox.checked = true;
    }

    function DeleteItem(Text) {
        var n = IsItemExists(Text);
        if (n > -1)
            SelectedItems.splice(n, 1);
    }

    function IsItemExists(Text) {
        for (var n = 0; n < SelectedItems.length; ++n)
            if (SelectedItems[n] == Text)
                return n;

        return -1;
    }   
</script>

<div id="gp" class="wrap_gp" > 
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">        
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="btnSearch_Click"/>
        </Triggers>
        <ContentTemplate>
   
              <div class="commontool">
                    <h3>Quản lý path</h3>
                    <div class="toolbar">      
                        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClick="btnAdd_Click"><span class="icon-32-new"></span>Thêm</asp:LinkButton>                                                                                                                         
                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClick="btnEdit_Click"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>                                            
                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn" OnClick="btnDelete_Click"><span class="icon-32-trash"></span>Xóa</asp:LinkButton>                                                                                    
                        <asp:HiddenField ID="hdnFldSelectedValues" runat="server" /> 
                    </div>
              </div>           
                 <div id="group_commands">        
                    <div class="left">&nbsp;</div>
                    <div class="right">
                        <div id="gp1" class="fillter_tool">        
                          Tên Ứng Dụng: <asp:DropDownList ID="ddlApp" AutoPostBack="True" runat="server"   CssClass="combobox" 
                                  onselectedindexchanged="ddlApp_SelectedIndexChanged"></asp:DropDownList>             
                           <asp:CustomValidator ID="CusValApp"  ControlToValidate="ddlApp" 
                           Text="*" ForeColor="red" ClientValidationFunction="ClientValidateSelection" 
                           runat="server" ErrorMessage="Invalid, please select one" SetFocusOnError="True" Display="Dynamic"/>                                     
                        </div>     
                      </div>               
                </div>   
                <div style="clear:both;">&nbsp;</div>             
                <div id="group_panel" >         
                            <asp:GridView ID="GridView1" DataKeyNames="PathId" PageSize="20"  runat="server" AllowPaging="True"  Height="100%"    
                                AllowSorting="True" CssClass="table_list" AutoGenerateColumns="False" 
                                CellPadding="3" EmptyDataText="No Data" GridLines="Vertical" ShowFooter="True"                                
                            onpageindexchanged="GridView1_SelectedIndexChanged" 
                            onrowcancelingedit="GridView1_RowCancelingEdit" onrowcreated="GridView1_RowCreated" 
                            onrowediting="GridView1_RowEditing"           
                            onselectedindexchanged="GridView1_SelectedIndexChanged" 
                            onselectedindexchanging="GridView1_SelectedIndexChanging" 
                            ondatabound="GridView1_DataBound" 
                            onpageindexchanging="GridView1_PageIndexChanging" 
                            onrowdatabound="GridView1_RowDataBound" onsorting="GridView1_Sorting" 
                                EnableModelValidation="True" EnableViewState="False">
                                <AlternatingRowStyle BackColor="#F5F5F5" />
                                <Columns>                                                            
                                   <asp:TemplateField HeaderText="x">
                                        <EditItemTemplate>                                         
                                            <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("PathId") %>' Width="30px"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkBxSelect" runat="server" />
                                            <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("PathId") %>' />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkBxHeader" onclick="javascript:HeaderClick(this);" runat="server" />
                                        </HeaderTemplate>
                                    </asp:TemplateField> 
                                        
                                    <asp:TemplateField HeaderText="No">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:BoundField DataField="PathId" HeaderText="PathId" Visible="true"/> 
                                    <asp:BoundField DataField="ApplicationId" HeaderText="ApplicationId"/>
                                    <asp:BoundField DataField="Path" HeaderText="Path"/>
                                    <asp:BoundField DataField="LoweredPath" HeaderText="LoweredPath" />                                                                        
                                </Columns>
                                <EmptyDataTemplate>No Data</EmptyDataTemplate>
                                  <PagerTemplate>
                                  <div class="pagination">
                                    <asp:ImageButton ID="imgBtnFirst" runat="server"
                                        ImageUrl="~/images/Icons/arrow_first.png"
                                        CommandArgument="First" CommandName="Page" Height="22px" Width="26px" />
                                        <asp:ImageButton ID="imgBtnPrev" runat="server"
                                        ImageUrl="~/images/Icons/arrow_previous.png"
                                        CommandArgument="Prev" CommandName="Page" Height="23px" Width="29px" />
                                             Page
                                    <asp:DropDownList ID="ddlPages" runat="server"
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged"> </asp:DropDownList> of <asp:Label ID="lblPageCount"
                                        runat="server"></asp:Label>
                                    <asp:ImageButton ID="imgBtnNext" runat="server"
                                        ImageUrl="~/images/Icons/arrow_next.png"
                                        CommandArgument="Next" CommandName="Page" Height="21px" Width="27px" />
                                    <asp:ImageButton ID="imgBtnLast" runat="server"
                                        ImageUrl="~/images/Icons/arrow_last.png"
                                        CommandArgument="Last" CommandName="Page" Height="21px" Width="31px" />
                                   </div>
                                </PagerTemplate> 
                                   
                            </asp:GridView> 
                </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>