﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
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
        private static List<string> walletList = new List<string>() { adminAddress, user1Address, user1Address }; // khởi đầu với 3 ví
        private static List<string> passwordList = new List<string>() { "admin", "user1", "user2" };
        private static string acc = ""; // tên ví, rỗng là chưa đăng nhập

        public void Load()
        {
            blockChain.CreateTransaction(new Transaction(adminAddress, user1Address, 200));
            blockChain.CreateTransaction(new Transaction(adminAddress, user2Address, 200));
            blockChain.MineBlock(minerAddress);
            blockChain.CreateTransaction(new Transaction(adminAddress, user1Address, 20));
            blockChain.CreateTransaction(new Transaction(adminAddress, user2Address, 130));
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

            ViewBag.AllChainContent = blockChain.GetHomeInfor();

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
            ViewBag.AllTransaction = blockChain.GetChainTransaction();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Ví điện tử demo";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Acc = acc;
            ViewBag.status = "";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckLogin(User _user)
        {
            if (walletList.Contains(_user.username))
            {
                var idx = walletList.IndexOf(_user.username);
                var pass = passwordList[idx];

                if (_user.password == pass)
                {
                    acc = _user.username;

                    ViewBag.AllChainContent = blockChain.GetHomeInfor();

                    return View("Index");
                }
                else
                {
                    ViewBag.Acc = acc;
                    ViewBag.status = "Sai mật khẩu";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.Acc = acc;
                ViewBag.status = "Sai tên đăng nhập";

                return View("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            acc = "";
            ViewBag.Acc = acc;
            ViewBag.status = "";
            return View("Login");
        }
    }
}