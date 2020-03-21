namespace TaskKiller.Helpers
{
    public static class Commands
    {
        public static string GetAllServicesCommand(string machineName)
        {
            return @"/C sc \\" + machineName + " queryex type=service state=all";
        }

        public static string KillServiceByPID(string machineName, string pid)
        {
            return @"/C taskkill /s " + machineName + " /f /pid " + pid;
        }
    }
}
