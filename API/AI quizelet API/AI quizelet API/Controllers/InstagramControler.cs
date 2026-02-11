using Services; // <- belangrijk voor PlayerService, ImageService, MusicService en MongoDbSettings
using Microsoft.AspNetCore.Mvc;
using AI_quizelet_API.DTOs;
using DTOs.Images;
using DTOs.Music;
using Entities;
using System.Runtime.Intrinsics.X86;

[ApiController]
[Route("insta")]
public class InstagramController : ControllerBase
{
    private readonly ImageService _imageService;
    private readonly MusicService _musicService;
    private readonly PlayerService _playerService;

    public InstagramController(ImageService imageService, MusicService musicService, PlayerService playerService)
    {
        _imageService = imageService;
        _musicService = musicService;
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<List<Post>> GetPost()
    {
        List<Image> images = await _imageService.GetAllAsync();
        List<Music> music = await _musicService.GetAllAsync();

        List<Post> posts = new();

        try
        {
            for (int i = 0; i < images.Count; i++)
            {
                ImageResponse imageRe = new(images[i]._Id, images[i].link);
                MusicResponse musicRe = null;
                if (music.Count <= i)
                {
                    musicRe = new(music[i]._Id, music[i].link);
                }
                posts.Add(new(images[i].postId, imageRe, musicRe));
            }

            Random ran = new Random();
            List<Post> selectedPots = new();
            for (int i = 0; i < 10; i++)
            {
                int selected = ran.Next(posts.Count-1);
                selectedPots.Add(posts[selected]);
                posts.Remove(posts[selected]);
            }

            return selectedPots;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    [HttpPost]
    public bool PostAnswer(string playerId, int postId, bool answer)
    {
        bool correct = true;

        Music music = _musicService.GetByPostIdAsync(postId).Result;
        if (music.AI == answer) correct = false;

        Image image = _imageService.GetByPostIdAsync(postId).Result;
        if (image.AI == answer) correct = false;

        if (correct)
        {
            int currentScore = _playerService.GetByIdAsync(playerId).Result.Score;
            _playerService.UpdateScoreAsync(playerId, currentScore + 1);
        }

        return correct;
    }

    [HttpPost("createaccount")]
    public bool CreateAccount(string name)
    {
        try
        {
            Player player = new(name);
            _playerService.CreateAsync(player);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
