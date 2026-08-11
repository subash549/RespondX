<%@ Page Title="Pending Reviews - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PendingReviews.aspx.cs" Inherits="RespondX.Expert.PendingReviews" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Expert.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-list-ul"></i> Pending Reviews</h1>
            <p>Review and evaluate content submitted for approval</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Content Type:</label>
                <asp:DropDownList ID="ddlContentType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlContentType_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Types</asp:ListItem>
                    <asp:ListItem Value="Module">Modules</asp:ListItem>
                    <asp:ListItem Value="Lesson">Lessons</asp:ListItem>
                    <asp:ListItem Value="Quiz">Quizzes</asp:ListItem>
                    <asp:ListItem Value="Scenario">Scenarios</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <label>Priority:</label>
                <asp:DropDownList ID="ddlPriority" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPriority_SelectedIndexChanged">
                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="High">High</asp:ListItem>
                    <asp:ListItem Value="Medium">Medium</asp:ListItem>
                    <asp:ListItem Value="Low">Low</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search reviews..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </div>

        <div class="review-queue">
            <asp:Repeater ID="rptPendingReviews" runat="server">
                <ItemTemplate>
                    <div class="review-item priority-<%# Eval("PriorityClass") %>">
                        <div class="review-header">
                            <div>
                                <h3 class="review-title"><%# Eval("Title") %></h3>
                                <div class="review-meta">
                                    <span><i class="fas fa-file-alt"></i> <%# Eval("ContentType") %></span>
                                    <span><i class="fas fa-user"></i> Submitted by: <%# Eval("SubmittedBy") %></span>
                                    <span><i class="fas fa-clock"></i> Assigned: <%# Eval("AssignedAt") %></span>
                                    <span><i class="fas fa-calendar"></i> Due: <%# Eval("DueDate") %></span>
                                </div>
                            </div>
                            <div>
                                <span class="priority-badge <%# Eval("PriorityClass") %>"><%# Eval("Priority") %></span>
                            </div>
                        </div>
                        <div class="review-description">
                            <%# Eval("Description") %>
                        </div>
                        <div class="review-actions">
                            <a href='ReviewContent.aspx?id=<%# Eval("ReviewID") %>' class="btn btn-primary">
                                <i class="fas fa-edit"></i> Review Content
                            </a>
                            <asp:Button ID="btnSkip" runat="server" Text="Skip" CssClass="btn btn-secondary" CommandName="Skip" CommandArgument='<%# Eval("ReviewID") %>' OnClick="btnSkip_Click" />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoReviews" runat="server" Text="No pending reviews found" Visible='<%# rptPendingReviews.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="pagination">
            <asp:Button ID="btnPrev" runat="server" Text="Previous" CssClass="btn btn-secondary" OnClick="btnPrev_Click" />
            <span class="page-info"><asp:Label ID="lblPageInfo" runat="server" Text="Page 1 of 1"></asp:Label></span>
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-secondary" OnClick="btnNext_Click" />
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
        .review-queue {
            display: grid;
            gap: 20px;
        }
        .review-item {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            border-left: 4px solid #a0aec0;
        }
        .review-item.priority-high {
            border-left-color: #fc8181;
            background: #fff5f5;
        }
        .review-item.priority-medium {
            border-left-color: #f6ad55;
            background: #fffbeb;
        }
        .review-item.priority-low {
            border-left-color: #63b3ed;
            background: #ebf8ff;
        }
        .review-title {
            margin: 0 0 10px;
            color: #2d3748;
        }
        .review-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            font-size: 14px;
            color: #718096;
            margin-bottom: 10px;
        }
        .review-meta i {
            margin-right: 3px;
        }
        .review-description {
            color: #4a5568;
            line-height: 1.6;
            margin-bottom: 15px;
        }
        .review-actions {
            display: flex;
            gap: 10px;
        }
        .filter-bar {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 20px;
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
        .pagination {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 15px;
            margin-top: 20px;
        }
        .page-info {
            color: #718096;
            font-size: 14px;
        }
        .text-center {
            text-align: center;
        }
        .d-block {
            display: block;
        }
        @media (max-width: 768px) {
            .filter-bar {
                flex-direction: column;
            }
            .filter-group {
                width: 100%;
            }
            .filter-group select, .filter-group input {
                flex: 1;
            }
            .review-meta {
                flex-direction: column;
                gap: 5px;
            }
            .review-actions {
                flex-wrap: wrap;
            }
        }
    </style>
</asp:Content>