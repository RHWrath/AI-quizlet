using AI_quizelet_API.DTOs;
using Application.Services;
using DTOs.Images;
using DTOs.Music;
using Entities;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
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
                            images[i].PostId,
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
        public bool Postanswer(string userName, string postId, bool answer)
        {
            //TODO validate answer

            //TODO save answer to DB

            throw new NotImplementedException();
        }
    }
}
