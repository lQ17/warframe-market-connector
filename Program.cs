using System;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Timers;

namespace MyProject{
    class Program
    {
        private static readonly IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json", optional: true, reloadOnChange: true).Build();

        static async Task Main(string[] args)
        {
            await ChooseModel();
            Console.WriteLine("输入任意字符关闭程序");
            Console.ReadKey();
        }

        private static async Task ChooseModel(){
            Console.WriteLine("输入1、2来选择模式");
            Console.WriteLine("1: 自动切换(根据游戏进程是否存在)");
            Console.WriteLine("2: 手动切换");
            Console.WriteLine("-------------------------------------------");
            Console.Write("您的输入: ");
            var key = Console.ReadKey();
            Console.WriteLine("");
            Console.WriteLine("-------------------------------------------");
            switch (key.KeyChar){
                case '1':
                    Console.WriteLine("已选择: 自动切换模式");
                    await AutoSwitchStatus();
                    break;
                case '2':
                    Console.WriteLine("已选择: 手动切换模式");
                    await ManualSwitchStatus();
                    break;
                default:
                    Console.WriteLine("请重新输入正确数字");
                    await ChooseModel();
                    break;
            }
        }

        public static async Task ManualSwitchStatus(){
            Console.WriteLine("输入1、2、3来改变在线状态");
            Console.WriteLine("1: 在线");
            Console.WriteLine("2: 游戏中");
            Console.WriteLine("3: 隐身");
            Console.WriteLine("-------------------------------------------");
            Console.Write("您的输入: ");
            var key = Console.ReadKey();
            Console.WriteLine("");
            Console.WriteLine("-------------------------------------------");
            switch (key.KeyChar){
                case '1':
                    Console.WriteLine("切换至: 在线");
                    await WebWork.ConnectWebSocketAsync("online");
                    break;
                case '2':
                    Console.WriteLine("切换至: 游戏中");
                    await WebWork.ConnectWebSocketAsync("ingame");
                    break;
                case '3':
                    Console.WriteLine("切换至: 隐身");
                    await WebWork.ConnectWebSocketAsync("invisible");
                    break;
                default:
                    Console.WriteLine("请重新输入正确数字");
                    await ManualSwitchStatus();
                    break;
            }
        }


        public static async Task AutoSwitchStatus(){
            if(double.TryParse(configuration["checkProcessRate"], out double checkProcessRate)){
                while (true){
                    if(IsGameRunning()){
                        await WebWork.ConnectWebSocketAsync("ingame");
                    }else{
                        await WebWork.ConnectWebSocketAsync("invisible");
                    }
                    await Task.Delay(TimeSpan.FromMinutes(checkProcessRate));
                }
            }else{
                Console.WriteLine("配置文件中的 checkProcessRate 格式不正确，无法转换为数字。");
                return;
            }
            
            
        }


        public static Boolean IsGameRunning(){
            
            var processName = configuration["processName"];

            // 获取进程
            Process[] processList = Process.GetProcessesByName(processName);
            bool isRunning = processList.Length > 0;
            
            // 释放 Process 对象占用的资源
            foreach (Process process in processList) {
                process.Dispose();
            }
            
            return isRunning;
            
        }
    }
}
