<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="admin_front_tabs.ascx.cs" Inherits="WebApp.modules.admin.tabs.admin_front_tabs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script type="text/javascript">
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
            if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf('chkBxSelect', 0) >= 0) {
                Inputs[n].checked = CheckBox.checked;
                if (CheckBox.checked)
                    SelectedItems.push(document.getElementById(Inputs[n].id.replace('chkBxSelect', 'hdnFldId')).value);
                else
                    DeleteItem(document.getElementById(Inputs[n].id.replace('chkBxSelect', 'hdnFldId')).value);
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
<script language="javascript" type="text/javascript">
    var add_tab_path = "/modules/admin/tabs/admin_front_tabs_add.aspx?mode=add";
    var edit_tab_path = "/modules/admin/tabs/admin_front_tabs_edit.aspx?mode=edit&idx=";
    var loading_path = "/loading.aspx";
    var tab_module_path = "/modules/admin/tabs/admin_tab_module.aspx?id=";

    function ShowAddModal() {
        var frame = $get('IframeAdd');
        frame.src = add_tab_path;
        $find('AddModalPopup').show();
    }

    function AddCancelScript() {
        var frame = $get('IframeAdd');
        frame.src = loading_url;
    }

    function AddOkayScript() {
        RefreshDataGrid();
        AddCancelScript();
    }      

    function ShowModalEdit() {
        //Get hidden field that wil contain string of selected item's Ids separated by '|'.
        var SelectedValues = document.getElementById('<%= this.hdnFldSelectedValues.ClientID %>').value;
        if (SelectedValues != '') {
            if (SelectedItems.length == 1) {                
                var frame = $get('IframeEdit');
                frame.src = edit_tab_path + SelectedValues;
                $find('EditModalPopup').show();
            } else {
                alert('Vui lòng chỉ check chọn 1 item để edit');
            }
        }
    }

    function ShowEditModal(idx) {                
        var frame = $get('IframeEdit');
        frame.src = edit_tab_path + idx;
        $find('EditModalPopup').show();
    }


    function EditCancelScript() {
        var frame = $get('IframeEdit');
        frame.src = loading_path;
    }

  
    function EditOkayScript() {
        RefreshDataGrid();
        EditCancelScript();
    }

    function RefreshDataGrid() {
        $get('btnReload').click();
    } 
</script>

<div class="gp">  
    <div class="commontool">
        <h3>Quản lý Tab Frontpage</h3>
        <div class="toolbar">
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClientClick="ShowAddModal();return false;"><span class="icon-32-new"></span>Thêm Tab</asp:LinkButton>                      
            <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClientClick="ShowModalEdit();return false;"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>
            <asp:LinkButton ID="btnMultipleDelete" runat="server" CssClass="btn" OnClick="btnMultipleDelete_Click"><span class="icon-32-trash"></span>Xóa nhiều dòng</asp:LinkButton> 
            <asp:LinkButton ID="btnReload" runat="server" CssClass="btn" onclick="btnReload_Click"><span class="icon-32-purge"></span>Reload</asp:LinkButton>    
            <asp:HiddenField ID="hdnFldSelectedValues" runat="server" />
        </div>
    </div>               
                         
    <div class="group_commands">              
        <div class="left">                                                      
        </div> 
        <div class="right">
                Chọn Application 
                <asp:DropDownList ID="ddlApplicationList" runat="server" CssClass="combobox"  
                    Width="149px" onselectedindexchanged="ddlApplicationList_SelectedIndexChanged" ></asp:DropDownList>                                   
                &nbsp;&nbsp;&nbsp;
                Chọn Portal :
                    <asp:DropDownList ID="ddlPortalList" CssClass="combobox" Width="147px" 
                    runat="server" onselectedindexchanged="ddlPortalList_SelectedIndexChanged">
                </asp:DropDownList>  
                 
        </div>
    </div>
    <div style="clear:both;"></div> 
                                     
    <div class="group_panel">
    <asp:UpdatePanel ID="UpdatePanel1" EnableViewState="true" UpdateMode="Conditional" runat="server">
        <Triggers>
             <asp:PostBackTrigger ControlID="ddlPortalList" />
             <asp:PostBackTrigger ControlID="ddlApplicationList" />
        </Triggers>

        <ContentTemplate>  
            <asp:GridView ID="GridView1" runat="server" CssClass="table_list" DataKeyNames="TabId"
                Width="100%" Height="100%" AllowPaging="True" 
                CellPadding="3" GridLines="Vertical"   
                AllowSorting="True" AutoGenerateColumns="False" 
                ondatabound="GridView1_DataBound" 
                onpageindexchanging="GridView1_PageIndexChanging" 
                onrowcancelingedit="GridView1_RowCancelingEdit" 
                onrowediting="GridView1_RowEditing" 
                onselectedindexchanging="GridView1_SelectedIndexChanging" 
                onrowdeleted="GridView1_RowDeleted" onrowdatabound="GridView1_RowDataBound" 
                onsorting="GridView1_Sorting" BackColor="White" BorderColor="#999999" 
                BorderStyle="Solid" BorderWidth="1px" ForeColor="Black" >
                <AlternatingRowStyle BackColor="#CCCCCC" />
                <Columns>  
                    <asp:TemplateField HeaderText="x">                       
                    <ItemTemplate>
                        <asp:CheckBox ID="chkBxSelect" runat="server" />
                        <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("TabId") %>' />
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
                <asp:BoundField DataField="TabId" HeaderText="TabId" /> 
                <asp:BoundField DataField="TabName" HeaderText="TabName" />                          
                <asp:BoundField DataField="Title" HeaderText="Title" />                              
                <asp:BoundField DataField="ParentId" HeaderText="ParentId"  Visible="false" />                                                                                
                <asp:BoundField DataField="TabOrder" HeaderText="TabOrder"/>        
                <asp:BoundField DataField="Description" HeaderText="Description" />    
                <asp:CheckBoxField DataField="DisplayTitle" HeaderText="DisplayTitle"  />  
                <asp:CheckBoxField DataField="PermanentRedirect" HeaderText="PermanentRedirect"  />                                                                                                                 
                <asp:CheckBoxField DataField="IsDeleted" HeaderText="IsDeleted"  />  
                <asp:CheckBoxField DataField="IsSecure" HeaderText="IsSecure"  />  
                <asp:CheckBoxField DataField="IsVisible" HeaderText="IsVisible"  />              
            </Columns>
            <EmptyDataTemplate>No Data</EmptyDataTemplate>                          
            <PagerTemplate>
                <div class="pagination">
                        <asp:Label ID="PagingInformation" runat="server"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
                    <FooterStyle BackColor="#CCCCCC" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#808080" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#383838" />
            </asp:GridView>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>  
</div>


<%----------------------------- ModalPopupExtender_Add -------------------------------------%>                                                                                                  
<ajaxToolkit:ModalPopupExtender ID="btnAdd_ModalPopupExtender" BackgroundCssClass="ModalPopupBG"
    runat="server" OkControlID="btnOkay" CancelControlID="btnCancel" TargetControlID="btnAdd"
    PopupControlID="DivAddWindow" Drag="true" PopupDragHandleControlID="PopupHeader" 
    OnOkScript="RefreshDataGrid();" OnCancelScript="EditCancelScript;" BehaviorID="AddModalPopup">
</ajaxToolkit:ModalPopupExtender>
<div class="popup_Buttons" style="display: none">
    <input id="btnOkay" value="Done" type="button" />
    <input id="btnCancel" value="Cancel" type="button" />
</div>
<div id="DivAddWindow" style="display: none;" class="popupConfirmation">
    <iframe id="IframeAdd" frameborder="0" height="550" width="900" scrolling="no"></iframe>
</div>


<%----------------------------- ModalPopupExtender_Edit -------------------------------------%>    
<asp:Button ID="ButtonEdit" runat="server" Text="Edit" style="display:none" />
<ajaxToolkit:ModalPopupExtender ID="btnEdit_ModalPopupExtender" BackgroundCssClass="ModalPopupBG"
        runat="server" CancelControlID="ButtonEditCancel" OkControlID="ButtonEditDone" 
        TargetControlID="ButtonEdit" PopupControlID="DivEditWindow" 
        OnCancelScript="EditCancelScript();" OnOkScript="EditOkayScript();"
        BehaviorID="EditModalPopup">
</ajaxToolkit:ModalPopupExtender>
<div class="popup_Buttons" style="display: none">
    <input id="ButtonEditDone" value="Done" type="button" onkeydown="" />
    <input id="ButtonEditCancel" value="Cancel" type="button" />
</div>
<div id="DivEditWindow" style="display: none;" class="popupConfirmation">
    <iframe id="IframeEdit" frameborder="0" height="740" width="900" scrolling="no">
    </iframe>
</div> 
<%----------------------------- Delete -------------------------------------------------------%>   
<ajaxToolkit:ModalPopupExtender BackgroundCssClass="ModalPopupBG" ID="btnMultipleDelete_ModalPopupExtender"
    runat="server" TargetControlID="btnMultipleDelete" PopupControlID="DivDeleteConfirmation"
    OkControlID="ButtonDeleleOkay" CancelControlID="ButtonDeleteCancel">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ConfirmButtonExtender ID="btnMultipleDelete_ConfirmButtonExtender" runat="server" Enabled="True"
    TargetControlID="btnMultipleDelete" DisplayModalPopupID="btnMultipleDelete_ModalPopupExtender">
</ajaxToolkit:ConfirmButtonExtender> 
<asp:Panel runat="server" ID="DivDeleteConfirmation" Style="display: none;" class="popupConfirmation">
    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Delete </div>
            <div class="TitlebarRight" onclick="$get('ButtonDeleteCancel').click();">
            </div>
        </div>
        <div class="popup_Body">
            <p>
                Are you sure, you want to delete the item?
            </p>
        </div>
        <div class="popup_Buttons">
            <input id="ButtonDeleleOkay" value="Okay" type="button" />
            <input id="ButtonDeleteCancel" value="Cancel" type="button" />
        </div>
    </div>
</asp:Panel>