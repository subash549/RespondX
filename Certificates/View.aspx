<%@ Page Title="View Certificate - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="RespondX.Certificates.View" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .certificate-container {
            max-width: 900px;
            margin: 0 auto;
            padding: 20px;
        }
        .certificate-image {
            width: 100%;
            border-radius: 10px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.15);
        }
        .certificate-actions {
            text-align: center;
            margin-top: 20px;
        }
        .verification-badge {
            display: inline-block;
            padding: 10px 20px;
            background: #48bb78;
            color: white;
            border-radius: 5px;
            font-weight: bold;
            margin-top: 15px;
        }
        .verification-badge i {
            margin-right: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="certificate-container">
        <asp:Panel ID="pnlCertificate" runat="server" Visible="false">
            <div class="text-center">
                <h1><asp:Label ID="lblTitle" runat="server"></asp:Label></h1>
                <p class="text-muted"><asp:Label ID="lblSubtitle" runat="server"></asp:Label></p>
                
                <div class="verification-badge">
                    <i class="fas fa-check-circle"></i> Verified Certificate
                </div>
                
                <div class="certificate-actions">
                    <a href='<%# Eval("DownloadUrl") %>' class="btn btn-primary">
                        <i class="fas fa-download"></i> Download
                    </a>
                    <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="btn btn-secondary" OnClientClick="window.print(); return false;" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-warning">
                <h4>Certificate Not Found</h4>
                <p>The certificate you're looking for doesn't exist or has been revoked.</p>
                <a runat="server" href="~/Learner/Certificates.aspx" class="btn btn-primary">Back to Certificates</a>
            </div>
        </asp:Panel>
    </div>
</asp:Content>