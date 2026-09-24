<%@ Page Title="Expert Dashboard - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExpertDashboard.aspx.cs" Inherits="RespondX.Expert.ExpertDashboard" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Expert.css" rel="stylesheet" />
    <link runat="server" href="~/Content/Dashboard.css" rel="stylesheet" />
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
            <div class="expert-stat-card">
                <div class="stat-icon assignments" style="color: #4299e1; background: rgba(66,153,225,0.1);"><i class="fas fa-tasks"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblTotalAssignments" runat="server" Text="0"></asp:Label></h3>
                    <p>Assignments</p>
                </div>
            </div>
            <div class="expert-stat-card">
                <div class="stat-icon grading" style="color: #ed8936; background: rgba(237,137,54,0.1);"><i class="fas fa-check-double"></i></div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblPendingGrades" runat="server" Text="0"></asp:Label></h3>
                    <p>Pending Grades</p>
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
            <a href="ManageAssignments.aspx" class="quick-action-btn">
                <i class="fas fa-tasks"></i>
                <span>Assignments</span>
            </a>
            <a href="GradeAssignments.aspx" class="quick-action-btn">
                <i class="fas fa-check-double"></i>
                <span>Grade Submissions</span>
                <span class="badge badge-warning"><asp:Label ID="lblPendingGradesBadge" runat="server" Text="0"></asp:Label></span>
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
</asp:Content>