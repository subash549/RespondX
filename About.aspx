<%@ Page Title="About RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="RespondX.About" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="Content/PublicInfo.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="public-info-page" aria-labelledby="aboutTitle">
        <div class="public-page-shell">
            <header class="public-page-hero">
                <p class="public-eyebrow"><i class="fas fa-shield-alt" aria-hidden="true"></i> RESPONDX TRAINING PLATFORM</p>
                <h1 id="aboutTitle">Prepare to respond. Train with confidence.</h1>
                <p class="public-page-lead">
                    RespondX helps learners build practical emergency response knowledge through guided lessons,
                    realistic scenarios, quizzes, and clear progress tracking.
                </p>
                <div class="public-page-actions">
                    <a runat="server" href="~/Signup.aspx" class="btn btn-light">Create an account</a>
                    <a runat="server" href="~/Contact.aspx" class="btn btn-outline-light">Get help</a>
                </div>
            </header>

            <section class="public-page-section" aria-labelledby="aboutPurpose">
                <div class="public-section-heading">
                    <p class="public-eyebrow public-eyebrow-dark">WHAT YOU CAN DO</p>
                    <h2 id="aboutPurpose">A clear path from learning to practice</h2>
                    <p>Study a topic, apply what you learned, and see your progress as you move through each module.</p>
                </div>

                <div class="public-info-grid">
                    <article class="public-info-card">
                        <div class="public-info-icon"><i class="fas fa-book-open" aria-hidden="true"></i></div>
                        <h3>Learn step by step</h3>
                        <p>Follow organized modules and lessons covering emergency response fundamentals, first aid, communication, and related topics.</p>
                    </article>
                    <article class="public-info-card">
                        <div class="public-info-icon"><i class="fas fa-people-arrows-left-right" aria-hidden="true"></i></div>
                        <h3>Practice decisions</h3>
                        <p>Work through scenarios that help you think about priorities, communication, and safe next steps in an emergency.</p>
                    </article>
                    <article class="public-info-card">
                        <div class="public-info-icon"><i class="fas fa-list-check" aria-hidden="true"></i></div>
                        <h3>Check your understanding</h3>
                        <p>Use quizzes and assignments to review key ideas and identify lessons you may want to revisit.</p>
                    </article>
                    <article class="public-info-card">
                        <div class="public-info-icon"><i class="fas fa-chart-line" aria-hidden="true"></i></div>
                        <h3>Track your progress</h3>
                        <p>Mark lessons complete, follow your module progress, and find earned certificates from your learner account.</p>
                    </article>
                </div>
            </section>

            <section class="public-page-section public-how-section" aria-labelledby="aboutHow">
                <div class="public-section-heading">
                    <p class="public-eyebrow public-eyebrow-dark">HOW IT WORKS</p>
                    <h2 id="aboutHow">Move through training at your own pace</h2>
                </div>
                <div class="public-steps">
                    <article class="public-step">
                        <span class="public-step-number">1</span>
                        <h3>Choose a module</h3>
                        <p>Start with a topic that fits your training plan.</p>
                    </article>
                    <article class="public-step">
                        <span class="public-step-number">2</span>
                        <h3>Complete its lessons</h3>
                        <p>Work through the learning material and mark each lesson complete.</p>
                    </article>
                    <article class="public-step">
                        <span class="public-step-number">3</span>
                        <h3>Take the quiz</h3>
                        <p>Once all lessons in the module are complete, its quiz becomes available.</p>
                    </article>
                    <article class="public-step">
                        <span class="public-step-number">4</span>
                        <h3>Review your result</h3>
                        <p>Use your score and progress to decide what to study next.</p>
                    </article>
                </div>
            </section>

            <aside class="public-safety-note">
                <div class="public-safety-icon"><i class="fas fa-triangle-exclamation" aria-hidden="true"></i></div>
                <div>
                    <h2>Training information, not emergency dispatch</h2>
                    <p>RespondX is for education and practice. If you are facing an emergency now, contact your local emergency services and follow the guidance of qualified responders.</p>
                </div>
            </aside>

            <section class="public-page-cta" aria-labelledby="aboutCta">
                <h2 id="aboutCta">Ready to begin your training?</h2>
                <p>Sign in to continue your learning or create a learner account to get started.</p>
                <div class="public-page-actions">
                    <a runat="server" href="~/Login.aspx" class="btn btn-light">Sign in</a>
                    <a runat="server" href="~/Signup.aspx" class="btn btn-outline-light">Create an account</a>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
