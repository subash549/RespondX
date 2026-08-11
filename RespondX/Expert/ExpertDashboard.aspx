<%@ Page Title="Expert Dashboard - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExpertDashboard.aspx.cs" Inherits="RespondX.Expert.ExpertDashboard" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Expert.css" rel="stylesheet" />
    <link href="~/Content/Dashboard.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-user-graduate"></i> Expert Dashboard</h1>
            <p>Welcome, <asp:Label ID="lblExpertName" runat="server" CssClass="text-primary font-weight-bold"></asp:Label>!</p>
        </div>

        <!-- Stats Cards -->
        <div class="expert-stats">
            <div class="expert-stat-card">
                <div class="stat-icon pending"><i class="fas fa-clock"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblPendingReviews" runat="server" Text="0"></asp:Label></h3>
                    <p>Pending Reviews</p>
                </div>
            </div>
            <div class="expert-stat-card">
                <div class="stat-icon reviewed"><i class="fas fa-check-circle"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblTotalReviewed" runat="server" Text="0"></asp:Label></h3>
                    <p>Total Reviewed</p>
                </div>
            </div>
            <div class="expert-stat-card">
                <div class="stat-icon total"><i class="fas fa-file-alt"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblTotalContent" runat="server" Text="0"></asp:Label></h3>
                    <p>Total Content</p>
                </div>
            </div>
            <div class="expert-stat-card">
                <div class="stat-icon overdue"><i class="fas fa-exclamation-triangle"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblOverdueReviews" runat="server" Text="0"></asp:Label></h3>
                    <p>Overdue Reviews</p>
                </div>
            </div>
        </div>

        <!-- Quick Actions -->
        <div class="quick-actions">
            <a href="PendingReviews.aspx" class="quick-action-btn">
                <i class="fas fa-list-ul"></i>
                <span>View Pending</span>
                <span class="badge badge-danger"><asp:Label ID="lblPendingBadge" runat="server" Text="0"></asp:Label></span>
            </a>
            <a href="ReviewHistory.aspx" class="quick-action-btn">
                <i class="fas fa-history"></i>
                <span>Review History</span>
            </a>
        </div>

        <!-- Dashboard Grid -->
        <div class="dashboard-grid">
            <!-- Recent Reviews -->
            <div class="card">
                <div class="card-title">
                    <i class="fas fa-clock"></i> Recent Reviews
                </div>
                <asp:Repeater ID="rptRecentReviews" runat="server">
                    <ItemTemplate>
                        <div class="activity-item">
                            <div class="activity-icon reviewed">
                                <i class="fas fa-check"></i>
                            </div>
                            <div class="activity-content">
                                <h4><%# Eval("Title") %></h4>
                                <p><%# Eval("ContentType") %> | Status: <span class="badge <%# Eval("StatusBadge") %>"><%# Eval("Status") %></span></p>
                            </div>
                            <span class="activity-time"><%# Eval("TimeAgo") %></span>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblNoReviews" runat="server" Text="No recent reviews" Visible='<%# rptRecentReviews.Items.Count == 0 %>' CssClass="text-muted" />
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <!-- Pending Reviews Queue -->
            <div class="card">
                <div class="card-title">
                    <i class="fas fa-hourglass-half"></i> Pending Reviews
                </div>
                <asp:Repeater ID="rptPendingReviews" runat="server">
                    <ItemTemplate>
                        <div class="review-item priority-<%# Eval("PriorityClass") %>">
                            <div class="review-header">
                                <span class="review-title"><%# Eval("Title") %></span>
                                <span class="priority-badge <%# Eval("PriorityClass") %>"><%# Eval("Priority") %></span>
                            </div>
                            <div class="review-meta">
                                <span><i class="fas fa-file-alt"></i> <%# Eval("ContentType") %></span>
                                <span><i class="fas fa-clock"></i> Assigned: <%# Eval("AssignedAt") %></span>
                            </div>
                            <div class="review-actions">
                                <a href='ReviewContent.aspx?id=<%# Eval("ReviewID") %>' class="btn btn-primary btn-sm">
                                    <i class="fas fa-edit"></i> Review
                                </a>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblNoPending" runat="server" Text="No pending reviews" Visible='<%# rptPendingReviews.Items.Count == 0 %>' CssClass="text-muted" />
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Charts -->
        <div class="charts-grid mt-2">
            <div class="card">
                <div class="card-title">Review Activity</div>
                <canvas id="reviewActivityChart" style="max-height: 250px;"></canvas>
            </div>
            <div class="card">
                <div class="card-title">Review Status Distribution</div>
                <canvas id="reviewStatusChart" style="max-height: 250px;"></canvas>
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
        .review-item {
            background: #f7fafc;
            border-radius: 8px;
            padding: 12px 15px;
            margin-bottom: 10px;
            border-left: 4px solid #a0aec0;
        }
        .review-item.priority-high {
            border-left-color: #fc8181;
        }
        .review-item.priority-medium {
            border-left-color: #f6ad55;
        }
        .review-item.priority-low {
            border-left-color: #63b3ed;
        }
        .review-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .review-title {
            font-weight: 600;
            color: #2d3748;
        }
        .priority-badge {
            padding: 2px 10px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 600;
        }
        .priority-badge.high {
            background: #fed7d7;
            color: #9b2c2c;
        }
        .priority-badge.medium {
            background: #feebc8;
            color: #7b341e;
        }
        .priority-badge.low {
            background: #bee3f8;
            color: #2a4365;
        }
        .review-meta {
            display: flex;
            gap: 15px;
            font-size: 13px;
            color: #718096;
            margin: 5px 0 10px;
        }
        .review-meta i {
            margin-right: 3px;
        }
        .charts-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
        }
        .mt-2 {
            margin-top: 20px;
        }
        .badge {
            font-size: 12px;
        }
        .quick-action-btn {
            position: relative;
        }
        .quick-action-btn .badge {
            position: absolute;
            top: -5px;
            right: -5px;
        }
        @media (max-width: 768px) {
            .charts-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>