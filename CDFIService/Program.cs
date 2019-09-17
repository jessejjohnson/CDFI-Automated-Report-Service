using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CDFIService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

//#if DEBUG

            CDFIService myService = new CDFIService();
            myService.OnDebug();
//#else

//            ServiceBase[] ServicesToRun;
//            ServicesToRun = new ServiceBase[]
//            {
//                new CDFIService()
//            };
//            ServiceBase.Run(ServicesToRun);
//#endif
        }
    }
}
