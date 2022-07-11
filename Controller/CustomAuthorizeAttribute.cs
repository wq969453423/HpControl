

using Microsoft.AspNetCore.Mvc.Filters;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace 子端.Controller
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 用户授权
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string pubkey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwphYx7wUP4Yzr1XCvB/R
hnDmj5gDZTpAOa8YdhoUnPTYMyRY/6oknKjYh+x/xB3VDDlw9ko8ZgjDO7M/dFWE
Lf8FIb8lur2hkkzQWVkyS7IBkUUnYfCQdMNuyJfZUz92Av3zugKmwrVVm1DStfoa
46HefEB8jODB5fKpnLePvxPTCyJavGqVrYVp2l9Gpu0DGs1Ax+mmCp0B8d3ThrHc
+Q+6op5Qe9i3hhPlL2vTeGG+7s8JHb2enjVH6kroC/KHuY3iGL5eoCMnt4MgS2L/
Vd/x0DOAvIQ8e6J0lfvpYH5IqRV44O25Ru7YUPQEbYxe5yTNShJBTmm+S+13mqNQ
AwIDAQAB
-----END PUBLIC KEY-----
";
            ////url获取token  
            //var content = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
            //HttpRequestBase request = content.Request;
            //string access_key = request.Params["access_key"];

            var bl = true;
            //校验IP
            if (bl)//验证通过放行
            {
                base.IsAuthorized(actionContext);
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    code = HttpStatusCode.Unauthorized,
                    data = "",
                    message = "access_key 或 sign 参数有误，请核对",
                });
            }
        }

        //public static string decryptData(string resData, string privateKey)
        //{
        //    //PKCS1
        //    var pkcs1KeyTuple = EncryptProvider.RSADecrypt(privateKey, resData);
            
        //}
        

}

}
