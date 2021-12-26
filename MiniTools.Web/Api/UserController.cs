using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiniTools.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;

        private readonly UserCollectionService userCollectionService;

        public UserController(ILogger<UserController> logger, UserCollectionService userCollectionService)
        {
            this.logger = logger;
            this.userCollectionService = userCollectionService;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}", Name ="apiUserGet")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        //[HttpPost]
        //public void Add([FromBody] string value)
        //{
        //}

        // POST api/<UserController>
        [HttpPost]
        public IActionResult Post([FromBody] AddUserRequest model)
        {
            //logger.LogInformation("AAAAAAAAAAAAAAAAAAAAAA");
            //logger.LogInformation(model.Username);

            User newUser = new User(model);

            userCollectionService.AddUser(newUser);

            //User newUser = new User
            //{
            //    Username = value.Username,
            //    Password = value.Password,
            //    FirstName = value.FirstName,
            //    LastName = value.LastName,
            //    Email = value.Email,
            //    Status = Models.UserStatus.Active
            //};

            //await _userCollection.InsertOneAsync(newUser);

            return CreatedAtAction(nameof(Post), newUser);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
