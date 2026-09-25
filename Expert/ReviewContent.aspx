<%@ Page Title="Review Content - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReviewContent.aspx.cs" Inherits="RespondX.Expert.ReviewContent" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Expert.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlReview" runat="server" Visible="false">
            <div class="review-detail">
                <div class="review-header">
                    <div>
                        <h1><asp:Label ID="lblTitle" runat="server"></asp:Label></h1>
                        <p class="text-muted"><asp:Label ID="lblContentType" runat="server"></asp:Label> | Submitted by: <asp:Label ID="lblSubmittedBy" runat="server"></asp:Label></p>
                    </div>
                    <div>
                        <span class="priority-badge <asp:Literal ID="lblPriorityClass" runat="server"></asp:Literal>">
                            Priority: <asp:Label ID="lblPriority" runat="server"></asp:Label>
                        </span>
                    </div>
                </div>

                <div class="review-section">
                    <h3>Content Preview</h3>
                    <div class="content-preview">
                        <asp:Literal ID="litContent" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="review-section">
                    <h3>Review Feedback</h3>
                    <asp:Panel ID="pnlFeedback" runat="server" CssClass="feedback-form">
                        <div class="form-group">
                            <label>Overall Rating *</label>
                            <div class="rating-group">
                                <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Horizontal" CssClass="rating-list">
                                    <asp:ListItem Value="5">5 - Excellent</asp:ListItem>
                                    <asp:ListItem Value="4">4 - Good</asp:ListItem>
                                    <asp:ListItem Value="3">3 - Average</asp:ListItem>
                                    <asp:ListItem Value="2">2 - Needs Improvement</asp:ListItem>
                                    <asp:ListItem Value="1">1 - Poor</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Feedback / Comments *</label>
                            <asp:TextBox ID="txtFeedback" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="Provide detailed feedback on the content..." />
                        </div>
                        <div class="form-group">
                            <label>Recommendation</label>
                            <asp:DropDownList ID="ddlRecommendation" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Approve">Approve</asp:ListItem>
                                <asp:ListItem Value="NeedsRevision">Needs Revision</asp:ListItem>
                                <asp:ListItem Value="Reject">Reject</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </asp:Panel>
                </div>

                <div class="review-actions">
                    <asp:Button ID="btnSubmitReview" runat="server" Text="Submit Review" CssClass="btn btn-primary" OnClick="btnSubmitReview_Click" />
                    <asp:Button ID="btnSaveDraft" runat="server" Text="Save Draft" CssClass="btn btn-secondary" OnClick="btnSaveDraft_Click" />
                    <a href="PendingReviews.aspx" class="btn btn-secondary">Cancel</a>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-warning">
                <h4>Review Not Found</h4>
                <p>The review you're looking for doesn't exist or has already been completed.</p>
                <a href="PendingReviews.aspx" class="btn btn-primary">Back to Pending Reviews</a>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>
    </div>

    <style>
        .review-detail {
            background: white;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .review-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 25px;
            padding-bottom: 20px;
            border-bottom: 2px solid #e2e8f0;
        }
        .review-header h1 {
            margin: 0 0 5px;
        }
        .text-muted {
            color: #718096;
        }
        .review-section {
            margin-bottom: 30px;
        }
        .review-section h3 {
            color: #2d3748;
            margin-bottom: 15px;
        }
        .content-preview {
            background: #f7fafc;
            padding: 25px;
            border-radius: 8px;
            border: 1px solid #e2e8f0;
            min-height: 200px;
        }
        .content-preview h2 { color: #2d3748; }
        .content-preview h3 { color: #4a5568; }
        .content-preview p { color: #4a5568; line-height: 1.8; }
        .content-preview ul, .content-preview ol { padding-left: 25px; }
        .feedback-form {
            background: #f7fafc;
            padding: 25px;
            border-radius: 8px;
            border: 1px solid #e2e8f0;
        }
        .rating-group {
            margin: 10px 0;
        }
        .rating-list {
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
        }
        .rating-list label {
            margin: 0;
            cursor: pointer;
            padding: 8px 15px;
            border: 2px solid #e2e8f0;
            border-radius: 5px;
            transition: all 0.3s;
        }
        .rating-list label:hover {
            border-color: #667eea;
        }
        .rating-list input[type="radio"] {
            margin-right: 5px;
        }
        .rating-list input[type="radio"]:checked + label {
            border-color: #667eea;
            background: #ebf4ff;
        }
        .review-actions {
            display: flex;
            gap: 10px;
            margin-top: 25px;
            padding-top: 20px;
            border-top: 2px solid #e2e8f0;
            flex-wrap: wrap;
        }
        .priority-badge {
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
        }
        .priority-badge.high {
            background: #fed7d7;
            color: #9b2c2c;
        }
        .priority-badge.medium {
            background: #feebc8;
            color: #7b341e;
        }
        .priority-badge.low {
            background: #bee3f8;
            color: #2a4365;
        }
        .form-group {
            margin-bottom: 20px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
            color: #2d3748;
        }
        .form-control {
            width: 100%;
            padding: 10px 15px;
            border: 2px solid #e2e8f0;
            border-radius: 5px;
            font-size: 14px;
            transition: border-color 0.3s;
        }
        .form-control:focus {
            border-color: #667eea;
            outline: none;
        }
        @media (max-width: 768px) {
            .review-header {
                flex-direction: column;
                gap: 10px;
            }
            .rating-list {
                flex-direction: column;
            }
            .review-actions {
                flex-direction: column;
            }
            .review-actions .btn {
                width: 100%;
            }
        }
    </style>
</asp:Content>