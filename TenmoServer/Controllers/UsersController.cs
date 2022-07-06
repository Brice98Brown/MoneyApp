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
    public class UsersController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;
        public UsersController(IAccountDAO account, IUserDAO user)
        {
            this.accountDAO = account;
            this.userDAO = user;
        }

        [HttpGet("all")]
        [Authorize]
        public ActionResult GetAllUsers()
        {
            List<User> users = userDAO.GetUsers();

            List<User> safeUsers = new List<User>();

            if(users == null)
            {
                return NotFound("whoopsies where are the users?");
            }

            foreach (User user in users)
            {
                User newSafeUser = new User();
                newSafeUser.Username = user.Username;
                newSafeUser.UserId = user.UserId;
                safeUsers.Add(newSafeUser);
            }

            return Ok(safeUsers);
        }
    }
}
