<%@ Page Title="Login - RespondX" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RespondX.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login - RespondX</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
        }
        .auth-container {
            display: flex;
            justify-content: center;
            align-items: center;
            width: 100%;
        }
        .auth-card {
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            width: 100%;
            max-width: 440px;
            overflow: hidden;
        }
        .auth-header {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }
        .auth-header h1 {
            margin: 0;
            font-size: 28px;
            font-weight: 700;
        }
        .auth-header h1 i {
            margin-right: 10px;
        }
        .auth-header p {
            margin: 5px 0 0;
            opacity: 0.9;
            font-size: 14px;
        }
        .auth-body {
            padding: 30px;
        }
        .auth-body h2 {
            margin: 0 0 5px;
            color: #333;
            font-size: 24px;
        }
        .auth-subtitle {
            color: #666;
            margin-bottom: 25px;
            font-size: 14px;
        }
        .form-group {
            margin-bottom: 20px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
            color: #333;
            font-size: 14px;
        }
        .input-group {
            position: relative;
            display: flex;
            align-items: center;
        }
        .input-group-icon {
            position: absolute;
            left: 12px;
            color: #999;
            z-index: 1;
        }
        .form-control {
            width: 100%;
            padding: 12px 15px 12px 40px;
            border: 2px solid #e1e5eb;
            border-radius: 8px;
            font-size: 14px;
            transition: border-color 0.3s;
        }
        .form-control:focus {
            border-color: #667eea;
            outline: none;
        }
        .input-group-btn {
            position: absolute;
            right: 10px;
            background: none;
            border: none;
            color: #999;
            cursor: pointer;
            padding: 5px;
        }
        .validation-error {
            color: #f5576c;
            font-size: 12px;
            margin-top: 5px;
            display: block;
        }
        .form-options {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            font-size: 14px;
        }
        .remember-me {
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .remember-me input[type="checkbox"] {
            width: 16px;
            height: 16px;
            accent-color: #667eea;
        }
        .forgot-password {
            color: #667eea;
            text-decoration: none;
        }
        .forgot-password:hover {
            text-decoration: underline;
        }
        .btn {
            padding: 12px 24px;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            width: 100%;
        }
        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }
        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 20px rgba(102, 126, 234, 0.4);
        }
        .btn-block {
            width: 100%;
        }
        .auth-footer {
            text-align: center;
            margin-top: 20px;
            padding-top: 20px;
            border-top: 1px solid #e1e5eb;
            font-size: 14px;
        }
        .auth-footer a {
            color: #667eea;
            text-decoration: none;
            font-weight: 600;
        }
        .auth-footer a:hover {
            text-decoration: underline;
        }
        .alert {
            padding: 12px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }
        .alert-danger {
            background-color: #fee;
            color: #c0392b;
            border: 1px solid #f5c6cb;
        }
        .alert-success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        .text-muted {
            color: #666;
        }
        .mt-1 {
            margin-top: 10px;
        }
        .text-center {
            text-align: center;
        }
    </style>
</head>
<body>
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