<%@ Page Title="Scenarios - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Scenarios.aspx.cs" Inherits="RespondX.Learner.Scenarios" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-users"></i> Practice Scenarios</h1>
            <p>Practice real-world emergency response scenarios</p>
        </div>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Difficulty:</label>
                <asp:DropDownList ID="ddlDifficulty" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDifficulty_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Levels</asp:ListItem>
                    <asp:ListItem Value="Beginner">Beginner</asp:ListItem>
                    <asp:ListItem Value="Intermediate">Intermediate</asp:ListItem>
                    <asp:ListItem Value="Advanced">Advanced</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search scenarios..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </div>

        <div class="scenario-grid">
            <asp:Repeater ID="rptScenarios" runat="server">
                <ItemTemplate>
                    <div class="scenario-card">
                        <div class="scenario-header">
                            <h3><%# Eval("Title") %></h3>
                            <span class="badge badge-primary"><%# Eval("Module") %></span>
                        </div>
                        <p class="scenario-description"><%# Eval("Description") %></p>
                        <div class="scenario-meta">
                            <span><i class="fas fa-signal"></i> Difficulty: <span class="text-<%# Eval("DifficultyClass") %>"><%# Eval("Difficulty") %></span></span>
                            <span><i class="fas fa-clock"></i> <%# Eval("TimeEstimate") %> min</span>
                            <span class="badge <%# Eval("StatusBadge") %>"><%# Eval("Status") %></span>
                        </div>
                        <div class="scenario-actions">
                            <a href='ScenarioPractice.aspx?id=<%# Eval("ScenarioID") %>' class="btn btn-success">
                                <i class="fas fa-play"></i> Practice
                            </a>
                            <span class="text-muted"><%# Eval("Attempts") %> attempts</span>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoScenarios" runat="server" Text="No scenarios found" Visible='<%# rptScenarios.Items.Count == 0 %>' CssClass="text-muted text-center" />
                </FooterTemplate>
            </asp:Repeater>
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
        .scenario-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 25px;
            margin-top: 20px;
        }
        .scenario-card {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            transition: all 0.3s;
            border-top: 4px solid #667eea;
        }
        .scenario-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.12);
        }
        .scenario-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 10px;
        }
        .scenario-header h3 {
            margin: 0;
            font-size: 18px;
            color: #2d3748;
        }
        .scenario-description {
            color: #718096;
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 15px;
        }
        .scenario-meta {
            display: flex;
            gap: 15px;
            font-size: 13px;
            color: #718096;
            flex-wrap: wrap;
            margin-bottom: 15px;
        }
        .scenario-meta i {
            margin-right: 3px;
        }
        .text-beginner { color: #48bb78; }
        .text-intermediate { color: #f6ad55; }
        .text-advanced { color: #fc8181; }
        .scenario-actions {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .filter-bar {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 25px;
            background: white;
            padding: 15px 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .filter-group {
            display: flex;
            gap: 10px;
            align-items: center;
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
        .text-center {
            text-align: center;
            width: 100%;
        }
        @media (max-width: 768px) {
            .scenario-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>