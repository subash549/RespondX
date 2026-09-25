<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="RespondX.Error" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Something went wrong - RespondX</title>
    <link runat="server" rel="icon" type="image/svg+xml" href="~/Content/Images/logo.svg" />
    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
            background: #f6f7f9;
            color: #334155;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            line-height: 1.6;
        }
        .error-card {
            width: 100%;
            max-width: 480px;
            padding: 40px 32px 32px;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            box-shadow: 0 6px 16px rgba(15, 23, 42, 0.08);
            text-align: center;
        }
        .error-brand {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 24px;
            color: #16213e;
            font-weight: 700;
            text-decoration: none;
        }
        .error-illustration { width: 200px; max-width: 70%; height: auto; margin-bottom: 20px; }
        h1 { margin-bottom: 8px; color: #0f172a; font-size: 24px; }
        p { color: #64748b; }
        .error-reference {
            margin-top: 16px;
            padding: 10px 12px;
            background: #f8fafc;
            border: 1px dashed #cbd5e1;
            border-radius: 6px;
            font-size: 13px;
        }
        .error-reference code { color: #0f172a; font-size: 12px; word-break: break-all; }
        .error-actions { display: flex; flex-wrap: wrap; justify-content: center; gap: 10px; margin-top: 24px; }
        .btn {
            display: inline-block;
            padding: 9px 18px;
            border: 1px solid transparent;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 600;
            text-decoration: none;
            cursor: pointer;
        }
        .btn-primary { background: #2554c7; color: #ffffff; }
        .btn-primary:hover { background: #1d3fa0; }
        .btn-secondary { background: #ffffff; color: #334155; border-color: #cbd5e1; }
        .btn-secondary:hover { background: #f1f5f9; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <main class="error-card">
            <a runat="server" href="~/Default.aspx" class="error-brand">
                <img runat="server" src="~/Content/Images/logo.svg" alt="" width="28" height="28" /> RespondX
            </a>
            <div>
                <asp:Image ID="imgIllustration" runat="server" CssClass="error-illustration" ImageUrl="~/Content/Images/error.svg" AlternateText="" />
            </div>
            <h1><asp:Literal ID="litHeading" runat="server" Text="Something went wrong" /></h1>
            <p><asp:Literal ID="litMessage" runat="server" Text="We're sorry, but an unexpected error occurred. Please try again in a moment." /></p>

            <asp:Panel ID="pnlErrorReference" runat="server" CssClass="error-reference" Visible="false">
                If this keeps happening, share this reference with support:<br />
                <code><asp:Label ID="lblErrorReference" runat="server"></asp:Label></code>
            </asp:Panel>

            <div class="error-actions">
                <a href="javascript:history.back()" class="btn btn-secondary">Go Back</a>
                <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-primary" NavigateUrl="~/Default.aspx" Text="Go to Home" />
            </div>
        </main>
    </form>
</body>
</html>
