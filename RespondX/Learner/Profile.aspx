<%@ Page Title="Profile - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="RespondX.Learner.Profile" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-user-edit"></i> My Profile</h1>
            <p>Update your personal information and preferences</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="profile-grid">
            <div class="profile-sidebar">
                <div class="profile-avatar">
                    <div class="avatar-image">
                        <i class="fas fa-user-circle fa-6x"></i>
                    </div>
                    <h3><asp:Label ID="lblFullName" runat="server"></asp:Label></h3>
                    <p class="text-muted"><asp:Label ID="lblRole" runat="server"></asp:Label></p>
                    <p class="text-muted"><asp:Label ID="lblMemberSince" runat="server"></asp:Label></p>
                </div>
                <div class="profile-stats">
                    <div class="stat-item">
                        <div class="stat-value"><asp:Label ID="lblModulesCompleted" runat="server" Text="0"></asp:Label></div>
                        <div class="stat-label">Modules</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-value"><asp:Label ID="lblQuizzesTaken" runat="server" Text="0"></asp:Label></div>
                        <div class="stat-label">Quizzes</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-value"><asp:Label ID="lblCertificatesEarned" runat="server" Text="0"></asp:Label></div>
                        <div class="stat-label">Certificates</div>
                    </div>
                </div>
            </div>

            <div class="profile-main">
                <div class="card">
                    <div class="card-title">Personal Information</div>
                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>First Name *</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" CssClass="validation-error" ErrorMessage="First name is required" />
                        </div>
                        <div class="form-group col-6">
                            <label>Last Name *</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" CssClass="validation-error" ErrorMessage="Last name is required" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label>Email *</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" CssClass="validation-error" ErrorMessage="Email is required" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$" CssClass="validation-error" ErrorMessage="Invalid email address" />
                    </div>
                    <div class="form-group">
                        <label>Phone Number</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>Date of Birth</label>
                            <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control datepicker" TextMode="Date" />
                        </div>
                        <div class="form-group col-6">
                            <label>Years of Experience</label>
                            <asp:TextBox ID="txtExperience" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                    </div>
                </div>

                <div class="card mt-2">
                    <div class="card-title">Professional Information</div>
                    <div class="form-group">
                        <label>Organization</label>
                        <asp:TextBox ID="txtOrganization" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <label>Job Title</label>
                        <asp:TextBox ID="txtJobTitle" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <label>Bio</label>
                        <asp:TextBox ID="txtBio" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                    </div>
                </div>

                <div class="card mt-2">
                    <div class="card-title">Change Password</div>
                    <div class="form-group">
                        <label>Current Password</label>
                        <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" />
                    </div>
                    <div class="form-row">
                        <div class="form-group col-6">
                            <label>New Password</label>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>
                        <div class="form-group col-6">
                            <label>Confirm New Password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>
                    </div>
                    <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-warning" OnClick="btnChangePassword_Click" />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" />
                </div>
            </div>
        </div>
    </div>

    <style>
        .page-header {
            margin-bottom: 30px;
        }
        .page-header h1 {
            margin-bottom: 5px;
        }
        .page-header p {
            color: #718096;
        }
        .profile-grid {
            display: grid;
            grid-template-columns: 300px 1fr;
            gap: 25px;
        }
        .profile-sidebar {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            text-align: center;
        }
        .profile-avatar {
            margin-bottom: 20px;
        }
        .avatar-image {
            color: #a0aec0;
            margin-bottom: 15px;
        }
        .profile-avatar h3 {
            margin: 0;
            color: #2d3748;
        }
        .profile-avatar .text-muted {
            color: #718096;
            margin: 5px 0;
        }
        .profile-stats {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 10px;
            border-top: 1px solid #e2e8f0;
            padding-top: 20px;
        }
        .profile-stats .stat-item {
            text-align: center;
        }
        .profile-stats .stat-value {
            font-size: 24px;
            font-weight: 700;
            color: #2d3748;
        }
        .profile-stats .stat-label {
            font-size: 12px;
            color: #718096;
        }
        .profile-main {
            display: grid;
            gap: 20px;
        }
        .form-row {
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
        }
        .form-row .form-group {
            flex: 1;
            min-width: 200px;
        }
        .form-actions {
            display: flex;
            gap: 10px;
            margin-top: 20px;
        }
        .validation-error {
            color: #fc8181;
            font-size: 12px;
            display: block;
            margin-top: 3px;
        }
        .mt-2 {
            margin-top: 20px;
        }
        @media (max-width: 768px) {
            .profile-grid {
                grid-template-columns: 1fr;
            }
            .form-row {
                flex-direction: column;
            }
            .form-row .form-group {
                min-width: auto;
            }
            .profile-stats {
                grid-template-columns: repeat(3, 1fr);
            }
        }
    </style>
</asp:Content>