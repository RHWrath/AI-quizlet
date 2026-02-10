using DTOs.Images;
using DTOs.Music;

namespace AI_quizelet_API.DTOs
{
    public class Post
    {
        public int id { get; set; }
        public ImageResponse image { get; set; }
        public MusicResponse music { get; set; }
        public Post(int id, ImageResponse image, MusicResponse music)
        {
            this.id = id;
            this.image = image;
            this.music = music;
        }
    }
}
