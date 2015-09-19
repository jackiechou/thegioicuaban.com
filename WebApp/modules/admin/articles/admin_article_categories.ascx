<%@ Control Language="C#" AutoEventWireup="true" Inherits="user_controls_admin_controls_admin_article_categories" Codebehind="admin_article_categories.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
   <style type="text/css">
.GVFixedHeader  
{
    font: bold 11px "Trebuchet MS", Verdana, Arial;	      
    background-color: #EDEDED;    
    overflow: auto;     
    position:fixed;
    left: 100;
    /*height:25px;position:absolute;*/
}
.GVFixedFooter  
{
    font: bold 11px "Trebuchet MS", Verdana, Arial;	
    background-color: #414141; 
    color:#FFFFFF; 
    /*position:relative;
    bottom:expression(getScrollBottom(this.parentNode.parentNode.parentNode.parentNode));
      */ 
}
</style>

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

    <%----------------------------- ModalPopupExtender JavaScript -----------------------------%>                                                                                                                                                                                                                                    
        <script language="javascript" type="text/javascript">
            var add_url = '<%=add_url %>';
            var edit_url = '<%=edit_url %>';
            var loading_url = '<%=loading_url %>';

            function ShowAddModal() {
                var frame = $get('IframeAdd');
                frame.src = add_url;
                $find('AddModalPopup').show();
            }

            function AddCancelScript() {
                var frame = $get('IframeAdd');
                frame.src = "loading.aspx";
            }

            function AddOkayScript() {
                AddCancelScript();
            }

            function ShowModalEdit() {
                //Get hidden field that wil contain string of selected item's Ids separated by '|'.
                var SelectedValues = document.getElementById('<%= this.hdnFldSelectedValues.ClientID %>').value;
                if (SelectedValues != '') {
                    if (SelectedItems.length == 1) {                        
                        var frame = $get('IframeEdit');
                        frame.src = edit_url + SelectedValues;
                        $find('EditModalPopup').show();
                    } else {
                        alert('Vui lòng chỉ check chọn 1 item để edit');
                    }
                }
            }

            function ShowEditModal(idx) {                
                var frame = $get('IframeEdit');
                frame.src = edit_url + idx;
                $find('EditModalPopup').show();
            }


            function EditCancelScript() {
                var frame = $get('IframeEdit');
                frame.src = loading_url;
            }

            function EditOkayScript() {
                EditCancelScript();
            }

            function RefreshDataGrid() {
                $get('btnReload').click();
            }           
    </script>

    
<%----------------------------- ModalPopupExtender_Add -------------------------------------%>                                                                                                  
    <ajaxToolkit:ModalPopupExtender ID="btnAdd_ModalPopupExtender" BackgroundCssClass="ModalPopupBG"
    runat="server" OkControlID="btnOkay" CancelControlID="btnCancel" TargetControlID="btnAdd"
    PopupControlID="DivAddWindow" Drag="true" PopupDragHandleControlID="PopupHeader" 
    OnOkScript="ShowAddModal();" OnCancelScript="AddCancelScript" BehaviorID="AddModalPopup">
</ajaxToolkit:ModalPopupExtender>
<div class="popup_Buttons" style="display: none">
    <input id="btnOkay" value="Done" type="button" />
    <input id="btnCancel" value="Cancel" type="button" />
</div>
<div id="DivAddWindow" style="display: none;" class="popupConfirmation">
    <iframe id="IframeAdd" frameborder="0" height="600" width="700" scrolling="no"></iframe>
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
    <iframe id="IframeEdit" frameborder="0" height="600" width="700" scrolling="no"></iframe>
</div>   

<div class="wrap_gp" > 
    <div class="commontool">
        <h3>Quản lý phân loại tin tức</h3>
        <div class="toolbar">          
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClientClick="ShowAddModal();return false;"><span class="icon-32-new"></span>Thêm</asp:LinkButton> 
            <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClientClick="ShowModalEdit();return false;"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>                    
            <asp:LinkButton ID="btnLock" runat="server" CssClass="btn" OnClick="btnLock_Click"><span class="icon-32-publish"></span>Khóa</asp:LinkButton>
            <asp:LinkButton ID="btnUnLock" runat="server" CssClass="btn" OnClick="btnUnLock_Click"><span class="icon-32-unpublish"></span>Mở Khóa</asp:LinkButton>                                          
            <asp:LinkButton ID="btnReload" runat="server" CssClass="btn" OnClick="btnReload_Click" Visible="false"><span class="icon-32-unpublish"></span>Reload</asp:LinkButton>         
            <asp:HiddenField ID="hdnFldSelectedValues" runat="server" />    
        </div>
    </div>   
    <div class="group_commands">       
            <div class="left">                                                      
            </div> 
            <div class="right">
                <div class="fillter_tool">                                               
                    Portal :<asp:DropDownList ID="ddlPortalList" runat="server" onselectedindexchanged="ddlPortalList_SelectedIndexChanged"></asp:DropDownList>  
                    &nbsp;
                    Culture: <asp:DropDownList ID="ddlCultureList" runat="server" onselectedindexchanged="ddlCultureList_SelectedIndexChanged"></asp:DropDownList>  
                 </div>  
            </div>   
    </div>
    <div style="clear:both;"></div> 
            
     <div class="group_panel" >
       <asp:UpdatePanel ID="UpdatePanel1" EnableViewState="true" UpdateMode="Conditional" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="ddlPortalList" />
                <asp:PostBackTrigger ControlID="ddlCultureList" />
            </Triggers>
            <ContentTemplate>    
          <asp:GridView ID="GridView1" DataKeyNames="CategoryId" runat="server"  CssClass="table_list" 
             AllowPaging="True" Width="100%" Height="100%"  PageSize="10" 
            AllowSorting="True" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
            CellPadding="3" EmptyDataText="No Data" GridLines="Vertical"         
           BorderStyle="Solid" BorderWidth="1px" ForeColor="Black"  BorderColor="#999999"
              onpageindexchanged="GridView1_SelectedIndexChanged" 
            onsorting="GridView1_Sorting" 
            onrowcancelingedit="GridView1_RowCancelingEdit" onrowcreated="GridView1_RowCreated" 
            onrowdeleting="GridView1_RowDeleting" onrowediting="GridView1_RowEditing" 
            onselectedindexchanged="GridView1_SelectedIndexChanged" 
            onselectedindexchanging="GridView1_SelectedIndexChanging" 
            ondatabound="GridView1_DataBound" 
            onpageindexchanging="GridView1_PageIndexChanging"
              onrowdatabound="GridView1_RowDataBound" BackColor="White"  
              onrowcommand="GridView1_RowCommand" >
            <AlternatingRowStyle BackColor="#CCCCCC" />
            <Columns>
                <asp:TemplateField HeaderText="x">                        
                    <HeaderTemplate>
                        <asp:CheckBox ID="chkBxHeader" onclick="javascript:HeaderClick(this);" runat="server" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkBxSelect" runat="server" />
                        <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("CategoryId") %>' />
                        <asp:HyperLink ID="hplGetRowSelected" runat="server">Chọn</asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:TemplateField HeaderText="No">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>            
                </asp:TemplateField>                 
                <asp:BoundField DataField="CategoryId" HeaderText="CategoryId" SortExpression="CategoryId" />                                              
                <asp:BoundField DataField="CategoryCode" HeaderText="CategoryCode" SortExpression="CategoryCode" />                   
                <asp:BoundField DataField="CategoryName" HeaderText="CategoryName" SortExpression="CategoryName" />  
                <asp:BoundField DataField="Alias" HeaderText="Alias" />           
                <asp:BoundField DataField="Description" HeaderText="Description"/> 
                <asp:BoundField DataField="ParentId" HeaderText="ParentId"/> 
                <asp:TemplateField HeaderText="Status">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtStatus" runat="server" Text='<%# Bind("Status") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                        </ItemTemplate>
                </asp:TemplateField>   
                <asp:BoundField DataField="SortKey" HeaderText="SortKey"/>                
                <asp:TemplateField HeaderText="Location">
                    <ItemTemplate>
                    <asp:Button ID="btnUp" CommandName="Up" ToolTip="UP" Text="&uArr;" ForeColor="White" Height="20px" Font-Bold="true" BackColor="#E07200" runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" /> 
                    <asp:Button ID="btnDown" CommandName="Down" ToolTip="Down" Text="&dArr;" ForeColor="White" Height="20px" Font-Bold="true" BackColor="#E07200" runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                    </ItemTemplate>
                </asp:TemplateField>                                                  
            </Columns>
           <EmptyDataTemplate>No Data</EmptyDataTemplate>
              <FooterStyle BackColor="#CCCCCC" />
              <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
              <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
           <PagerTemplate>
               <div class="pagination">
                        <asp:ImageButton ID="imgBtnFirst" runat="server"
                            ImageUrl="~/images/icons/arrow_first.png"
                            CommandArgument="First" CommandName="Page" Height="22px" Width="26px" />
                            <asp:ImageButton ID="imgBtnPrev" runat="server"
                            ImageUrl="~/images/icons/arrow_previous.png"
                            CommandArgument="Prev" CommandName="Page" Height="23px" Width="29px" />
                                 Page
                        <asp:DropDownList ID="ddlPages" runat="server"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged"> </asp:DropDownList> of <asp:Label ID="lblPageCount"
                            runat="server"></asp:Label>
                        <asp:ImageButton ID="imgBtnNext" runat="server"
                            ImageUrl="~/images/icons/arrow_next.png"
                            CommandArgument="Next" CommandName="Page" Height="21px" Width="27px" />
                        <asp:ImageButton ID="imgBtnLast" runat="server"
                            ImageUrl="~/images/icons/arrow_last.png"
                            CommandArgument="Last" CommandName="Page" Height="21px" Width="31px" />
                </div>
           </PagerTemplate>             
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
