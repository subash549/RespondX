<%@ Page Title="Admin Dashboard - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="RespondX.Admin.AdminDashboard" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Admin.css" rel="stylesheet" />
    <link href="~/Content/Dashboard.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-shield-alt"></i> Admin Dashboard</h1>
            <p>Welcome, <asp:Label ID="lblAdminName" runat="server" CssClass="text-primary font-weight-bold"></asp:Label>!</p>
        </div>

        <div class="admin-stats">
            <div class="admin-stat-card blue">
                <div class="stat-icon"><i class="fas fa-users"></i></div>
                <div class="stat-number"><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Total Users</div>
            </div>
            <div class="admin-stat-card green">
                <div class="stat-icon"><i class="fas fa-book"></i></div>
                <div class="stat-number"><asp:Label ID="lblTotalModules" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Modules</div>
            </div>
            <div class="admin-stat-card orange">
                <div class="stat-icon"><i class="fas fa-question-circle"></i></div>
                <div class="stat-number"><asp:Label ID="lblTotalQuizzes" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Quizzes</div>
            </div>
            <div class="admin-stat-card purple">
                <div class="stat-icon"><i class="fas fa-trophy"></i></div>
                <div class="stat-number"><asp:Label ID="lblTotalCertificates" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Certificates</div>
            </div>
        </div>

        <div class="quick-actions">
            <a href="ManageUsers.aspx" class="quick-action-btn">
                <i class="fas fa-users-cog"></i>
                <span>Manage Users</span>
            </a>
            <a href="ManageModules.aspx" class="quick-action-btn">
                <i class="fas fa-book"></i>
                <span>Manage Modules</span>
            </a>
            <a href="ManageQuizzes.aspx" class="quick-action-btn">
                <i class="fas fa-question-circle"></i>
                <span>Manage Quizzes</span>
            </a>
            <a href="Reports.aspx" class="quick-action-btn">
                <i class="fas fa-chart-bar"></i>
                <span>Reports</span>
            </a>
        </div>

        <div class="dashboard-grid">
            <div class="card">
                <div class="card-title">
                    <i class="fas fa-user-plus"></i> Recent Users
                </div>
                <asp:Repeater ID="rptRecentUsers" runat="server">
                    <ItemTemplate>
                        <div class="user-item">
                            <div class="user-info">
                                <strong><%# Eval("FullName") %></strong>
                                <span class="text-muted"><%# Eval("Email") %></span>
                            </div>
                            <div class="user-status">
                                <span class="user-status <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "inactive" %>">
                                    <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                                </span>
                                <span class="badge badge-secondary"><%# Eval("Role") %></span>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblNoUsers" runat="server" Text="No users found" Visible='<%# rptRecentUsers.Items.Count == 0 %>' CssClass="text-muted" />
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <div class="card">
                <div class="card-title">
                    <i class="fas fa-history"></i> Recent Activity
                </div>
                <asp:Repeater ID="rptRecentActivity" runat="server">
                    <ItemTemplate>
                        <div class="activity-item">
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

        <div class="charts-grid mt-2">
            <div class="card">
                <div class="card-title">User Growth</div>
                <canvas id="userGrowthChart" style="max-height: 250px;"></canvas>
            </div>
            <div class="card">
                <div class="card-title">Module Completion</div>
                <canvas id="moduleCompletionChart" style="max-height: 250px;"></canvas>
            </div>
        </div>

        <div class="card mt-2">
            <div class="card-title">
                <i class="fas fa-bell"></i> Recent Alerts
            </div>
            <asp:Repeater ID="rptAlerts" runat="server">
                <ItemTemplate>
                    <div class="alert-item <%# Eval("PriorityClass") %>">
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
                <a href="ManageAlerts.aspx" class="btn btn-sm btn-primary">Manage Alerts</a>
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
            font-size: 16px;
        }
        .text-primary {
            color: #667eea !important;
        }
        .user-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 0;
            border-bottom: 1px solid #f7fafc;
        }
        .user-item:last-child {
            border-bottom: none;
        }
        .user-info {
            display: flex;
            flex-direction: column;
        }
        .user-info .text-muted {
            font-size: 13px;
            color: #718096;
        }
        .user-status {
            display: flex;
            gap: 8px;
            align-items: center;
        }
        .charts-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
        }
        .mt-2 {
            margin-top: 20px;
        }
        .mt-1 {
            margin-top: 10px;
        }
        .alert-item {
            padding: 12px 0;
            border-bottom: 1px solid #f7fafc;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .alert-item:last-child {
            border-bottom: none;
        }
        .alert-content h4 {
            margin: 0;
            font-size: 14px;
        }
        .alert-content p {
            margin: 3px 0 0;
            font-size: 13px;
            color: #718096;
        }
        .alert-item.critical {
            border-left: 4px solid #fc8181;
            padding-left: 12px;
        }
        .alert-item.high {
            border-left: 4px solid #fc8181;
            padding-left: 12px;
        }
        .alert-item.medium {
            border-left: 4px solid #f6ad55;
            padding-left: 12px;
        }
        @media (max-width: 768px) {
            .charts-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>