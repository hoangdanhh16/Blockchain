using System;
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
        private static List<string> walletList = new List<string>() { adminAddress, user1Address, user2Address }; // khởi đầu với 3 ví
        private static List<string> passwordList = new List<string>() { "admin", "user1", "user2" };
        private static string acc = ""; // tên ví, rỗng là chưa đăng nhập

        public void Load()
        {
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

            return View("Index");
        }

        public ActionResult CreateWallet()
        {
            ViewBag.status = "";
            ViewBag.Acc = acc;
            return View("CreateWallet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckCreateWallet(User _user)
        {
            if (walletList.Contains(_user.username))
            {
                ViewBag.status = "Tên đăng nhập đã tồn tại!";
                ViewBag.Acc = acc;

                return View("CreateWallet");
            }
            else
            {
                walletList.Add(_user.username);
                passwordList.Add(_user.password);
                acc = _user.username;

                ViewBag.AllChainContent = blockChain.GetHomeInfor();

                return View("Index");
            }
        }


        public ActionResult Account()
        {
            if (acc == "")
            {
                ViewBag.Acc = acc;
                ViewBag.Money = 0;

                return View("Account");
            }
            else
            {
                ViewBag.Acc = acc;
                ViewBag.Money = blockChain.GetBalance(acc);

                return View("Account");
            }
        }

        public ActionResult Transfer()
        {
            ViewBag.Acc = acc;
            ViewBag.status = "";

            return View("Transfer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferClick(WalletTransactions wallTrans)
        {
            if (!walletList.Contains(wallTrans.walletname))
            {
                ViewBag.Acc = acc;
                ViewBag.status = "Ví đích không tồn tại!";

                return View("Transfer");
            }
            else
            {
                var mon = int.Parse(wallTrans.money);
                var balance = blockChain.GetBalance(acc);

                if (balance <= mon)
                {
                    ViewBag.Acc = acc;
                    ViewBag.status = "Số tiền chuyển không thể vượt quá số tiền trong ví" + " (" + balance.ToString() + " VHDCOIN) !";

                    return View("Transfer");
                }
                else
                {
                    blockChain.CreateTransaction(new Transaction(acc, wallTrans.walletname, mon));
                    blockChain.MineBlock(minerAddress);

                    ViewBag.AllChainContent = blockChain.GetHomeInfor();

                    return View("Index");
                }
            }
        }


        public ActionResult History()
        {
            ViewBag.AllTransaction = blockChain.GetChainTransaction();
            return View("History");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Ví điện tử demo";

            return View("About");
        }

        public ActionResult Login()
        {
            ViewBag.Acc = acc;
            ViewBag.status = "";

            return View("Login");
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