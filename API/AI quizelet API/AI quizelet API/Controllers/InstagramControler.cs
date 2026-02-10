using AI_quizelet_API.DTOs;
using Application.Services;
using DTOs.Images;
using DTOs.Music;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AI_quizelet_API.Controllers
{
    [ApiController]
    [Route("insta")]
    public class InstagramControler
    {
        public ImageService imageService;
        public MusicService musicService;
        public PlayerService playerService;

        InstagramControler()
        {
            MongoDbSettings settings = new();
            imageService = new ImageService(settings);
            musicService = new MusicService(settings);
            playerService = new PlayerService(settings);
        }

        [HttpGet]
        async public Task<List<Post>> GetPost()
        {
            List<Image> images = imageService.GetAllAsync().Result;
            List<Music> music = musicService.GetAllAsync().Result;

            List<Post> posts = new List<Post>();

            try
            {
                for (int i = 0; i < images.Count(); i++)
                {
                        ImageResponse imageRe = new(
                            images[i].Id,
                            images[i].Link);

                        MusicResponse musicRe = new(
                            music[i].Id,
                            music[i].Link);

                        posts.Add(new(
                            images[i].PostID,
                            imageRe,
                            musicRe));
                }
                return posts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }

        [HttpPost]
        public bool Postanswer(string playerId, int postId, bool answer)
        {
            bool correct = true;

            Music music = musicService.GetByPostIdAsync(postId).Result;
            if (music.AI == answer)
            {
                correct = false;
            }

            Image image = imageService.GetByPostIdAsync(postId).Result;
            if (image.AI == answer)
            {
                correct = false;
            }

            if (correct)
            {
                int currentScore = playerService.GetByIdAsync(playerId).Result.Score;
                int newScore = currentScore + 1;

                playerService.UpdateScoreAsync(playerId, newScore);
            }

            return correct;
        }

        [HttpPost]
        [Route("createacount")]
        public bool CreateAcount(string name)
        {
            try
            {
                Player player = new(name);

                playerService.CreateAsync(player);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
