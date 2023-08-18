 [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
 
# Battlebit Remastered Random Revenger Mode (WIP)
* [x] Server API
  * [x] `ServerRules`
  * [x] K/D, Score, Nickname, Server Group Ranking(By Points Earned Lifetime in Community Server)
  * [x] Killer's HP, Killreason by `Message`.
  * [x] Random Sets assigned on each respawn weapon sets and gadgets, throwouts.
  * [x] Random Spawn Positions range. And should be 20M away from any enemy.
* [ ] Gameplay
  * [x] Killer gets victim's gadgets and throwouts. If one Weapon and gadget set is the same, it won't change.
  * [ ] Victim successful revenge will cost additional 10 score of the opposite team.
  * [x] Victim respawn show real-time last killer position to revenge in `Message`. Until killer died(Killed by you, others or suicide).
  * [x] Limited map pool.
  * [x] Limited Day Map only.
  * [x] 32 vs 32.
* [ ] TO-DO List
  * [ ] Kill streak Player Statics Boost. On 3/5/10/15/20/30.
  * [ ] Die streak Player Statics Boost. On 2/3/5/7/11/13.
  * [ ] Down streak Player Statics Boost. On 3/5/7/11/13/17.
  * [ ] Mongo DB connection and store player data.
  * [ ] Web based API to communicate with Server API to read/write ranking data.
  * [ ] Other upcoming server mode should view below. ⬇️

## 随机复仇模式玩法测试功能 2023年8月19日：
* [x] 服务器
	* [x] `ServerRules`通知。
	* [x] 地图模式：TDM
	* [x] 地图池：`Salhan`（萨尔罕）, `Azagor`（阿扎戈尔）, `Dusty Dew`（尘露谷）, `Sandy Sunset`（日落沙丘）, `Wine Paradise`（酿酒圣地）, `Frugis`（弗鲁吉斯城）, `Tensa Town`（坦萨小镇）顺序轮换。
	* [x] 最少 8 v 8, 最大 32 v 32 (由于是游戏模式限制所以暂时还扩大不了)。
	* [x] 服务器 Tickrate 刷新率 240hz。 
* [x] 游戏设置
	* [x] 无流血、限量绷带、无扶人。
	* [x] 角色间有碰撞体积。
	* [x] 跳跃变换方向时会损失移动速度。
	* [x] 被击倒立即死亡并刷新在新的位置。
	* [x] 地图永远是白天，禁止使用夜视仪。
	* [x] 换弹速度为武器实际速度的 70%。
	* [x] 右上角会展示击杀通知。
	* [x] 所有武器伤害值降低到 75%，符合大部分 TTK 逻辑。
* [x] `Message`小窗通知
    * [x] 玩家出生时，提示玩家昵称、游戏时长、K/D、当前总积分、当前在此类型的排名（通过得分计算）
    * [x] 玩家被击杀时，提示死亡原因，对方剩余血量。
    * [x] 玩家被击杀复活后，小窗新增一行内容显示当前标记的仇人。
* [x] 玩家被击杀复活后，永久标记上一次击杀此玩家的当前位置。击杀仇人将减少对面 10 点人口，标记和人口惩罚直到仇人死亡一次后消失。
* [x] 玩家在击杀某个玩家后，获得所击杀玩家的主副道具和投掷物，并治疗自身 20 HP（不可超过生命值上限）。
* [x] 无论玩家选择任何出生点，将在地图上随机出生位置，并和其它敌对玩家至少保持 20M 的安全距离。
* [x] 玩家出生时无论设置的主副武器、道具、投掷物是怎样，服务器会在装备列表配置中随机抽取几个组合分配给玩家。

### 即将推出
* [ ] 玩家进入服务器后从主 API 获取玩家当前的数据，并与本地数据进行对比更新，若玩家的等级、武器解锁数据比存储中则更新玩家的游戏数据。
* [ ] 玩家在游戏中累计击杀、死亡将获得奖励性数值加成。

## Upcoming Community Plan —— Develop Side of RamboArena
* [ ] 日志 Logs
	* [x] 基础日志 Basic Logs
	* [x] 日志文件 Log4j Service
    * [ ] 日志入库 Log Control Panel
	* [ ] 日志记录 —— RamboAdmin RCON 
	* [ ] 错误日志 —— Database
	* [ ] 错误日志 —— RamboAdmin RCON
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
* [x] 环境变量
* [x] 聊天命令
	* [x] 聊天指令 指令和回调
	* [x] 执行指令 执行回调内容
* [x] API Commands
	* [x] 帮助指令 展示 API 中的指令集
* [ ] 数据库 Database and ElasticSearch
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
* [x] Map Pool Control
	* [x] 使用聊天指令更新地图池
* [x] Game Modes Control
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
* [x] Match or Blitz
	* [x] 小队击杀增减
	* [x] 战队击杀增减
