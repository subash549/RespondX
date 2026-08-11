<%@ Page Title="Lesson - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lesson.aspx.cs" Inherits="RespondX.Learner.Lesson" %>
<%@ MasterType VirtualPath="~/Site.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>
<asp:Label ID="lblModuleId" runat="server" Visible="false"></asp:Label>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlLesson" runat="server" Visible="false">
            <div class="lesson-progress-bar">
                <div class="progress">
                    <div class="progress-bar" style="width: <asp:Label ID="lblProgress" runat="server" Text="0"></asp:Label>%"></div>
                </div>
                <div class="progress-text">
                    <asp:Label ID="lblProgressText" runat="server"></asp:Label>
                </div>
            </div>

            <div class="lesson-header">
                <div class="breadcrumb">
                    <a href="Modules.aspx">Modules</a> &gt;
                    <a href='ModuleDetails.aspx?id=<asp:Label ID="lblModuleId" runat="server"></asp:Label>'><asp:Label ID="lblModuleTitle" runat="server"></asp:Label></a> &gt;
                    <span><asp:Label ID="lblLessonTitle" runat="server"></asp:Label></span>
                </div>
                <h1><asp:Label ID="Label1" runat="server"></asp:Label></h1>
                <div class="lesson-meta">
                    <span><i class="fas fa-clock"></i> Estimated: <asp:Label ID="lblEstimatedTime" runat="server"></asp:Label> min</span>
                    <span class="badge <asp:Label ID="lblStatusBadge" runat="server"></asp:Label>">
                        <asp:Label ID="lblStatus" runat="server"></asp:Label>
                    </span>
                </div>
            </div>

            <div class="lesson-content">
                <asp:Literal ID="litContent" runat="server"></asp:Literal>
            </div>

            <div class="lesson-navigation">
                <asp:Button ID="btnPrevLesson" runat="server" Text="← Previous Lesson" CssClass="btn btn-secondary" OnClick="btnPrevLesson_Click" />
                <div class="lesson-nav-center">
                    <asp:Button ID="btnMarkComplete" runat="server" Text="Mark as Complete" CssClass="btn btn-success" OnClick="btnMarkComplete_Click" />
                    <span class="text-muted ml-1"><asp:Label ID="lblLessonCompleteStatus" runat="server"></asp:Label></span>
                </div>
                <asp:Button ID="btnNextLesson" runat="server" Text="Next Lesson →" CssClass="btn btn-primary" OnClick="btnNextLesson_Click" />
            </div>

            <div class="lesson-resources mt-2">
                <div class="card">
                    <div class="card-title"><i class="fas fa-paperclip"></i> Resources</div>
                    <asp:Repeater ID="rptResources" runat="server">
                        <ItemTemplate>
                            <div class="resource-item">
                                <i class="fas fa-<%# Eval("Icon") %>"></i>
                                <a href='<%# Eval("Url") %>' target="_blank"><%# Eval("Title") %></a>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblNoResources" runat="server" Text="No additional resources" Visible='<%# rptResources.Items.Count == 0 %>' CssClass="text-muted" />
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-warning">
                <h4>Lesson Not Found</h4>
                <p>The lesson you're looking for doesn't exist.</p>
                <a href="Modules.aspx" class="btn btn-primary">Back to Modules</a>
            </div>
        </asp:Panel>
    </div>

    <style>
        .lesson-progress-bar {
            margin-bottom: 25px;
        }
        .progress-text {
            margin-top: 5px;
            text-align: center;
            color: #718096;
            font-size: 14px;
        }
        .lesson-header {
            margin-bottom: 25px;
        }
        .breadcrumb {
            font-size: 14px;
            color: #718096;
            margin-bottom: 10px;
        }
        .breadcrumb a {
            color: #667eea;
            text-decoration: none;
        }
        .breadcrumb a:hover {
            text-decoration: underline;
        }
        .lesson-meta {
            display: flex;
            gap: 15px;
            margin-top: 10px;
            flex-wrap: wrap;
        }
        .lesson-content {
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            line-height: 1.8;
            font-size: 16px;
        }
        .lesson-content h2 {
            color: #2d3748;
            margin-top: 25px;
        }
        .lesson-content h3 {
            color: #4a5568;
            margin-top: 20px;
        }
        .lesson-content ul, .lesson-content ol {
            padding-left: 25px;
        }
        .lesson-content img {
            max-width: 100%;
            border-radius: 5px;
        }
        .lesson-navigation {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #e2e8f0;
        }
        .lesson-nav-center {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .resource-item {
            padding: 8px 0;
            border-bottom: 1px solid #f7fafc;
        }
        .resource-item:last-child {
            border-bottom: none;
        }
        .resource-item i {
            margin-right: 10px;
            color: #667eea;
            width: 20px;
        }
        .mt-2 {
            margin-top: 20px;
        }
        @media (max-width: 768px) {
            .lesson-navigation {
                flex-direction: column;
                gap: 10px;
            }
            .lesson-nav-center {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>