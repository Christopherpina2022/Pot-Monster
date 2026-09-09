using System.IO;

public class Queries
{
    public static string SmokeTest = Load("SmokeTest.graphql");
    public static string GetOwnerByTournamentSlug = Load("GetOwnerByTournamentSlug.graphql");
    public static string TournamentHeadCount = Load("TournamentHeadCount.graphql");
    public static string SearchTournamentsByOwner = Load("SearchTournamentsByOwner.graphql");
    public static string TournamentTop8 = Load("TournamentTop8.graphql");

    private static string Load(string file)
    {
        string query = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Queries", file));
        return query;
    }
}