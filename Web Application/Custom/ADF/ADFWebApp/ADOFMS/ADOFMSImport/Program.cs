/**
 * Jack foresees this application to be extended later on to import all transaction types, so he made
 * it easily extendable.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADOFMSImport
{
   class Program
   {
      static void Main(string[] args)
      {
         Defaults defaults = new Defaults();

         ArgParser argParser = new ArgParser();
         bool ok = argParser.ParseInputArguments(args, ref defaults);

         if (!ok && defaults.InputFile != null)
         {
            // argument parse failure
            Console.WriteLine(ArgParser.Usage(Environment.GetCommandLineArgs()[0]));
         }
         else
         {
            // argument parse success

            // set up logger
            using (Logger logger = LoggerManager.CreateLogger(defaults.LoggerKey))
            {
               logger.ErrorFile = defaults.ErrorFile;
               logger.ProgressFile = defaults.ProgressFile;
               logger.Flush();

               Runner runner = new Runner();
               bool runOk = runner.Run(defaults);

               if (runOk)
               {
                  LoggerManager.LogProgress(defaults.LoggerKey, Runner.MESSAGE_SUCCESSFUL);
               }
               else
               {
                  LoggerManager.LogProgress(defaults.LoggerKey, Runner.MESSAGE_WITHERRORS);
               }
            }
         }

#if DEBUG
         Console.ReadKey();
#endif // DEBUG
      }
   }
}
