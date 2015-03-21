using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace InternetKillSwitchService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if(!DEBUG)
           ServiceBase[] ServicesToRun;
           ServicesToRun = new ServiceBase[] 
	   { 
	        new MyService() 
	   };
           ServiceBase.Run(ServicesToRun);
#else
            Service1 myServ = new Service1();
            myServ.OnDebug();
            // here Process is my Service function
            // that will run when my service onstart is call
            // you need to call your own method or function name here instead of Process();
#endif
        }
    }
}
