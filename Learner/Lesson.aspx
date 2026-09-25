<%@ Page Title="Lesson" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lesson.aspx.cs" Inherits="RespondX.Learner.Lesson" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:HiddenField ID="hfModuleId" runat="server" />
        <asp:Panel ID="pnlLesson" runat="server" Visible="false">
            <%-- Only the header/progress and the completion controls refresh when a lesson is
                 marked complete, so the lesson content and any video keep playing. --%>
            <asp:UpdatePanel ID="upLessonHeader" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="lesson-progress-bar">
                        <div class="progress">
                            <asp:Panel ID="pnlLessonProgress" runat="server" CssClass="progress-bar"></asp:Panel>
                        </div>
                        <div class="progress-text">
                            <asp:Label ID="lblProgressText" runat="server"></asp:Label>
                        </div>
                    </div>

                    <div class="lesson-header">
                        <div class="breadcrumb">
                            <a runat="server" href="~/Learner/Modules.aspx">Modules</a> &gt;
                            <asp:HyperLink ID="lnkModule" runat="server"></asp:HyperLink> &gt;
                            <span><asp:Label ID="lblLessonTitle" runat="server"></asp:Label></span>
                        </div>
                        <h1><asp:Label ID="lblLessonHeading" runat="server"></asp:Label></h1>
                        <div class="lesson-meta">
                            <span><i class="fas fa-clock"></i> Estimated: <asp:Label ID="lblEstimatedTime" runat="server"></asp:Label> min</span>
                            <asp:Label ID="lblStatus" runat="server" CssClass="badge"></asp:Label>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:Panel ID="pnlLessonVideo" runat="server" CssClass="lesson-video" Visible="false">
                <iframe id="iframeLessonVideo" runat="server" title="Lesson video" allowfullscreen="true"
                    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"></iframe>
            </asp:Panel>

            <div class="lesson-content">
                <asp:Literal ID="litContent" runat="server"></asp:Literal>
            </div>

            <asp:UpdatePanel ID="upLessonNav" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="lesson-navigation">
                        <asp:HyperLink ID="hlPrevLesson" runat="server" CssClass="btn btn-secondary">
                            <img runat="server" src="~/Content/Images/icons/arrow-left.svg" alt="" class="img-icon" />Previous Lesson
                        </asp:HyperLink>
                        <div class="lesson-nav-center">
                            <asp:Button ID="btnMarkComplete" runat="server" Text="Mark as Complete" CssClass="btn btn-success" OnClick="btnMarkComplete_Click" />
                            <span class="lesson-complete-status"><asp:Literal ID="lblLessonCompleteStatus" runat="server"></asp:Literal></span>
                        </div>
                        <asp:HyperLink ID="hlNextLesson" runat="server" CssClass="btn btn-primary">
                            <asp:Literal ID="litNextText" runat="server" Text="Next Lesson" /><img runat="server" src="~/Content/Images/icons/arrow-right.svg" alt="" class="img-icon" />
                        </asp:HyperLink>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="lesson-resources mt-2">
                <div class="card">
                    <div class="card-title"><i class="fas fa-paperclip"></i> Resources</div>
                    <asp:Repeater ID="rptResources" runat="server">
                        <ItemTemplate>
                            <div class="resource-item">
                                <i class="fas fa-<%#: Eval("Icon") %>"></i>
                                <a href='<%#: Eval("Url") %>' target="_blank" rel="noopener noreferrer"><%#: Eval("Title") %></a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblNoResources" runat="server" Text="No additional resources for this lesson." CssClass="text-muted" Visible="false" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="status-page">
                <img runat="server" src="~/Content/Images/not-found.svg" alt="" class="status-illustration" />
                <h1>Lesson not found</h1>
                <p>The lesson you're looking for doesn't exist or is no longer available.</p>
                <div class="status-actions">
                    <a runat="server" href="~/Learner/Modules.aspx" class="btn btn-primary">Back to Modules</a>
                </div>
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
            color: var(--accent);
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
            color: var(--accent);
            width: 20px;
        }
        .mt-2 {
            margin-top: 20px;
        }
        .lesson-video {
            position: relative;
            margin-bottom: 20px;
            padding-top: 56.25%;
            overflow: hidden;
            border-radius: 10px;
            background: #0f172a;
        }
        .lesson-video iframe {
            position: absolute;
            inset: 0;
            width: 100%;
            height: 100%;
            border: 0;
        }
        .lesson-complete-status {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            color: var(--success);
            font-weight: 600;
        }
        .lesson-navigation .btn .img-icon:first-child {
            margin-left: 0;
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
