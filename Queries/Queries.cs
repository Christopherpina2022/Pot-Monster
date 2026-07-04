using System.IO;

public class Queries
{
    public static string SmokeTest = Load("SmokeTest.graphql");
    public static string SearchTournamentsBySlug = Load("SearchTournamentsBySlug.graphql");
    public static string SearchTournaments = Load("SearchTournaments.graphql");
    public static string TournamentHeadCount = Load("TournamentHeadCount.graphql");
    public static string TournamentGetUser = Load("TournamentGetUser.graphql");
    public static string TournamentTop8 = Load("TournamentTop8.graphql");

    private static string Load(string file)
    {
        string query = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Queries", file));
        return query;
    }
}