namespace DTOs.Music;

public class MusicResponse
{
    public string Id { get; set; }

    public string Link { get; set; }

    public MusicResponse(string id, string link)
    {
        Id = id;
        Link = link;
    }
}
