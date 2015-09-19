<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rights_edit.aspx.cs" Inherits="WebApp.modules.admin.security.roles.rights_edit" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
	<meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title>edit</title>
   <script language="javascript" type="text/javascript">
       function getbacktostepone() {
           window.location = "rights_edit.aspx";
       }
       function onSuccess() {
           setTimeout(okay, 2000);
       }
       function onError() {
           setTimeout(getbacktostepone, 2000);
       }
       function okay() {
           var mode = $get('hdnWindowUIMODE').value;
           if (mode == "edit") {
               parent.location.href = parent.location.href;
               window.parent.document.getElementById('ButtonEditDone').click();
           } else {
               parent.location.href = parent.location.href;
               window.parent.document.getElementById('btnOkay').click();
           }
       }
       function cancel() {
           var mode = $get('hdnWindowUIMODE').value;
           if (mode == "edit")
               window.parent.document.getElementById('ButtonEditCancel').click();
           else
               window.parent.document.getElementById('btnCancel').click();
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
    </script>
    <script type="text/javascript">
        function OnCheckBoxCheckChanged(evt) {
            var src = window.event != window.undefined ? window.event.srcElement : evt.target;
            var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");
            if (isChkBoxClick) {
                var parentTable = GetParentByTagName("table", src);
                var nxtSibling = parentTable.nextSibling;
                if (nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node
                {
                    if (nxtSibling.tagName.toLowerCase() == "div") //if node has children
                    {
                        //check or uncheck children at all levels
                        CheckUncheckChildren(parentTable.nextSibling, src.checked);
                    }
                }
                //check or uncheck parents at all levels
                CheckUncheckParents(src, src.checked);
            }
        }
        function CheckUncheckChildren(childContainer, check) {
            var childChkBoxes = childContainer.getElementsByTagName("input");
            var childChkBoxCount = childChkBoxes.length;
            for (var i = 0; i < childChkBoxCount; i++) {
                childChkBoxes[i].checked = check;
            }
        }
        function CheckUncheckParents(srcChild, check) {
            var parentDiv = GetParentByTagName("div", srcChild);
            var parentNodeTable = parentDiv.previousSibling;

            if (parentNodeTable) {
                var checkUncheckSwitch;

                if (check) //checkbox checked
                {
                    var isAllSiblingsChecked = AreAllSiblingsChecked(srcChild);
                    if (isAllSiblingsChecked)
                        checkUncheckSwitch = true;
                    else
                        return; //do not need to check parent if any(one or more) child not checked
                }
                else //checkbox unchecked
                {
                    checkUncheckSwitch = false;
                }

                var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
                if (inpElemsInParentTable.length > 0) {
                    var parentNodeChkBox = inpElemsInParentTable[0];
                    parentNodeChkBox.checked = checkUncheckSwitch;
                    //do the same recursively
                    CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
                }
            }
        }
        function AreAllSiblingsChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;
            for (var i = 0; i < childCount; i++) {
                if (parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node
                {
                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                        //if any of sibling nodes are not checked, return false
                        if (!prevChkBox.checked) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        //utility function to get the container of an element by tagname
        function GetParentByTagName(parentTagName, childElementObj) {
            var parent = childElementObj.parentNode;
            while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
                parent = parent.parentNode;
            }
            return parent;
        }
            </script>
</head>
<body>
    <form id="form1" runat="server">
  <input type="hidden" value="" runat="server" id="hdnWindowUIMODE" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
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
                    <table border="0" cellspacing="0" cellpadding="4">
                            
                       <tr>
                          <td align="left">
                              Application</td>
                            <td align="left">
                                 <asp:DropDownList ID="ddlApplicationList" runat="server" CssClass="combobox"  Width="149px" ></asp:DropDownList>             
                                 <ajaxToolkit:CascadingDropDown ID="CascadingDropDown1" runat="server" TargetControlID="ddlApplicationList"
                                        Category="ApplicationId"  PromptText="- Select -"  LoadingText="[Loading ...]"
                                        ServicePath="~/services/WebServices.asmx" ServiceMethod="GetApplicationList" /> 
                            </td>
                       </tr> 

                        <tr>
                            <td align="left">
                                Role
                            </td>
                            <td align="left">
                                 <asp:DropDownList ID="ddlRoleList"  runat="server"   
                                        CssClass="combobox" Width="147px" ></asp:DropDownList>             
                                     <ajaxToolkit:CascadingDropDown ID="CascadingDropDown2"
                                           runat="server"
                                           Category="RoleId"
                                           TargetControlID="ddlRoleList"
                                           ParentControlID="ddlApplicationList"
                                           PromptText="- Select -"
                                           LoadingText="Loading ..."
                                           ServicePath="~/services/WebServices.asmx"
                                           ServiceMethod="GetRoleList">
                                    </ajaxToolkit:CascadingDropDown>      
                            </td>
                        </tr>
                        <tr>
                          <td align="left">
                              TabTree: </td>
                            <td align="left">
                             <asp:Panel ID="Panel2" runat="server">
                                <asp:TreeView ID="TreeView2" runat="server" ShowCheckBoxes="All" ShowLines="True">
                                </asp:TreeView>              
                            </asp:Panel>
                            </td>
                       </tr>  
                       <tr>
                          <td align="left">
                              TabTree: </td>
                            <td align="left">
                             <asp:Panel ID="Panel1" runat="server">
                                <asp:TreeView ID="TreeView1" runat="server" ShowCheckBoxes="All" ShowLines="True">
                                </asp:TreeView>              
                            </asp:Panel>
                            </td>
                       </tr>  
                       
                        <tr>
                          <td align="left">
                              Permissions (*)</td>
                            <td align="left">
                             <style type="text/css">               
                                .cb label
                                {                    
                                    margin-right:10px;                    
                                }
                                 .cb input
                                {   
                                    margin-left:10px;                                   
                                    margin-right:5px;                      
                                }
                            </style>          
                            <asp:CheckBoxList ID="CheckBoxList_Permission" CssClass="cb" runat="server" 
                                RepeatDirection="Horizontal">
                            </asp:CheckBoxList>
                            <%--<asp:ListBox ID="lstPermission" AutoPostBack="false" runat="server" Height="70px" Width="214px">
                            </asp:ListBox>--%>
                            </td>
                       </tr>                                                                                                                                                                                                         
                        <tr>
                            <td align="left">
                                Allow Access</td>
                            <td align="left">
                                <asp:CheckBoxList ID="chkAllowAccess" runat="server">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                UserId
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtUserId" runat="server" Width="280px"></asp:TextBox>
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