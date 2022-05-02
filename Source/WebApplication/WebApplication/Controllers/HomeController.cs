using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;


namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {

        const string minerAddress = "miner1";
        const string adminAddress = "admin";
        const string user1Address = "user1";
        const string user2Address = "user2";

        // không dùng static thì mỗi khi gọi actionResult biến blockChain sẽ bị reset về chain rỗng (1 block gốc)
        private static BlockChain blockChain = new BlockChain(proofOfWorkDifficulty: 2, miningReward: 10);
        private static bool isLoaded = false; // chưa load -> true thì không load lại

        public void Load()
        {
            blockChain.CreateTransaction(new Transaction(adminAddress, user1Address, 200));
            blockChain.CreateTransaction(new Transaction(adminAddress, user2Address, 200));
            blockChain.MineBlock(minerAddress);
        }

        public ActionResult Index()
        {
            // Index sẽ là nơi vào đầu tiên
            if (!isLoaded) // chưa được load (!false => true)
            {
                this.Load();
                isLoaded = true;
            }

            ViewBag.ChainContent = blockChain.GetChainTransaction(blockChain);

            return View();
        }

        public ActionResult CreateWallet()
        {
            return View();
        }

        public ActionResult Account()
        {
            return View();
        }

        public ActionResult Transfer()
        {
            return View();
        }
        public ActionResult History()
        {
            ViewBag.AllTransaction = blockChain.GetChainTransaction(blockChain);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Ví điện tử demo";

            return View();
        }
    }
}