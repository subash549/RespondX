/*
    RespondX SQL Server schema

    Run this script in SQL Server Management Studio or sqlcmd while connected
    to the target database. It creates missing application tables. Existing
    tables and data are not dropped.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Accounts and learner profiles */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username     NVARCHAR(50) NOT NULL,
        Email        NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        Salt         NVARCHAR(50) NOT NULL,
        FirstName    NVARCHAR(50) NOT NULL,
        LastName     NVARCHAR(50) NOT NULL,
        Role         NVARCHAR(20) NOT NULL,
        IsActive     BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
        CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (GETDATE()),
        LastLogin    DATETIME2(0) NULL,
        ProfileImage NVARCHAR(255) NULL,
        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END;
GO

/* Add the profile image column to databases created before profile photos were supported. */
IF COL_LENGTH(N'dbo.Users', N'ProfileImage') IS NULL
    ALTER TABLE dbo.Users ADD ProfileImage NVARCHAR(255) NULL;
GO

/* Development administrator account: username admin, password Admin@123.
   Change this password immediately after the first login. Run this schema on
   the same database configured for the deployed website. */
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin' OR Email = N'admin@respondx.local')
BEGIN
    INSERT INTO dbo.Users
        (Username, Email, PasswordHash, Salt, FirstName, LastName, Role, IsActive)
    VALUES
        (N'admin', N'admin@respondx.local',
         N'PTuAGIDAoVNTrWb51bHPIbiY3s2o/NUjxUue1HhkFt4=',
         N'AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8=',
         N'System', N'Administrator', N'Admin', 1);
END;
GO

IF OBJECT_ID(N'dbo.Learners', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Learners
    (
        LearnerID       INT NOT NULL CONSTRAINT PK_Learners PRIMARY KEY,
        DateOfBirth     DATE NULL,
        PhoneNumber     NVARCHAR(20) NULL,
        Address         NVARCHAR(255) NULL,
        City            NVARCHAR(50) NULL,
        State           NVARCHAR(50) NULL,
        ZipCode         NVARCHAR(20) NULL,
        Organization    NVARCHAR(100) NULL,
        JobTitle        NVARCHAR(100) NULL,
        ExperienceYears INT NOT NULL CONSTRAINT DF_Learners_ExperienceYears DEFAULT (0),
        Certifications  NVARCHAR(MAX) NULL,
        Bio             NVARCHAR(500) NULL,
        CONSTRAINT FK_Learners_Users FOREIGN KEY (LearnerID) REFERENCES dbo.Users(UserID),
        CONSTRAINT CK_Learners_ExperienceYears CHECK (ExperienceYears >= 0)
    );
END;
GO

/* Course catalog */
IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        CategoryID  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive    BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1)
    );
END;
GO

IF OBJECT_ID(N'dbo.Modules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Modules
    (
        ModuleID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Modules PRIMARY KEY,
        Title           NVARCHAR(100) NOT NULL,
        Description     NVARCHAR(500) NULL,
        ModuleOrder     INT NOT NULL CONSTRAINT DF_Modules_ModuleOrder DEFAULT (1),
        IsActive        BIT NOT NULL CONSTRAINT DF_Modules_IsActive DEFAULT (1),
        CreatedBy       INT NULL,
        CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Modules_CreatedAt DEFAULT (GETDATE()),
        UpdatedAt       DATETIME2(0) NULL,
        EstimatedHours  INT NOT NULL CONSTRAINT DF_Modules_EstimatedHours DEFAULT (0),
        IsMandatory     BIT NOT NULL CONSTRAINT DF_Modules_IsMandatory DEFAULT (0),
        CategoryID      INT NULL,
        ThumbnailUrl    NVARCHAR(500) NULL,
        YouTubeVideoUrl NVARCHAR(500) NULL,
        VideoFileUrl    NVARCHAR(500) NULL,
        InstructorID    INT NULL,
        CONSTRAINT FK_Modules_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserID),
        CONSTRAINT FK_Modules_Category FOREIGN KEY (CategoryID) REFERENCES dbo.Categories(CategoryID),
        CONSTRAINT FK_Modules_Instructor FOREIGN KEY (InstructorID) REFERENCES dbo.Users(UserID),
        CONSTRAINT CK_Modules_ModuleOrder CHECK (ModuleOrder >= 1),
        CONSTRAINT CK_Modules_EstimatedHours CHECK (EstimatedHours >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.Lessons', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Lessons
    (
        LessonID    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Lessons PRIMARY KEY,
        ModuleID    INT NOT NULL,
        Title       NVARCHAR(100) NOT NULL,
        Content     NVARCHAR(MAX) NOT NULL,
        LessonOrder INT NOT NULL CONSTRAINT DF_Lessons_LessonOrder DEFAULT (1),
        IsActive    BIT NOT NULL CONSTRAINT DF_Lessons_IsActive DEFAULT (1),
        CreatedAt   DATETIME2(0) NOT NULL CONSTRAINT DF_Lessons_CreatedAt DEFAULT (GETDATE()),
        UpdatedAt   DATETIME2(0) NULL,
        VideoUrl    NVARCHAR(1000) NULL,
        ResourceUrl NVARCHAR(1000) NULL,
        CONSTRAINT FK_Lessons_Modules FOREIGN KEY (ModuleID) REFERENCES dbo.Modules(ModuleID),
        CONSTRAINT CK_Lessons_LessonOrder CHECK (LessonOrder >= 1)
    );
END;
GO

IF OBJECT_ID(N'dbo.Scenarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Scenarios
    (
        ScenarioID      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Scenarios PRIMARY KEY,
        ModuleID        INT NOT NULL,
        Title           NVARCHAR(100) NOT NULL,
        Description     NVARCHAR(500) NULL,
        ScenarioText    NVARCHAR(MAX) NOT NULL,
        ScenarioOrder   INT NOT NULL CONSTRAINT DF_Scenarios_ScenarioOrder DEFAULT (1),
        DifficultyLevel INT NOT NULL CONSTRAINT DF_Scenarios_DifficultyLevel DEFAULT (1),
        IsActive        BIT NOT NULL CONSTRAINT DF_Scenarios_IsActive DEFAULT (1),
        CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Scenarios_CreatedAt DEFAULT (GETDATE()),
        UpdatedAt       DATETIME2(0) NULL,
        CONSTRAINT FK_Scenarios_Modules FOREIGN KEY (ModuleID) REFERENCES dbo.Modules(ModuleID),
        CONSTRAINT CK_Scenarios_ScenarioOrder CHECK (ScenarioOrder >= 1),
        CONSTRAINT CK_Scenarios_DifficultyLevel CHECK (DifficultyLevel BETWEEN 1 AND 5)
    );
END;
GO

IF OBJECT_ID(N'dbo.ScenarioOptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ScenarioOptions
    (
        OptionID    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ScenarioOptions PRIMARY KEY,
        ScenarioID  INT NOT NULL,
        OptionText  NVARCHAR(MAX) NOT NULL,
        IsCorrect   BIT NOT NULL CONSTRAINT DF_ScenarioOptions_IsCorrect DEFAULT (0),
        Feedback    NVARCHAR(500) NULL,
        OptionOrder INT NOT NULL CONSTRAINT DF_ScenarioOptions_OptionOrder DEFAULT (1),
        CONSTRAINT FK_ScenarioOptions_Scenarios FOREIGN KEY (ScenarioID) REFERENCES dbo.Scenarios(ScenarioID),
        CONSTRAINT CK_ScenarioOptions_OptionOrder CHECK (OptionOrder >= 1)
    );
END;
GO

/* Learner progress. IsCompleted is a computed column used by dashboard queries. */
IF OBJECT_ID(N'dbo.LearnerProgress', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LearnerProgress
    (
        ProgressID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LearnerProgress PRIMARY KEY,
        LearnerID         INT NOT NULL,
        ModuleID          INT NOT NULL,
        LessonID          INT NULL,
        ScenarioID        INT NULL,
        Status            NVARCHAR(20) NOT NULL CONSTRAINT DF_LearnerProgress_Status DEFAULT (N'NotStarted'),
        StartedAt         DATETIME2(0) NOT NULL CONSTRAINT DF_LearnerProgress_StartedAt DEFAULT (GETDATE()),
        CompletedAt       DATETIME2(0) NULL,
        LastAccessedAt    DATETIME2(0) NULL,
        ProgressPercentage TINYINT NOT NULL CONSTRAINT DF_LearnerProgress_Percentage DEFAULT (0),
        TimeSpentMinutes  INT NOT NULL CONSTRAINT DF_LearnerProgress_TimeSpent DEFAULT (0),
        IsCompleted       AS (CONVERT(BIT, CASE WHEN Status IN (N'Completed', N'Certified') THEN 1 ELSE 0 END)) PERSISTED,
        CONSTRAINT FK_LearnerProgress_Learners FOREIGN KEY (LearnerID) REFERENCES dbo.Learners(LearnerID),
        CONSTRAINT FK_LearnerProgress_Modules FOREIGN KEY (ModuleID) REFERENCES dbo.Modules(ModuleID),
        CONSTRAINT FK_LearnerProgress_Lessons FOREIGN KEY (LessonID) REFERENCES dbo.Lessons(LessonID),
        CONSTRAINT FK_LearnerProgress_Scenarios FOREIGN KEY (ScenarioID) REFERENCES dbo.Scenarios(ScenarioID),
        CONSTRAINT CK_LearnerProgress_Percentage CHECK (ProgressPercentage BETWEEN 0 AND 100),
        CONSTRAINT CK_LearnerProgress_TimeSpent CHECK (TimeSpentMinutes >= 0)
    );
END;
GO

/* Quizzes and learner attempts */
IF OBJECT_ID(N'dbo.Quizzes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Quizzes
    (
        QuizID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Quizzes PRIMARY KEY,
        ModuleID         INT NOT NULL,
        Title            NVARCHAR(100) NOT NULL,
        Description      NVARCHAR(500) NULL,
        TimeLimitMinutes INT NOT NULL CONSTRAINT DF_Quizzes_TimeLimit DEFAULT (30),
        PassingScore     INT NOT NULL CONSTRAINT DF_Quizzes_PassingScore DEFAULT (70),
        MaxAttempts      INT NOT NULL CONSTRAINT DF_Quizzes_MaxAttempts DEFAULT (3),
        IsActive         BIT NOT NULL CONSTRAINT DF_Quizzes_IsActive DEFAULT (1),
        CreatedAt        DATETIME2(0) NOT NULL CONSTRAINT DF_Quizzes_CreatedAt DEFAULT (GETDATE()),
        UpdatedAt        DATETIME2(0) NULL,
        CONSTRAINT FK_Quizzes_Modules FOREIGN KEY (ModuleID) REFERENCES dbo.Modules(ModuleID),
        CONSTRAINT CK_Quizzes_TimeLimit CHECK (TimeLimitMinutes BETWEEN 1 AND 180),
        CONSTRAINT CK_Quizzes_PassingScore CHECK (PassingScore BETWEEN 0 AND 100),
        CONSTRAINT CK_Quizzes_MaxAttempts CHECK (MaxAttempts BETWEEN 1 AND 10)
    );
END;
GO

IF OBJECT_ID(N'dbo.Questions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Questions
    (
        QuestionID      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Questions PRIMARY KEY,
        QuizID          INT NOT NULL,
        QuestionText    NVARCHAR(MAX) NOT NULL,
        QuestionType    NVARCHAR(20) NOT NULL,
        DifficultyLevel TINYINT NOT NULL CONSTRAINT DF_Questions_Difficulty DEFAULT (1),
        Points          INT NOT NULL CONSTRAINT DF_Questions_Points DEFAULT (1),
        QuestionOrder   INT NOT NULL CONSTRAINT DF_Questions_Order DEFAULT (1),
        CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Questions_CreatedAt DEFAULT (GETDATE()),
        CONSTRAINT FK_Questions_Quizzes FOREIGN KEY (QuizID) REFERENCES dbo.Quizzes(QuizID),
        CONSTRAINT CK_Questions_Difficulty CHECK (DifficultyLevel BETWEEN 1 AND 5),
        CONSTRAINT CK_Questions_Points CHECK (Points BETWEEN 1 AND 100),
        CONSTRAINT CK_Questions_Order CHECK (QuestionOrder >= 1)
    );
END;
GO

IF OBJECT_ID(N'dbo.QuizOptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuizOptions
    (
        OptionID    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_QuizOptions PRIMARY KEY,
        QuestionID  INT NOT NULL,
        OptionText  NVARCHAR(MAX) NOT NULL,
        IsCorrect   BIT NOT NULL CONSTRAINT DF_QuizOptions_IsCorrect DEFAULT (0),
        OptionOrder INT NOT NULL CONSTRAINT DF_QuizOptions_Order DEFAULT (1),
        CONSTRAINT FK_QuizOptions_Questions FOREIGN KEY (QuestionID) REFERENCES dbo.Questions(QuestionID),
        CONSTRAINT CK_QuizOptions_Order CHECK (OptionOrder >= 1)
    );
END;
GO

IF OBJECT_ID(N'dbo.QuizAttempts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuizAttempts
    (
        AttemptID       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_QuizAttempts PRIMARY KEY,
        LearnerID       INT NOT NULL,
        QuizID          INT NOT NULL,
        StartTime       DATETIME2(0) NOT NULL CONSTRAINT DF_QuizAttempts_StartTime DEFAULT (GETDATE()),
        EndTime         DATETIME2(0) NULL,
        Score           INT NULL,
        PercentageScore DECIMAL(5,2) NULL,
        IsPassed        BIT NOT NULL CONSTRAINT DF_QuizAttempts_IsPassed DEFAULT (0),
        AttemptNumber   INT NOT NULL,
        CONSTRAINT FK_QuizAttempts_Learners FOREIGN KEY (LearnerID) REFERENCES dbo.Learners(LearnerID),
        CONSTRAINT FK_QuizAttempts_Quizzes FOREIGN KEY (QuizID) REFERENCES dbo.Quizzes(QuizID),
        CONSTRAINT UQ_QuizAttempts_LearnerQuizNumber UNIQUE (LearnerID, QuizID, AttemptNumber),
        CONSTRAINT CK_QuizAttempts_AttemptNumber CHECK (AttemptNumber >= 1),
        CONSTRAINT CK_QuizAttempts_Percentage CHECK (PercentageScore IS NULL OR PercentageScore BETWEEN 0 AND 100)
    );
END;
GO

IF OBJECT_ID(N'dbo.QuizAnswers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuizAnswers
    (
        AnswerID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_QuizAnswers PRIMARY KEY,
        AttemptID       INT NOT NULL,
        QuestionID      INT NOT NULL,
        SelectedOptionID INT NULL,
        AnswerText      NVARCHAR(MAX) NULL,
        IsCorrect       BIT NOT NULL CONSTRAINT DF_QuizAnswers_IsCorrect DEFAULT (0),
        PointsEarned    INT NOT NULL CONSTRAINT DF_QuizAnswers_Points DEFAULT (0),
        CONSTRAINT FK_QuizAnswers_Attempts FOREIGN KEY (AttemptID) REFERENCES dbo.QuizAttempts(AttemptID),
        CONSTRAINT FK_QuizAnswers_Questions FOREIGN KEY (QuestionID) REFERENCES dbo.Questions(QuestionID),
        CONSTRAINT FK_QuizAnswers_Options FOREIGN KEY (SelectedOptionID) REFERENCES dbo.QuizOptions(OptionID),
        CONSTRAINT UQ_QuizAnswers_AttemptQuestion UNIQUE (AttemptID, QuestionID),
        CONSTRAINT CK_QuizAnswers_Points CHECK (PointsEarned >= 0)
    );
END;
GO

/* Assignments and submissions */
IF OBJECT_ID(N'dbo.Assignments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Assignments
    (
        AssignmentID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Assignments PRIMARY KEY,
        ModuleID     INT NOT NULL,
        Title        NVARCHAR(200) NOT NULL,
        Description  NVARCHAR(MAX) NOT NULL,
        DueDate      DATETIME2(0) NOT NULL,
        MaxScore     INT NOT NULL,
        IsActive     BIT NOT NULL CONSTRAINT DF_Assignments_IsActive DEFAULT (1),
        CONSTRAINT FK_Assignments_Modules FOREIGN KEY (ModuleID) REFERENCES dbo.Modules(ModuleID),
        CONSTRAINT CK_Assignments_MaxScore CHECK (MaxScore BETWEEN 1 AND 1000)
    );
END;
GO

IF OBJECT_ID(N'dbo.AssignmentSubmissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AssignmentSubmissions
    (
        SubmissionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AssignmentSubmissions PRIMARY KEY,
        AssignmentID  INT NOT NULL,
        LearnerID     INT NOT NULL,
        Content       NVARCHAR(MAX) NULL,
        FileUrl       NVARCHAR(1000) NULL,
        Score         INT NULL,
        Feedback      NVARCHAR(MAX) NULL,
        Status        NVARCHAR(20) NOT NULL CONSTRAINT DF_AssignmentSubmissions_Status DEFAULT (N'Submitted'),
        SubmittedAt   DATETIME2(0) NOT NULL CONSTRAINT DF_AssignmentSubmissions_SubmittedAt DEFAULT (GETDATE()),
        CONSTRAINT FK_AssignmentSubmissions_Assignments FOREIGN KEY (AssignmentID) REFERENCES dbo.Assignments(AssignmentID),
        CONSTRAINT FK_AssignmentSubmissions_Learners FOREIGN KEY (LearnerID) REFERENCES dbo.Learners(LearnerID),
        CONSTRAINT UQ_AssignmentSubmissions_AssignmentLearner UNIQUE (AssignmentID, LearnerID),
        CONSTRAINT CK_AssignmentSubmissions_Score CHECK (Score IS NULL OR Score >= 0)
    );
END;
GO

/* Learner support and completion records */
IF OBJECT_ID(N'dbo.EmergencyContacts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmergencyContacts
    (
        ContactID    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmergencyContacts PRIMARY KEY,
        LearnerID    INT NOT NULL,
        FullName     NVARCHAR(100) NOT NULL,
        Relationship NVARCHAR(50) NOT NULL,
        PhoneNumber  NVARCHAR(20) NOT NULL,
        Email        NVARCHAR(100) NULL,
        IsPrimary    BIT NOT NULL CONSTRAINT DF_EmergencyContacts_IsPrimary DEFAULT (0),
        CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_EmergencyContacts_CreatedAt DEFAULT (GETDATE()),
        CONSTRAINT FK_EmergencyContacts_Learners FOREIGN KEY (LearnerID) REFERENCES dbo.Learners(LearnerID)
    );
END;
GO

IF OBJECT_ID(N'dbo.Certificates', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Certificates
    (
        CertificateID     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Certificates PRIMARY KEY,
        LearnerID         INT NOT NULL,
        ModuleID          INT NOT NULL,
        CertificateNumber NVARCHAR(50) NOT NULL,
        IssueDate         DATETIME2(0) NOT NULL CONSTRAINT DF_Certificates_IssueDate DEFAULT (GETDATE()),
        ExpiryDate        DATETIME2(0) NULL,
        IsActive          BIT NOT NULL CONSTRAINT DF_Certificates_IsActive DEFAULT (1),
        PdfPath           NVARCHAR(255) NULL,
        VerificationCode  NVARCHAR(50) NOT NULL,
        Score             DECIMAL(5,2) NOT NULL CONSTRAINT DF_Certificates_Score DEFAULT (0),
        Issuer            NVARCHAR(150) NOT NULL CONSTRAINT DF_Certificates_Issuer DEFAULT (N'RespondX Training Institute'),
        CONSTRAINT FK_Certificates_Learners FOREIGN KEY (LearnerID) REFERENCES dbo.Learners(LearnerID),
        CONSTRAINT FK_Certificates_Modules FOREIGN KEY (ModuleID) REFERENCES dbo.Modules(ModuleID),
        CONSTRAINT UQ_Certificates_CertificateNumber UNIQUE (CertificateNumber),
        CONSTRAINT UQ_Certificates_VerificationCode UNIQUE (VerificationCode),
        CONSTRAINT CK_Certificates_Score CHECK (Score BETWEEN 0 AND 100)
    );
END;
GO

IF OBJECT_ID(N'dbo.Alerts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Alerts
    (
        AlertID         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Alerts PRIMARY KEY,
        LearnerID       INT NULL,
        AdminID         INT NULL,
        AlertType       NVARCHAR(50) NOT NULL,
        Title           NVARCHAR(100) NOT NULL,
        Message         NVARCHAR(MAX) NOT NULL,
        Priority        TINYINT NOT NULL CONSTRAINT DF_Alerts_Priority DEFAULT (3),
        IsRead          BIT NOT NULL CONSTRAINT DF_Alerts_IsRead DEFAULT (0),
        IsAcknowledged  BIT NOT NULL CONSTRAINT DF_Alerts_IsAcknowledged DEFAULT (0),
        CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Alerts_CreatedAt DEFAULT (GETDATE()),
        ExpiresAt       DATETIME2(0) NULL,
        CONSTRAINT FK_Alerts_Learners FOREIGN KEY (LearnerID) REFERENCES dbo.Users(UserID),
        CONSTRAINT FK_Alerts_Admins FOREIGN KEY (AdminID) REFERENCES dbo.Users(UserID),
        CONSTRAINT CK_Alerts_Priority CHECK (Priority BETWEEN 1 AND 5)
    );
END;
GO

/* General in-app notifications, addressed to all users of a role. */
IF OBJECT_ID(N'dbo.Notifications', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notifications
    (
        NotificationID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notifications PRIMARY KEY,
        RecipientUserID INT NOT NULL,
        ActorUserID INT NULL,
        NotificationType NVARCHAR(30) NOT NULL,
        Title NVARCHAR(150) NOT NULL,
        Message NVARCHAR(500) NOT NULL,
        TargetUrl NVARCHAR(255) NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Notifications_CreatedAt DEFAULT (GETDATE()),
        ReadAt DATETIME2(0) NULL,
        CONSTRAINT FK_Notifications_Recipient FOREIGN KEY (RecipientUserID) REFERENCES dbo.Users(UserID),
        CONSTRAINT FK_Notifications_Actor FOREIGN KEY (ActorUserID) REFERENCES dbo.Users(UserID)
    );
    CREATE INDEX IX_Notifications_Recipient_Unread ON dbo.Notifications(RecipientUserID, ReadAt, CreatedAt DESC);
END;
GO

IF OBJECT_ID(N'dbo.Notifications', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Notifications_Recipient_Unread' AND object_id = OBJECT_ID(N'dbo.Notifications'))
    CREATE INDEX IX_Notifications_Recipient_Unread ON dbo.Notifications(RecipientUserID, ReadAt, CreatedAt DESC);
GO

/* ContentType/ContentID is polymorphic (Module, Lesson, Quiz, or Scenario). */
IF OBJECT_ID(N'dbo.ContentReviews', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContentReviews
    (
        ReviewID    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ContentReviews PRIMARY KEY,
        ExpertID    INT NOT NULL,
        ContentType NVARCHAR(50) NOT NULL,
        ContentID   INT NOT NULL,
        Status      NVARCHAR(20) NOT NULL CONSTRAINT DF_ContentReviews_Status DEFAULT (N'Pending'),
        Feedback    NVARCHAR(MAX) NULL,
        ReviewedAt  DATETIME2(0) NULL,
        AssignedAt  DATETIME2(0) NOT NULL CONSTRAINT DF_ContentReviews_AssignedAt DEFAULT (GETDATE()),
        Priority    TINYINT NOT NULL CONSTRAINT DF_ContentReviews_Priority DEFAULT (3),
        CONSTRAINT FK_ContentReviews_Experts FOREIGN KEY (ExpertID) REFERENCES dbo.Users(UserID),
        CONSTRAINT CK_ContentReviews_Priority CHECK (Priority BETWEEN 1 AND 5)
    );
END;
GO

/* Small indexes used by learner-facing lists and lookups. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Lessons_Module_Active_Order' AND object_id = OBJECT_ID(N'dbo.Lessons'))
    CREATE INDEX IX_Lessons_Module_Active_Order ON dbo.Lessons(ModuleID, IsActive, LessonOrder);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LearnerProgress_Learner_Module' AND object_id = OBJECT_ID(N'dbo.LearnerProgress'))
    CREATE INDEX IX_LearnerProgress_Learner_Module ON dbo.LearnerProgress(LearnerID, ModuleID, IsCompleted);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LearnerProgress_Lesson_Learner' AND object_id = OBJECT_ID(N'dbo.LearnerProgress'))
    CREATE INDEX IX_LearnerProgress_Lesson_Learner ON dbo.LearnerProgress(LessonID, LearnerID, IsCompleted);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AssignmentSubmissions_Learner_Status' AND object_id = OBJECT_ID(N'dbo.AssignmentSubmissions'))
    CREATE INDEX IX_AssignmentSubmissions_Learner_Status ON dbo.AssignmentSubmissions(LearnerID, Status);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Certificates_Verification' AND object_id = OBJECT_ID(N'dbo.Certificates'))
    CREATE INDEX IX_Certificates_Verification ON dbo.Certificates(VerificationCode, IsActive);
GO

/* Compatibility for databases created before these fields were added. */
IF OBJECT_ID(N'dbo.Modules', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Modules', N'YouTubeVideoUrl') IS NULL
    ALTER TABLE dbo.Modules ADD YouTubeVideoUrl NVARCHAR(500) NULL;
GO

IF OBJECT_ID(N'dbo.Modules', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Modules', N'VideoFileUrl') IS NULL
    ALTER TABLE dbo.Modules ADD VideoFileUrl NVARCHAR(500) NULL;
GO

IF OBJECT_ID(N'dbo.LearnerProgress', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.LearnerProgress', N'IsCompleted') IS NULL
    ALTER TABLE dbo.LearnerProgress ADD IsCompleted AS (CONVERT(BIT, CASE WHEN Status IN (N'Completed', N'Certified') THEN 1 ELSE 0 END)) PERSISTED;
GO
