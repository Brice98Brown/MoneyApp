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
        private readonly IUserDAO userDAO;
        public TransferController(ITransferDAO transfer, IAccountDAO account, IUserDAO user)
        {
            this.transferDAO = transfer;
            this.accountDAO = account;
            this.userDAO = user;
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

            if (currentUserAccount == null)
            {
                return NotFound("Could not find your account. Feels bad man");
            }

            if (transfers.SenderUserId != currentUserAccount.UserId)
            {
                return BadRequest("You cannot transfer money from an account that isn't yours bruh");
            }

            Account targetAccount = accountDAO.GetAccountByUserId(transfers.RecipientUserId);

            if (targetAccount == null)
            {
                return NotFound("could not find target account broseph");
            }

            if (transfers.TransferAmount > currentUserAccount.Balance)
            {
                return BadRequest("You need more cash money");
            }
            if (transfers.TransferAmount <= 0)
            {
                return BadRequest("That ain't very cash money");
            }
            if (transfers.RecipientUserId == transfers.SenderUserId)
            {
                return BadRequest("You can't transfer money to yourself buddy pal");
            }
            transfers = transferDAO.TransferMoneyToUser(targetAccount.AccountId, transfers.TransferAmount, currentUserAccount.AccountId);
            transfers.SenderUserId = currentUserAccount.UserId;
            transfers.RecipientUserId = targetAccount.UserId;
            transfers.AccountFrom = 0;
            transfers.AccountTo = 0;
            return Ok(transfers);
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult GetAllTransfersByUser(int id)// current user's id
        {
            int currentUserId = LoggedInUserId;
            if (currentUserId <= 0)
            {
                return Unauthorized("Please use valid Login Credientals homeslice");
            }
            if (id != currentUserId)
            {
                return BadRequest("Sorry, you can only look at your own transfer history");
            }

            List<Transfers> transfers = transferDAO.GetAllTransfersForOneUser(id);

            List<Transfers> safeTransfers = new List<Transfers>();
            List<User> users = userDAO.GetUsers();

            foreach (Transfers transfer in transfers)
            {
                Transfers newSafeTransfer = new Transfers();

                Account accountTo = accountDAO.GetAccountByAccountId(transfer.AccountTo);
                Account accountFrom = accountDAO.GetAccountByAccountId(transfer.AccountFrom);

                foreach (User user in users)
                {
                    if (user.UserId == accountTo.UserId)
                    {
                        newSafeTransfer.RecipientUsername = user.Username;
                    }
                    else if (user.UserId == accountFrom.UserId)
                    {
                        newSafeTransfer.SenderUsername = user.Username;
                    }
                }
                newSafeTransfer.TransferId = transfer.TransferId;
                newSafeTransfer.TransferAmount = transfer.TransferAmount;
                newSafeTransfer.TransferStatus = transfer.TransferStatus;
                newSafeTransfer.TransferType = transfer.TransferType;
                newSafeTransfer.AccountTo = 0;
                newSafeTransfer.AccountFrom = 0;

                safeTransfers.Add(newSafeTransfer);
            }
            return Ok(safeTransfers);
        }
    }
}
