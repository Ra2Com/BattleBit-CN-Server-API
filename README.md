# MujAPi
Battle bit server api with custom extentions and methods

![GitHub CI](https://github.com/muji2498/BattleBit-Community-Server-API/actions/workflows/build.yml/badge.svg)

[![pre-release](https://github.com/muji2498/BattleBit-Community-Server-API/actions/workflows/pre-release.yml/badge.svg)](https://github.com/muji2498/BattleBit-Community-Server-API/actions/workflows/pre-release.yml)

  * [ ] `Logging`
	* [x] `Basic Logging`
    * [ ] `Send Logs - DataBase`
	* [ ] `Send Logs - Discord Bot`
	* [ ] `Send Errors - Database`
	* [ ] `Send Errors - Discord Bot`
  * [ ] `Discord Bot` - integrate discord bot
	* [ ] `Start Server`
	* [ ] `Issue Commands`
	* [ ] `Take Actions` - could be used if you have something scanning chat for profanity or OnPlayerReported
  * [ ] `Handle Environment Variables`
  * [x] `ChatCommandHandler`
	* [x] `AddCommand` - ("command", Callback)
	* [x] `ExecuteCommand` - used to execute the call back
  * [x] `APICommandHandler`
	* [x] `help command` - show a list of available commands for api server
  * [ ] `DataBase` Integration
	* [ ] save stats
	* [ ] save match data
	* [ ] save logs
	* [ ] get stats
	* [ ] get mappool
	* [ ] get gamemodepool
	* [ ] get weapon bans
	* [ ] get class bans
	* [ ] get premium members
	* [ ] get flags for cross, trollmode etc
  * [x] `MapPool`
	* [x] Update MapPool using chat commands
  * [x] `GameModePool`
	* [x] Update GamePool using chat command
  * [ ] `WeaponBan`
	* [x] ability to update weapon ban using chat command
	* [ ] set class ban in db
  * [ ] `ClassBan`
	* [x] ability to update class ban using chat command
	* [ ] set class ban in db
  * [ ] `GadgetBan`
	* [x] ability to update class ban using chat command
	* [ ] set class ban in db
  * [x] `Match` class used for tracking match statistics
	* [x] AddTeamKill
 * [x] RemoveTeamKill
 * [x] AddSquadKill
 * [x] RemoveSquadKill
