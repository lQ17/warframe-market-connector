using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MyProject{
  class WebWork{

    private static readonly IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json", optional: true, reloadOnChange: true).Build();
    public static async Task ConnectWebSocketAsync(String changeStatusTo){
        
        
        var JWT = configuration["JWT"];
      
        // WebSocket 服务器的 URI
        var serverUri = "wss://warframe.market/socket?platform=pc";

        // 创建一个 ClientWebSocket 实例
        using (var websocket = new ClientWebSocket()){
            try{

                // 添加自定义请求头
                websocket.Options.SetRequestHeader("Origin", "https://warframe.market");
                websocket.Options.SetRequestHeader("Cookie", "_ga=GA1.1.1134347670.1713084319; JWT=" + JWT + "; _ga_S7F4866DDR=GS1.1.1713168089.11.1.1713168140.0.0.0");

                // 连接到 WebSocket 服务器
                await websocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);

                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 将时间格式化为 "yyyy-MM-dd HH:mm:ss" 格式
                string formattedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");

                Console.WriteLine($"已连接至Warframe Market, 时间: {formattedTime}");

                // 发送消息到服务器
                string message = "{\"type\": \"@WS/USER/SET_STATUS\", \"payload\": \"" + changeStatusTo + "\"}";
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await websocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                // Console.WriteLine("发送消息: " + message);

                while (true){
                // 接收服务器的回应
                var buffer = new byte[1024];
                var result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Console.WriteLine("服务器回应: " + response);

                if(response == message){
                    switch(changeStatusTo){
                        case "invisible":
                            Console.WriteLine(">>>>>>成功切换至“隐  身”状态<<<<<<");
                            break;
                        case "online":
                            Console.WriteLine(">>>>>>成功切换至“在  线”状态<<<<<<");
                            break;
                        case "ingame":
                            Console.WriteLine(">>>>>>成功切换至“游戏中”状态<<<<<<");
                            break;
                        default:
                            Console.WriteLine("意外错误，切换失败！");
                            break;
                        
                    }
                    break;
                }
                }
            }catch (Exception ex){
                Console.WriteLine("General Exception: " + ex.Message);

            }finally{
                // 关闭 WebSocket 连接
                if (websocket != null && websocket.State == WebSocketState.Open) {
                    try {
                        await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }catch (Exception) {
                        // Console.WriteLine(closeEx.Message, "关闭WebSocket时发生异常");
                    }
                }
            }
        }
    }

  }
}
