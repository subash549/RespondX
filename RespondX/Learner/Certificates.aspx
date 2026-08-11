<%@ Page Title="Certificates - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Certificates.aspx.cs" Inherits="RespondX.Learner.Certificates" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-award"></i> My Certificates</h1>
            <p>View and download your earned certificates</p>
        </div>

        <div class="certificates-grid">
            <asp:Repeater ID="rptCertificates" runat="server">
                <ItemTemplate>
                    <div class="certificate-card">
                        <div class="certificate-preview">
                            <div class="certificate-seal">
                                <i class="fas fa-shield-alt"></i>
                            </div>
                            <h3><%# Eval("ModuleTitle") %></h3>
                            <p>Certificate of Completion</p>
                            <div class="certificate-issued">
                                <span>Issued to:</span>
                                <strong><%# Eval("LearnerName") %></strong>
                            </div>
                            <div class="certificate-meta">
                                <span>Date: <%# Eval("IssueDate", "{0:MMMM dd, yyyy}") %></span>
                                <span>Score: <%# Eval("Score") %>%</span>
                            </div>
                            <div class="certificate-code">
                                <span>Verification Code: <%# Eval("VerificationCode") %></span>
                            </div>
                            <div class="certificate-status">
                                <span class="badge <%# Convert.ToBoolean(Eval("IsValid")) ? "badge-success" : "badge-danger" %>">
                                    <%# Convert.ToBoolean(Eval("IsValid")) ? "Valid" : "Expired" %>
                                </span>
                            </div>
                        </div>
                        <div class="certificate-actions">
                            <a href='<%# Eval("DownloadUrl") %>' class="btn btn-primary btn-sm">
                                <i class="fas fa-download"></i> Download PDF
                            </a>
                            <a href='<%# Eval("ViewUrl") %>' class="btn btn-info btn-sm" target="_blank">
                                <i class="fas fa-eye"></i> View
                            </a>
                            <asp:Button ID="btnVerify" runat="server" Text="Verify" CssClass="btn btn-secondary btn-sm" OnClick="btnVerify_Click" CommandArgument='<%# Eval("VerificationCode") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Panel ID="pnlNoCertificates" runat="server" Visible='<%# rptCertificates.Items.Count == 0 %>'>
                        <div class="no-certificates">
                            <i class="fas fa-certificate fa-4x text-muted"></i>
                            <h3>No Certificates Yet</h3>
                            <p>Complete modules and pass quizzes to earn certificates.</p>
                            <a href="Modules.aspx" class="btn btn-primary">Start Learning</a>
                        </div>
                    </asp:Panel>
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
        .certificates-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 25px;
        }
        .certificate-card {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            transition: all 0.3s;
            border: 2px solid #e2e8f0;
        }
        .certificate-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.12);
            border-color: #667eea;
        }
        .certificate-preview {
            text-align: center;
            padding: 20px 0;
            border-bottom: 2px solid #e2e8f0;
            margin-bottom: 15px;
        }
        .certificate-seal {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            background: linear-gradient(135deg, #f093fb, #f5576c);
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin: 0 auto 15px;
        }
        .certificate-preview h3 {
            margin: 0 0 5px;
            color: #2d3748;
        }
        .certificate-preview p {
            color: #718096;
            margin-bottom: 15px;
        }
        .certificate-issued {
            background: #f7fafc;
            padding: 8px;
            border-radius: 5px;
            margin-bottom: 10px;
        }
        .certificate-issued span {
            color: #718096;
        }
        .certificate-issued strong {
            color: #2d3748;
        }
        .certificate-meta {
            display: flex;
            justify-content: center;
            gap: 20px;
            font-size: 14px;
            color: #718096;
            margin-bottom: 8px;
        }
        .certificate-code {
            font-size: 12px;
            color: #a0aec0;
            background: #f7fafc;
            padding: 5px;
            border-radius: 3px;
            display: inline-block;
        }
        .certificate-status {
            margin-top: 10px;
        }
        .certificate-actions {
            display: flex;
            gap: 10px;
            justify-content: center;
            margin-top: 15px;
        }
        .no-certificates {
            text-align: center;
            padding: 60px 20px;
            background: white;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .no-certificates h3 {
            margin: 20px 0 10px;
            color: #2d3748;
        }
        .no-certificates p {
            color: #718096;
            margin-bottom: 20px;
        }
        @media (max-width: 768px) {
            .certificates-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>