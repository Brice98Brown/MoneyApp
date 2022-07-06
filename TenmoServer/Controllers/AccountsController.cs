using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;
        public AccountsController(IAccountDAO account, IUserDAO user)
        {
            this.accountDAO = account;
            this.userDAO = user;
        }
        /// <summary>
        /// Gets and returns the ID of the logged in user from their JWT.
        /// This will return -1 if the user is not logged in.
        /// </summary>
        private int LoggedInUserId
        {
            get
            {
                Claim idClaim = User.FindFirst("sub");
                if (idClaim == null)
                {
                    // User is not logged in
                    return -1;
                }
                else
                {
                    // User is logged in. Their subject (sub) claim is their ID
                    return int.Parse(idClaim.Value);
                }
            }
        }
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult GetAccount(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Account id must be positive homes");
            }
            Account account = accountDAO.GetAccountByAccountId(id);
            if (account == null)
            {
                return NotFound("could not find account number broseph");
            }
            int userId = LoggedInUserId;
            if (userId != account.UserId)
            {
                return Forbid("This aint your account homie-g");
            }
            return Ok(account);
        }
    }
    
}
