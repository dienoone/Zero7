using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using O7.Core.Interfaces;
using O7.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O7.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // api/account/register:
        //[Authorize(Roles ="SuperAdmin")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.Register(model);

                if (result.IsAuthenticated)
                {
                    return Ok(result); // Status Code: 200 OK
                }
                return BadRequest(result.Message); // Status Code: 400 BadRequest!!
            }
            return BadRequest(ModelState); // Status Code: 400 BadRequest!!
        }

        // api/account/login: => Done:
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.Login(model);

                if (result.IsAuthenticated)
                {
                    // TODO: Email 

                    return Ok(result); // Status Code: 200 OK
                }
                return BadRequest(result.Message); // Status Code: 400 BadRequest!!
            }
            return BadRequest(ModelState); // Status Code: 400 BadRequest!!
        }

        // api/account/refreshToken => Done:
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.RefreshToken(model.refreshToken);
                if (result.IsAuthenticated)
                {
                    return Ok(result); // Status Code: 200 OK
                }
                return BadRequest(result); // Status Code: 400 BadRequest!!
            }
            return BadRequest(ModelState); // Status Code: 400 BadRequest!!
        }

        // api/account/revokeToken => Done:
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Token))
                {
                    var result = await _accountRepository.RevokeToken(model.Token);
                    if (result)
                    {
                        return Ok(); // Status Code: 200 OK
                    }
                    return BadRequest("Token Is Invalid");  // Status Code: 400 BadRequest!!
                }
                return BadRequest("Token Is Required");  // Status Code: 400 BadRequest!!
            }
            return BadRequest(ModelState); // Status Code: 400 BadRequest!!
        }


    }
}
