# warframe-market-connector

### 简介

这个项目的功能为改变warframe market（以下简称wm）的在线状态，支持手动更改和自动更改。

其中，自动更改模式3分钟检索一次游戏进程是否存在，来自动更改。



### 实现

wm的openAPI并没有提供用于更改用户在线状态的接口。唯一的更新方式是建立WebSocket，向服务器发送更新状态的数据，并使用浏览器cookie中的token来进行身份验证。

目前token需要手动的复制浏览器的token，wm的token有效期为两个月。如果手动在浏览器退出账号，则当前token失效。

配置文件 settings.json 可更改项
1. Token
2. 进程名(默认值: "Warframe.x64")
3. 进程检测的频率(默认值: 3, 单位: 分钟)



### 更新计划

由于这是我的第一个C#项目，很多语法我并不熟悉，总的来说，感谢ChatGPT吧……

1. 自动获取token
2. 添加图形化界面
3. 如果官方能提供API，其实就很简单了……
