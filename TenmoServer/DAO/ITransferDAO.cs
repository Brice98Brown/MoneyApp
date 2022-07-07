using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfers TransferMoneyToUser(int accountIdTo, decimal ammountToTransfer, int accountIdFrom);
        List<Transfers> GetAllTransfersForOneUser(int userId);

        
    }
}
