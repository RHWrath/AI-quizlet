using DTOs.Images
using DTOs.Music;

namespace AI_quizelet_API.DTOs
{
    public class Post
    {
        public string id { get; set; }
        public ImageResponse image { get; set; }
        public MusicResponse music { get; set; }
        public Post(string id, ImageResponse image, MusicResponse music)
        {
            this.id = id;
            this.image = image;
            this.music = music;
        }
    }
}
