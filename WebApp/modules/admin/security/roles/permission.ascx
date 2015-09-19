<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="permission.ascx.cs" Inherits="WebApp.modules.admin.security.roles.permission" %>
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

    //This Function is used to change the ROW backGroundColor on Click. ================================
    var previousRow;
    function ChangeRowColor(row) {
        if (previousRow == row)
            return;

        else if (previousRow != null)
            var color = row.style.backgroundColor;

        if (previousRow != null) {
            if (color == "bisque") {
                previousRow.style.backgroundColor = "white";
            }
            else if (color == "white") {
                previousRow.style.backgroundColor = "bisque";
            }
        }

        row.style.backgroundColor = "#ffffda";
        previousRow = row;

    }

    //This Function is used to change the ROW backGroundColor on MouseOver. ============================
    var lastColorUsed;
    function ChangeBackColor(row, highlight, rowHighlightColor) {
        if (highlight) {
            // set the background colour
            lastColorUsed = row.style.backgroundColor;
            row.style.backgroundColor = rowHighlightColor;
        }
        else {
            // restore the colour
            row.style.backgroundColor = lastColorUsed;
        }
    }    
      
</script> 

<%----- ModalPopupExtender JavaScript -------%>                                                                                                  
 <script language="javascript" type="text/javascript">
     function ShowAddModal() {
         var frame = $get('IframeAdd');
         frame.src = "modules/admin/security/roles/permission_edit.aspx?mode=add";
         $find('AddModalPopup').show();
     }

     function AddCancelScript() {
         var frame = $get('IframeAdd');
         frame.src = "loading.aspx";
     }

     function AddOkayScript() {
         RefreshDataGrid();
         AddCancelScript();
     }

     function ShowModalEdit() {
         var idx = 0;
         //Get hidden field that wil contain string of selected item's Ids separated by '|'.
         var SelectedValues = document.getElementById('<%= this.hdnFldSelectedValues.ClientID %>');

         //Get an array of selected item's Ids.
         if (SelectedValues.value == '')
             SelectedItems = new Array();
         else
             SelectedItems = SelectedValues.value.split('|');

         if (SelectedItems.length == 1) {
             idx = SelectedItems[0];
             var edit_path = "modules/admin/security/roles/permission_edit.aspx?mode=edit&idx=";
             var frame = $get('IframeEdit');
             frame.src = edit_path + idx;
             $find('EditModalPopup').show();
         } else {
             alert('Vui lòng chỉ chechk chọn 1 item để edit');
             return false;
         }
     }

     function ShowEditModal(idx) {
         if (idx == '') {
             //Get hidden field that wil contain string of selected item's Ids separated by '|'.
             var SelectedValues = document.getElementById('<%= this.hdnFldSelectedValues.ClientID %>');

             //Get an array of selected item's Ids.
             if (SelectedValues.value == '')
                 SelectedItems = new Array();
             else
                 SelectedItems = SelectedValues.value.split('|');

             if (SelectedItems.length == 1) {
                 idx = SelectedItems[0];
             } else {
                 alert('Vui lòng chỉ check chọn 1 item để edit');
                 return false;
             }
         }
         var edit_path = "modules/admin/security/roles/permission_edit.aspx?mode=edit&idx=";
         var frame = $get('IframeEdit');
         frame.src = edit_path + idx;
         $find('EditModalPopup').show();
     }

     function EditCancelScript() {
         var frame = $get('IframeEdit');
         frame.src = "loading.aspx";
     }

     function EditOkayScript() {
         var frame = $get('IframeEdit');
         frame.src = "loading.aspx";
     } 

</script>


<%----------------------------- ModalPopupExtender_Add -------------------------------------%>                                                                                                  
<ajaxToolkit:ModalPopupExtender ID="btnAdd_ModalPopupExtender" BackgroundCssClass="ModalPopupBG"
    runat="server" CancelControlID="btnCancel" OkControlID="btnOk" TargetControlID="btnAdd"
    PopupControlID="Panel1" Drag="true" PopupDragHandleControlID="PopupHeader" OnCancelScript="AddCancelScript"
OnOkScript="ShowAddModal();"  BehaviorID="AddModalPopup">
</ajaxToolkit:ModalPopupExtender>
<div class="popup_Buttons" style="display: none">
<input id="btnOk" value="Done" type="button" />
<input id="btnCancel" value="Cancel" type="button" />
</div>
<div id="Panel1" style="display: none;" class="popupConfirmation">
<iframe id="IframeAdd" frameborder="0" width="500" height="403"
    scrolling="no"></iframe>
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
    <input id="ButtonEditDone" value="Done" type="button" />
    <input id="ButtonEditCancel" value="Cancel" type="button" />
</div>
<div id="DivEditWindow" style="display: none;" class="popupConfirmation">
    <iframe id="IframeEdit" frameborder="0" height="403" width="500px" scrolling="no">
    </iframe>
</div>  
    
<div class="gp">   
        <div class="commontool">
            <h3>Quản lý Permission </h3>
            <div class="toolbar">             
                <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClientClick="ShowAddModal();return false;"><span class="icon-32-new"></span>Thêm</asp:LinkButton>                              
                <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClientClick="ShowModalEdit();return false;"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>             
                <asp:HiddenField ID="hdnFldSelectedValues" EnableViewState="true" runat="server" />    
            </div>
        </div>       
        <div class="group_commands">              
            <div class="left"> &nbsp;</div> 
            <div class="right">
                <div class="fillter_tool">                               
                    &nbsp;
                </div>  
            </div>   
        </div>
        <div style="clear:both;"></div>
                  
        <div class="group_panel">           
                              
            <asp:GridView ID="GridView1" runat="server" DataKeyNames="PermissionID" 
                AutoGenerateColumns="False" EmptyDataText="No Data" 
                    AllowPaging="True" Width="100%" Height="100%"    
                AllowSorting="True" ShowFooter="True"                             
                onrowcancelingedit="GridView1_RowCancelingEdit"                    
                onrowdatabound="GridView1_RowDataBound" 
                onrowediting="GridView1_RowEditing" 
                onpageindexchanging="GridView1_PageIndexChanging" 
                onselectedindexchanged="GridView1_SelectedIndexChanged" 
                onselectedindexchanging="GridView1_SelectedIndexChanging" 
                onsorting="GridView1_Sorting" 
                ondatabound="GridView1_DataBound" BackColor="White" BorderColor="#999999" 
                BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" 
                GridLines="Vertical" onrowcommand="GridView1_RowCommand" 
                onrowdeleting="GridView1_RowDeleting" >
                    <AlternatingRowStyle BackColor="#CCCCCC" />
                    <Columns>      
                        <asp:TemplateField HeaderText="x">
                        <EditItemTemplate>                                         
                            <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("PermissionID") %>' Width="30px"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkBxSelect" runat="server" />
                            <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("PermissionID") %>' />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkBxHeader" onclick="javascript:HeaderClick(this);" runat="server" />
                        </HeaderTemplate>
                    </asp:TemplateField>                
                    <asp:BoundField DataField="PermissionID" HeaderText="PermissionID"/>                                       
                    <asp:BoundField DataField="PermissionCode" HeaderText="PermissionCode" />                                 
                    <asp:BoundField DataField="PermissionKey" HeaderText="PermissionKey" /> 
                    <asp:BoundField DataField="PermissionName" HeaderText="PermissionName" /> 
                    <asp:BoundField DataField="ViewOrder" HeaderText="ViewOrder" />     
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
                    <FooterStyle BackColor="#CCCCCC" />
                    <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#808080" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#383838" />
            </asp:GridView>
                    
        </div>  
</div>
