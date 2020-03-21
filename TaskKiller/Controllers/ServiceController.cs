using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskKiller.Helpers;
using TaskKiller.Models;
using TaskKiller.Services;

namespace TaskKiller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        CommandService _commandService;
        ParsingService _parsingService;

        public ServiceController()
        {
            _commandService = new CommandService();
            _parsingService = new ParsingService();
        }

        // GET: api/Service/5
        [HttpGet("{machineName}", Name = "Get")]
        public List<Service> Get(string machineName)
        {
            string getServicesCommand = Commands.GetAllServicesCommand(machineName);

            string serviceOutput = _commandService.ExecuteCommand(getServicesCommand);

            List<Service> services = _parsingService.ParseServices(serviceOutput);

            services.ForEach(s => s.MachineName = machineName);

            return services;
        }




        // POST: api/Service
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Service/5
        [HttpPut("{machineName}")]
        public KillResponse Put(string machineName, [FromBody] KillCommand cmd)
        {
            string killServiceByPid = Commands.KillServiceByPID(machineName, cmd.PID);

            string cmdOuput = _commandService.ExecuteCommand(killServiceByPid);

            return new KillResponse { Message = cmdOuput };
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}