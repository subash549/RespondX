<%@ Page Title="My Progress - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyProgress.aspx.cs" Inherits="RespondX.Learner.MyProgress" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Learner.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-chart-line"></i> My Progress</h1>
            <p>Track your learning journey and achievements</p>
        </div>

        <div class="progress-overview">
            <div class="progress-stat">
                <div class="stat-value"><asp:Label ID="lblOverallProgress" runat="server" Text="0"></asp:Label>%</div>
                <div class="stat-label">Overall Progress</div>
                <div class="progress">
                    <div class="progress-bar" style="width: <asp:Literal ID="lblOverallProgressBar" runat="server" Text="0"></asp:Literal>%"></div>
                </div>
            </div>
            <div class="progress-stat">
                <div class="stat-value"><asp:Label ID="lblCompletedModules" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Completed Modules</div>
            </div>
            <div class="progress-stat">
                <div class="stat-value"><asp:Label ID="lblCertificatesEarned" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Certificates Earned</div>
            </div>
            <div class="progress-stat">
                <div class="stat-value"><asp:Label ID="lblTotalHours" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Hours Spent</div>
            </div>
        </div>

        <div class="progress-charts">
            <div class="card">
                <div class="card-title">Module Progress</div>
                <canvas id="moduleProgressChart" style="max-height: 300px;"></canvas>
            </div>
            <div class="card">
                <div class="card-title">Performance Distribution</div>
                <canvas id="performanceChart" style="max-height: 300px;"></canvas>
            </div>
        </div>

        <div class="card">
            <div class="card-title">Module Details</div>
            <div class="table-responsive">
                <asp:Repeater ID="rptModuleProgress" runat="server">
                    <HeaderTemplate>
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Module</th>
                                    <th>Status</th>
                                    <th>Progress</th>
                                    <th>Time Spent</th>
                                    <th>Last Activity</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("ModuleTitle") %></td>
                            <td><span class="badge <%# Eval("StatusBadge") %>"><%# Eval("Status") %></span></td>
                            <td>
                                <div class="progress" style="width: 100px; height: 20px; display: inline-block;">
                                    <div class="progress-bar" style="width: <%# Eval("ProgressPercentage") %>%"></div>
                                </div>
                                <%# Eval("ProgressPercentage") %>% 
                            </td>
                            <td><%# Eval("TimeSpent") %></td>
                            <td><%# Eval("LastActivity") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="card mt-2">
            <div class="card-title">Achievements</div>
            <div class="achievements-grid">
                <asp:Repeater ID="rptAchievements" runat="server">
                    <ItemTemplate>
                        <div class="achievement-item <%# Convert.ToBoolean(Eval("IsEarned")) ? "earned" : "locked" %>">
                            <div class="achievement-icon">
                                <i class="fas fa-<%# Eval("Icon") %>"></i>
                            </div>
                            <div class="achievement-info">
                                <h4><%# Eval("Title") %></h4>
                                <p><%# Eval("Description") %></p>
                                <%# Convert.ToBoolean(Eval("IsEarned")) ? "<span class='badge badge-success'>Earned</span>" : "<span class='badge badge-secondary'>Locked</span>" %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
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
        .progress-overview {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        .progress-stat {
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            text-align: center;
        }
        .progress-stat .stat-value {
            font-size: 32px;
            font-weight: 700;
            color: #2d3748;
        }
        .progress-stat .stat-label {
            color: #718096;
            font-size: 14px;
            margin-top: 5px;
        }
        .progress-stat .progress {
            margin-top: 10px;
            height: 8px;
        }
        .progress-charts {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-bottom: 30px;
        }
        .achievements-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
            gap: 15px;
        }
        .achievement-item {
            display: flex;
            align-items: center;
            gap: 15px;
            padding: 15px;
            border-radius: 8px;
            background: #f7fafc;
            border: 2px solid #e2e8f0;
            transition: all 0.3s;
        }
        .achievement-item.earned {
            border-color: #48bb78;
            background: #f0fff4;
        }
        .achievement-item.locked {
            opacity: 0.6;
        }
        .achievement-icon {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            flex-shrink: 0;
        }
        .achievement-item.earned .achievement-icon {
            background: #48bb78;
            color: white;
        }
        .achievement-item.locked .achievement-icon {
            background: #a0aec0;
            color: white;
        }
        .achievement-info h4 {
            margin: 0;
            font-size: 14px;
            color: #2d3748;
        }
        .achievement-info p {
            margin: 3px 0 0;
            font-size: 12px;
            color: #718096;
        }
        .mt-2 {
            margin-top: 20px;
        }
        @media (max-width: 768px) {
            .progress-charts {
                grid-template-columns: 1fr;
            }
            .achievements-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>