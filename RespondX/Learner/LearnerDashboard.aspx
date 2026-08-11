<%@ Page Title="Dashboard - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LearnerDashboard.aspx.cs" Inherits="RespondX.Learner.LearnerDashboard" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Dashboard.css" rel="stylesheet" />
    <link href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="dashboard-header">
            <h1><i class="fas fa-tachometer-alt"></i> Learner Dashboard</h1>
            <p>Welcome back, <asp:Label ID="lblUserName" runat="server" CssClass="text-primary font-weight-bold"></asp:Label>!</p>
        </div>

        <!-- Stats Cards -->
        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-icon blue"><i class="fas fa-book"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblTotalModules" runat="server" Text="0"></asp:Label></h3>
                    <p>Total Modules</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon green"><i class="fas fa-check-circle"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblCompletedModules" runat="server" Text="0"></asp:Label></h3>
                    <p>Completed</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon orange"><i class="fas fa-spinner"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblInProgressModules" runat="server" Text="0"></asp:Label></h3>
                    <p>In Progress</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon purple"><i class="fas fa-award"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblCertificates" runat="server" Text="0"></asp:Label></h3>
                    <p>Certificates Earned</p>
                </div>
            </div>
        </div>

        <!-- Quick Actions -->
        <div class="quick-actions">
            <a href="Modules.aspx" class="quick-action-btn">
                <i class="fas fa-book-open"></i>
                <span>Browse Modules</span>
            </a>
            <a href="Quizzes.aspx" class="quick-action-btn">
                <i class="fas fa-question-circle"></i>
                <span>Take Quizzes</span>
            </a>
            <a href="Scenarios.aspx" class="quick-action-btn">
                <i class="fas fa-users"></i>
                <span>Practice Scenarios</span>
            </a>
            <a href="MyProgress.aspx" class="quick-action-btn">
                <i class="fas fa-chart-line"></i>
                <span>View Progress</span>
            </a>
            <a href="Certificates.aspx" class="quick-action-btn">
                <i class="fas fa-award"></i>
                <span>My Certificates</span>
            </a>
            <a href="Profile.aspx" class="quick-action-btn">
                <i class="fas fa-user-edit"></i>
                <span>Update Profile</span>
            </a>
        </div>

        <!-- Dashboard Grid -->
        <div class="dashboard-grid">
            <!-- Recent Modules -->
            <div class="card">
                <div class="card-title">
                    <i class="fas fa-clock"></i> Recent Modules
                </div>
                <asp:Repeater ID="rptRecentModules" runat="server">
                    <ItemTemplate>
                        <div class="activity-item">
                            <div class="activity-icon completed">
                                <i class="fas fa-check"></i>
                            </div>
                            <div class="activity-content">
                                <h4><%# Eval("Title") %></h4>
                                <p>Status: <%# Eval("Status") %> | Progress: <%# Eval("ProgressPercentage") %>%</p>
                            </div>
                            <a href='ModuleDetails.aspx?id=<%# Eval("ModuleID") %>' class="btn btn-sm btn-primary">Continue</a>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblNoModules" runat="server" Text="No modules in progress. Start learning today!" Visible='<%# rptRecentModules.Items.Count == 0 %>' CssClass="text-muted" />
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <!-- Recent Activity -->
            <div class="card">
                <div class="card-title">
                    <i class="fas fa-history"></i> Recent Activity
                </div>
                <asp:Repeater ID="rptRecentActivity" runat="server">
                    <ItemTemplate>
                        <div class="activity-item">
                            <div class='activity-icon <%# Eval("ActivityType") %>'>
                                <i class='fas fa-<%# Eval("Icon") %>'></i>
                            </div>
                            <div class="activity-content">
                                <h4><%# Eval("Title") %></h4>
                                <p><%# Eval("Description") %></p>
                            </div>
                            <span class="activity-time"><%# Eval("TimeAgo") %></span>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblNoActivity" runat="server" Text="No recent activity" Visible='<%# rptRecentActivity.Items.Count == 0 %>' CssClass="text-muted" />
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Upcoming Alerts -->
        <div class="card mt-2">
            <div class="card-title">
                <i class="fas fa-bell"></i> Alerts & Notifications
            </div>
            <asp:Repeater ID="rptAlerts" runat="server">
                <ItemTemplate>
                    <div class='alert-item <%# Eval("AlertType").ToString().ToLower() %>'>
                        <div class="alert-content">
                            <h4><%# Eval("Title") %></h4>
                            <p><%# Eval("Message") %></p>
                        </div>
                        <span class="activity-time"><%# Eval("TimeAgo") %></span>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoAlerts" runat="server" Text="No alerts" Visible='<%# rptAlerts.Items.Count == 0 %>' CssClass="text-muted" />
                </FooterTemplate>
            </asp:Repeater>
            <div class="mt-1">
                <a href="Alerts.aspx" class="btn btn-sm btn-primary">View All Alerts</a>
            </div>
        </div>
    </div>

    <style>
        .dashboard-header {
            margin-bottom: 30px;
        }
        .dashboard-header h1 {
            margin-bottom: 5px;
        }
        .dashboard-header p {
            color: #718096;
            font-size: 16px;
        }
        .text-primary {
            color: #667eea !important;
        }
    </style>
</asp:Content>