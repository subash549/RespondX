<%@ Page Title="Review History - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReviewHistory.aspx.cs" Inherits="RespondX.Expert.ReviewHistory" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Expert.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-history"></i> Review History</h1>
            <p>View all your completed content reviews</p>
        </div>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Approved">Approved</asp:ListItem>
                    <asp:ListItem Value="NeedsRevision">Needs Revision</asp:ListItem>
                    <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
                </asp:DropDownList>
            </div>
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
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search history..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </div>

        <div class="table-responsive">
            <asp:Repeater ID="rptHistory" runat="server">
                <HeaderTemplate>
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>Content</th>
                                <th>Type</th>
                                <th>Status</th>
                                <th>Rating</th>
                                <th>Reviewed</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><strong><%# Eval("Title") %></strong></td>
                        <td><span class="badge badge-secondary"><%# Eval("ContentType") %></span></td>
                        <td><span class="status-badge <%# Eval("Status").ToString().ToLower().Replace(" ", "") %>"><%# Eval("Status") %></span></td>
                        <td><%# Eval("Rating") %> ⭐</td>
                        <td><%# Eval("ReviewedAt") %></td>
                        <td>
                            <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-info btn-sm" CommandName="View" CommandArgument='<%# Eval("ReviewID") %>' OnClick="btnView_Click" />
                            <asp:Button ID="btnReReview" runat="server" Text="Re-review" CssClass="btn btn-warning btn-sm" CommandName="ReReview" CommandArgument='<%# Eval("ReviewID") %>' OnClick="btnReReview_Click" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNoHistory" runat="server" Text="No review history found" Visible='<%# rptHistory.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
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
        .status-badge {
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            display: inline-block;
        }
        .status-badge.approved {
            background: #c6f6d5;
            color: #22543d;
        }
        .status-badge.needrevision, .status-badge.needsrevision {
            background: #feebc8;
            color: #7b341e;
        }
        .status-badge.rejected {
            background: #fed7d7;
            color: #9b2c2c;
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
            .admin-table {
                font-size: 14px;
            }
            .admin-table th, .admin-table td {
                padding: 8px 10px;
            }
        }
    </style>
</asp:Content>