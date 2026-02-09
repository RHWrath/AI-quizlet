namespace Services
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string DatabaseName { get; set; } = "quizlet";
        public string PlayersCollection { get; set; } = "players";
        public string MusicCollection { get; set; } = "music";
        public string ImagesCollection { get; set; } = "images";
    }
}
