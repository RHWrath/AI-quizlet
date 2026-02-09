using AI_quizelet_API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AI_quizelet_API.Controllers
{
    [ApiController]
    [Route("insta")]
    public class InstagramControler
    {
        InstagramControler() { }

        [HttpGet]
        public Post GetPost()
        {
            //TODO retrieve IGpost from DB

            throw new NotImplementedException();
        }

        [HttpPost]
        public bool Postanswer(string userName, int postId)
        {
            //TODO validate answer

            //TODO save answer to DB

            throw new NotImplementedException();
        }
    }
}
