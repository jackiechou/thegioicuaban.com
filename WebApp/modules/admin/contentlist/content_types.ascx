<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="content_types.ascx.cs" Inherits="WebApp.modules.admin.contentlist.content_types" %>
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

    //This Function is used to change the ROW backGroundColor on Click.
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
         frame.src = "loading.aspx";
     }

     function EditOkayScript() {
         EditCancelScript();
     }   
</script>

<div class="wrap_gp" >   
            <div class="commontool">
                <h3>Quản lý Content List</h3>
                <div class="toolbar">           
                    <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClientClick="ShowAddModal();return false;"><span class="icon-32-new"></span>Thêm</asp:LinkButton>  
                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn" OnClientClick="ShowModalEdit();return false;"><span class="icon-32-edit"></span>Sửa</asp:LinkButton>             
                    <asp:LinkButton ID="btnMultipleDelete" runat="server" CssClass="btn" OnClick="btnMultipleDelete_Click"><span class="icon-32-trash"></span>Xóa nhiều dòng</asp:LinkButton>        
                    <asp:HiddenField ID="hdnFldSelectedValues" runat="server" />    
                </div>
            </div>       
             <div class="group_commands">              
                <div class="left">&nbsp;</div> 
                <div class="right">
                    <div class="fillter_tool">&nbsp;</div>  
                </div>   
             </div>
            <div style="clear:both;"></div>
                  
            <div class="group_panel">
             <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">  
                <ContentTemplate>   
                    <asp:GridView ID="GridView1" runat="server" DataKeyNames="ContentTypeID" 
                        CssClass="table_list" Width="100%" Height="100%"
                        AutoGenerateColumns="False" EmptyDataText="No Data" 
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
                                    <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("ContentTypeID") %>' Width="30px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkBxSelect" runat="server" />
                                    <asp:HiddenField ID="hdnFldId" runat="server" Value='<%# Eval("ContentTypeID") %>' />
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
                            <asp:BoundField DataField="ContentTypeID" HeaderText="ContentTypeID" />                                 
                            <asp:BoundField DataField="ContentType" HeaderText="ContentType" />                                                                        
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
                      </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID = "GridView1" />
                            <asp:AsyncPostBackTrigger ControlID = "btnMultipleDelete" />                            
                            <%--<asp:AsyncPostBackTrigger ControlID = "btnOkay" />--%>
                        </Triggers>
                    </asp:UpdatePanel>
             </div>
  
</div>




<%----------------------------- ModalPopupExtender_Add -------------------------------------%>                                                                                                  
 <ajaxToolkit:ModalPopupExtender ID="btnAdd_ModalPopupExtender" BackgroundCssClass="ModalPopupBG"
    runat="server" CancelControlID="btnCancel" OkControlID="btnOkay" TargetControlID="btnAdd" BehaviorID="AddModalPopup"
    PopupControlID="Panel1" Drag="true" PopupDragHandleControlID="PopupHeader" OnOkScript="ShowAddModal();">
</ajaxToolkit:ModalPopupExtender>
<div class="popup_Buttons" style="display: none">
    <input id="btnOkay" value="Done" type="button" />
    <input id="btnCancel" value="Cancel" type="button" />
</div>
<div id="Panel1" style="display: none;" class="popupConfirmation">
    <iframe id="IframeAdd" frameborder="0" width="400" height="403"
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
    <iframe id="IframeEdit" frameborder="0" height="403" width="700px" scrolling="no">
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