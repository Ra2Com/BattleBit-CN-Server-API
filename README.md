# RamboArena's Random Revenger Mode (WIP)
* [ ] Server API
	* [x] MOTD
	* [x] K/D, Score, Nickname, Server Group Ranking
    * [ ] Killer's HP, Killreason
	* [ ] Random Sets of respawn weapon sets and gadgets, throwouts.
	* [ ] Random Spawn Positions range.
* [ ] Gameplay
	* [x] Killer gets victim's Primary, Secondary weapon sets and gadgets.
	* [ ] Victim kills latest killer will cost 10 score of the opposite team.
	* [ ] Victim respawn show real-time last killer position to revenge. Until killer died(Killed by you, others or suicide).
	* [x] Limited map pool.
	* [x] Limited Day Map only.
	* [x] 64 vs 64.

当前玩法功能：
1. 服务器MOTD通知
    a. 进入服务器时展示 MOTD
2. 小窗通知
    a. 玩家出生时，提示玩家昵称、游戏时长、K/D、当前总积分、当前排名
    b. 玩家被击杀时，提示死亡原因，对方剩余血量
    c. 玩家被击杀复活后，小窗新增一行显示当前标记的仇人
3. 玩家被击杀复活后，永久标记上一次击杀此玩家的当前位置
4. 玩家在击杀某个玩家后，获得所击杀玩家的主武器（包括附件和弹药数）、副武器（包括附件和弹药数），并治疗自身20HP（不可超过上限）
5. 无论玩家选择任何出生点，将在地图上随机出生位置，并和其它玩家至少保持20M的安全距离
6. 玩家出生时无论设置的套装是怎样，在以下装备列表中随机几个组合分配给玩家。
7. 玩家进入服务器后从主 API 获取玩家当前的数据，并与本地数据进行对比更新，若玩家的数据更新则更新玩家的游戏数据。



# BBR服务器API

 [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
 
这个是BBR（像素战地）的服务端API


## 兰博对战平台服务器功能（后续研发中）
  * [ ] 日志
	* [x] 基础日志
	* [x] 日志文件
    * [ ] 日志入库
	* [ ] 日志记录 —— RamboAdmin RCON 面板
	* [ ] 错误日志 —— 数据库
	* [ ] 错误日志 —— RamboAdmin RCON 面板
  * [ ]  RamboAdmin RCON 接入
	* [ ] 开服指令
	* [ ] 发送 RCON
	* [ ] RCON 管理员操作 —— 如聊天有屏蔽词或者局内举报、现行作弊等情况
  * [ ]  `RamboK8S` 集群接入
	* [ ] 批量开关服
	* [x] 资源动态伸缩
	* [ ] 注入特定配置
	* [ ] 手动操作
	* [ ] 赛程预约
  * [ ] `RamboProtect` 反作弊接入
  	* [ ] EAC 兼容性
  	* [x] Ring 0 驱动签名
  	* [x] API 上报
  	* [ ] Steam64 ID 上报
  	* [ ] 击杀视角回放
  	* [x] DMA 异常设备检测
  	* [ ] Lockstep 帧同步大数据
  	* [x] 聚类算法特征码
  	* [x] 异常封包特征码
  	* [x] 异常行为特征算法
  	* [x] 异常数据特征算法
   	* [x] 样本搜集功能
  * [ ] `RamboSkill` MMR 算法接入
  	* [x] 单人 TDM 天梯匹配
  	* [ ] 8 V 8 TDM 天梯匹配
  	* [x] 5人 250 人吃鸡天梯
  	* [ ] 250 人吃鸡休闲娱乐
  	* [ ] Rating Pro 算法
  * [x] 环境变量`
  * [x] 聊天命令`
	* [x] 聊天指令 指令和回调
	* [x] 执行指令 执行回调内容
  * [x] `API指令集`
	* [x] 帮助指令 展示 API 中的指令集
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

