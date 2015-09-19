<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="permission_modules_add.aspx.cs" Inherits="WebApp.modules.admin.security.roles.permission_modules_add" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
      
        <script type="text/javascript">
            function getbacktostepone() {
                window.location = "permission_tabs_add.aspx";
            }
            function onSuccess() {
                setTimeout(okay, 2000);
            }
            function onError() {
                setTimeout(getbacktostepone, 2000);
            }
            function okay() {
                parent.location.href = parent.location.href;
                window.parent.document.getElementById('ButtonAddModulePermissionCancel').click();
            }
            function cancel() {
                window.parent.document.getElementById('ButtonAddModulePermissionCancel').click();
            }

            // disables the button specified and sets its style to a disabled "look".
            function disableButtonOnClick(oButton, sButtonText, sCssClass) {
                oButton.disabled = true;      // set button to disabled so you can't click on it.
                oButton.value = sButtonText;   // change the text of the button.
                oButton.setAttribute('className', sCssClass); // IE uses className for the css property.
                oButton.setAttribute('class', sCssClass); // Firefox, Safari use class for the css property.  (doesn't hurt to do both).
            }

            function data_change(field) {
                var check = true;
                var value = field.value; //get characters
                //check that all characters are digits, ., -, or ""
                for (var i = 0; i < field.value.length; ++i) {
                    var new_key = value.charAt(i); //cycle through characters
                    if (((new_key < "0") || (new_key > "9")) &&
                    !(new_key == "")) {
                        check = false;
                        break;
                    }
                }
                //apply appropriate colour based on value
                if (!check) {
                    field.style.backgroundColor = "red";
                }
                else {
                    field.style.backgroundColor = "white";
                }
            }
            function Check_Click(objRef) {
                //Get the Row based on checkbox
                var row = objRef.parentNode.parentNode;

                if (objRef.checked) {
                    //If checked change color to Aqua
                    row.style.backgroundColor = "aqua";
                }

                else {
                    //If not checked change back to original color
                    if (row.rowIndex % 2 == 0) {
                        //Alternating Row Color
                        row.style.backgroundColor = "#C2D69B";
                    }

                    else {
                        row.style.backgroundColor = "white";
                    }
                }

                //Get the reference of GridView
                //            var GridView = row.parentNode;

                //            //Get all input elements in Gridview
                //            var inputList = GridView.getElementsByTagName("input");

                //            for (var i = 0; i < inputList.length; i++) {
                //                //The First element is the Header Checkbox
                //                var headerCheckBox = inputList[0];

                //                //Based on all or none checkboxes are checked check/uncheck Header Checkbox
                //                var checked = true;

                //                if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                //                    if (!inputList[i].checked) {
                //                        checked = false;
                //                        break;
                //                    }
                //                }
                //            }
                //            headerCheckBox.checked = checked;
            }
            //=================================================================================================================



            //Hieu ung chuot tren gridview ====================================================================================
            function MouseEvents(objRef, evt) {
                var checkbox = objRef.getElementsByTagName("input")[0];
                if (evt.type == "mouseover") {
                    objRef.style.backgroundColor = "orange";
                }
                else {
                    if (checkbox.checked)
                        objRef.style.backgroundColor = "aqua";
                    else if (evt.type == "mouseout") {   //Alternating Row Color
                        if (objRef.rowIndex % 2 == 0)
                            objRef.style.backgroundColor = "#C2D69B";
                        else
                            objRef.style.backgroundColor = "white";
                    }
                }
            }
            //===================================================================================================================
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />

    <div class="popup_Container">
        <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                Edit
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">         
           <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ViewInput" runat="server">
                     <script type="text/javascript" src="../../../../scripts/jquery/jquery.min.js"></script> 
                       <script type="text/javascript">
                           jQuery(document).ready(function ($) {
                               //PERMISSION ==================================================================================================
                               $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllPermissionRight']:checkbox").click(function () {
                                   if ($(this).is(':checked')) {
                                       $("#<%=GridView1.ClientID%> input[id*='chkSelectedPermissionRight']:checkbox").attr('checked', true);
                                   } else
                                       $("#<%=GridView1.ClientID%> input[id*='chkSelectedPermissionRight']:checkbox").attr('checked', false);
                               });

                               $("#<%=GridView1.ClientID%> input[id*='chkSelectedPermissionRight']:checkbox").click(function () {
                                   CheckSelectedAllPermissionRight();
                               });
                               function CheckSelectedAllPermissionRight() {
                                   var flag = true;
                                   $("#<%=GridView1.ClientID%> input[id*='chkSelectedPermissionRight']:checkbox").each(function () {
                                       if (this.checked == false)
                                           flag = false;
                                   });
                                   $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllPermissionRight']:checkbox").attr('checked', flag);
                               }

                               //ALLOW ACCESS ================================================================================================
                               $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllAllowAccessRight']:checkbox").click(function () {
                                   if ($(this).is(':checked')) {
                                       $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllowAccessRight']:checkbox").attr('checked', true);
                                   } else
                                       $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllowAccessRight']:checkbox").attr('checked', false);
                               })
                               $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllowAccessRight']:checkbox").click(function () {
                                   CheckSelectedAllAllowAccessRight();
                               });
                               function CheckSelectedAllAllowAccessRight() {
                                   var flag = true;
                                   $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllowAccessRight']:checkbox").each(function () {
                                       if (this.checked == false)
                                           flag = false;
                                   });
                                   $("#<%=GridView1.ClientID%> input[id*='chkSelectedAllAllowAccessRight']:checkbox").attr('checked', flag);
                               }
                           })

                        </script>
                    

                    <table border="0" cellspacing="0" cellpadding="4">                            
                       <tr>
                          <td align="left">
                              Portal</td>
                            <td align="left">
                                <asp:DropDownList ID="ddlPortalList" runat="server" CssClass="combobox" 
                                    Width="149px">
                                </asp:DropDownList>
                           </td>
                           <td align="left">
                               Application</td>
                            <td align="left">
                                 <asp:DropDownList ID="ddlApplicationList" runat="server" CssClass="combobox" Width="149px" ></asp:DropDownList>                                            
                            </td>
                           <td align="left">
                               Role</td>
                           <td align="left">
                                <asp:DropDownList ID="ddlRoleList"  runat="server"   
                                        CssClass="combobox" Width="150px" ></asp:DropDownList>
                           </td>
                           <td align="left">
                               Permission Rights</td>
                           <td align="left">                                
                               <asp:DropDownList ID="ddlPermissionList" runat="server" Width="150px" 
                                   onselectedindexchanged="ddlPermissionList_SelectedIndexChanged">
                               </asp:DropDownList>                                
                           </td>
                       </tr> 

                        <tr>
                          <td align="left">
                              TabTree: </td>
                            <td align="left" colspan="7">
                                <asp:Panel ID="Panel1" runat="server" Height="300px" ScrollBars="Vertical">
                                    <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
                                        AlternatingRowStyle-BackColor="#C2D69B" AutoGenerateColumns="False" 
                                        CellPadding="3" DataKeyNames="ModuleId" EmptyDataText="No Data" 
                                        HeaderStyle-BackColor="#8FB53D" HeaderStyle-Font-Bold="true" Height="100%" 
                                        ondatabound="GridView1_DataBound" 
                                        onpageindexchanging="GridView1_PageIndexChanging" 
                                        onprerender="GridView1_PreRender" 
                                        onrowcancelingedit="GridView1_RowCancelingEdit" 
                                        onrowdatabound="GridView1_RowDataBound" onrowdeleted="GridView1_RowDeleted" 
                                        onrowediting="GridView1_RowEditing" 
                                        onselectedindexchanging="GridView1_SelectedIndexChanging" 
                                        onsorting="GridView1_Sorting" Width="800px">
                                        <AlternatingRowStyle BackColor="#C2D69B" />
                                        <Columns>
                                            <asp:BoundField DataField="ModuleId" HeaderText="ModuleId" 
                                                ItemStyle-Width="40px" />
                                            <asp:TemplateField HeaderText="ModuleTitle" ItemStyle-Width="350px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblModuleTitle" runat="server" Text='<%# Bind("ModuleTitle") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Selected Permission" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSelectedPermissionRight" runat="server" Checked="false" 
                                                        onclick="Check_Click(this)" />
                                                </ItemTemplate>
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="chkSelectedAllPermissionRight" runat="server" 
                                                        Text="PERMISSION" />
                                                </HeaderTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Allow Access" ItemStyle-Width="140px">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSelectedAllowAccessRight" runat="server" Checked="false" 
                                                        onclick="Check_Click(this)" />
                                                </ItemTemplate>
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="chkSelectedAllAllowAccessRight" runat="server" 
                                                        Text="ALLOW ACCESS" />
                                                </HeaderTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="USER ID" ItemStyle-Width="140px">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlUserList" runat="server" Width="140px">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            No Data
                                        </EmptyDataTemplate>
                                        <PagerTemplate>
                                            <div class="pagination">
                                                <asp:Label ID="PagingInformation" runat="server"></asp:Label>
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:ImageButton ID="imgBtnFirst" runat="server" CommandArgument="First" 
                                                    CommandName="Page" Height="22px" ImageUrl="~/images/Icons/arrow_first.png" 
                                                    Width="26px" />
                                                <asp:ImageButton ID="imgBtnPrev" runat="server" CommandArgument="Prev" 
                                                    CommandName="Page" Height="23px" ImageUrl="~/images/Icons/arrow_previous.png" 
                                                    Width="29px" />
                                                Page
                                                <asp:DropDownList ID="ddlPages" runat="server" AutoPostBack="True" 
                                                    OnSelectedIndexChanged="ddlPages_SelectedIndexChanged">
                                                </asp:DropDownList>
                                                of
                                                <asp:Label ID="lblPageCount" runat="server"></asp:Label>
                                                <asp:ImageButton ID="imgBtnNext" runat="server" CommandArgument="Next" 
                                                    CommandName="Page" Height="21px" ImageUrl="~/images/Icons/arrow_next.png" 
                                                    Width="27px" />
                                                <asp:ImageButton ID="imgBtnLast" runat="server" CommandArgument="Last" 
                                                    CommandName="Page" Height="21px" ImageUrl="~/images/Icons/arrow_last.png" 
                                                    Width="31px" />
                                            </div>
                                        </PagerTemplate>
                                        <PagerStyle BackColor="#9DB765" ForeColor="#FFFFFF" HorizontalAlign="Center" />
                                        <HeaderStyle Font-Size="12px" Height="20px" />
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                       </tr>                                                                                                                                                                                                         
                   </table>
                    <div class="popup_Buttons">
                        <asp:Button ID="btnOkay" Text="Done" runat="server" ValidationGroup="checkValidation" OnClientClick="return false;" OnClick="btnOkay_Click" />
                        <input id="btnCancel" value="Cancel" type="button" onclick="cancel();" />
                    </div>
                </asp:View>
                 <asp:View ID="ViewSuccess" runat="server">
                     <p>
                         <asp:Label ID="lblResult" runat="server"></asp:Label>
                     </p>                    
                     <input id="btnClose" value="Đóng" type="button" onclick="cancel();" />
               </asp:View>               
                <asp:View ID="ViewError" runat="server">
                   <p>
                         <asp:Label ID="lblErrorMsg" runat="server"></asp:Label>
                     </p>                    
                     <input id="btnExit" value="Đóng" type="button" onclick="cancel();" />
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
    </form>
</body>
</html>
