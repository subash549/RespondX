<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RespondX._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <section class="hero-section">
        <div class="container">
            <div class="hero-content">
                <h1 class="hero-title">Emergency Response Training, Ready When It Matters</h1>
                <p class="hero-subtitle">RespondX helps learners, experts, and organizations build life-saving skills through interactive modules, real-world scenarios, and certified assessments.</p>
                <div class="hero-actions">
                    <a href="Signup.aspx" class="btn btn-light btn-lg"><i class="fas fa-user-plus"></i> Get Started Free</a>
                    <a href="Login.aspx" class="btn btn-outline-light btn-lg"><i class="fas fa-sign-in-alt"></i> Sign In</a>
                </div>
            </div>
        </div>
    </section>

    <div class="container">

        <h2 class="section-title">Everything You Need to Be Prepared</h2>
        <p class="section-subtitle">A complete training platform built around how emergencies actually unfold.</p>
        <div class="grid-4">
            <div class="feature-card">
                <div class="feature-icon"><i class="fas fa-book-open"></i></div>
                <h3>Interactive Modules</h3>
                <p>Self-paced lessons covering first aid, fire safety, disaster preparedness, and more.</p>
            </div>
            <div class="feature-card">
                <div class="feature-icon"><i class="fas fa-users"></i></div>
                <h3>Real-World Scenarios</h3>
                <p>Practice decision-making in simulated emergencies before it counts for real.</p>
            </div>
            <div class="feature-card">
                <div class="feature-icon"><i class="fas fa-question-circle"></i></div>
                <h3>Knowledge Quizzes</h3>
                <p>Test your understanding after every module and track where you need more practice.</p>
            </div>
            <div class="feature-card">
                <div class="feature-icon"><i class="fas fa-award"></i></div>
                <h3>Verified Certificates</h3>
                <p>Earn shareable certificates reviewed and approved by subject-matter experts.</p>
            </div>
        </div>

        <h2 class="section-title">Featured Training Modules</h2>
        <p class="section-subtitle">Start with the courses learners complete most.</p>
        <div class="grid-3">
            <div class="course-card">
                <div class="course-meta">
                    <span><i class="fas fa-clock"></i> 2 hours</span>
                    <span><i class="fas fa-signal"></i> Beginner</span>
                </div>
                <h3>CPR &amp; First Aid Essentials</h3>
                <p>Learn the fundamentals of CPR, wound care, and how to respond in the critical first minutes of an emergency.</p>
                <a href="Signup.aspx" class="btn btn-primary btn-sm">Start Learning</a>
            </div>
            <div class="course-card">
                <div class="course-meta">
                    <span><i class="fas fa-clock"></i> 1.5 hours</span>
                    <span><i class="fas fa-signal"></i> Beginner</span>
                </div>
                <h3>Fire Safety &amp; Evacuation</h3>
                <p>Understand fire prevention, extinguisher use, and how to lead a safe, calm evacuation.</p>
                <a href="Signup.aspx" class="btn btn-primary btn-sm">Start Learning</a>
            </div>
            <div class="course-card">
                <div class="course-meta">
                    <span><i class="fas fa-clock"></i> 3 hours</span>
                    <span><i class="fas fa-signal"></i> Intermediate</span>
                </div>
                <h3>Disaster Preparedness Basics</h3>
                <p>Build an emergency kit, create a family plan, and know what to do before, during, and after a disaster.</p>
                <a href="Signup.aspx" class="btn btn-primary btn-sm">Start Learning</a>
            </div>
        </div>

        <div class="how-it-works-section">
            <h2 class="section-title">How RespondX Works</h2>
            <div class="steps-container">
                <div class="step-card">
                    <div class="step-number">1</div>
                    <div class="step-title">Create Your Account</div>
                    <div class="step-desc">Sign up as a learner in under a minute, free of charge.</div>
                </div>
                <div class="step-card">
                    <div class="step-number">2</div>
                    <div class="step-title">Complete Modules</div>
                    <div class="step-desc">Work through lessons at your own pace, whenever you have time.</div>
                </div>
                <div class="step-card">
                    <div class="step-number">3</div>
                    <div class="step-title">Practice Scenarios</div>
                    <div class="step-desc">Apply what you learned in guided, real-world simulations.</div>
                </div>
                <div class="step-card">
                    <div class="step-number">4</div>
                    <div class="step-title">Get Certified</div>
                    <div class="step-desc">Pass your assessment and earn an expert-reviewed certificate.</div>
                </div>
            </div>
        </div>

        <h2 class="section-title">Why Preparedness Matters</h2>
        <p class="section-subtitle">A few habits make the difference when every second counts.</p>
        <ul class="safety-list">
            <li>Know your building's evacuation routes and nearest exits before an emergency happens.</li>
            <li>Keep a stocked emergency kit with first aid supplies, water, and a flashlight.</li>
            <li>Learn basic CPR and first aid so you can act in the first critical minutes.</li>
            <li>Stay informed through official alerts and have a family communication plan.</li>
        </ul>

        <div class="cta-section">
            <div class="cta-content">
                <h2>Ready to Get Prepared?</h2>
                <p>Join RespondX today and start building the skills that matter most in an emergency.</p>
                <div class="cta-buttons">
                    <a href="Signup.aspx" class="btn btn-light btn-lg">Create Free Account</a>
                    <a href="About.aspx" class="btn btn-outline-light btn-lg">Learn More</a>
                </div>
            </div>
        </div>

    </div>

</asp:Content>
