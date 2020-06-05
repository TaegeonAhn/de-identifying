using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using de_identifying;
using System.Reflection.Emit;

namespace de_identify
{
    class Program
    {
        static List<string> errorFiles = new List<string>();

        static void Main(string[] args)
        {

            Stopwatch stopWatch = new Stopwatch(); // 스탑워치

            stopWatch.Start();

            string folderName = @"d:\챠트프로";
            string pathString = Path.Combine(folderName, "ExportFiles");
            Directory.CreateDirectory(pathString);  // 폴더 만들기

            try
            {
                var filesPathList = GetFiles();

                foreach (string filePath in filesPathList)
                {
                    Console.WriteLine(filePath); // 파일경로 정상 획득 확인

                    var mdbPath = filePath;
                    var fileName = Path.GetFileName(mdbPath); //파일경로 중 파일명 획득

                    //var tables = GetTables(mdbPath); // 테이블명 불러오기
                    //var columns = GetColumns(mdbPath); // 컬럼명 불러오기


                    //var patientList = ReadData(mdbPath, table, pathString, fileName); // 환자리스트
                    /*
                    string[] readlines = File.ReadAllLines(@"D:\TEST\인명사전3.txt");
                    var hash = new Hashtable(); // hash table 만들기
                    var k = 0;
                    */
                    var nickNames = File.ReadAllLines(@"D:\TEST\인명사전3.txt")
                                        .Where(x => !string.IsNullOrWhiteSpace(x))
                                        .Select(x => x.Trim())
                                        .ToArray();
                    var nickNamesCount = nickNames.Length;

                    var items = new List<Item>();
                    var table = ("환자정보");
                    var data = ReadData(mdbPath, table, pathString, fileName);
                    //var data = new List<PatientRegistration>();

                    for (int i = 0; i < data.Count; i++)
                    {
                        var d = data[i];

                        var nickname = nickNames[i % nickNamesCount];
                        var item = new Item()
                        {
                            Index = i,
                            ChartNumber = d.chartNumber,
                            PatientName = d.patientName,
                            Nickname = nickname
                        };
                        items.Add(item);
                        Console.WriteLine("{0} | {1} | {2} | {3}", item.Index, item.ChartNumber, item.PatientName, item.Nickname);
                    }

                }
                /*
                foreach (string line in readlines)
                {
                    hash.Add(k, line);
                    k += 1;
                }
                var a = 0;
                var b = 0;
                foreach (var key in hash.Keys)
                {

                    foreach(var pr in patientList)
                    {

                        a = Convert.ToInt32(pr.chartNumber) % 3000;
                        Console.WriteLine(a);
                        //b = key;

                    }



                    Console.WriteLine("{0},{1}", key, hash[key]);*/

                foreach (var file in errorFiles)
                {
                    Console.WriteLine(file);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("완료");
            }

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine("Elapsed Time is {0:00}:{1:00}:{2:00}",
                                ts.Hours, ts.Minutes, ts.Seconds);

            Console.ReadLine();
        }
        // public static bool isAccessAble(string path)

        public static string[] GetFiles()
        {
            string[] filesPathList = Directory.GetFiles(@"D:\TEST\챠트프로2", "*.mdb", SearchOption.AllDirectories);

            return filesPathList;
        }

        public static string[] GetTables(string mdbPath) //테이블명 얻기
        {
            try
            {
                var results = new List<string>();
                var connString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={mdbPath};User ID=admin;JET OLEDB:Database Password=;";
                var connection = new OleDbConnection(connString);
                using (connection)
                {
                    connection.Open();
                    var restrictions = new string[4];
                    restrictions[3] = "Table";
                    var tables = connection.GetSchema("Tables", restrictions); // 테이블명 
                    foreach (DataRow row in tables.Rows)
                    {
                        results.Add(row[2].ToString());
                        //Console.WriteLine(row[2]);
                    }
                }
                return results.ToArray();
            }
            catch (Exception e)
            {
                errorFiles.Add(mdbPath);
                Console.WriteLine(e);
                return new string[0];
            }
        }

        public static string[] GetColumns(string mdbPath) // 컬럼명 얻기
        {
            try
            {
                var results = new List<string>();
                var connString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={mdbPath};User ID=admin;JET OLEDB:Database Password=;";
                var connection = new OleDbConnection(connString);
                using (connection)
                {
                    connection.Open();

                    var tables = connection.GetSchema("Tables"); // 테이블명 
                    foreach (DataRow row in tables.Rows)
                    {
                        string TableName = row.ItemArray[2].ToString();

                        var restrictions = new string[4];
                        restrictions[2] = TableName;
                        var columns = connection.GetSchema("Columns", restrictions);

                        foreach (DataRow rowColumn in columns.Rows)
                        {
                            results.Add(rowColumn[3].ToString());
                            //Console.WriteLine(rowColumn[3]); //컬럼명출력
                        }
                    }
                }
                return results.ToArray();
            }
            catch (Exception e)
            {
                errorFiles.Add(mdbPath);
                Console.WriteLine(e);
                return new string[0];
            }
        }


        public static List<PatientInfo> ReadData(string mdbPath, string tableName, string pathString, string fileName)
        {
            var patientList = new List<PatientInfo>();
            var connString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={mdbPath};User ID=admin;JET OLEDB:Database Password=;";
            var connection = new OleDbConnection(connString);
            using (connection)
            {
                var command = new OleDbCommand($"SELECT * FROM [{tableName}]", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PatientInfo pi = new PatientInfo(reader);
                        patientList.Add(pi);
                    }
                }
                return patientList;
            }
        }


        /*
            public static void PrintKeysAndValues(Hashtable hash) // hash테이블 출력 함수
            {
                Console.WriteLine("\t-KEY-\t-VALUE-");
                foreach (DictionaryEntry de in hash)
                    Console.WriteLine($"\t{de.Key}:\t{de.Value}");
                Console.WriteLine();
            }*/

        static void DisplayHashCode(String Operand)
        {
            int HashCode = Operand.GetHashCode();
            Console.WriteLine("The hash code for \"{0}\" is: 0x{1:X8}, {1}",
                              Operand, HashCode);
        }

        /*

        private static void Test(string mdbPath, string tableName, string pathString, string fileName)
        {
            var nickNames = File.ReadAllLines(@"D:\TEST\인명사전3.txt")
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => x.Trim())
                                .ToArray();
            var nickNamesCount = nickNames.Length;

            var items = new List<Item>();
            var data = ReadData(string mdbPath, string tableName, string pathString, string fileName);
            //var data = new List<PatientRegistration>();
            for (int i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var nickname = nickNames[i % nickNamesCount];
                var item = new Item()
                {
                    Index = i,
                    ChartNumber = d.chartNumber,
                    //PatientName = d.name,
                    Nickname = nickname
                };
                items.Add(item);
            }

                    }*/




        class Item
        {
            public int Index { get; set; }

            public string ChartNumber { get; set; }

            public string PatientName { get; set; }

            public string Nickname { get; set; }
        }
    }
}

