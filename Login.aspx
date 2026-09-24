<%@ Page Title="Login - RespondX" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RespondX.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login - RespondX</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <link runat="server" href="~/Content/Site.css" rel="stylesheet" />
    <link runat="server" href="~/Content/Auth.css" rel="stylesheet" />
</head>
<body class="auth-page">
    <form id="form1" runat="server">
        <div class="auth-container">
            <div class="auth-card">
                <div class="auth-header">
                    <h1><i class="fas fa-shield-alt"></i> RespondX</h1>
                    <p>Emergency Response Training Platform</p>
                </div>

                <div class="auth-body">
                    <h2>Welcome Back</h2>
                    <p class="auth-subtitle">Sign in to continue your training</p>

                    <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                        <asp:Label ID="lblError" runat="server"></asp:Label>
                    </asp:Panel>

                    <div class="form-group">
                        <label for="txtUsername">Username or Email</label>
                        <div class="input-group">
                            <span class="input-group-icon"><i class="fas fa-user"></i></span>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username or email" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" CssClass="validation-error" ErrorMessage="Username is required" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="txtPassword">Password</label>
                        <div class="input-group">
                            <span class="input-group-icon"><i class="fas fa-lock"></i></span>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password" />
                            <button type="button" class="input-group-btn" id="togglePassword">
                                <i class="fas fa-eye"></i>
                            </button>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" CssClass="validation-error" ErrorMessage="Password is required" Display="Dynamic" />
                    </div>

                    <div class="form-options">
                        <div class="remember-me">
                            <asp:CheckBox ID="chkRememberMe" runat="server" />
                            <label for="chkRememberMe">Remember me</label>
                        </div>
                        <a href="#" class="forgot-password">Forgot Password?</a>
                    </div>

                    <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn btn-primary btn-block" OnClick="btnLogin_Click" />

                    <div class="auth-footer">
                        <p>Don't have an account? <a href="Signup.aspx">Sign Up</a></p>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script>
        document.getElementById('togglePassword').addEventListener('click', function () {
            const passwordInput = document.getElementById('<%= txtPassword.ClientID %>');
            const icon = this.querySelector('i');
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                icon.className = 'fas fa-eye-slash';
            } else {
                passwordInput.type = 'password';
                icon.className = 'fas fa-eye';
            }
        });
    </script>
</body>
</html>
