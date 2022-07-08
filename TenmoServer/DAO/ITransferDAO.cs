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
        Transfers RequestTransferMoneyFromUser(int RecepiantAccountId, decimal ammountToTransfer, int SenderAccountId);
        Transfers UpdateTrasferStatus(int RecepiantAccountId, decimal ammountToTransfer, int transferStatusId, int SenderAccountId, int transferId);
        List<Transfers> GetAllTransfersForOneUser(int userId);
        Transfers GetTransferById(int transferId);


        
    }
}
