<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="admin_languages.ascx.cs" Inherits="WebApp.modules.admin.languages.admin_languages" %>

<%--begin Gridview Fixed Header--%>
<script type='text/javascript' src="../../../../scripts/js/x.js"></script>
<script type='text/javascript' src="../../../../scripts/js/xtableheaderfixed.js"></script>
<script type='text/javascript'>
    xAddEventListener(window, 'load', function () { new xTableHeaderFixed('GridView1', 'panelContainer', 0); }, false);   
</script>
<script type="text/javascript">
    function SelectAllCheckboxes(spanChk) {
        var IsChecked = spanChk.checked;
        var Chk = spanChk;
        Parent = Chk.form.elements;
        for (i = 0; i < Parent.length; i++) {
            if (Parent[i].type == "checkbox" && Parent[i].id != Chk.id) {
                if (Parent[i].checked != IsChecked)
                    Parent[i].click();
            }
        }
    }
    function SelectAllCheckboxesSpecific(spanChk) {

        var IsChecked = spanChk.checked;
        var Chk = spanChk;
        Parent = document.getElementById('<%= this.GridView1.ClientID %>');
        var items = Parent.getElementsByTagName('input');
        for (i = 0; i < items.length; i++) {
            if (items[i].id != Chk && items[i].type == "checkbox") {
                if (items[i].checked != IsChecked) {
                    items[i].click();
                }
            }
        }
    }

    function SelectAllCheckboxesMoreSpecific(spanChk) {
        var IsChecked = spanChk.checked;
        var Chk = spanChk;
        Parent = document.getElementById('<%= this.GridView1.ClientID %>');
        for (i = 0; i < Parent.rows.length; i++) {
            var tr = Parent.rows[i];
            //var td = tr.childNodes[0];			   		   
            var td = tr.firstChild;
            var item = td.firstChild;
            if (item.id != Chk && item.type == "checkbox") {
                if (item.checked != IsChecked) {
                    item.click();
                }
            }
        }
    }


    function HighlightRow(chkB) {
        var IsChecked = chkB.checked;
        if (IsChecked) {
            chkB.parentElement.parentElement.style.backgroundColor = '#228b22';  // GridView1.SelectedItemStyle.BackColor
            chkB.parentElement.parentElement.style.color = 'white'; // GridView1.SelectedItemStyle.ForeColor
        }
        else {
            chkB.parentElement.parentElement.style.backgroundColor = 'white'; //GridView1.ItemStyle.BackColor
            chkB.parentElement.parentElement.style.color = 'black'; //GridView1.ItemStyle.ForeColor
        }
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

    //=================================================================================================  
</script>
<%----- ModalPopupExtender JavaScript -------%>                                                                                                  
 <script language="javascript" type="text/javascript">
     function ShowAddModal() {
         var frame = $get('IframeAdd');
         frame.src = "modules/admin/languages/admin_languages_edit.aspx?mode=add";
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
         var frame = $get('IframeEdit');
         var edit_path = "modules/admin/languages/admin_languages_edit.aspx?mode=edit&idx=";
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


<div class="wrap_panel">  
    <div class="common_tool">
        <h3>Quản lý ngôn ngữ</h3>
        <div class="toolbar">   
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn" OnClientClick="ShowAddModal();return false;"><span class="icon-32-new"></span>Thêm</asp:LinkButton>             
            <asp:LinkButton ID="btnLock" runat="server" CssClass="btn" OnClick="btnLock_Click"><span class="icon-32-publish"></span>Khóa</asp:LinkButton>
            <asp:LinkButton ID="btnUnLock" runat="server" CssClass="btn" OnClick="btnUnLock_Click"><span class="icon-32-unpublish"></span>Mở Khóa</asp:LinkButton>                                                      
            <asp:LinkButton ID="btnReload" runat="server" CssClass="btn" OnClick="btnReload_Click" Visible="false"><span class="icon-32-unpublish"></span>Reload</asp:LinkButton>         
        </div>
    </div>                                
    <div class="common_commands">              
        <div class="left">    
         &nbsp;                                                                
        </div> 
        <div class="right">
            <div class="fillter_tool">                              
                   <asp:DropDownList ID="ddlDiscontinued" runat="server" onselectedindexchanged="ddlDiscontinued_SelectedIndexChanged"></asp:DropDownList>  
            </div>  
        </div>   
    </div>
    <div class="content_panel">                      
            <div id='panelContainer'>
                <asp:GridView ID="GridView1" CssClass="GridView1" runat="server"  Width="100%"
                    AutoGenerateColumns="False"  DataKeyNames="CultureId" CellPadding="3"
                    BackColor="WhiteSmoke" ShowHeaderWhenEmpty="True"
                    AllowPaging="True" AllowSorting="True" AlternatingRowStyle-BackColor="#FFFFFF"
                    RowStyle-VerticalAlign="Bottom" 
                        onrowdatabound="GridView1_RowDataBound" onprerender="GridView1_PreRender" 
                    EmptyDataText="No Data" ondatabound="GridView1_DataBound" 
                    onpageindexchanging="GridView1_PageIndexChanging" 
                    onrowcancelingedit="GridView1_RowCancelingEdit" 
                     onrowediting="GridView1_RowEditing" 
                    onselectedindexchanged="GridView1_SelectedIndexChanged" 
                    onselectedindexchanging="GridView1_SelectedIndexChanging" 
                    onsorting="GridView1_Sorting"
                    >                 
<AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
                    <Columns>
                        <asp:TemplateField HeaderText="x">
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkBxHeader" onclick="javascript:SelectAllCheckboxesSpecific(this);" runat="server" />
                            </HeaderTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtIdx" runat="server" Text='<%# Bind("CultureId") %>' 
                                    Width="30px"></asp:TextBox>
                            </EditItemTemplate>                       
                            <ItemTemplate>                           
                                <asp:CheckBox ID="chkBxSelect" onclick="javascript:HighlightRow(this);" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>                   
                        <asp:TemplateField HeaderText="No">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>                       
                        </asp:TemplateField>
                        <asp:BoundField DataField="CultureId" HeaderText="CultureId" />
                        <asp:BoundField DataField="CultureCode" HeaderText="CultureCode" />
                        <asp:BoundField DataField="CultureName" HeaderText="CultureName"/>                      
                        <asp:CheckBoxField DataField="Discontinued" HeaderText="Discontinued" />
                    </Columns>
                    <EmptyDataTemplate>
                        No Data
                    </EmptyDataTemplate>
                                      
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
                        <RowStyle VerticalAlign="Bottom"></RowStyle>
                </asp:GridView>  
                        
            </div>               
           
     </div>
 </div>