﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="admin_article_comments" Codebehind="admin_article_comments.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

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
     //===============================================================================
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
     //This Function is used to change the ROW backGroundColor on MouseOver. 
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
    </script>

    <%----------------------------- ModalPopupExtender JavaScript -----------------------------%>                                                                                                                                                                                                                                    
        <script type="text/javascript">
            var edit_url = "/modules/admin/articles/admin_article_comments_edit.aspx?mode=edit&idx=";
            var loading_url = "/loading.aspx";

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
                RefreshDataGrid();
                EditCancelScript();
            }

            function RefreshDataGrid() {
                $get('btnReload').click();
            }
    </script> 
<div class="wrap_gp">  
    <div class="commontool">
        <h3>Quản lý bình luận tin tức trực tuyến</h3>
        <div class="toolbar">      
            <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClientClick="ShowModalEdit();return false;"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>                    
            <asp:LinkButton ID="btnLock" runat="server" CssClass="btn" OnClick="btnLock_Click"><span class="icon-32-publish"></span>Khóa</asp:LinkButton>
            <asp:LinkButton ID="btnUnLock" runat="server" CssClass="btn" OnClick="btnUnLock_Click"><span class="icon-32-unpublish"></span>Mở Khóa</asp:LinkButton>              
            <asp:LinkButton ID="btnMultipleDelete" runat="server" CssClass="btn" 
                onclick="btnMultipleDelete_Click"><span class="icon-32-trash"></span>Xóa nhiều dòng</asp:LinkButton>                                                                                    
                <asp:HiddenField ID="hdnFldSelectedValues" runat="server" /> 
        </div>
    </div>
    <div class="group_commands">        
        <div class="fleft">
         Portal :<asp:DropDownList ID="ddlPortalList" runat="server" onselectedindexchanged="ddlPortalList_SelectedIndexChanged"></asp:DropDownList>  
                    &nbsp;
                    Culture: <asp:DropDownList ID="ddlCultureList" runat="server" onselectedindexchanged="ddlCultureList_SelectedIndexChanged"></asp:DropDownList>  
        </div>
        <div class="fright">
                Nhóm tin tức:                   
                <asp:DropDownList ID="ddlCategory" runat="server"  CssClass="combobox" 
                    onselectedindexchanged="ddlCategory_SelectedIndexChanged">
                </asp:DropDownList>
                &nbsp; Tin tức:                   
                <asp:DropDownList ID="ddlArticles" runat="server"  CssClass="combobox" 
                    onselectedindexchanged="ddlArticles_SelectedIndexChanged" >
                </asp:DropDownList>
                &nbsp;&nbsp; Trạng thái:
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="combobox" onselectedindexchanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>  

         </div>               
    </div>
    <div style="clear:both;">&nbsp;</div>             
    <div class="group_panel" >
        <asp:UpdatePanel ID="UpdatePanel1" EnableViewState="true" UpdateMode="Conditional" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="ddlPortalList" />
                <asp:PostBackTrigger ControlID="ddlCultureList" />
                <asp:PostBackTrigger ControlID="ddlCategory" />
                <asp:PostBackTrigger ControlID="ddlStatus" />
                <asp:PostBackTrigger ControlID="ddlArticles" />
            </Triggers>
            <ContentTemplate>  
            <asp:GridView ID="GridView1" DataKeyNames="CommentId" runat="server" 
                AllowPaging="True" Width="100%" Height="100%" ShowHeaderWhenEmpty="true" PageSize="10"   
            AllowSorting="True" AutoGenerateColumns="False" BackColor="White" 
            BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" 
            CellPadding="10" EmptyDataText="No Data" ForeColor="Black"
            ShowFooter="false"  EnableModelValidation="True"
            onpageindexchanged="GridView1_SelectedIndexChanged" 
            onrowcancelingedit="GridView1_RowCancelingEdit" 
             onrowediting="GridView1_RowEditing" 
            onselectedindexchanged="GridView1_SelectedIndexChanged" 
            onselectedindexchanging="GridView1_SelectedIndexChanging" 
            ondatabound="GridView1_DataBound" 
            onpageindexchanging="GridView1_PageIndexChanging" 
            onrowdatabound="GridView1_RowDataBound" >
            <AlternatingRowStyle BackColor="#CCCCCC" />
            <Columns>
                <asp:TemplateField HeaderText="x">
                <EditItemTemplate>                                         
                    <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("CommentId") %>' Width="30px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkBxSelect" runat="server" />
                    <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("CommentId") %>' />
                </ItemTemplate>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkBxHeader" onclick="javascript:HeaderClick(this);" runat="server" />
                </HeaderTemplate>
            </asp:TemplateField>                                      
                <asp:BoundField DataField="CommentId" HeaderText="CommentId" />
                <asp:BoundField DataField="CommentName" HeaderText="CommentName" />
                <asp:BoundField DataField="CommentEmail" HeaderText="CommentEmail" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Publish") %>'></asp:Label>                               
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtStatus" runat="server" Text='<%# Bind("Publish") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>                
            </Columns>
            <EmptyDataTemplate>No Data</EmptyDataTemplate>
                <PagerTemplate>
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
                </PagerTemplate> 
                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
        </asp:GridView>           
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
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
    <iframe id="IframeEdit" frameborder="0"  width="900" height="600" scrolling="no">
    </iframe>
</div>   