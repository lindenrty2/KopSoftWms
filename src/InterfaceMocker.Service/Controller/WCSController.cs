using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfaceMocker.Service
{
    [Route("/{controller}")]
    public class WCSController : Controller
    {

        [HttpGet("ping")]
        public string Ping()
        {
            return "OK";
        }

    }
}
