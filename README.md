# BBR服务器API

 [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
 
这个是BBR（像素战地）的服务端API

## 如何开始

### 系统需求

- 拥有 BBR 服务端的开服权限，且满足开服条件。
- 可以写基于 [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 的 C# 代码.
- 生产环境中可以部署此代码.

### 如何制作功能

查看维基 [此页面](https://github.com/MrOkiDoki/BattleBit-Community-Server-API/wiki).

使用这个 API 将开启一个`ServerListener` 的监听进程，来了解你的服务端中正在发生什么。
如果想给你的服务端添加功能，可以直接把功能写在 `Program.cs` 中，当然也可以按照框架规范进行其他的功能纂写。


### 编译

可以直接使用 [`dotnet build`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build) 的命令进行编译，或者在你的 VSC 等 IDE 中自定义编译.

### 连接服务端

当你将此项目的功能写完并进行了编译后，需要将此 API 进行部署。此服务可以部署在本地网络、本机网络或者广域网中。我们强烈建议以「最低延迟」为基准进行部署，以保证 `ServerListener` 可以同时监听*多个*游戏服务端。
你可以在游戏服务端的启动配置文件中对 API 的地址和端口进行设定。

## 兰博对战平台服务器功能（研发中）

# 基于 —— MujAPi
![GitHub CI](https://github.com/muji2498/BattleBit-Community-Server-API/actions/workflows/build.yml/badge.svg)
  * [ ] `日志`
	* [x] `基础日志`
	* [x] `日志文件`
    * [ ] `日志入库`
	* [ ] `日志记录 —— RamboAdmin RCON 面板`
	* [ ] `错误日志 —— 数据库`
	* [ ] `错误日志 —— RamboAdmin RCON 面板`
  * [ ]  `RamboAdmin` RCON 接入
	* [ ] `开服指令`
	* [ ] `发送 RCON`
	* [ ] `RCON 管理员操作` —— 如聊天有屏蔽词或者局内举报、现行作弊等情况
  * [ ]  `RamboK8S` 集群接入
	* [ ] `批量开关服`
	* [x] `资源动态伸缩`
	* [ ] `注入特定配置`
	* [ ] `手动操作`
	* [ ] `赛程预约`
  * [ ] `RamboProtect` 反作弊接入
  	* [ ] `EAC 兼容性`
  	* [x] `Ring 0 驱动签名`
  	* [x] `API 上报`
  	* [ ] `Steam64 ID 上报`
  	* [ ] `击杀视角回放`
  	* [x] `DMA 异常设备检测`
  	* [ ] `Lockstep 帧同步大数据`
  	* [x] `聚类算法特征码`
  	* [x] `异常封包特征码`
  	* [x] `异常行为特征算法`
  	* [x] `异常数据特征算法`
   	* [x] `样本搜集功能`
  * [ ] `RamboSkill` MMR 算法接入
  	* [x] `单人 TDM 天梯匹配`
  	* [ ] `8 V 8 TDM 天梯匹配`
  	* [x] `5人 250 人吃鸡天梯`
  	* [ ] `250 人吃鸡休闲娱乐`
  	* [ ] `Rating Pro 算法`
  * [x] `环境变量`
  * [x] `聊天命令`
	* [x] `聊天指令` 指令和回调
	* [x] `执行指令` 执行回调内容
  * [x] `API指令集`
	* [x] `帮助指令` 展示 API 中的指令集
  * [ ] `数据库` 接入
	* [ ] 玩家数据
	* [ ] 对局数据
	* [ ] 对局日志
	* [ ] 获取玩家数据
	* [ ] 获取地图池
	* [ ] 获取游戏模式
	* [ ] 获取禁用武器
	* [ ] 获取禁用职业
	* [ ] 获取禁用建造物
	* [ ] 获取禁用士兵着装
	* [x] 获取今日通知
	* [x] 获取 VIP 玩家
	* [x] 娱乐模式的设置
  * [x] `地图池`
	* [x] 使用聊天指令更新地图池
  * [x] `游戏模式`
	* [x] 使用聊天指令更新游戏模式
  * [ ] `禁用武器`
	* [x] 使用聊天指令更新禁用武器
	* [ ] 使用数据库储存禁用武器
  * [ ] `禁用职业`
	* [x] 使用聊天指令更新禁用职业
	* [ ] 使用数据库储存禁用职业
  * [ ] `禁用建造物`
	* [x] 使用聊天指令更新禁用建造物
	* [ ] 使用数据库储存禁用建造物
  * [ ] `禁用士兵着装`
	* [x] 使用聊天指令更新禁用士兵着装
	* [ ] 使用数据库储存禁用士兵着装
  * [x] `赛事` 数据追踪
	* [x] 小队击杀增减
	* [x] 战队击杀增减

