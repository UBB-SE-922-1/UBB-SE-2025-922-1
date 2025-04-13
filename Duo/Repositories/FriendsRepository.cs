using Duo.Data;
using Duo.Helpers;
using Duo.Interfaces;
using Duo.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Repositories;

public class FriendsRepository : IFriendsRepository
{
    private readonly IDataLink dataLink;

    public IDataLink DataLink => dataLink;

    public FriendsRepository(IDataLink dataLink)
    {
        this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
    }

    public void AddFriend(int userId, int friendId)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@FriendId", friendId)
        };
        dataLink.ExecuteNonQuery("AddFriend", parameters);
    }

    public List<LeaderboardEntry> GetTopFriendsByCompletedQuizzes(int userId)
    {
        SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

        var dataTable = dataLink.ExecuteReader("GetTopFriendsByCompletedQuizzes", parameter);
        List<LeaderboardEntry> users = new List<LeaderboardEntry>();
        int index = 1;
        foreach (DataRow row in dataTable.Rows)
        {

            users.Add(new LeaderboardEntry()
            {
                Rank = index++,
                UserId = Convert.ToInt32(row["UserId"]),
                Username = row["UserName"].ToString()!,
                CompletedQuizzes = Convert.ToInt32(row["QuizzesCompleted"]),
                Accuracy = Convert.ToDecimal(row["Accuracy"]),
                ProfilePicture = "../../Assets/" + row["ProfileImage"].ToString()!
            });
        }

        return users;
    }
    public List<LeaderboardEntry> GetTopFriendsByAccuracy(int userId)
    {
        SqlParameter[] parameter = new SqlParameter[]
        {
                new SqlParameter("@UserId", userId)
            };
        var dataTable = dataLink.ExecuteReader("GetTopFriendsByAccuracy", parameter);
        List<LeaderboardEntry> users = new List<LeaderboardEntry>();
        int index = 1;
        foreach (DataRow row in dataTable.Rows)
        {

            users.Add(new LeaderboardEntry()
            {
                Rank = index++,
                UserId = Convert.ToInt32(row["UserId"]),
                Username = row["UserName"].ToString()!,
                CompletedQuizzes = Convert.ToInt32(row["QuizzesCompleted"]),
                Accuracy = Convert.ToDecimal(row["Accuracy"]),
                ProfilePicture = "../../Assets/" + row["ProfileImage"].ToString()!
            });
        }

        return users;
    }
}
