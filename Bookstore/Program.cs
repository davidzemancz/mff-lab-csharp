﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore
{
    public class Program
    {
        public static class Commands
        {
            public const string DATA_BEGIN = "DATA-BEGIN";
            public const string GET = "GET";
            public const string DATA_END = "DATA-END";
        }

        public static void Main(string[] args)
        {
            bool testFile = false;

            IInputReader inputReader;
            IOutputWriter outputWriter;
            IDataSource dataSource = new LocalDataSource();

            if (testFile)
            {
                inputReader = new FileInputReader(new StreamReader(@"C:\Users\david.zeman\Downloads\Example\NezarkaTest.in"));
                outputWriter = new FileOutputWriter(new StreamWriter(@"C:\Users\david.zeman\Downloads\Example\NezarkaTestDZ.out"));
            }
            else
            {
                inputReader = new ConsoleInputReader();
                outputWriter = new ConsoleOutputWriter();
            }

            int i = 0;
            while (true)
            {
                i++;
                string line = inputReader.ReadLine();
                if (line == null) break;
                string[] lineParts = line.Split(' ');

                IService service = null;
                ServiceResult serviceResult = null;

                if (line == Commands.DATA_BEGIN)
                {
                    service = new CsvDataImporterService(inputReader, dataSource);
                    serviceResult = service.Run(line);
                }
                else if (lineParts[0] == Commands.GET)
                {
                    service = new HtmlDataProviderService(outputWriter, dataSource);
                    serviceResult = service.Run(line);
                    outputWriter.WriteLine("====");
                }
                else
                {
                    service = new HtmlDataProviderService(outputWriter, dataSource);
                    serviceResult = new ServiceResult();
                    ((HtmlDataProviderService)service).WriteInvalidRequest();
                }

                if (!serviceResult.Success)
                {
                    outputWriter.WriteLine(serviceResult.Message);
                    break;
                }
            }

            if (inputReader is IDisposable disposable1) disposable1.Dispose();
            if (outputWriter is IDisposable disposable2) disposable2.Dispose();   
        }
    }
}