using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using UserAPISample.Attributes;
using UserAPISample.Model;
using UserAPISample.Bll;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPISample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // GET: api/<UserController>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return _userService.GetAsync();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUserAsync(int id)
        {
            var user = await _userService.GetAsync(id).ConfigureAwait(false);
            return user ?? (ActionResult<User>)NotFound(id);
        }

        // POST api/<UserController>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ValidateModel] - Commented out to be able to unit test the method against invalid model
        public async Task<ActionResult<User>> CreateUserAsync([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newUser = await _userService.CreateAsync(user).ConfigureAwait(false);

            return CreatedAtAction(
                nameof(GetUserAsync),
                new { id = newUser.Id }, newUser);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateModel]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] User user)
        {
            //Validate the input data:
            if (id != user.Id)
            {
                return BadRequest();
            }

            var dbUser = await _userService.GetAsync(id).ConfigureAwait(false);
            if (dbUser == null)
            {
                return NotFound(id);
            }

            //Update the user
            await _userService.UpdateAsync(id, user).ConfigureAwait(false);

            return NoContent();
        }

        // Sample for JSON PATCH
        //
        //[HttpPatch("{id}")]
        //public async Task<IActionResult> JsonPatchWithModelStateAsync(int id, 
        //    [FromBody] JsonPatchDocument<User> patchDoc)
        //{
        //    if (patchDoc != null)
        //    {
        //        var dbUser = await _userService.GetAsync(id).ConfigureAwait(false);
        //        if (dbUser == null)
        //        {
        //            return NotFound();
        //        }

        //        patchDoc.ApplyTo(dbUser, ModelState);

        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        return new ObjectResult(dbUser);
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState);
        //    }
        //}

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var dbUser = await _userService.GetAsync(id).ConfigureAwait(false);
            if (dbUser == null)
            {
                return NotFound(id);
            }

            await _userService.RemoveAsync(dbUser.Id).ConfigureAwait(false);

            return NoContent();
        }
    }
}
