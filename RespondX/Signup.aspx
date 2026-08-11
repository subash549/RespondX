<%@ Page Title="Sign Up - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="RespondX.Signup" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-container">
        <div class="auth-card">
            <div class="auth-header">
                <h1><i class="fas fa-user-plus"></i> Create Account</h1>
                <p>Join RespondX Emergency Response Training</p>
            </div>
            
            <div class="auth-body">
                <h2>Get Started</h2>
                <p class="auth-subtitle">Create your account to begin training</p>
                
                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>
                
                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                </asp:Panel>

                <div class="row">
                    <div class="col-6">
                        <div class="form-group">
                            <label for="txtFirstName">First Name *</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" 
                                         placeholder="Enter first name" />
                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
                                ControlToValidate="txtFirstName" CssClass="validation-error" 
                                ErrorMessage="First name is required" Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-group">
                            <label for="txtLastName">Last Name *</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" 
                                         placeholder="Enter last name" />
                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
                                ControlToValidate="txtLastName" CssClass="validation-error" 
                                ErrorMessage="Last name is required" Display="Dynamic" />
                        </div>
                    </div>
                </div>
                
                <div class="form-group">
                    <label for="txtEmail">Email Address *</label>
                    <div class="input-group">
                        <span class="input-group-icon"><i class="fas fa-envelope"></i></span>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                     placeholder="Enter your email" TextMode="Email" />
                    </div>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="txtEmail" CssClass="validation-error" 
                        ErrorMessage="Email is required" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                        ControlToValidate="txtEmail" CssClass="validation-error" 
                        ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$" 
                        ErrorMessage="Please enter a valid email address" Display="Dynamic" />
                </div>
                
                <div class="form-group">
                    <label for="txtUsername">Username *</label>
                    <div class="input-group">
                        <span class="input-group-icon"><i class="fas fa-user"></i></span>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                                     placeholder="Choose a username" />
                    </div>
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                        ControlToValidate="txtUsername" CssClass="validation-error" 
                        ErrorMessage="Username is required" Display="Dynamic" />
                    <asp:CustomValidator ID="cvUsername" runat="server" 
                        ControlToValidate="txtUsername" CssClass="validation-error" 
                        OnServerValidate="cvUsername_ServerValidate" 
                        ErrorMessage="Username is already taken" Display="Dynamic" />
                </div>
                
                <div class="form-group">
                    <label for="txtPassword">Password *</label>
                    <div class="input-group">
                        <span class="input-group-icon"><i class="fas fa-lock"></i></span>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" 
                                     CssClass="form-control" placeholder="Create a password" />
                        <button type="button" class="input-group-btn" id="togglePassword">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
                        ControlToValidate="txtPassword" CssClass="validation-error" 
                        ErrorMessage="Password is required" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revPassword" runat="server" 
                        ControlToValidate="txtPassword" CssClass="validation-error" 
                        ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$" 
                        ErrorMessage="Password must be at least 8 characters with uppercase, lowercase, number, and special character" 
                        Display="Dynamic" />
                </div>
                
                <div class="form-group">
                    <label for="txtConfirmPassword">Confirm Password *</label>
                    <div class="input-group">
                        <span class="input-group-icon"><i class="fas fa-check-circle"></i></span>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" 
                                     CssClass="form-control" placeholder="Confirm your password" />
                    </div>
                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" 
                        ControlToValidate="txtConfirmPassword" CssClass="validation-error" 
                        ErrorMessage="Please confirm your password" Display="Dynamic" />
                    <asp:CompareValidator ID="cvConfirmPassword" runat="server" 
                        ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" 
                        CssClass="validation-error" ErrorMessage="Passwords do not match" 
                        Display="Dynamic" />
                </div>
                
                <div class="form-group">
                    <label for="ddlRole">Role</label>
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                        <asp:ListItem Value="Learner">Learner</asp:ListItem>
                        <asp:ListItem Value="Expert">Expert</asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <div class="form-group">
                    <div class="terms-check">
                        <asp:CheckBox ID="chkTerms" runat="server" />
                        <label for="chkTerms">I agree to the <a href="#" target="_blank">Terms of Service</a> and <a href="#" target="_blank">Privacy Policy</a></label>
                    </div>
                    <asp:CustomValidator ID="cvTerms" runat="server" 
                        CssClass="validation-error" 
                        OnServerValidate="cvTerms_ServerValidate" 
                        ErrorMessage="You must accept the terms to continue" 
                        Display="Dynamic" />
                </div>
                
                <asp:Button ID="btnSignup" runat="server" Text="Create Account" 
                            CssClass="btn btn-primary btn-block" OnClick="btnSignup_Click" />
                
                <div class="auth-footer">
                    <p>Already have an account? <a href="Login.aspx">Sign In</a></p>
                </div>
            </div>
        </div>
    </div>

    <style>
        .row {
            display: flex;
            gap: 15px;
            margin-bottom: 0;
        }
        .col-6 {
            flex: 0 0 calc(50% - 7.5px);
        }
        .auth-container {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 80vh;
            padding: 20px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        }
        .auth-card {
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            width: 100%;
            max-width: 520px;
            overflow: hidden;
            margin: 20px 0;
        }
        .auth-header {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
            padding: 25px 30px;
            text-align: center;
        }
        .auth-header h1 {
            margin: 0;
            font-size: 24px;
            font-weight: 700;
        }
        .auth-header p {
            margin: 5px 0 0;
            opacity: 0.9;
            font-size: 13px;
        }
        .auth-body {
            padding: 25px 30px 30px;
        }
        .auth-body h2 {
            margin: 0 0 5px;
            color: #333;
            font-size: 22px;
        }
        .auth-subtitle {
            color: #666;
            margin-bottom: 20px;
            font-size: 13px;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
            color: #333;
            font-size: 13px;
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
            padding: 10px 15px 10px 40px;
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
            margin-top: 3px;
            display: block;
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
            padding: 10px 15px;
            border-radius: 8px;
            margin-bottom: 15px;
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
        .terms-check {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .terms-check input[type="checkbox"] {
            width: 16px;
            height: 16px;
            accent-color: #667eea;
            flex-shrink: 0;
        }
        .terms-check label {
            margin: 0;
            font-weight: normal;
            font-size: 13px;
        }
        .terms-check a {
            color: #667eea;
            text-decoration: none;
        }
        .terms-check a:hover {
            text-decoration: underline;
        }
        select.form-control {
            padding: 10px 15px;
            appearance: auto;
        }
    </style>

    <script>
        document.getElementById('togglePassword').addEventListener('click', function() {
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
</asp:Content>