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
        private BlockChain blockChain = new BlockChain(proofOfWorkDifficulty: 2, miningReward: 10);

        public ActionResult Index()
        {
            blockChain.CreateTransaction(new Transaction(adminAddress, user1Address, 200));
            blockChain.CreateTransaction(new Transaction(adminAddress, user2Address, 200));
            blockChain.MineBlock(minerAddress);
            ViewBag.ChainContent = blockChain.GetChainInfor(blockChain);

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
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Ví điện tử demo.";

            return View();
        }
    }
}