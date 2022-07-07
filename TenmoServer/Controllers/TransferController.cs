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
    public class TransferController : ControllerBase
    {
        private readonly ITransferDAO transferDAO;
        private readonly IAccountDAO accountDAO;
        public TransferController(ITransferDAO transfer, IAccountDAO account)
        {
            this.transferDAO = transfer;
            this.accountDAO = account;
        }
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
        [HttpPost()]
        [Authorize]
        public ActionResult Transfer(Transfers transfers)
        {
            int currentUserId = LoggedInUserId;

            if (currentUserId <= 0)
            {
                return Unauthorized("Please use valid Login Credientals homeslice");
            }

            Account currentUserAccount = accountDAO.GetAccountByUserId(currentUserId);
            
            if(currentUserAccount == null)
            {
                return NotFound("Could not find your account. Feels bad man");
            }

            if (transfers.AccountFrom != currentUserAccount.AccountId)
            {
                return BadRequest("You cannot transfer money from an account that isn't yours bruh");
            }

            Account targetAccount = accountDAO.GetAccountByAccountId(transfers.AccountTo);

            if (targetAccount == null)
            {
                return NotFound("could not find target account broseph");
            }
            
            if(transfers.TransferAmount > currentUserAccount.Balance)
            {
                return BadRequest("You need more cash money");
            }
            if(transfers.TransferAmount<=0)
            {
                return BadRequest("That ain't very cash money");
            }
            if (transfers.AccountTo == transfers.AccountFrom)
            {
                return BadRequest("You can't transfer money to yourself buddy pal");
            }
            transfers = transferDAO.TransferMoneyToUser(transfers.AccountTo, transfers.TransferAmount, transfers.AccountFrom);
            return Ok(transfers);
        }
    }
}
