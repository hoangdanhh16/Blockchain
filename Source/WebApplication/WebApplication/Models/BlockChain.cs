using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace WebApplication.Models
{
    public class BlockChain
    {
        private readonly int _proofOfWorkDifficulty;
        private readonly double _miningReward;
        private List<Transaction> _pendingTransactions;
        public List<Block> Chain { get; set; }
        public BlockChain(int proofOfWorkDifficulty, int miningReward)
        {
            _proofOfWorkDifficulty = proofOfWorkDifficulty;
            _miningReward = miningReward;
            _pendingTransactions = new List<Transaction>();
            Chain = new List<Block> { CreateGenesisBlock() };
        }

        public void CreateTransaction(Transaction transaction)
        {
            _pendingTransactions.Add(transaction);
        }

        public void MineBlock(string minerAddress)
        {
            Transaction minerRewardTransaction = new Transaction(null, minerAddress, _miningReward);
            _pendingTransactions.Add(minerRewardTransaction);
            Block block = new Block(DateTime.Now, _pendingTransactions);
            block.MineBlock(_proofOfWorkDifficulty);
            block.PreviousHash = Chain.Last().Hash;
            Chain.Add(block);
            _pendingTransactions = new List<Transaction>();
        }

        public bool IsValidChain()
        {
            for(int i = 1; i < Chain.Count; i++)
            {
                Block prevBlock = Chain[i - 1];
                Block curBlock = Chain[i];
                if (curBlock.Hash != curBlock.CreateHash())
                    return false;
                if (curBlock.PreviousHash != prevBlock.Hash)
                    return false;
            }
            return true;
        }

        public double GetBalance(string address)
        {
            double balance = 0;
            foreach(Block block in Chain)
            {
                foreach(Transaction transaction in block.Transactions)
                {
                    if(transaction.From == address)
                    {
                        balance -= transaction.Amount;
                    }
                    if(transaction.To == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }

        private Block CreateGenesisBlock()
        {
            List<Transaction> transactions = new List<Transaction> { new Transaction("", "", 0) };
            return new Block(DateTime.Now, transactions, "0");
        }

        public String PrintChain(BlockChain blockChain)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("----------------- Start Blockchain -----------------\r\n");
            builder.Append("\n");
            foreach (Block block in blockChain.Chain)
            {
                Console.WriteLine("** 1 block exist **");
                builder.Append("\n");
                builder.Append("------ Start Block ------\n");
                builder.Append("Hash: ");
                builder.Append(block.Hash);
                builder.Append("\n");
                builder.Append("Previous Hash: ");
                builder.Append(block.PreviousHash);
                builder.Append("\n");
                builder.Append("--- Start Transactions ---\n");
                foreach (Transaction transaction in block.Transactions)
                {
                    builder.Append("From: ");
                    builder.Append(transaction.From);
                    builder.Append(" To ");
                    builder.Append(transaction.To);
                    builder.Append(" Amount ");
                    builder.Append(transaction.Amount.ToString());
                    builder.Append("\n");
                }
                builder.Append("--- End Transactions ---\n");
                builder.Append("------ End Block ------\n");
            }
            builder.Append("----------------- End Blockchain -----------------\n");
            String res = builder.ToString();
            return res;
        }

        public List<string> GetChainInfor(BlockChain blockChain)
        {
            List<string> ls = new List<string>();
            ls.Add("----------------- Start Blockchain -----------------");
            foreach (Block block in blockChain.Chain)
            {
                ls.Add("\n");
                ls.Add("------ Start Block ------");
                ls.Add("Hash: " + block.Hash);
                ls.Add("Previous Hash: " + block.PreviousHash);
                ls.Add("--- Start Transactions ---");
                foreach (Transaction transaction in block.Transactions)
                {
                    ls.Add("From: " + transaction.From + " To " + transaction.To + " Amount " + transaction.Amount.ToString());
                }
                ls.Add("--- End Transactions ---");
                ls.Add("------ End Block ------");
            }
            ls.Add("----------------- End Blockchain -----------------\n");
            return ls;
        }

        public List<string> GetChainTransaction(BlockChain blockChain)
        {
            List<string> ls = new List<string>();
            foreach (Block block in blockChain.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    ls.Add("[" + transaction.From + "] đã chuyển cho [" + transaction.To + "] số tiền " + transaction.Amount.ToString() + " (VHDCOIN)");
                }
            }
            return ls;
        }
    }
}