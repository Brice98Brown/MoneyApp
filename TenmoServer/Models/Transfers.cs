using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfers
    {
        public int TransferId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public string TransferStatus { get; set; }
        public string TransferType { get; set; }
        public decimal TransferAmount { get; set; }
        public int SenderUserId { get; set; }
        public int RecipientUserId { get; set; }
        public string SenderUsername { get; set; } = "";
        public string RecipientUsername { get; set; } = "";


    }
}
