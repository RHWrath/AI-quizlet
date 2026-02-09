using System.Collections;

namespace AI_quizelet_API.DTOs
{
    public class Post
    {
        public int id { get; set; }
        public string discription { get; set; }
        public BitArray BitArray { get; set; }

        public Post(int id, string discription, BitArray bitArray)
        {
            this.id = id;
            this.discription = discription;
            this.BitArray = bitArray;
        }
    }
}
