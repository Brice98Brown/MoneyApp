using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using System.Data.SqlClient;

namespace TenmoServer.DAO
{
    public class TransferDAO : ITransferDAO
    {
        private readonly string connectionString;
        public TransferDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public Transfers TransferMoneyToUser(int accountIdTo, decimal ammountToTransfer, int accountIdFrom)
        {


            Transfers transfers = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                const string query = "BEGIN TRANSACTION UPDATE accounts SET balance = balance - @amount WHERE account_id = @account_from UPDATE accounts SET balance = balance + @amount WHERE account_id = @account_to INSERT INTO transfers (transfer_type_id, transfer_status_id,account_from, account_to, amount) VALUES (1001,2001, @account_from, @account_to, @amount) COMMIT TRANSACTION";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", ammountToTransfer);
                cmd.Parameters.AddWithValue("@account_from", accountIdFrom);
                cmd.Parameters.AddWithValue("@account_to", accountIdTo);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                int transferId = Convert.ToInt32(cmd.ExecuteScalar());
                transfers.TransferId = transferId;
                transfers.TransferType = "Send";
                transfers.TransferStatus = "Approved";
                transfers.AccountTo = accountIdTo;
                transfers.AccountFrom = accountIdFrom;
                transfers.TransferAmount = ammountToTransfer;



            }

            return transfers;

        }
    }
}
