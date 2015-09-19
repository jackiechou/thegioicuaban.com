<%@ Control Language="C#" AutoEventWireup="true" Inherits="admin_banner_positions" Codebehind="admin_banner_positions.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

  <%--begin Gridview Fixed Header--%>
<script type='text/javascript' src="../../../../scripts/js/x.js"></script>
<script type='text/javascript' src="../../../../scripts/js/xtableheaderfixed.js"></script>
<script type='text/javascript'>
    xAddEventListener(window, 'load', function () { new xTableHeaderFixed('GridView1', 'panelContainer', 0); }, false);   
</script> 
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
  
  <%----- ModalPopupExtender JavaScript -------%>                                                                                                  
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
         RefreshDataGrid();
         AddCancelScript();
     }



     function ShowEditModal(idx) {
         var edit_path = edit_url;
         var frame = $get('IframeEdit');
         frame.src = edit_path + idx;
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
           
<div class="wrap_panel">  
    <div class="common_tool">
        <h3>Quản lý vị trí Banner</h3>
        <div class="toolbar">   
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClientClick="ShowAddModal();return false;"><span class="icon-32-new"></span>Thêm</asp:LinkButton>
            <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClientClick="ShowModalEdit();return false;"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>             
            <asp:LinkButton ID="btnLock" runat="server" CssClass="btn" OnClick="btnLock_Click"><span class="icon-32-publish"></span>Khóa</asp:LinkButton>
            <asp:LinkButton ID="btnUnLock" runat="server" CssClass="btn" OnClick="btnUnLock_Click"><span class="icon-32-unpublish"></span>Mở Khóa</asp:LinkButton>              
            <asp:LinkButton ID="btnMultipleDelete" runat="server" CssClass="btn" OnClick="btnMultipleDelete_Click"><span class="icon-32-trash"></span>Xóa nhiều dòng</asp:LinkButton>        
            <asp:HiddenField ID="hdnFldSelectedValues" runat="server" />
        </div>
    </div>                                
    <div class="common_commands">              
        <div class="left">    
         &nbsp;                                                                
        </div> 
        <div class="right">
            <div class="fillter_tool">                              
                   &nbsp;  
            </div>  
        </div>   
    </div>
    <div class="content_panel">                      
         <div id='panelContainer'>
                <asp:GridView ID="GridView1" runat="server" DataKeyNames="Id" Width="100%" Height="100%" 
                AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" 
                BorderStyle="Solid" BorderWidth="1px" CellPadding="3" EmptyDataText="No Data" 
                EnableModelValidation="True" ForeColor="Black" ShowFooter="True" 
                onpageindexchanging="GridView1_PageIndexChanging" 
                onpageindexchanged="GridView1_SelectedIndexChanged" 
                onsorting="GridView1_Sorting" 
                onrowcreated="GridView1_RowCreated" 
                onrowdatabound="GridView1_RowDataBound" 
                ondatabound="GridView1_DataBound" 
                onselectedindexchanged="GridView1_SelectedIndexChanged" 
                onselectedindexchanging="GridView1_SelectedIndexChanging" 
                onrowdeleting="GridView1_RowDeleting" 
                onrowediting="GridView1_RowEditing"  
                onrowcancelingedit="GridView1_RowCancelingEdit" 
                onrowcommand="GridView1_RowCommand" 
                    PageSize="30" >

                <AlternatingRowStyle BackColor="#CCCCCC" />
                <Columns>
                     <asp:TemplateField HeaderText="x">
                        <EditItemTemplate>                                         
                            <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("Id") %>' Width="30px"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkBxSelect" runat="server" />
                            <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("Id") %>' />
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
                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" 
                        ReadOnly="True" SortExpression="Id" />
                    <asp:BoundField DataField="BannerPosition" HeaderText="BannerPosition" SortExpression="BannerPosition" />     
                    <asp:TemplateField HeaderText="PostedDate" SortExpression="PostedDate">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPostedDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"PostedDate","{0:MM/dd/yyyy}") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblPostedDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"PostedDate","{0:MM/dd/yyyy}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SortKey" SortExpression="SortKey">
                        <ItemTemplate>                                
                            <asp:TextBox ID="txtSortKey" runat="server" Text='<%# Bind("SortKey") %>' Width="20px"></asp:TextBox>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditSortKey" runat="server" Text='<%# Bind("SortKey") %>' Width="20px"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="Status">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtStatus" runat="server" Text='<%# Bind("Status") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:ButtonField Text="btnUpdateSortKey" ControlStyle-BorderStyle="None" 
                            ControlStyle-BorderWidth="0" CommandName="UpdateSortKey" ButtonType="Image" 
                            ImageUrl="~/images/icons/edit_icon01.gif" >                            
                      </asp:ButtonField>                              
                    
                </Columns>
                  <FooterStyle BackColor="#CCCCCC" />                
                <EmptyDataTemplate>
                    No Data
                </EmptyDataTemplate>
               <PagerTemplate>
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
            </PagerTemplate> 
                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" Height="20px" HorizontalAlign="Center" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" 
                        HorizontalAlign="Center" />
            </asp:GridView>             
         </div>
    </div>
</div>

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
<iframe id="IframeAdd" frameborder="0" width="600" height="500" scrolling="no"></iframe>
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
    <iframe id="IframeEdit" frameborder="0"  width="600" height="500" scrolling="no">
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