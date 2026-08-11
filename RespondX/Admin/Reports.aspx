<%@ Page Title="Reports - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="RespondX.Admin.Reports" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Admin.css" rel="stylesheet" />
    <link href="~/Content/Dashboard.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-chart-bar"></i> Reports & Analytics</h1>
            <p>View comprehensive reports and analytics</p>
        </div>

        <div class="report-filters">
            <div class="filter-bar">
                <div class="filter-group">
                    <label>Report Type:</label>
                    <asp:DropDownList ID="ddlReportType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlReportType_SelectedIndexChanged">
                        <asp:ListItem Value="Overview">Overview</asp:ListItem>
                        <asp:ListItem Value="Users">User Analytics</asp:ListItem>
                        <asp:ListItem Value="Modules">Module Performance</asp:ListItem>
                        <asp:ListItem Value="Quizzes">Quiz Analytics</asp:ListItem>
                        <asp:ListItem Value="Certificates">Certificate Reports</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label>Period:</label>
                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                        <asp:ListItem Value="7">Last 7 Days</asp:ListItem>
                        <asp:ListItem Value="30">Last 30 Days</asp:ListItem>
                        <asp:ListItem Value="90">Last 90 Days</asp:ListItem>
                        <asp:ListItem Value="365">Last Year</asp:ListItem>
                        <asp:ListItem Value="0">All Time</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <asp:Button ID="btnExportPDF" runat="server" Text="Export PDF" CssClass="btn btn-danger" OnClick="btnExportPDF_Click" />
                    <asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" CssClass="btn btn-success" OnClick="btnExportExcel_Click" />
                </div>
            </div>
        </div>

        <!-- Overview Stats -->
        <asp:Panel ID="pnlOverview" runat="server" Visible="true">
            <div class="admin-stats">
                <div class="admin-stat-card blue">
                    <div class="stat-icon"><i class="fas fa-users"></i></div>
                    <div class="stat-number"><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></div>
                    <div class="stat-label">Total Users</div>
                    <div class="stat-change"><asp:Label ID="lblUserChange" runat="server"></asp:Label></div>
                </div>
                <div class="admin-stat-card green">
                    <div class="stat-icon"><i class="fas fa-user-graduate"></i></div>
                    <div class="stat-number"><asp:Label ID="lblActiveLearners" runat="server" Text="0"></asp:Label></div>
                    <div class="stat-label">Active Learners</div>
                    <div class="stat-change"><asp:Label ID="lblLearnerChange" runat="server"></asp:Label></div>
                </div>
                <div class="admin-stat-card orange">
                    <div class="stat-icon"><i class="fas fa-check-circle"></i></div>
                    <div class="stat-number"><asp:Label ID="lblCompletionRate" runat="server" Text="0%"></asp:Label></div>
                    <div class="stat-label">Completion Rate</div>
                    <div class="stat-change"><asp:Label ID="lblCompletionChange" runat="server"></asp:Label></div>
                </div>
                <div class="admin-stat-card purple">
                    <div class="stat-icon"><i class="fas fa-award"></i></div>
                    <div class="stat-number"><asp:Label ID="lblCertificatesIssued" runat="server" Text="0"></asp:Label></div>
                    <div class="stat-label">Certificates Issued</div>
                    <div class="stat-change"><asp:Label ID="lblCertChange" runat="server"></asp:Label></div>
                </div>
            </div>

            <div class="charts-grid">
                <div class="card">
                    <div class="card-title">User Growth</div>
                    <canvas id="userGrowthChart" style="max-height: 300px;"></canvas>
                </div>
                <div class="card">
                    <div class="card-title">Module Completion</div>
                    <canvas id="moduleCompletionChart" style="max-height: 300px;"></canvas>
                </div>
            </div>

            <div class="charts-grid mt-2">
                <div class="card">
                    <div class="card-title">Quiz Performance</div>
                    <canvas id="quizPerformanceChart" style="max-height: 300px;"></canvas>
                </div>
                <div class="card">
                    <div class="card-title">User Engagement</div>
                    <canvas id="engagementChart" style="max-height: 300px;"></canvas>
                </div>
            </div>
        </asp:Panel>

        <!-- User Report -->
        <asp:Panel ID="pnlUsers" runat="server" Visible="false">
            <div class="card">
                <div class="card-title">User Analytics</div>
                <div class="table-responsive">
                    <asp:Repeater ID="rptUserReport" runat="server">
                        <HeaderTemplate>
                            <table class="table admin-table">
                                <thead>
                                    <tr>
                                        <th>Role</th>
                                        <th>Total</th>
                                        <th>Active</th>
                                        <th>Inactive</th>
                                        <th>New (30 days)</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Role") %></td>
                                <td><%# Eval("Total") %></td>
                                <td><%# Eval("Active") %></td>
                                <td><%# Eval("Inactive") %></td>
                                <td><%# Eval("New") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>

        <!-- Module Report -->
        <asp:Panel ID="pnlModules" runat="server" Visible="false">
            <div class="card">
                <div class="card-title">Module Performance</div>
                <div class="table-responsive">
                    <asp:Repeater ID="rptModuleReport" runat="server">
                        <HeaderTemplate>
                            <table class="table admin-table">
                                <thead>
                                    <tr>
                                        <th>Module</th>
                                        <th>Enrollments</th>
                                        <th>Completion Rate</th>
                                        <th>Avg. Score</th>
                                        <th>Avg. Time</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("ModuleName") %></td>
                                <td><%# Eval("Enrollments") %></td>
                                <td>
                                    <div class="progress" style="width: 100px; height: 20px; display: inline-block;">
                                        <div class="progress-bar" style="width: <%# Eval("CompletionRate") %>%"></div>
                                    </div>
                                    <%# Eval("CompletionRate") %>%
                                </td>
                                <td><%# Eval("AvgScore") %>%</td>
                                <td><%# Eval("AvgTime") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>

        <!-- Quiz Report -->
        <asp:Panel ID="pnlQuizzes" runat="server" Visible="false">
            <div class="card">
                <div class="card-title">Quiz Analytics</div>
                <div class="table-responsive">
                    <asp:Repeater ID="rptQuizReport" runat="server">
                        <HeaderTemplate>
                            <table class="table admin-table">
                                <thead>
                                    <tr>
                                        <th>Quiz</th>
                                        <th>Attempts</th>
                                        <th>Pass Rate</th>
                                        <th>Avg. Score</th>
                                        <th>Avg. Time</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("QuizName") %></td>
                                <td><%# Eval("Attempts") %></td>
                                <td><%# Eval("PassRate") %>%</td>
                                <td><%# Eval("AvgScore") %>%</td>
                                <td><%# Eval("AvgTime") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>

        <!-- Certificate Report -->
        <asp:Panel ID="pnlCertificates" runat="server" Visible="false">
            <div class="card">
                <div class="card-title">Certificate Reports</div>
                <div class="table-responsive">
                    <asp:Repeater ID="rptCertificateReport" runat="server">
                        <HeaderTemplate>
                            <table class="table admin-table">
                                <thead>
                                    <tr>
                                        <th>Month</th>
                                        <th>Issued</th>
                                        <th>Valid</th>
                                        <th>Expired</th>
                                        <th>Revoked</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Month") %></td>
                                <td><%# Eval("Issued") %></td>
                                <td><%# Eval("Valid") %></td>
                                <td><%# Eval("Expired") %></td>
                                <td><%# Eval("Revoked") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>
    </div>

    <style>
        .page-header {
            margin-bottom: 30px;
        }
        .report-filters {
            margin-bottom: 25px;
        }
        .filter-bar {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 15px;
            background: white;
            padding: 15px 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .filter-group {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
        }
        .filter-group label {
            margin: 0;
            font-weight: 600;
        }
        .filter-group select, .filter-group input {
            padding: 8px 12px;
            border: 2px solid #e2e8f0;
            border-radius: 5px;
        }
        .admin-stat-card {
            position: relative;
        }
        .stat-change {
            font-size: 12px;
            margin-top: 5px;
        }
        .stat-change.positive {
            color: #48bb78;
        }
        .stat-change.negative {
            color: #fc8181;
        }
        .stat-change.neutral {
            color: #a0aec0;
        }
        .charts-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
        }
        .mt-2 {
            margin-top: 20px;
        }
        .admin-table {
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .admin-table thead {
            background: #f7fafc;
        }
        .admin-table th {
            padding: 12px 15px;
            font-weight: 600;
            color: #2d3748;
        }
        .admin-table td {
            padding: 12px 15px;
            vertical-align: middle;
        }
        @media (max-width: 768px) {
            .charts-grid {
                grid-template-columns: 1fr;
            }
            .filter-bar {
                flex-direction: column;
            }
            .filter-group {
                width: 100%;
            }
            .filter-group select, .filter-group input {
                flex: 1;
            }
        }
    </style>
</asp:Content>