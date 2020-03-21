using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskKiller.Models;

namespace TaskKiller.Services
{
    public class ParsingService
    {
        public List<Service> ParseServices(string blob)
        {
            List<Service> services = new List<Service>();

            List<string> servicePieces = blob.Split(new char[] { '\n' }).
                Where(z => z != "\r" && !string.IsNullOrEmpty(z)).ToList();

            bool firstServiceAdded = false;

            Service service = new Service();

            foreach (var servicePiece in servicePieces)
            {
                if (!servicePiece.Contains(":"))
                {
                    service.State += servicePiece;
                    continue;
                }

                if (GetStringBeforeColon(servicePiece).Contains("SERVICE_NAME"))
                {
                    if(!firstServiceAdded)
                        firstServiceAdded = true;
                    else
                        services.Add(service);
                    
                    service = new Service();
                    service.ServiceName = GetStringAfterColon(servicePiece);
                }
                else if (GetStringBeforeColon(servicePiece).Contains("DISPLAY_NAME"))
                    service.DisplayName = GetStringAfterColon(servicePiece);

                else if (servicePiece.Contains("TYPE"))
                    service.Type = GetStringAfterColon(servicePiece);

                else if (GetStringBeforeColon(servicePiece).Contains("STATE"))
                    service.State = GetStringAfterColon(servicePiece);
                else if (GetStringBeforeColon(servicePiece).Contains("PID"))
                    service.PID = GetStringAfterColon(servicePiece);
            }

            services.Add(service);

            return services;
        }

        private string GetStringAfterColon(string str)
        {
            return str.Substring(str.IndexOf(":") + 1).Trim();
        }

        private string GetStringBeforeColon(string str)
        {
            return str.Substring(0, str.IndexOf(":")).Trim();
        }
    }
}
