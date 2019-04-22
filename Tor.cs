using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TorUnmask
{
    public class Tor : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // pre-processing
            List<string> remoteOctets = context.HttpContext.Connection.RemoteIpAddress.ToString().Split(".").ToList();
            remoteOctets.Reverse();
            string remoteOctetsreversed = String.Join(".", remoteOctets);

            List<string> serverOctets = context.HttpContext.Connection.LocalIpAddress.ToString().Split(".").ToList();
            serverOctets.Reverse();
            string serverOctetsreversed = String.Join(".", serverOctets);

            string tordnsel = String.Join(".", remoteOctetsreversed,
                                                            context.HttpContext.Connection.LocalPort.ToString(),
                                                            serverOctetsreversed);

            IPHostEntry tor = Dns.GetHostEntry(tordnsel + ".ip-port.exitlist.torproject.org");
            if (tor.HostName == "127.0.0.2")
            {
                // tor node
                context.HttpContext.Response.Redirect($"./api/pit/{Guid.NewGuid()}");
            }
            // clearnet node ->  smooth sailing
            base.OnActionExecuting(context);
        }
    }
}
