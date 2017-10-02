﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Permissions;

/**************************************************************
 *                                                            *
 * Project: Module of EnhancedCloudMuisc - CloudMusicHelper   *
 * Author: Ayaka Neko                                         *
 * Repo: https://github.com/nekomeowww/EnhancedCloudMusic     *
 * License: GNU General Public License v3.0                   *
 *                                                            *
 * Copyright (C) 2017  Ayaka Neko                             *
 *                                                            *
 **************************************************************/

namespace CloudMusicHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime localDate = DateTime.Now;
            var format = new CultureInfo("zh-CN");
            Console.WriteLine("[Welcome!!] " + localDate.ToString(format) + " : " + "欢迎使用Enhanced CloudMusic喵~");
            Debug.Logger("你好喵，欢迎来到Enhanced CloudMusic Debug Log模式w");
            Debug.Logger("初始化中，Web服务初始化中...");

            DynamicControl.HistoryFileTracker(FileControl.GetHistory());
            do
            {
                HistoryControl.History();
            }
            while(false);

            Console.ReadLine();
        }
    }

    class CloudMusic
    {

    }

    class Helper
    {
        
    }

    class History
    {

    }

    class HistoryControl
    {
        public static void History()
        {
            Read();
        }

        private static string Read()
        {
            string historytext = null;
            string path = FileControl.GetHistory();

            //Read the file into historytext
            try
            {
                historytext = System.IO.File.ReadAllText(path);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //debug
            //Console.WriteLine(historytext);

            //Read and send data to processor
            JsonControl.HistoryParser(historytext);

            return historytext;
        }
    }

    //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")] //管理员权限，webapp需求
    class FileControl
    {
        public static string GetHistory() //GetHistory方法，获得history文件
        {
            string history = LoadFile("\\webdata\\file", "history"); //在确认网易云本地API前请勿更改此行

            if (history == null)
            {
                Console.WriteLine("Exception: FileNotFound, 文件未找到，检查是否正确运行过网易云音乐w");
                return null;
            }
            else
            {
                return history;
            }
        }

        private static string LoadFile(string subPath, string file)
        {
            var pathWithEnv = @"%USERPROFILE%\AppData\Local\Netease\CloudMusic"; //设置环境量
            string pathEnv = Environment.ExpandEnvironmentVariables(pathWithEnv); //转化环境量
            string pathSource = pathEnv + subPath; //将要寻找的子目录和网易云音乐本地文件API目录合并
            string[] filelist = Directory.GetFiles(pathSource); //保存所有的文件绝对地址到filelist
            string filepath = pathSource + "\\" + file; //将判定值设定为 目录+文件名
            int count = 0;

            try
            {
                foreach (string filename in filelist)
                {
                    //Print all files - Debug Only
                    //Console.WriteLine("正在搜索...");

                    //发行版本中请注释以下Console.WriteLine()
                    //Console.WriteLine(filename);
                    count++;
                    //Console.WriteLine(count);

                    if (filelist.Length == 0)
                    {
                        Console.WriteLine("出现了致命错误：没有发现任何文件。");
                        Console.WriteLine("你真的有安装网易云音乐吗？试试看运行一次呢？");
                        Console.WriteLine("这个错误也许是因为开发的问题或者网易云API变更导致的w，可以汇报到issue喔w");
                        return null;
                    }
                    else
                    {
                        if (filename == filepath)
                        {
                            return filepath;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }

    class JsonControl
    {
        public static Hierarchy HistoryParser(string jsonfile)
        {
            //file in
            string lines = jsonfile;
            //json into hierarchy
            //Hierarchy hierarchy = JsonConvert.DeserializeObject<Hierarchy>(lines);
            JArray jarray = (JArray)JsonConvert.DeserializeObject(lines);

            Hierarchy hirerachy = new Hierarchy();

            //list
            string hierarchy = jarray[0].ToString();
            string id = jarray[0]["id"].ToString();
            string track = jarray[0]["track"].ToString();
            string track_name = jarray[0]["track"]["name"].ToString();
            string artist_name = jarray[0]["track"]["artists"][0]["name"].ToString();
            string album_name = jarray[0]["track"]["album"]["name"].ToString();
            string source = jarray[0]["text"].ToString();

            Debug.Logger("正在播放：" + track_name + " by " + artist_name);
            Debug.Logger("专辑信息：" + album_name);
            Debug.Logger("项目来源：" + source);
            //hirerachy.track.name = jobj["track"];
            //hirerachy.track.id = ;

            return hirerachy;
        }
    }

    class WebApp
    {

    }

    class Modules
    {

    }

    class DynamicControl
    {
        public static void HistoryFileTracker(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            /* Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories. */

            string filename = "history";
            path = System.Text.RegularExpressions.Regex.Replace(path, "history", "");

            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | 
                                   NotifyFilters.LastWrite | 
                                   NotifyFilters.FileName | 
                                   NotifyFilters.DirectoryName;

            watcher.Filter = "history";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Error += new ErrorEventHandler(OnError);

            watcher.EnableRaisingEvents = true;
        }

        private static void HistoryUpdated()
        {
            HistoryControl.History();
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            //Debug tool = new Debug();

            int i = 0;
            int counter;
            i++;

            counter = Debug.methodCallCount();

            if(Debug.CallCount == 2)
            {
                Debug.Logger("数据已经刷新啦，正在解析呢喵...");
                Thread.Sleep(500); //Avoid file io stream error
                HistoryUpdated();

                Debug.CallCount = 0;
            }
            else
            {
                if(Debug.CallCount >= 3)
                {
                    Debug.CallCount = 0;
                }
            }

            //HistoryUpdated();

            /*
             * 这里有个小bug，因为读取之后似乎文件被改变了，导致这个event被触发了一次
             * 正在思考如何解决这个问题w
             * 
             * 这个bug解决啦w
             * 使用的方法是采用方法调用计数，一旦调用超过一定数值才会响应w
             */

            /*
            if (e.ChangeType.ToString() == "Changed")
            {
                Debug.Logger("数据更新中...");
            }
            */
        }
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        private static void OnError(object source, ErrorEventArgs e)
        {
            //  Show that an error has been detected.
            Console.WriteLine("The FileSystemWatcher has detected an error");
            //  Give more information if the error is due to an internal buffer overflow.
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                //  This can happen if Windows is reporting many file system events quickly 
                //  and internal buffer of the  FileSystemWatcher is not large enough to handle this
                //  rate of events. The InternalBufferOverflowException error informs the application
                //  that some of the file system events are being lost.
                Console.WriteLine(("The file system watcher experienced an internal buffer overflow: " + e.GetException().Message));
            }
        }
    }

    class Data
    {

    }

    class Debug
    {
        public static int CallCount = 0;
        public static void NullTracker()
        {

        }

        public static void Logger(string text)
        {
            DateTime localDate = DateTime.Now;
            var format = new CultureInfo("zh-CN");
            Console.WriteLine("[Debug Log] " + localDate.ToString(format) + " : " + text);
        }

        public static int methodCallCount()
        {
            CallCount++;
            int i= 0;
            i++;

            return i;
        }
    }
}
