<%@ Page Title="Contact RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="RespondX.Contact" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="Content/PublicInfo.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="public-info-page" aria-labelledby="contactTitle">
        <div class="public-page-shell">
            <header class="public-page-hero public-contact-hero">
                <p class="public-eyebrow"><i class="fas fa-comments" aria-hidden="true"></i> RESPONDX SUPPORT</p>
                <h1 id="contactTitle">How can we help?</h1>
                <p class="public-page-lead">
                    Find the right person for account access, course questions, or technical problems with your training.
                    RespondX support is provided through your school or training organization.
                </p>
            </header>

            <section class="public-page-section" aria-labelledby="contactOptions">
                <div class="public-section-heading">
                    <p class="public-eyebrow public-eyebrow-dark">CONTACT THE RIGHT TEAM</p>
                    <h2 id="contactOptions">Choose the kind of help you need</h2>
                    <p>Contact details depend on the organization that gave you access to RespondX.</p>
                </div>

                <div class="public-info-grid public-contact-grid">
                    <article class="public-info-card">
                        <div class="public-info-icon"><i class="fas fa-user-lock" aria-hidden="true"></i></div>
                        <h3>Sign-in or account help</h3>
                        <p>For a forgotten password, account activation, or incorrect learner details, contact your program administrator.</p>
                    </article>
                    <article class="public-info-card">
                        <div class="public-info-icon"><i class="fas fa-graduation-cap" aria-hidden="true"></i></div>
                        <h3>Lessons and training</h3>
                        <p>For questions about course content, module completion, assignments, or quiz requirements, contact your instructor or training coordinator.</p>
                    </article>
                    <article class="public-info-card">
                        <div class="public-info-icon"><i class="fas fa-laptop-medical" aria-hidden="true"></i></div>
                        <h3>Technical problems</h3>
                        <p>If a page or feature is not working, tell your administrator which page you were using and what happened. Include a screenshot if you can.</p>
                    </article>
                </div>
            </section>

            <section class="public-page-section public-contact-guide" aria-labelledby="contactDetails">
                <div class="public-section-heading">
                    <p class="public-eyebrow public-eyebrow-dark">HELP US RESOLVE IT</p>
                    <h2 id="contactDetails">What to include in a support request</h2>
                    <p>A few details can help your school or training team find the issue faster.</p>
                </div>
                <ul class="public-check-list">
                    <li><i class="fas fa-check-circle" aria-hidden="true"></i><span>The email address or username you use to sign in. Never include your password.</span></li>
                    <li><i class="fas fa-check-circle" aria-hidden="true"></i><span>The page or module where the problem happened.</span></li>
                    <li><i class="fas fa-check-circle" aria-hidden="true"></i><span>What you expected to happen and what happened instead.</span></li>
                    <li><i class="fas fa-check-circle" aria-hidden="true"></i><span>The approximate time of the issue and a screenshot, if available.</span></li>
                </ul>
            </section>

            <aside class="public-safety-note">
                <div class="public-safety-icon"><i class="fas fa-triangle-exclamation" aria-hidden="true"></i></div>
                <div>
                    <h2>Need help during a real emergency?</h2>
                    <p>RespondX is a training platform and is not monitored as an emergency service. Contact your local emergency number immediately.</p>
                </div>
            </aside>

            <section class="public-page-cta" aria-labelledby="contactCta">
                <h2 id="contactCta">Continue your training</h2>
                <p>Sign in to see your modules, assignments, quiz results, and progress.</p>
                <div class="public-page-actions">
                    <a runat="server" href="~/Login.aspx" class="btn btn-light">Sign in</a>
                    <a runat="server" href="~/About.aspx" class="btn btn-outline-light">About RespondX</a>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
