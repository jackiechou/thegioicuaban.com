<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="admin_skin_packages.ascx.cs" Inherits="WebApp.modules.admin.ui.admin_skin_packages" %>
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

     <%----- ModalPopupExtender JavaScript -------%>  
  <script language="javascript" type="text/javascript">
      var add_path = "/modules/admin/ui/admin_skin_packages_edit.aspx?mode=add";
      var edit_path = "/modules/admin/ui/admin_skin_packages_edit.aspx?mode=edit&idx=";

      function ShowAddModal() {
          var frame = $get('IframeAdd');
          frame.src = add_path;
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
          //Get hidden field that wil contain string of selected item's Ids separated by '|'.
          var SelectedValues = document.getElementById('<%= this.hdnFldSelectedValues.ClientID %>').value;
          if (SelectedValues != '') {
              if (SelectedItems.length == 1) {
                  var frame = $get('IframeEdit');
                  frame.src = edit_path + SelectedValues;
                  $find('EditModalPopup').show();
              } else {
                  alert('Vui lòng chỉ check chọn 1 item để edit');
              }
          }
      }

      function ShowEditModal(idx) {
          var frame = $get('IframeEdit');
          frame.src = edit_path + idx;
          $find('EditModalPopup').show();
      }

      function EditCancelScript() {
          var frame = $get('IframeEdit');
          frame.src = "loading.aspx";
      }

      function EditOkayScript() {
          RefreshDataGrid();
          EditCancelScript();
      }

      function RefreshDataGrid() {
          $get('btnReload').click();
      } 

</script>   
           
<div class="wrap_gp" >      
    <div class="commontool">
        <h3>QUẢN LÝ HỆ THỐNG SKIN PACKAGE</h3>
        <div class="toolbar">          
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClientClick="ShowAddModal();return false;"><span class="icon-32-new"></span>Thêm</asp:LinkButton>
            <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClientClick="ShowModalEdit();return false;"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>                                                 
            <asp:LinkButton ID="btnReload" runat="server" CssClass="btn" OnClick="btnReload_Click" Visible="false"><span class="icon-32-unpublish"></span>Reload</asp:LinkButton>         
            <asp:HiddenField ID="hdnFldSelectedValues" runat="server" />    
        </div>
    </div>      
    <div class="group_commands">       
            <div class="left">                                                      
            </div> 
            <div class="right">
                <div class="fillter_tool">
                    <asp:DropDownList ID="ddlApplicationList" runat="server" CssClass="combobox" onselectedindexchanged="ddlApplicationList_SelectedIndexChanged"></asp:DropDownList>   
                    &nbsp;
                    Portal :<asp:DropDownList ID="ddlPortalList" runat="server" onselectedindexchanged="ddlPortalList_SelectedIndexChanged"></asp:DropDownList>  
                    &nbsp;
                    <asp:DropDownList ID="ddlSkinTypeList" runat="server" AutoPostBack="true"
                        onselectedindexchanged="ddlSkinTypeList_SelectedIndexChanged"></asp:DropDownList>
                </div>  
            </div>   
    </div>
    <div style="clear:both;"></div>

    <div class="group_panel">   
        <asp:GridView ID="GridView1" runat="server" DataKeyNames="SkinPackageId" Width="100%" Height="100%"  PageSize="30" 
        AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" 
        BorderStyle="Solid" BorderWidth="1px" CellPadding="3" EmptyDataText="No Data" AllowPaging="True"
        EnableModelValidation="True" ForeColor="Black" ShowFooter="True" 
        onpageindexchanging="GridView1_PageIndexChanging" 
        onrowcreated="GridView1_RowCreated" onrowdatabound="GridView1_RowDataBound" 
        onselectedindexchanged="GridView1_SelectedIndexChanged" 
        onselectedindexchanging="GridView1_SelectedIndexChanging" >
        <AlternatingRowStyle BackColor="#CCCCCC" />
        <Columns>
            <asp:TemplateField HeaderText="x">
            <EditItemTemplate>                                         
                <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("SkinPackageId") %>' Width="30px"></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkBxSelect" runat="server" />
                <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("SkinPackageId") %>' />
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
            <asp:BoundField DataField="SkinPackageId" HeaderText="SkinPackageId" SortExpression="SkinPackageId" />
            <asp:BoundField DataField="PortalId" HeaderText="PortalId" SortExpression="PortalId" />    
            <asp:BoundField DataField="SkinName" HeaderText="SkinName" SortExpression="SkinName" />    
            <asp:BoundField DataField="SkinType" HeaderText="SkinType" SortExpression="SkinType" />        
            <asp:TemplateField HeaderText="SkinIcon">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Eval("SkinIcon") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server" Width="50px" Height="50px" 
                        ImageUrl='<%# Eval("SkinIcon", "~/user_files/images/skin_images/{0}") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <RowStyle HorizontalAlign="Center" />
        <EmptyDataTemplate>
            No Data
        </EmptyDataTemplate>
        <PagerTemplate>
                <asp:ImageButton ID="ImageButton1" runat="server" BorderWidth="0" BorderStyle="None"
                    ImageUrl="~/images/icons/arrow_first.png"
                    CommandArgument="First" CommandName="Page" Height="22px" Width="26px" />
                    <asp:ImageButton ID="ImageButton2" runat="server" BorderWidth="0" BorderStyle="None"
                    ImageUrl="~/images/icons/arrow_previous.png"
                    CommandArgument="Prev" CommandName="Page" Height="23px" Width="29px" />
                            Page
                <asp:DropDownList ID="ddlPages" runat="server" BorderWidth="0" BorderStyle="None"
                    AutoPostBack="True" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged">
                </asp:DropDownList> of <asp:Label ID="lblPageCount"
                    runat="server"></asp:Label>
                <asp:ImageButton ID="ImageButton3" runat="server" BorderWidth="0" BorderStyle="None"
                    ImageUrl="~/images/icons/arrow_next.png"
                    CommandArgument="Next" CommandName="Page" Height="21px" Width="27px" />
                <asp:ImageButton ID="ImageButton4" runat="server" BorderWidth="0" BorderStyle="None"
                    ImageUrl="~/images/Icons/arrow_last.png"
                    CommandArgument="Last" CommandName="Page" Height="21px" Width="31px" />
        </PagerTemplate> 
        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" 
                Height="20px" />
        <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" 
                HorizontalAlign="Center" />
    </asp:GridView>
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