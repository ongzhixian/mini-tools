using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.DataEntities;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiniTools.Web.Api
{

    //[Authorize("AuthorizedBearer")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;

        private readonly IUserCollectionService userCollectionService;

        public UserController(ILogger<UserController> logger, IUserCollectionService userCollectionService)
        {
            this.logger = logger;
            this.userCollectionService = userCollectionService;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]ushort page, ushort pageSize)
        {
            Models.PageData<UserAccount>? result = await userCollectionService.GetUserAccountListAsync(new Options.DataPageOption
            {
                PageSize = pageSize,
                Page = page,
            });

            return Ok(result);
        }

        // GET api/<UserController>/5
        //[HttpGet("{id}", Name ="apiUserGet")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] AddUserRequest model)
        {
            IActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
                logger.LogInformation("Result {result}", result);
                return result;
            }

            try
            {
                UserAccount newUser = new UserAccount
                {
                    Username = model.Username,
                    Password = model.Password
                };

                await userCollectionService.AddUserAsync(newUser);

                return CreatedAtAction(nameof(PostAsync), newUser);
            }
            catch (Exception ex)
            {
                //return UnprocessableEntity(model); // HTTP 422 
                //return StatusCode(StatusCodes.Status500InternalServerError);

                logger.LogError(ex, "model {@model}", model);
                return Problem(
                    detail: ex.Message, 
                    instance: Request?.Path, 
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: $"Cannot add user", 
                    type: nameof(PostAsync)
                    );
            }
        }

        // PUT api/<UserController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
