﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static int male = 0;
        public static int k = 0;
        public static int missinglines = 0;
        public static int readlines = 0;
        public static int totallines = 0;
        public static Dictionary<int,Dictionary<string,int>> dataset = new Dictionary<int, Dictionary<string, int>>();
        public static bool dataFlag = true;
        public static bool flag = true;
        public static StreamReader bulkdata = new StreamReader("../../../../Traffic_Violations.tsv");
        public static char[] chunk = new char[5* 1024 * 1024];
        public static JObject JsonConvert { get; private set; }

        public static void Main(string[] args)
        {
            ProcessStart();
            Console.ReadKey();
        }
        public async static void ProcessStart() {
            List<string> data = new List<string>();
            while (dataFlag)
            {
                if (flag)
                {    
                    data = await Reader();
                    flag = false;
                }
                else
                {
                    var readTask = Reader();
                    var processTask = Process(data);
                    await Task.WhenAll(readTask, processTask);
                    data = readTask.Result;                
                }
            }
            await Process(data);
            foreach (int keys in dataset.Keys) {
                Console.WriteLine("Year of violation - {0} ", keys);
                foreach (string violation in dataset[keys].Keys) {
                    Console.WriteLine("Violation type - {0} count - {1}",violation,  dataset[keys][violation]);                
                }
            }
            Console.WriteLine("total lines - {0}", totallines);
            Console.WriteLine("Missing lines - {0}", missinglines);

        }
        public static Task<List<string>> Reader()
        {
            
            return Task<List<string>>.Factory.StartNew(() =>
            {
                int linecount = 0;
                List<string> data = new List<string>();
                while (linecount < 10000)
                {
                    try
                    {
                        string line = bulkdata.ReadLine();
                        if (line == null )
                        {                            
                            dataFlag = false;
                            break;                            
                        }
                        data.Add(line);
                        totallines++;

                    }
                    catch
                    {
                        k++;
                        Console.WriteLine(k);
                    }
                    linecount++;
                }
                return data;
            });
        }
        public async static Task Process(List<string> data)
        {
            Task loop1 = CountMale(data);
            await loop1;

        }
        public static Task CountMale(List<string> array)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (string row in array)
                {
                    string[] rowarray = null;
                    try
                    {
            
                        rowarray = row.Split("\t");
                    }
                    catch {
            
                    }
                    if (rowarray.Length == 35)
                    {
                        try
                        {
                            int yearvalue =Convert.ToInt32(rowarray[0].Substring(rowarray[0].LastIndexOf("/") + 1));
                            if (yearvalue >= 2013 && yearvalue <= 2015)
                            {
                                string year = rowarray[24];
                                if (dataset.ContainsKey(yearvalue))
                                {
                                    if (dataset[yearvalue].ContainsKey(year))
                                    {
                                        dataset[yearvalue][year]++;
                                    }
                                    else {
                                        dataset[yearvalue].Add(year, 1);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, int> temp = new Dictionary<string, int>();
                                    temp.Add(year, 1);
                                    dataset.Add(yearvalue,temp);
                                }
                            }
                        }
                        catch
                        {
            
                        }
                    }
                    else {
            
                    }
            
                        
                }
            });
       }
    }
}