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

            


            config.Formatters
                .JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));

            server = new HttpSelfHostServer(config);

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}", new { id = RouteParameter.Optional }, null,
                new BasicAuthorizationHandler(server.Configuration));

            //var cors = new EnableCorsAttribute("*", "*", "*"); //跨域允许设置

            //config.EnableCors(cors);
            
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
