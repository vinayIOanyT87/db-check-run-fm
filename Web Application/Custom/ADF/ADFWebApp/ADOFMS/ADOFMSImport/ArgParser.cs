using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gnu.Getopt;

namespace ADOFMSImport
{
   public class ArgParser
   {
      public bool ParseInputArguments(string[] a_args, ref Defaults a_defaults)
      {
         bool result = true;
         int c;

         Getopt parser = new Getopt(Environment.GetCommandLineArgs()[0], a_args, "si:o:");
         while ((c = parser.getopt()) != -1)
         {
            switch (c)
            {
               case 'i':      // input csv file, must be entered by the caller
                  a_defaults.InputFile = parser.Optarg;
                  break;
               case 'o':      // output progress log file, defaults to Defaults.FILE_PROGRESS
                  a_defaults.ProgressFile = parser.Optarg;
                  break;
               case 's':      // stop on error, defaults to Defaults.STOP_ON_ERROR
                  a_defaults.StopOnError = true;
                  break;
               case '?':   // unrecognised
                  result = false;
                  break;
            }

            // ensures nothing else will be parsed if parsing encounters error with the last arg
            if (!result)
            {
               break;
            }
         }

         return result;
      }

      public static string Usage(string a_exeName)
      {
         string result = "";

         result += "Application should be called like so: \n";
         result += a_exeName + " <-i [input file]|-o [output file]|-s>\tdefinitions:\n";
         result += "-i\tName of the input file, this must be specified by the user.\n";
         result += "-o\tName of the output log where progress will be written, this defaults to " + Defaults.FILE_PROGRESS + ".\n";
         result += "-s\tWhen used, will stop execution on first processing error.\n";

         return result;
      }
   }
}
