<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="RespondX.Error" %>

<!DOCTYPE html>
<html>
<head>
    <title>Error - RespondX</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align:center;padding:50px;">
            <h1>Something went wrong</h1>
            <p>We're sorry, but an error occurred. Please try again later.</p>
            <asp:Panel ID="pnlErrorReference" runat="server" Visible="false">
                <p>Error reference: <asp:Label ID="lblErrorReference" runat="server"></asp:Label></p>
            </asp:Panel>
            <a href="Login.aspx">Go to Login</a>
        </div>
    </form>
</body>
</html>
