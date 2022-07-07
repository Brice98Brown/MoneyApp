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
        public TransferController(ITransferDAO transfer)
        {
            this.transferDAO = transfer;
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
        [HttpPost]
        [Authorize]
        public ActionResult Transfer(Transfers transfers)
        {
            
            
            int id = LoggedInUserId;
            transfers.AccountFrom = id;
            transfers.AccountTo = ;
            transfers.TransferAmount = ;
            transfers.TransferType = ;
            transfers.TransferStatus = ;


            return Ok();
        }
    }
}
