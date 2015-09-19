<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="path_add.ascx.cs" Inherits="WebApp.modules.admin.security.path_add" %>
<div id="wrap_gp" class="wrap_gp" >   
    <div class="commontool">
            <h3>Quản Lý Path: Thêm thông tin</h3>
            <div class="toolbar">     
                 <asp:LinkButton ID="btnInsert" runat="server" CssClass="btn" OnClick="btnInsert_Click" ValidationGroup="ValidationCheck">
                    <span class="icon-32-save"></span>Lưu
                 </asp:LinkButton>
                 &nbsp;
                 <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn" OnClick="btnCancel_Click">
                    <span class="icon-32-cancel"></span>Thoát
                 </asp:LinkButton>                                 
            </div>
    </div>
    <table border="0" cellspacing="0" cellpadding="4" class="form">
       <tr>
           <td width="37" style="width: 24px; height: 15px">&nbsp;</td>
           <td width="127" >&nbsp;</td>
           <td align="left" style="width: 26px; height: 15px">&nbsp;</td>
       </tr>
       <tr>
          <td style="width: 24px; height: 15px" align="right">&nbsp;</td>
          <td align="left" >
              * Application Id</td>
            <td align="left" style="height: 15px">
                <asp:DropDownList ID="ddlApp" AutoPostBack="True" runat="server"   CssClass="combobox" ></asp:DropDownList>             
                <asp:CustomValidator ID="CusValApp"  ControlToValidate="ddlApp" 
                Text="*" ForeColor="red" ClientValidationFunction="ClientValidateSelection" 
                runat="server" ErrorMessage="Invalid, please select one" SetFocusOnError="True" Display="Dynamic"/>                                     
            </td>
       </tr>                                                                                                                                              
       <tr>
          <td style="width: 24px; height: 15px" align="right">&nbsp;</td>
          <td align="left" >
              * Path</td>
            <td align="left" style="height: 15px">
                <asp:TextBox ID="txtPath" runat="server" Width="485px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPath" runat="server" ErrorMessage="*" 
                    SetFocusOnError="True" ControlToValidate="txtPath" 
                    ValidationGroup="ValidationCheck"></asp:RequiredFieldValidator>
            </td>
       </tr>                                                                                                                                              
       </table>
</div>