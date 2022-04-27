using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_App
{
    internal class Blockchain
    {
        private readonly int _proofOfWorkDifficulty;
        private readonly double _miningReward;
        private List<Transactions> _pendingTransactions;
        public List<Block> Chain { get; set; }

        public Blockchain(int proofOfWorkDifficulty, int miningReward)
        {
            _proofOfWorkDifficulty = proofOfWorkDifficulty;
            _miningReward = miningReward;
            _pendingTransactions = new List<Transactions>();
            Chain = new List<Block> { CreateGenesisBlock() };
        }

        private Block CreateGenesisBlock()
        {
            List<Transactions> transactions = new List<Transactions> { new Transactions("", "", 0) };
            return new Block(DateTime.Now, transactions, "0");
        }

        public double GetBalance(string address)
        {
            double balance = 0;
            foreach (Block block in Chain)
            {
                foreach (Transactions transaction in block.transactions)
                {
                    if (transaction.From == address)
                    {
                        balance -= transaction.Amount;
                    }
                    if (transaction.To == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }

        public bool IsValidChain()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block previousBlock = Chain[i - 1];
                Block currentBlock = Chain[i];
                if (currentBlock.Hash != currentBlock.CreateHash())
                    return false;
                if (currentBlock.PreviousHash != previousBlock.Hash)
                    return false;
            }
            return true;
        }

        public void MineBlock(string minerAddress)
        {
            Transactions minerRewardTransaction = new Transactions(null, minerAddress, _miningReward);
            _pendingTransactions.Add(minerRewardTransaction);
            Block block = new Block(DateTime.Now, _pendingTransactions);
            block.MineBlock(_proofOfWorkDifficulty);
            block.PreviousHash = Chain.Last().Hash;
            Chain.Add(block);
            _pendingTransactions = new List<Transactions>();
        }

        public void CreateTransaction(Transactions transaction)
        {
            _pendingTransactions.Add(transaction);
        }
    }
}
