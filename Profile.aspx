<%@ Page Title="My Profile - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="RespondX.Profile" %>
<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/Profile.css") %>?v=2" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main class="profile-page">
        <header class="profile-heading"><h1><i class="fas fa-user-edit"></i> My Profile</h1><p>Update your personal information and preferences.</p></header>
        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success profile-alert" Visible="false"><asp:Label ID="lblSuccess" runat="server" /></asp:Panel>
        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger profile-alert" Visible="false"><asp:Label ID="lblError" runat="server" /></asp:Panel>
        <div class="profile-layout">
            <aside class="profile-summary">
                <asp:Image ID="imgProfile" runat="server" AlternateText="Profile photo" CssClass="profile-avatar" />
                <h2><asp:Label ID="lblFullName" runat="server" /></h2><span class="profile-role"><asp:Label ID="lblRole" runat="server" /></span>
                <p class="profile-since">Member since <asp:Label ID="lblMemberSince" runat="server" /></p>
                <asp:Panel ID="pnlLearnerStats" runat="server" CssClass="profile-stats" Visible="false">
                    <div><strong><asp:Label ID="lblModuleCount" runat="server" Text="0" /></strong><span>Modules</span></div>
                    <div><strong><asp:Label ID="lblQuizCount" runat="server" Text="0" /></strong><span>Quizzes</span></div>
                    <div><strong><asp:Label ID="lblCertificateCount" runat="server" Text="0" /></strong><span>Certificates</span></div>
                </asp:Panel>
                <div class="profile-photo"><label for="<%= fuProfileImage.ClientID %>">Profile photo</label><asp:FileUpload ID="fuProfileImage" runat="server" CssClass="profile-file" accept="image/jpeg,image/png,image/gif" /><small>JPG, PNG or GIF; up to 5 MB.</small></div>
            </aside>
            <div class="profile-forms">
                <section class="profile-card"><h2>Personal Information</h2>
                    <div class="profile-grid"><div class="profile-field"><label for="<%= txtFirstName.ClientID %>">First Name *</label><asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="50" /></div>
                    <div class="profile-field"><label for="<%= txtLastName.ClientID %>">Last Name *</label><asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="50" /></div>
                    <div class="profile-field profile-span"><label for="<%= txtEmail.ClientID %>">Email *</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="100" /></div>
                    <asp:Panel ID="pnlLearnerFields" runat="server" CssClass="profile-grid profile-span" Visible="false">
                        <div class="profile-field profile-span"><label for="<%= txtPhoneNumber.ClientID %>">Phone Number</label><asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" MaxLength="20" /></div>
                        <div class="profile-field"><label for="<%= txtDateOfBirth.ClientID %>">Date of Birth</label><asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date" /></div>
                        <div class="profile-field"><label for="<%= txtExperience.ClientID %>">Years of Experience</label><asp:TextBox ID="txtExperience" runat="server" CssClass="form-control" TextMode="Number" /></div>
                    </asp:Panel>
                </div></section>
                <asp:Panel ID="pnlProfessional" runat="server" CssClass="profile-card" Visible="false"><h2>Professional Information</h2>
                    <div class="profile-grid"><div class="profile-field profile-span"><label for="<%= txtOrganization.ClientID %>">Organization</label><asp:TextBox ID="txtOrganization" runat="server" CssClass="form-control" MaxLength="100" /></div>
                    <div class="profile-field profile-span"><label for="<%= txtJobTitle.ClientID %>">Job Title</label><asp:TextBox ID="txtJobTitle" runat="server" CssClass="form-control" MaxLength="100" /></div>
                    <div class="profile-field profile-span"><label for="<%= txtBio.ClientID %>">Bio</label><asp:TextBox ID="txtBio" runat="server" CssClass="form-control profile-bio" TextMode="MultiLine" MaxLength="500" Rows="3" /></div></div>
                </asp:Panel>
                <section class="profile-card"><h2>Change Password</h2>
                    <div class="profile-grid"><div class="profile-field profile-span"><label for="<%= txtCurrentPassword.ClientID %>">Current Password</label><asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" autocomplete="current-password" /></div>
                    <div class="profile-field"><label for="<%= txtNewPassword.ClientID %>">New Password</label><asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" /></div>
                    <div class="profile-field"><label for="<%= txtConfirmPassword.ClientID %>">Confirm New Password</label><asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" /></div></div>
                    <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-warning profile-action" OnClick="btnChangePassword_Click" />
                </section>
                <div class="profile-actions"><asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSave_Click" /><a runat="server" href="~/Default.aspx" class="btn btn-default">Cancel</a></div>
            </div>
        </div>
    </main>
</asp:Content>
