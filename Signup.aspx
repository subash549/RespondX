<%@ Page Title="Sign Up - RespondX" Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="RespondX.Signup" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Sign Up - RespondX</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <link runat="server" href="~/Content/Site.css" rel="stylesheet" />
    <link runat="server" href="~/Content/Auth.css" rel="stylesheet" />
</head>
<body class="auth-page">
    <form id="form1" runat="server">
        <div class="auth-container">
            <div class="auth-card auth-card-large">
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

                    <div class="auth-row">
                        <div class="auth-col-6">
                            <div class="form-group">
                                <label for="txtFirstName">First Name *</label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"
                                             placeholder="Enter first name" />
                                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                                    ControlToValidate="txtFirstName" CssClass="validation-error"
                                    ErrorMessage="First name is required" Display="Dynamic" />
                            </div>
                        </div>
                        <div class="auth-col-6">
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
