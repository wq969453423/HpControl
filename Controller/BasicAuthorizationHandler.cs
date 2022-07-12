using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace 子端.Controller
{
    public class BasicAuthorizationHandler : DelegatingHandler {

        public BasicAuthorizationHandler(HttpConfiguration httpConfiguration)
        {
            InnerHandler = new HttpControllerDispatcher(httpConfiguration);
        }

        private bool ValidateRequest(HttpRequestMessage message)
        {
            string pubkey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwphYx7wUP4Yzr1XCvB/R
hnDmj5gDZTpAOa8YdhoUnPTYMyRY/6oknKjYh+x/xB3VDDlw9ko8ZgjDO7M/dFWE
Lf8FIb8lur2hkkzQWVkyS7IBkUUnYfCQdMNuyJfZUz92Av3zugKmwrVVm1DStfoa
46HefEB8jODB5fKpnLePvxPTCyJavGqVrYVp2l9Gpu0DGs1Ax+mmCp0B8d3ThrHc
+Q+6op5Qe9i3hhPlL2vTeGG+7s8JHb2enjVH6kroC/KHuY3iGL5eoCMnt4MgS2L/
Vd/x0DOAvIQ8e6J0lfvpYH5IqRV44O25Ru7YUPQEbYxe5yTNShJBTmm+S+13mqNQ
AwIDAQAB
-----END PUBLIC KEY-----";

            return true;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           
            var response = new HttpResponseMessage();
            try
            {
                if (request.Method == HttpMethod.Options)
                {
                    //response.StatusCode = HttpStatusCode.OK;
                    //return response;
                    //前端验证跨域
                    response = await base.SendAsync(request, cancellationToken);
                    response.StatusCode = HttpStatusCode.OK;
                }
                else if (!ValidateRequest(request))
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                    var content = new Result
                    {
                        success = false,
                        errs = new[] { "服务端拒绝访问：你没有权限" }
                    };
                    response.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");//content添加错误信息
                }
                else
                {
                    response = await base.SendAsync(request, cancellationToken);
                }
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Cache-Control", "no-cache");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,Authorization,token");//允许自定义头
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                return response;
            }
            catch (Exception e)
            {
                return response;//返回响应
                
            }


           

        }

    }
    

    public class Result//构建用于返回错误信息的对象
    {
        public bool success { get; set; }
        public string[] errs { get; set; }
    }

}
