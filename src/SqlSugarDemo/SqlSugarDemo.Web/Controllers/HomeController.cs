using Microsoft.AspNetCore.Mvc;
using SqlSugarDemo.Busines;
using SqlSugarDemo.Web.Controllers.Base;
using System;

namespace SqlSugarDemo.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly Lazy<HomeManager> _HomeManager;

        public HomeController()
        {
            this._HomeManager = new Lazy<HomeManager>();
        }

        /// <summary>
        /// 主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            //var result = this._HomeManager.Value.DBTest();    //DB测试
            //var result = this._HomeManager.Value.NotAutoDisposeTest();      //不自动释放测试
            var result = this._HomeManager.Value.ExecTest();      //不自动释放测试
            ViewBag.Result = result;
            return View();
        }
    }
}