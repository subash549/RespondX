<%@ Page Title="Module Details - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModuleDetails.aspx.cs" Inherits="RespondX.Learner.ModuleDetails" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlModule" runat="server" Visible="false">
            <div class="module-header" style="position: relative;">
                <asp:Image ID="imgModuleCover" runat="server" CssClass="module-cover-img" />
                <div class="module-header-content">
                    <h1><asp:Label ID="lblModuleTitle" runat="server"></asp:Label></h1>
                    <span class="badge badge-info mb-2"><asp:Label ID="lblCategory" runat="server"></asp:Label></span>
                    <div class="module-meta-info">
                        <span><i class="fas fa-clock"></i> <asp:Label ID="lblEstimatedHours" runat="server"></asp:Label> hours</span>
                        <span><i class="fas fa-list"></i> <asp:Label ID="lblLessonCount" runat="server"></asp:Label> lessons</span>
                        <span><i class="fas fa-user-tie"></i> <asp:Label ID="lblInstructor" runat="server"></asp:Label></span>
                        <asp:Label ID="lblStatusBadge" runat="server" CssClass="badge"></asp:Label>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-main">
                    <div class="card">
                        <div class="card-title"><i class="fas fa-align-left"></i> About This Module</div>
                        <p class="module-about-text"><asp:Label ID="lblModuleDescription" runat="server"></asp:Label></p>
                    </div>

                    <div class="card mt-2">
                        <div class="card-title"><i class="fas fa-video"></i> Module Video</div>
                        <asp:Panel ID="pnlYouTubeVideo" runat="server" Visible="false" CssClass="video-wrapper">
                            <iframe id="iframeYouTube" runat="server" class="module-video-frame" allowfullscreen="true"
                                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"></iframe>
                        </asp:Panel>
                        <asp:Panel ID="pnlUploadedVideo" runat="server" Visible="false" CssClass="video-wrapper">
                            <video id="videoUploaded" runat="server" class="module-video-player" controls="controls"></video>
                        </asp:Panel>
                        <asp:Panel ID="pnlNoModuleVideo" runat="server" Visible="false">
                            <p class="text-muted">No module video has been added yet. An administrator can add a YouTube link or upload a video from Manage Modules.</p>
                        </asp:Panel>
                    </div>

                    <div class="card mt-2">
                        <div class="card-title">Module Content</div>
                        <asp:Repeater ID="rptLessons" runat="server">
                            <ItemTemplate>
                                <div class="lesson-item">
                                    <div class="lesson-status">
                                        <i class='fas fa-<%# Convert.ToBoolean(Eval("IsCompleted")) ? "check-circle text-success" : "circle text-muted" %>'></i>
                                    </div>
                                    <div class="lesson-content">
                                        <h4><%# Eval("Title") %></h4>
                                        <p><%# Eval("Description") %></p>
                                    </div>
                                    <div class="lesson-actions">
                                        <a href='Lesson.aspx?id=<%# Eval("LessonID") %>' class="btn btn-sm btn-primary">
                                            <i class="fas fa-play"></i> 
                                            <%# Convert.ToBoolean(Eval("IsCompleted")) ? "Review" : "Start" %>
                                        </a>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblNoLessons" runat="server" Text="No lessons found" Visible='<%# rptLessons.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="card mt-2">
                        <div class="card-title">Practice Scenarios</div>
                        <asp:Repeater ID="rptScenarios" runat="server">
                            <ItemTemplate>
                                <div class="scenario-item">
                                    <div class="scenario-info">
                                        <h4><%# Eval("Title") %></h4>
                                        <p><%# Eval("Description") %></p>
                                        <span class="badge badge-info">Difficulty: <%# Eval("Difficulty") %></span>
                                    </div>
                                    <a href='ScenarioPractice.aspx?id=<%# Eval("ScenarioID") %>' class="btn btn-sm btn-success">
                                        <i class="fas fa-users"></i> Practice
                                    </a>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblNoScenarios" runat="server" Text="No scenarios found" Visible='<%# rptScenarios.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="card mt-2">
                        <div class="card-title">Module Quiz</div>
                        <asp:Panel ID="pnlQuiz" runat="server">
                            <p><asp:Label ID="lblQuizInfo" runat="server"></asp:Label></p>
                            <asp:Button ID="btnTakeQuiz" runat="server" Text="Take Quiz" CssClass="btn btn-primary" OnClick="btnTakeQuiz_Click" />
                        </asp:Panel>
                        <asp:Panel ID="pnlNoQuiz" runat="server" Visible="false">
                            <p class="text-muted">No quiz available for this module.</p>
                        </asp:Panel>
                    </div>
                </div>

                <div class="col-sidebar">
                    <div class="card">
                        <div class="card-title">Progress</div>
                        <div class="progress-large">
                            <div class="progress">
                                <asp:Panel ID="pnlProgressBar" runat="server" CssClass="progress-bar">
                                    <asp:Label ID="lblProgressDisplay" runat="server" Text="0%"></asp:Label>
                                </asp:Panel>
                            </div>
                            <p class="text-center mt-1">
                                <asp:Label ID="lblCompletedLessons" runat="server" Text="0"></asp:Label> of 
                                <asp:Label ID="lblTotalLessons" runat="server" Text="0"></asp:Label> lessons completed
                            </p>
                        </div>
                    </div>

                    <div class="card mt-1">
                        <div class="card-title">Resources</div>
                        <ul class="resource-list">
                            <li><a href="#"><i class="fas fa-file-pdf"></i> Module Guide</a></li>
                            <li><a href="#"><i class="fas fa-video"></i> Video Tutorial</a></li>
                            <li><a href="#"><i class="fas fa-download"></i> Download Materials</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-warning">
                <h4>Module Not Found</h4>
                <p>The module you're looking for doesn't exist or you don't have access to it.</p>
                <a href="Modules.aspx" class="btn btn-primary">Back to Modules</a>
            </div>
        </asp:Panel>
    </div>

    <style>
        .module-header {
            margin-bottom: 30px;
        }
        .module-header h1 {
            margin-bottom: 5px;
        }
        .module-header p {
            color: #718096;
            font-size: 16px;
        }
        .module-meta-info {
            display: flex;
            gap: 20px;
            margin-top: 15px;
            flex-wrap: wrap;
        }
        .module-meta-info span {
            color: #4a5568;
            font-size: 14px;
        }
        .row {
            display: flex;
            gap: 25px;
        }
        .col-main {
            flex: 2;
        }
        .col-sidebar {
            flex: 1;
        }
        .lesson-item {
            display: flex;
            align-items: center;
            padding: 15px 0;
            border-bottom: 1px solid #e2e8f0;
            gap: 15px;
        }
        .lesson-item:last-child {
            border-bottom: none;
        }
        .lesson-status {
            font-size: 20px;
        }
        .lesson-content {
            flex: 1;
        }
        .lesson-content h4 {
            margin: 0;
            font-size: 16px;
        }
        .lesson-content p {
            margin: 5px 0 0;
            color: #718096;
            font-size: 14px;
        }
        .lesson-actions {
            flex-shrink: 0;
        }
        .scenario-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 0;
            border-bottom: 1px solid #e2e8f0;
        }
        .scenario-item:last-child {
            border-bottom: none;
        }
        .scenario-info h4 {
            margin: 0;
            font-size: 15px;
        }
        .scenario-info p {
            margin: 3px 0 0;
            color: #718096;
            font-size: 13px;
        }
        .progress-large {
            padding: 10px 0;
        }
        .resource-list {
            list-style: none;
            padding: 0;
            margin: 0;
        }
        .resource-list li {
            padding: 8px 0;
            border-bottom: 1px solid #e2e8f0;
        }
        .resource-list li:last-child {
            border-bottom: none;
        }
        .resource-list li a {
            color: #667eea;
            text-decoration: none;
        }
        .resource-list li a:hover {
            text-decoration: underline;
        }
        .resource-list li i {
            margin-right: 10px;
            width: 20px;
        }
        .mt-2 {
            margin-top: 20px;
        }
        .module-cover-img {
            width: 100%;
            height: 250px;
            object-fit: cover;
            border-radius: 10px;
            margin-bottom: 20px;
        }
        .module-header-content {
            padding: 0 10px;
        }
        .module-about-text {
            color: #4a5568;
            font-size: 16px;
            line-height: 1.6;
            margin: 0;
        }
        .video-wrapper {
            margin-bottom: 15px;
        }
        .module-video-frame,
        .module-video-player {
            width: 100%;
            max-width: 100%;
            border-radius: 10px;
            border: none;
            background: #000;
        }
        .module-video-frame {
            aspect-ratio: 16 / 9;
            min-height: 280px;
        }
        .module-video-player {
            max-height: 480px;
        }
        @media (max-width: 768px) {
            .row {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>
