<%@ Page Title="Modules - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Modules.aspx.cs" Inherits="RespondX.Learner.Modules" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-book"></i> Learning Modules</h1>
            <p>Complete all modules to earn your certification</p>
        </div>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Filter:</label>
                <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Modules</asp:ListItem>
                    <asp:ListItem Value="InProgress">In Progress</asp:ListItem>
                    <asp:ListItem Value="Completed">Completed</asp:ListItem>
                    <asp:ListItem Value="NotStarted">Not Started</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search modules..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </div>

        <div class="module-grid">
            <asp:Repeater ID="rptModules" runat="server">
                <ItemTemplate>
                    <div class="module-card">
                        <asp:Image ID="imgThumbnail" runat="server" ImageUrl='<%# string.IsNullOrEmpty(Eval("ThumbnailUrl")?.ToString()) ? "~/Content/Images/default-module.svg" : Eval("ThumbnailUrl") %>' CssClass="module-thumbnail" />
                        <div class="module-content-area">
                            <div class="module-title"><%# Eval("Title") %></div>
                            <span class="badge badge-info mb-2"><%# Eval("CategoryName") %></span>
                            <div class="module-description"><%# Eval("Description") %></div>
                            <div class="module-meta">
                            <span><i class="fas fa-clock"></i> <%# Eval("EstimatedHours") %> hours</span>
                            <span><span class="badge <%# Eval("StatusBadge") %>"><%# Eval("Status") %></span></span>
                        </div>
                        <div class="module-progress">
                            <div class="progress">
                                <div class="progress-bar" style="width: <%# Eval("ProgressPercentage") %>%">
                                    <%# Eval("ProgressPercentage") %>%
                                </div>
                            </div>
                        </div>
                        <div class="module-actions">
                            <a href='ModuleDetails.aspx?id=<%# Eval("ModuleID") %>' class="btn btn-primary btn-sm">
                                <i class="fas fa-arrow-right"></i> 
                                <%# Convert.ToBoolean(Eval("IsCompleted")) ? "Review" : "Start" %>
                            </a>
                            <span class="text-muted"><%# Eval("LessonCount") %> lessons</span>
                        </div>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoModules" runat="server" Text="No modules found" Visible='<%# rptModules.Items.Count == 0 %>' CssClass="text-muted text-center" />
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
        .module-thumbnail {
            width: 100%;
            height: 150px;
            object-fit: cover;
            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
        }
        .module-content-area {
            padding: 20px;
        }
        .text-center {
            text-align: center;
            width: 100%;
        }
        .mb-2 {
            margin-bottom: 0.5rem;
            display: inline-block;
        }
    </style>
</asp:Content>