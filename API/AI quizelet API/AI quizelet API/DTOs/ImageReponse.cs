namespace DTOs.Images;

public class ImageResponse
{
    public string Id { get; set; }
    public string Url { get; set; }

    public ImageResponse(string id, string link)
    {
        Id = id;
        Url = link;
    }
}
