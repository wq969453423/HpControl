using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;
using System.Web.Http.SelfHost;

namespace 子端.Controller
{
    public class HttpServer
    {
        private HttpSelfHostServer server;

        public HttpServer(string ip, int port)
        {
            var config = new HttpSelfHostConfiguration($"http://{ip}:{port}");
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}");
            config.Formatters
                .JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));

            //var cors = new EnableCorsAttribute("*", "*", "*"); //跨域允许设置
            
            //config.EnableCors(cors);
            server = new HttpSelfHostServer(config);
        }

        public Task StartHttpServer()
        {
            return server.OpenAsync();
        }
        

        public Task CloseHttpServer()
        {
            return server.CloseAsync();
        }
    }
}
