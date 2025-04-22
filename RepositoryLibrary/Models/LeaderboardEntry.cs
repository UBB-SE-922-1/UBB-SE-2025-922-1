namespace Duo.Models;

/// <summary>
/// Represents an entry in the leaderboard with user ranking information
/// </summary>
public class LeaderboardEntry
{
    /// <summary>
    /// Gets or sets the unique identifier of the user
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the rank of the user in the leaderboard
    /// </summary>
    public int Rank { get; set; }
    
    /// <summary>
    /// Gets or sets the username of the user
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the profile picture path of the user
    /// </summary>
    public string ProfilePicture { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of completed quizzes
    /// </summary>
    public int CompletedQuizzes { get; set; }
    
    /// <summary>
    /// Gets or sets the accuracy percentage of the user
    /// </summary>
    public decimal Accuracy { get; set; }
    
    /// <summary>
    /// Gets or sets the score value for the leaderboard
    /// </summary>
    public decimal ScoreValue { get; set; }
}
