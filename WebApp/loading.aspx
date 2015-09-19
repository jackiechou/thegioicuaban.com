<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="loading.aspx.cs" Theme="default" Inherits="WebApp.loading" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
    .loading 
    {
        width:220px;
        background-color: #F5F5F5;
        border: 1px solid #CCCCCC;
        padding:1px 5px 1px 20px;   
        line-height:2px;  
        color:#999898;   
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">    
        <div class="loading">      
            <p>Please wait ... </p>        
            <p><img alt="loading" src="templates/default_temp/images/popup/loading.gif" /></p>  
        </div>
    </form>
</body>
</html>
