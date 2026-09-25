using System.Collections.Generic;

namespace RespondX.Helpers
{
    public sealed class QuizDefinition
    {
        public int QuizID { get; set; }
        public string Title { get; set; }
        public string ModuleTitle { get; set; }
        public string Description { get; set; }
        public int TimeLimitMinutes { get; set; }
        public int PassingScore { get; set; }
        public int MaxAttempts { get; set; }
    }

    public static class QuizCatalog
    {
        public static List<QuizDefinition> GetAll()
        {
            return new List<QuizDefinition>
            {
                new QuizDefinition
                {
                    QuizID = 1,
                    Title = "Emergency Response Fundamentals",
                    ModuleTitle = "Introduction to Emergency Response",
                    Description = "Test your knowledge of emergency response basics.",
                    TimeLimitMinutes = 30,
                    PassingScore = 70,
                    MaxAttempts = 3
                },
                new QuizDefinition
                {
                    QuizID = 2,
                    Title = "CPR and First Aid Certification",
                    ModuleTitle = "CPR and First Aid",
                    Description = "Comprehensive assessment of CPR and first aid knowledge.",
                    TimeLimitMinutes = 45,
                    PassingScore = 80,
                    MaxAttempts = 3
                },
                new QuizDefinition
                {
                    QuizID = 3,
                    Title = "Emergency Communication",
                    ModuleTitle = "Emergency Communication",
                    Description = "Test your communication skills in emergency situations.",
                    TimeLimitMinutes = 20,
                    PassingScore = 70,
                    MaxAttempts = 2
                }
            };
        }

        public static QuizDefinition GetById(int quizId)
        {
            return GetAll().Find(quiz => quiz.QuizID == quizId);
        }
    }
}
