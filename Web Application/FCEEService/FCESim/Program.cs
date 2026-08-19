using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.ComponentModel.Design;
using System.Reflection;
using System.Collections.Concurrent;

public class Program
{
    public static readonly HashSet<string> ValidPatterns = new HashSet<string>
    {
         "step",
         "sinusoid",
         "static"
    };
    private static bool runStatus = true;
    private static List<long> ApiResponseTimes = new List<long>();
    private static Stopwatch ProgramTimer;
    private static Logger Logger;
    private static HttpClient httpClient;
    private static SemaphoreSlim semaphore;

    private static JsonConfig fceSimConfig; 
    private static string threadingStrategy = "parallel";
    public static Queue<Decimal> responseTimes = new Queue<Decimal>();
    private static int requestSent = 0;

   private static JsonConfig ReadConfig(string path)
   {
      try
      {
         string jsonFile = File.ReadAllText(path);
         return JsonSerializer.Deserialize<JsonConfig>(jsonFile);
      }
      catch (Exception e)
      {
         Console.WriteLine("Error found. Details: " + e.Message);
         throw;
      }
   }

   private static List<CsvConfig> ReadCsvFile(string path)
   {
      List<CsvConfig> output = new List<CsvConfig>();

      using (var read = new StreamReader(path))
      {
         string headerLine = read.ReadLine();
         int LineCounter = 1;
         while (!read.EndOfStream)
         {
            LineCounter++;
            var line = read.ReadLine();
            var values = line.Split(',');

            if (checkInput(values[0]))
            {
               Console.WriteLine("IMEI is invalid (it is too short, must be 15 digits). \n " +
                  "This is your current IMEI: " + values[0] + " on line: " + LineCounter + " in the CSV file.\n");
               Environment.Exit(0);
            }
            if (checkInput(values[1], 1, Int32.MaxValue))
            {
               Console.WriteLine("numPoints must be an integer of 1 or greater. \n " +
                  "This is your current numPoints: " + values[1] + " on line: " + LineCounter + " in the CSV. \n" );
               Environment.Exit(0);
            }
            if (checkInput(values[2], 1, 20))
            {
               Console.WriteLine("msgType must be in the range 1 - 20. \n This is your current msgType: " + values[2] + " on line: " + LineCounter
                  + " in the CSV. \n");
               Environment.Exit(0);
            }
            if (checkInput(values[3]) || !ValidPatterns.Contains(values[3]))
            {
               Console.WriteLine("ValuePattern is invalid, must be either step, static, or sinusoid. \n This is your current ValuePattern: "
                  + values[3] + " on line: " + LineCounter + " in the CSV. \n");
               Environment.Exit(0);
            }
            if (checkInput(values[4], 0, Int32.MaxValue))
            {
               Console.WriteLine("IntervalSeconds is invalid, must be an integer greater than 0. \n This is your current IntervalSeconds: "
                  + values[4] + " on line " + LineCounter + " in the CSV. \n");
               Environment.Exit(0);
            }
            if (checkInput(values[5], 0, Int32.MaxValue))
            {
               Console.WriteLine("MinValue is invalid, must be an integer greater than 0. \n This is your current MinValue: "
                  + values[5] + " on line: " + LineCounter + " in the CSV. \n");
               Environment.Exit(0);
            }
            if (checkInput(values[6], Int32.Parse(values[5]) + 1, Int32.MaxValue))
            {
               Console.WriteLine("MaxValue is invalid, must be an integer greater than MinValue. \n This is your current MaxValue: "
                  + values[6] + " on line: " + LineCounter + " in the CSV. \n");
               Environment.Exit(0);
            }
            if (values[3].ToLower().Equals("step"))
            {
               if (checkInput(values[7], 0, Int32.MaxValue))
               {
                  Console.WriteLine("ValuePattern is configured as step. StepOffset  must be an integer greater than 0. \n This is " +
                     "your current StepOffset: " + values[7] + " on line: " + LineCounter + " in the CSV. \n");
                  Environment.Exit(0);
               }
               
            }
            if (values[3].ToLower().Equals("sinusoid"))
            {
               if (checkInput(values[8], Int32.Parse(values[4]), Int32.MaxValue))
               {
                  Console.WriteLine("ValuePattern is conifgured as sinusoid. WavelengthSeconds must be an integer greater than your IntervalSeconds. \n" +
                     "This is your current WavelengthSeconds: " + values[8] + " and IntervalSeconds: " + values[4] + " on line: " + LineCounter +
                     " in the CSV. \n");
                  Environment.Exit(0);
               }
               
            }
            CsvConfig processedLine = new CsvConfig(values[0], Int32.Parse(values[1]), ushort.Parse(values[2]),
               (values[3]), Int32.Parse(values[4]), Int32.Parse(values[5]), Int32.Parse(values[6]), Int32.Parse(values[7]),
               Int32.Parse(values[8]));

            
            output.Add(processedLine);
         }

      }

      return output;
   }


   private static bool checkInput(string valueToCheck)
   {
      bool output = false;
      if (valueToCheck == null || valueToCheck.Length < 0 || valueToCheck.Any(x => char.IsSymbol(x)))
      {
         output = true;
      }
      return output;
   }

   private static bool checkInput(string valueToCheck, int lowerBound, int upperBound)
   {
      bool output = false;
      if (valueToCheck == null || valueToCheck.Equals("") || valueToCheck.Any(x => char.IsLetter(x) || char.IsSymbol(x)) || Int32.Parse(valueToCheck) < lowerBound
         || Int32.Parse(valueToCheck) > upperBound)
      {
         output = true;
      }
      return output;
   }

   public static async Task Main(string[] args)
    {
        /* Read JSON config and get config */
        string jsonFilePath = @"data.json";
        JsonConfig inputJsonConfig = ReadConfig(jsonFilePath);
        fceSimConfig = inputJsonConfig;
        // optimal inputJsonConfig.concurrency seems to be around .75-1.5x the logical processors on the host
        threadingStrategy = inputJsonConfig.threadingStrategy;
        semaphore = new SemaphoreSlim(inputJsonConfig.concurrency);
        httpClient = new HttpClient(new HttpClientHandler()
        {
            MaxConnectionsPerServer = inputJsonConfig.concurrency,
            UseProxy = false,
        });

        Console.Title = ($"FCEE API response time: request pending ms      Avg Last 10: request pending ms");
        Logger = new Logger(@"log.txt", @"log.csv", inputJsonConfig.logTxt, inputJsonConfig.logCsv);

    /* List with each parsed CSV row */
      string csvFilePath = @"fcedata.csv";
      List<CsvConfig> messageConfigs = ReadCsvFile(csvFilePath);

      /* runs the WaitForStop on another thread */
      ProgramTimer = System.Diagnostics.Stopwatch.StartNew();
      Thread inputThread = new Thread(new ThreadStart(WaitForStop));
      Thread loggerThread = new Thread(new ThreadStart(Logger.writeLog));
      Thread csvLoggerThread = new Thread(new ThreadStart(Logger.writeCSVLog));
      inputThread.Start();
      loggerThread.Start();
      csvLoggerThread.Start();

        /* List that holds all the threads */
        List<Thread> threads = new List<Thread>();

        switch (threadingStrategy)
        {
            case "parallel":
                Parallel.ForEach(messageConfigs, new ParallelOptions { MaxDegreeOfParallelism = inputJsonConfig.concurrency }, config =>
                {
                    SendMessageAtInterval(config, inputJsonConfig);
                });
                break;
            case "semaphore":
                {
                    /* Start a new thread for each message, then wait for all the threads to complete */
                    foreach (var config in messageConfigs)
                    {
                        {
                            SendMessageAtInterval(config, inputJsonConfig);
                        }
                    }
                }
                break;
            case "threads":
                {
                    foreach (var config in messageConfigs)
                    {

                        {
                            var thread = new Thread(() => SendMessageAtInterval(config, inputJsonConfig));
                            threads.Add(thread);
                            thread.Start();
                            Thread.Sleep(inputJsonConfig.staggerIntervalMs);

                        }
                    }
                }
                break;
            default:
                {
                    Task.Run(() => Console.Out.WriteLineAsync("Specify parallel, semaphore, or threads for threadingStrategy in data.json"));
                    runStatus = false;

                }
                break;
        }

        foreach (var thread in threads)
      {
         thread.Join();
      }
      inputThread.Join();

      loggerThread.Join();
      csvLoggerThread.Join();


        Console.WriteLine("Automation has Completed");
    }
 
    /* Generates base payload format as well as call to create the data for the custom type */
    private static byte[] GenerateByteArray(CsvConfig c, long simulatedValue, int currIndex)
    {
        byte[] byteArray = new byte[22];
 
        /* IMEI */
        string temp = c.IMEI;
        byte[] imeiVal = Encoding.UTF8.GetBytes(temp);
        Buffer.BlockCopy(imeiVal, 0, byteArray, 0, imeiVal.Length);
 

        /* Time Stamp */

        uint timeStamp = (uint) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string timeStampHex = timeStamp.ToString("X");

        // makes sure timeStampHex is 8 characters long
        timeStampHex = timeStampHex.PadLeft(8, '0');
        for (int i = 0; i < 4; i++)
        {
            byteArray[15 + i] = Convert.ToByte(timeStampHex.Substring(i * 2, 2), 16);
        }
 

        /* type */

        // ushort type = (ushort) rand.Next(0, 21);
        ushort type = c.msgType;
        byteArray[19] = (byte) (type >> 8);
        byteArray[20] = (byte) type;


      /* index */
        uint index = (uint) currIndex;
        byteArray[21] = (byte) index;
 

        /* data */
        byte[] data = GenerateData(type, simulatedValue);
 

        /* final result */
        byte[] solution = new byte[byteArray.Length + data.Length];
        Buffer.BlockCopy(byteArray, 0, solution, 0, byteArray.Length);
        Buffer.BlockCopy(data, 0, solution, byteArray.Length, data.Length);
 
        return solution;
    }

   private static byte[] GenerateByteArray(CsvConfig c, double simulatedValue, int currIndex)
   {
      byte[] byteArray = new byte[22];

      /* IMEI */
      string temp = c.IMEI;
      byte[] imeiVal = Encoding.UTF8.GetBytes(temp);
      Buffer.BlockCopy(imeiVal, 0, byteArray, 0, imeiVal.Length);


      /* Time Stamp */

      uint timeStamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
      string timeStampHex = timeStamp.ToString("X");

      // makes sure timeStampHex is 8 characters long
      timeStampHex = timeStampHex.PadLeft(8, '0');
      for (int i = 0; i < 4; i++)
      {
         byteArray[15 + i] = Convert.ToByte(timeStampHex.Substring(i * 2, 2), 16);
      }


      /* type */

      // ushort type = (ushort) rand.Next(0, 21);
      ushort type = c.msgType;
      byteArray[19] = (byte)(type >> 8);
      byteArray[20] = (byte)type;


      /* index */
      uint index = (uint)currIndex;
      byteArray[21] = (byte)index;


      /* data */
      byte[] data = GenerateData(type, simulatedValue);


      /* final result */
      byte[] solution = new byte[byteArray.Length + data.Length];
      Buffer.BlockCopy(byteArray, 0, solution, 0, byteArray.Length);
      Buffer.BlockCopy(data, 0, solution, byteArray.Length, data.Length);

      return solution;
   }


   /* Generates the Data portion custom to the necessary type */
   private static byte[] GenerateData(ushort type, long simulatedValue)
    {
        switch (type)
        {
            // heartbeat. counter is the value to change
            case 1:
                byte[] data = longToByteArray(4, simulatedValue);
                return data;
 
            case 2:
                byte[] swVer = Encoding.UTF8.GetBytes("sw ver");
                byte[] min = longToByteArray(32, 0);
                byte[] max = longToByteArray(4, 0);
                byte[] lvlDead = longToByteArray(4, 0);
                byte[] tempDead = longToByteArray(4, 0);
                byte[] hb = longToByteArray(4, 0);
                byte[] tl = longToByteArray(1, 0);
                byte[] mbMap = longToByteArray(1, 0);
                byte[] midOff = longToByteArray(2, 0);
                byte[] shortDead = longToByteArray(4, 0);
                byte[] shortTime = longToByteArray(2, 0);
                byte[] longDead = longToByteArray(4, 0);
                byte[] longTime = longToByteArray(2, 0);
                List<byte[]> bl =
                    new List<byte[]> {swVer, min, max, lvlDead, hb, tl, mbMap, midOff, shortDead,
                        shortTime, longDead, longTime};
                return mergeByteArrays(bl);
               
            case 3:
                byte[] deviceType = longToByteArray(1, 0);
                byte[] deviceStatus = longToByteArray(2, 0);
                List<byte[]> byteList = new List<byte[]> {deviceType, deviceStatus};
                byte[] typeStatus = mergeByteArrays(byteList);
                return typeStatus;
 
            case 4: //enraf. level product is the value to change. Everything else static.
                byte[] lvl = longToByteArray(4, simulatedValue);
                 byte[] temp = longToByteArray(4, 60);
                 byte[] waterLvl = longToByteArray(4, 1);
                 byte[] pos = longToByteArray(4, 0);
                 byte[] gStat = longToByteArray(2, 0);
                 byte[] pntStat = longToByteArray(2, 0);
                // something about this it not supported until a later .NET version. We should be good with static 0 for pnt and gauge status for now
                // longToByteArray(gStat, enumChoose([258, 203, 260, 204, 206, 201, 218, 66, 155, 165, 39, 172], rand));

                return mergeByteArrays(new List<byte[]> {lvl, temp, waterLvl, pos, gStat, pntStat});
 
            case 5: //enraf density. density is the value to change. 
                byte[] d = longToByteArray(4, simulatedValue);
                byte[] dTemp = longToByteArray(4, 60);
                byte[] dTime = longToByteArray(4, 1);
                return mergeByteArrays(new List<byte[]> {d, dTemp, dTime});
           
            case 6:
                pntStat = longToByteArray(2, 0);
                return pntStat;
 
            case 7:
                byte[] val1 = longToByteArray(4, 0);
                return val1;
 
            case 8:
                lvl = longToByteArray(4, 0);
                temp = longToByteArray(4, 0);
                waterLvl = longToByteArray(4, 0);
                d = longToByteArray(4, 0);
                byte[] v1 = longToByteArray(4, 0);
                byte[] aFlag = longToByteArray(2, 0);
                pntStat = longToByteArray(2, 0);
                return mergeByteArrays(new List<byte[]> {lvl, temp, waterLvl, d, v1, aFlag, pntStat});
 
            case 9:
                lvl = longToByteArray(4, 0);
                temp = longToByteArray(4, 0);
                waterLvl = longToByteArray(4, 0);
                d = longToByteArray(4, 0); //gross volume in this instance
                v1 = longToByteArray(4, 0); //net vol in this instance
                aFlag = longToByteArray(4, 0); //water vol in this instance
                pntStat = longToByteArray(4, 0); //ullage in this instance
                return mergeByteArrays(new List<byte[]> {lvl, temp, waterLvl, d, v1, aFlag, pntStat});
 
            case 10:
                pntStat = longToByteArray(2, 0);
                return pntStat;
 
            case 11:
                pntStat = longToByteArray(2, 0);
                return pntStat;
 
            case 12:
                pntStat = longToByteArray(2, 0);
                return pntStat;
 
            case 13:
                lvl = longToByteArray(4, 0); //height in this instance
                temp = longToByteArray(4, 0);
                waterLvl = longToByteArray(4, 0); //vol in this isntance
                d = longToByteArray(4, 0); //tc volume in this instance
                v1 = longToByteArray(4, 0); //water in this instance
                aFlag = longToByteArray(4, 0); //ullage in this instance
                pntStat = longToByteArray(4, 0); //water vol in this instance
                return mergeByteArrays(new List<byte[]> {lvl, temp, waterLvl, d, v1, aFlag, pntStat});
 
            case 14:
                byte[] tankStat = longToByteArray(4, 0);
                return tankStat;
 
            case 15:
                pntStat = longToByteArray(2, 0); //sensor stat in this instance
                return pntStat;
 
            case 16:
                lvl = longToByteArray(1, 0);//modbus Map in this instance
                temp = longToByteArray(1, 0); //device
                waterLvl = longToByteArray(2, 0); //level in this isntance
                d = longToByteArray(2, 0);//temp in this instance
                v1 = longToByteArray(2, 0); //waterlevel in this instance
                aFlag = longToByteArray(2, 0); //position in this instance
                pntStat = longToByteArray(2, 0); //guagestatus in this instance
                byte[] d2 = longToByteArray(2, 0); //watersump in this instance
                byte[] v2 = longToByteArray(2, 0); //fuel volume in this instance
                byte[] aFlag2 = longToByteArray(2, 0);//water volume in this instance
                return mergeByteArrays(new List<byte[]> {lvl, temp, waterLvl, d, v1, aFlag, pntStat, d2, v2, aFlag2});

            case 17:
                lvl = longToByteArray(1, 0); //modbus Map in this instance
                temp = longToByteArray(1, 0); //device
                waterLvl = longToByteArray(2, 0); //density in this isntance
                d = longToByteArray(2, 0);  //densityTemp in this instance
                v1 = longToByteArray(4, 0);  //densityTime in this instance
                aFlag = longToByteArray(2, 0);  //trouble info in this instance
                pntStat = longToByteArray(2, 0);  //level alarm in this instance
                return mergeByteArrays(new List<byte[]> {lvl, temp, waterLvl, d, v1, aFlag, pntStat});
 
            case 18:
                mbMap = longToByteArray(1, 0);
                byte[] facStat = longToByteArray(4, 0);
                return mergeByteArrays(new List<byte[]> {mbMap, facStat});
 
            case 19:
                lvl = longToByteArray(1, 0);  //modbus Map in this instance
                temp = longToByteArray(1, 0);  //device
                waterLvl = longToByteArray(4, 0); //level in this isntance
                d = longToByteArray(4, 0);  //temp volume in this instance
                v1 = longToByteArray(4, 0);  //water level in this instance
                aFlag = longToByteArray(4, 0); //position in this instance
                pntStat = longToByteArray(4, 0);  //density in this instanc
                return mergeByteArrays(new List<byte[]> {lvl, temp, waterLvl, d, v1, aFlag, pntStat});
 
            case 20:
                byte[] cmdStat = longToByteArray(1, 0);
                byte[] cmdSched = longToByteArray(4, 0);
                return mergeByteArrays(new List<byte[]> {cmdStat, cmdSched});
 
            default:
                return new byte[0];
        }
    }


   private static byte[] GenerateData(ushort type, double simulatedValue)
   {
      switch (type)
      {
         // heartbeat. counter is the value to change
         case 1:
            byte[] data = doubleToByteArray(4, simulatedValue);
            return data;

         case 2:
            byte[] swVer = Encoding.UTF8.GetBytes("sw ver");
            byte[] min = doubleToByteArray(32, 0);
            byte[] max = doubleToByteArray(4, 0);
            byte[] lvlDead = doubleToByteArray(4, 0);
            byte[] tempDead = doubleToByteArray(4, 0);
            byte[] hb = doubleToByteArray(4, 0);
            byte[] tl = doubleToByteArray(1, 0);
            byte[] mbMap = doubleToByteArray(1, 0);
            byte[] midOff = doubleToByteArray(2, 0);
            byte[] shortDead = doubleToByteArray(4, 0);
            byte[] shortTime = doubleToByteArray(2, 0);
            byte[] longDead = doubleToByteArray(4, 0);
            byte[] longTime = doubleToByteArray(2, 0);
            List<byte[]> bl =
                new List<byte[]> {swVer, min, max, lvlDead, hb, tl, mbMap, midOff, shortDead,
                        shortTime, longDead, longTime};
            return mergeByteArrays(bl);

         case 3:
            byte[] deviceType = doubleToByteArray(1, 0);
            byte[] deviceStatus = doubleToByteArray(2, 0);
            List<byte[]> byteList = new List<byte[]> { deviceType, deviceStatus };
            byte[] typeStatus = mergeByteArrays(byteList);
            return typeStatus;

         case 4: //enraf. level product is the value to change. Everything else static.
            byte[] lvl = doubleToByteArray(4, simulatedValue);
            byte[] temp = doubleToByteArray(4, 60);
            byte[] waterLvl = doubleToByteArray(4, 1);
            byte[] pos = doubleToByteArray(4, 0);
            byte[] gStat = doubleToByteArray(2, 0);
            byte[] pntStat = doubleToByteArray(2, 0);
            // something about this it not supported until a later .NET version. We should be good with static 0 for pnt and gauge status for now
            // doubleToByteArray(gStat, enumChoose([258, 203, 260, 204, 206, 201, 218, 66, 155, 165, 39, 172], rand));

            return mergeByteArrays(new List<byte[]> { lvl, temp, waterLvl, pos, gStat, pntStat });

         case 5: //enraf density. density is the value to change. 
            byte[] d = doubleToByteArray(4, simulatedValue);
            byte[] dTemp = doubleToByteArray(4, 60);
            byte[] dTime = doubleToByteArray(4, 1);
            return mergeByteArrays(new List<byte[]> { d, dTemp, dTime });

         case 6:
            pntStat = doubleToByteArray(2, 0);
            return pntStat;

         case 7:
            byte[] val1 = doubleToByteArray(4, 0);
            return val1;

         case 8:
            lvl = doubleToByteArray(4, 0);
            temp = doubleToByteArray(4, 0);
            waterLvl = doubleToByteArray(4, 0);
            d = doubleToByteArray(4, 0);
            byte[] v1 = doubleToByteArray(4, 0);
            byte[] aFlag = doubleToByteArray(2, 0);
            pntStat = doubleToByteArray(2, 0);
            return mergeByteArrays(new List<byte[]> { lvl, temp, waterLvl, d, v1, aFlag, pntStat });

         case 9:
            lvl = doubleToByteArray(4, 0);
            temp = doubleToByteArray(4, 0);
            waterLvl = doubleToByteArray(4, 0);
            d = doubleToByteArray(4, 0); //gross volume in this instance
            v1 = doubleToByteArray(4, 0); //net vol in this instance
            aFlag = doubleToByteArray(4, 0); //water vol in this instance
            pntStat = doubleToByteArray(4, 0); //ullage in this instance
            return mergeByteArrays(new List<byte[]> { lvl, temp, waterLvl, d, v1, aFlag, pntStat });

         case 10:
            pntStat = doubleToByteArray(2, 0);
            return pntStat;

         case 11:
            pntStat = doubleToByteArray(2, 0);
            return pntStat;

         case 12:
            pntStat = doubleToByteArray(2, 0);
            return pntStat;

         case 13:
            lvl = doubleToByteArray(4, 0); //height in this instance
            temp = doubleToByteArray(4, 0);
            waterLvl = doubleToByteArray(4, 0); //vol in this isntance
            d = doubleToByteArray(4, 0); //tc volume in this instance
            v1 = doubleToByteArray(4, 0); //water in this instance
            aFlag = doubleToByteArray(4, 0); //ullage in this instance
            pntStat = doubleToByteArray(4, 0); //water vol in this instance
            return mergeByteArrays(new List<byte[]> { lvl, temp, waterLvl, d, v1, aFlag, pntStat });

         case 14:
            byte[] tankStat = doubleToByteArray(4, 0);
            return tankStat;

         case 15:
            pntStat = doubleToByteArray(2, 0); //sensor stat in this instance
            return pntStat;

         case 16:
            lvl = doubleToByteArray(1, 0);//modbus Map in this instance
            temp = doubleToByteArray(1, 0); //device
            waterLvl = doubleToByteArray(2, 0); //level in this isntance
            d = doubleToByteArray(2, 0);//temp in this instance
            v1 = doubleToByteArray(2, 0); //waterlevel in this instance
            aFlag = doubleToByteArray(2, 0); //position in this instance
            pntStat = doubleToByteArray(2, 0); //guagestatus in this instance
            byte[] d2 = doubleToByteArray(2, 0); //watersump in this instance
            byte[] v2 = doubleToByteArray(2, 0); //fuel volume in this instance
            byte[] aFlag2 = doubleToByteArray(2, 0);//water volume in this instance
            return mergeByteArrays(new List<byte[]> { lvl, temp, waterLvl, d, v1, aFlag, pntStat, d2, v2, aFlag2 });

         case 17:
            lvl = doubleToByteArray(1, 0); //modbus Map in this instance
            temp = doubleToByteArray(1, 0); //device
            waterLvl = doubleToByteArray(2, 0); //density in this isntance
            d = doubleToByteArray(2, 0);  //densityTemp in this instance
            v1 = doubleToByteArray(4, 0);  //densityTime in this instance
            aFlag = doubleToByteArray(2, 0);  //trouble info in this instance
            pntStat = doubleToByteArray(2, 0);  //level alarm in this instance
            return mergeByteArrays(new List<byte[]> { lvl, temp, waterLvl, d, v1, aFlag, pntStat });

         case 18:
            mbMap = doubleToByteArray(1, 0);
            byte[] facStat = doubleToByteArray(4, 0);
            return mergeByteArrays(new List<byte[]> { mbMap, facStat });

         case 19:
            lvl = doubleToByteArray(1, 0);  //modbus Map in this instance
            temp = doubleToByteArray(1, 0);  //device
            waterLvl = doubleToByteArray(4, 0); //level in this isntance
            d = doubleToByteArray(4, 0);  //temp volume in this instance
            v1 = doubleToByteArray(4, 0);  //water level in this instance
            aFlag = doubleToByteArray(4, 0); //position in this instance
            pntStat = doubleToByteArray(4, 0);  //density in this instanc
            return mergeByteArrays(new List<byte[]> { lvl, temp, waterLvl, d, v1, aFlag, pntStat });

         case 20:
            byte[] cmdStat = doubleToByteArray(1, 0);
            byte[] cmdSched = doubleToByteArray(4, 0);
            return mergeByteArrays(new List<byte[]> { cmdStat, cmdSched });

         default:
            return new byte[0];
      }
   }

   /* sends the result to the API endpoint using MemoryStream */
   private static async Task SendByteArrayToApi(byte[] arr, JsonConfig c)
    {
        using (MemoryStream str = new MemoryStream(arr))
        {
            HttpContent content = new StreamContent(str);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(c.APIURL, content);
                requestSent++;
                watch.Stop();
                ApiResponseTimes.Add(watch.ElapsedMilliseconds);

                //string responseMsg = await response.Content.ReadAsStringAsync();

                var requestsPerSecond = requestSent / (ProgramTimer.ElapsedMilliseconds / 1000);
                
                //Title bar does not need to be updated constantly.
                if (requestSent % 50 == 0)
                    Console.Title = ($"FCEE API response time: {watch.ElapsedMilliseconds} ms     Avg Last 10: {UpdateAverage(watch.ElapsedMilliseconds)} ms    Requests/sec: {requestsPerSecond}");

                if (c.logTxt == "true")
                Logger.Log($"API response time: {watch.ElapsedMilliseconds} ms");
                if (c.logCsv == "true")
                Logger.LogToCSV($"{watch.ElapsedMilliseconds}");

                //Task.Run(() => Console.Out.WriteLineAsync(watch.ElapsedMilliseconds.ToString()));

                if (!response.IsSuccessStatusCode)
                {
                    Task.Run(() => Console.Out.WriteLineAsync(response.StatusCode.ToString()));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine is blocking and should not be used in CPU or IO bound scenarios
                //Console.WriteLine(ex.Message);
                Task.Run(() => Console.Out.WriteLineAsync(ex.Message));
                watch.Stop();
            }

        }
    }

    public static string UpdateAverage(Decimal lastMs)
    {
        decimal avg;
        lock (responseTimes)
        {
            responseTimes.Enqueue(lastMs);
            if (responseTimes.Count > 10)
            {
                responseTimes.Dequeue();
            }


            avg = responseTimes.Sum() / responseTimes.Count;
        }
        
        return avg.ToString("F");
    }

    private static byte[] longToByteArray(int len, long value)
    {
        byte[] data = new byte[len];
        data = BitConverter.GetBytes(Convert.ToSingle(value));
        Array.Reverse(data); //endian mismatch
        return data;
    }

   private static byte[] doubleToByteArray(int len, double value)
   {
      byte[] data = new byte[len];
      data = BitConverter.GetBytes(Convert.ToSingle(value));
      Array.Reverse(data);
      return data;
   }

   /* helper method to properlt combine byte arrays after doing calculations for each portion */
   private static byte[] mergeByteArrays(List<byte[]> list)
    {
        int totalLength = 0;
        foreach (byte[] arr in list)
        {
            totalLength += arr.Length;
        }
 
        byte[] ans = new byte[totalLength];
        int offset = 0;
 
        foreach (byte[] arr in list)
        {
            Buffer.BlockCopy(arr, 0, ans, offset, arr.Length);
            offset += arr.Length;
        }
 
        return ans;
    }
 
    /* Method for proper multithreading */
    private static async void WaitForStop() {
        Console.WriteLine("Press 'q' to stop the script");
        while(true){
            //when the Q keypress is read
            if (Console.ReadKey(true).Key == ConsoleKey.Q) {
                Task.Run(() => Console.Out.WriteLineAsync("------------- STOPPING -------------"));
                runStatus = false;
                Thread.Sleep(1000);
                printApiMetrics();
                
                ProgramTimer.Stop();
                Console.ReadLine();
                Logger.setRunningStatus(false);
                break;

            }
        }
    }

    /* for later implementation */
    //private static int enumChoose(int[] arr, Random rand) {
    //    return arr[(int)rand.NextInt64(0, arr.Length)];
    //}
    public static async void SendMessageAtInterval(CsvConfig config, JsonConfig j)
   {
      if (threadingStrategy == "semaphore")
        semaphore.Wait();
      long simulatedValue = config.minValue - 1 + config.stepOffset;
      double amplitude = (config.maxValue - config.minValue) / 2.0;
      double simulatedValueDouble = 0.00;
      double midVal = config.minValue + amplitude;
      double wavelength = config.wavelengthSeconds / 1.0;
      double elapsedTime = 0.0;
      while (runStatus)
      {
         // update simulation here and call GenerateByteArray(config, value);
         switch (config.valuePattern)
         {
            case "static":
               break;
            case "step":
               simulatedValue = config.minValue + ((simulatedValue - config.minValue + 1) % (config.maxValue - config.minValue + 1));
               //Console.WriteLine($"Simulated Value: {simulatedValue}");
               break;
            case "sinusoid":
               double radian = (2 * Math.PI * elapsedTime) / wavelength;
               simulatedValueDouble = amplitude * Math.Sin(radian) + midVal;
               //Console.WriteLine("Simulated Value: " + simulatedValueDouble);
               break;
         }
         elapsedTime += config.intervalSeconds;
         if (elapsedTime >= wavelength)
         {
            elapsedTime = 0.0;
         }

         //loops and iterates the proper indices based on numPoints
         int startingIndex = (config.msgType <= 19 && config.msgType >= 13) ? 1 : 0;

         for (int index = 0; index < config.numPoints; index++)
         {
            byte[] byteArray;
            if (config.valuePattern.Equals("sinusoid"))
            {
               byteArray = GenerateByteArray(config, simulatedValueDouble, startingIndex + index);
            } 
            else
            {
               byteArray = GenerateByteArray(config, simulatedValue, startingIndex + index);
            }
            string hex = BitConverter.ToString(byteArray);
            //Console.WriteLine(hex);
            Task.Run(() => Console.Out.WriteLineAsync(hex));

                await SendByteArrayToApi(byteArray, j);
         }
        //Thread.Sleep(1000 * config.intervalSeconds);
        if (threadingStrategy == "semaphore")
            semaphore.Release();
         await Task.Delay(1000 * config.intervalSeconds);

      }
   }

    public static async void SendMessageAtIntervalTimer(object state, ElapsedEventArgs e, CsvConfig c)
    {
        CsvConfig config = c as CsvConfig;
        long simulatedValue = config.minValue - 1 + config.stepOffset;
        double amplitude = (config.maxValue - config.minValue) / 2.0;
        double simulatedValueDouble = 0.00;
        double midVal = config.minValue + amplitude;
        double wavelength = config.wavelengthSeconds / 1.0;
        double elapsedTime = 0.0;
        while (runStatus)
        {
            // update simulation here and call GenerateByteArray(config, value);
            switch (config.valuePattern)
            {
                case "static":
                    break;
                case "step":
                    simulatedValue = config.minValue + ((simulatedValue - config.minValue + 1) % (config.maxValue - config.minValue + 1));
                    //Console.WriteLine($"Simulated Value: {simulatedValue}");
                    break;
                case "sinusoid":
                    double radian = (2 * Math.PI * elapsedTime) / wavelength;
                    simulatedValueDouble = amplitude * Math.Sin(radian) + midVal;
                    //Console.WriteLine("Simulated Value: " + simulatedValueDouble);
                    break;
            }
            elapsedTime += config.intervalSeconds;
            if (elapsedTime >= wavelength)
            {
                elapsedTime = 0.0;
            }

            //loops and iterates the proper indices based on numPoints
            int startingIndex = (config.msgType <= 19 && config.msgType >= 13) ? 1 : 0;

            for (int index = 0; index < config.numPoints; index++)
            {
                byte[] byteArray;
                if (config.valuePattern.Equals("sinusoid"))
                {
                    byteArray = GenerateByteArray(config, simulatedValueDouble, startingIndex + index);
                }
                else
                {
                    byteArray = GenerateByteArray(config, simulatedValue, startingIndex + index);
                }
                string hex = BitConverter.ToString(byteArray);
                //Console.WriteLine(hex);
                Task.Run(() => Console.Out.WriteLineAsync(hex));
                await SendByteArrayToApi(byteArray, fceSimConfig);
            }

            Thread.Sleep(1000 * config.intervalSeconds);

        }
    }

    private static void printApiMetrics()
   {
      double ResponseTimeAverage = 0.0;
      long low = long.MaxValue;
      long high = 0;
      foreach (long t in ApiResponseTimes)
      {
         ResponseTimeAverage += t;
         if (t < low)
         {
            low = t;
         }
         if (t > high)
         {
            high = t;
         }
      }
      ResponseTimeAverage /= ApiResponseTimes.Count;
      var requestsPerSecond = requestSent / (ProgramTimer.ElapsedMilliseconds / 1000);

      Console.WriteLine($"Total run time: {ProgramTimer.Elapsed} (hr:min:s:ms).");
      Console.WriteLine($"Total messages sent to endpoint: {ApiResponseTimes.Count}.");
      Console.WriteLine($"Average Requests Per Second: {requestsPerSecond}.");
      Console.WriteLine($"Average response time from endpoint: {ResponseTimeAverage} ms.");
      Console.WriteLine($"Fastest response time {low} ms.");
      Console.WriteLine($"Slowest response time: {high} ms.");
      Logger.Log($"Total run time: {ProgramTimer.Elapsed} (hr:min:s:ms).");
      Logger.Log($"Total messages sent to endpoint: {ApiResponseTimes.Count}.");
      Logger.Log($"Average response time from endpoint: {ResponseTimeAverage} ms.");
      Logger.Log($"Fastest response time {low} ms.");
      Logger.Log($"Slowest response time: {high} ms. \n");

   }

}

public class JsonConfig
{
    // short hand syntax
    public string APIURL { get; set; }
    public string threadingStrategy { get; set; }
    public int staggerIntervalMs { get; set; }
    public string logCsv { get; set; }
    public string logTxt { get; set; }

    public int concurrency {  get; set; }

}

// Add patterns here as they are created
public class CsvConfig
{
   public string IMEI { get; set; }
   public int numPoints { get; set; }
   public ushort msgType { get; set; }
   public string valuePattern { get; set; }
   public int intervalSeconds { get; set; }
   public int minValue { get; set; }
   public int maxValue { get; set; }
   public int stepOffset { get; set; }
   public int wavelengthSeconds { get; set; }

   public CsvConfig(string imei, int NumPoints, ushort MsgType, string ValuePattern, int IntervalSeconds, int MinValue, int MaxValue, int StepOffset, int WavelengthSeconds)
   {
      
      IMEI = imei;
      
      numPoints = NumPoints;
      
      msgType = MsgType;
      
      valuePattern = ValuePattern;
      
      intervalSeconds = IntervalSeconds;
      
      minValue = MinValue;
      
      maxValue = MaxValue;
      
      stepOffset = StepOffset;

      wavelengthSeconds = WavelengthSeconds;
   }
}

public class Logger
{
   private readonly string filePath;
   private readonly string csvFilePath;
   private Queue<string> logQueue;
   private Queue<string> csvQueue;
   private bool isTxtRunning;
    private bool isCsvRunning;

    public Logger(string filePath, string csvPath, string logTxt = "false", string logCsv = "false")
   { 
      this.filePath = filePath;
      this.csvFilePath = csvPath;
      this.logQueue = new Queue<string>();
      this.csvQueue = new Queue<string>();
        if (logTxt == "true")
      this.isTxtRunning = true;
        if (logCsv == "true")
      this.isCsvRunning = true;
   }

   public void Log(string message)
   {
      String completeMessage = $"{DateTime.Now}: " + message + "\n";
      logQueue.Enqueue(completeMessage);
   }

   public void LogToCSV(string message)
   {
      csvQueue.Enqueue($"{DateTime.Now},{message}");
   }

   public void writeLog()
   {
      while (this.isTxtRunning)
      {
         while (logQueue.Count > 0)
         {
            string msg = logQueue.Dequeue();
            using (StreamWriter sw = new StreamWriter(this.filePath, true))
            {
               sw.WriteLine(msg);
            }
         }
      }
   }

   public void writeCSVLog()
   {
      while (this.isCsvRunning)
      {
         while (csvQueue.Count > 0)
         {
            string msg = csvQueue.Dequeue();
            using (StreamWriter sw = new StreamWriter(this.csvFilePath, true))
            {
               sw.WriteLine(msg);
            }
         }
      }
   }

   public void setRunningStatus(bool status)
   {
      this.isCsvRunning = status;
        this.isTxtRunning = status;
   }


}
