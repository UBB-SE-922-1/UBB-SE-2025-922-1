using System;

namespace Duo.Constants
{
    public static class VerificationConstants
    {
        /// Minimum value for a verification code
        public const int MinimumVerificationCodeValue = 100000;

        /// Maximum value for a verification code 
        public const int MaximumVerificationCodeValue = 999999;

        /// Number of milliseconds to simulate API delay for verification code sending         
        public const int VerificationCodeSendingDelayMilliseconds = 1000;
    }


    public static class MockDataConstants
    {
        /// Milliseconds delay for simulating asynchronous operations in mock services
        public const int MockAsyncOperationDelayMilliseconds = 100;

        /// Minimum number of random quizzes to generate
        public const int MinimumRandomQuizCount = 10;

         
        /// Maximum number of random quizzes to generate
        public const int MaximumRandomQuizCount = 21;

        /// Maximum days in the past for generated completion dates
        public const int MaximumCompletionDaysInPast = 365;

        /// Minimum random quiz index
        public const int MinimumRandomIndex = 1;

        /// Decimal places for rounding accuracy percentages
        public const int AccuracyDecimalPlaces = 2;
    }

     
    /// Application-wide constants for leaderboard operations
     
    public static class LeaderboardConstants
    {
        /// Criteria for sorting by completed quizzes
        public const string CompletedQuizzesCriteria = "CompletedQuizzes";

         
        /// Criteria for sorting by accuracy
        public const string AccuracyCriteria = "Accuracy";

         
        /// Default value for no rank
        public const int NoRankValue = -1;

         
        /// Adjustment to convert from zero-based index to one-based rank
        public const int RankIndexAdjustment = 1;
    }

    /// Application-wide constants for UI-related values
    public static class UserInterfaceConstants
    {
        /// Default friend count message template
        public const string FriendCountTemplate = "{0} friends";
    }
} 