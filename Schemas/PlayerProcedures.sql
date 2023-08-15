drop procedure if exists AddUser;
drop procedure if exists RemoveUser;
drop procedure if exists UpdateUserStats;
DROP PROCEDURE IF EXISTS GetUser;
DROP PROCEDURE IF EXISTS GetUsers;
DROP PROCEDURE IF EXISTS GetUserPermissions;
drop procedure if exists GetUserWarns;
drop procedure if exists AddUserWarns;


DELIMITER $$
CREATE PROCEDURE AddUser(
	IN p_SteamId BIGINT,
	in p_Name varchar(50)						
)
BEGIN
	if exists (select 1 from players where SteamId = p_SteamId) then
		Select concat('Player already exists SteamId: ', cast(p_SteamId as char)) as Message;
	else
		-- insert into player table
		insert into players (SteamId, Name)
        values (p_SteamId, p_Name);
	
		-- select the player
        select 1 from players where SteamId = p_SteamId;
    end if;
END $$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE RemoveUser(
	IN p_SteamId BIGINT
)
BEGIN
	if exists (select 1 from players where SteamId = p_SteamId) then
		delete from players where SteamId = p_SteamId;
        
        select concat('Records have been removed for: ', cast(p_SteamId AS CHAR)) as Message;
	else
		select concat('Player does not exist SteamId: ', cast(p_SteamId as char)) as Message;
    end if;
END $$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE UpdateUserStats(
	IN p_SteamId BIGINT,
    in p_LastTimePlayed datetime,
    in p_Kills int,
    in p_Deaths int,
    in p_Wins int,
    in p_Losses int,
    in p_Rank int,
    in p_Exp int,
    in p_FavouriteWeapon varchar(50),
    in p_LongestKill decimal(5,2),
    in p_TotalHeadShots int,
    in p_TotalPlayTime int
)
BEGIN
	if exists (select 1 from players where SteamId = p_SteamId) then
		UPDATE players
        SET
            LastTimePlayed = p_LastTimePlayed,
            Kills = p_Kills,
            Deaths = p_Deaths,
            Wins = p_Wins,
            Losses = p_Losses,
            `Rank` = p_Rank,
            Exp = p_Exp,
            FavouriteWeapon = p_FavouriteWeapon,
            LongestKill = p_LongestKill,
            TotalHeadShots = p_TotalHeadShots,
            TotalPlayTime = p_TotalPlayTime
        WHERE SteamId = p_SteamId;
        
        SELECT 'Player statistics updated' AS Message;
	else
		select concat('Player does not exist SteamId: ', cast(p_SteamId as char)) as Message;
    end if;
END $$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE GetUser(IN p_SteamId BIGINT)
BEGIN
	if exists (select 1 from players where SteamId = p_SteamId) then
		SELECT 
			SteamId,
			Name, 
			LastTimePlayed, 
			CreatedAt,
			Kills, 
			Deaths, 
			Wins, 
			Losses, 
			`Rank`, 
			Exp, 
			FavouriteWeapon, 
			LongestKill, 
			TotalHeadShots, 
			TotalPlayTime 
			FROM players 
			WHERE SteamId = p_SteamId;
	else
		select concat('Player does not exist SteamId: ', cast(p_SteamId as char)) as Message;
	end if;
END $$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE GetUsers()
BEGIN
	SELECT * FROM players;
END $$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE GetUserPermissions(IN p_SteamId bigint)
BEGIN
	if exists (select 1 from players where SteamId = p_SteamId) then
		SELECT 
			SteamId, 
			IsAdmin, 
			IsModerator, 
			IsVip, 
			IsSpecial, 
			IsTrollFlagController, 
			IsPremium, 
			IsBanned 
			FROM playerpermissions
			WHERE SteamId = p_SteamId;
	else
		select concat('Player does not exist SteamId: ', cast(p_SteamId as char)) as Message;
	end if;
END $$
DELIMITER ;

delimiter $$
create procedure GetUserWarns(
	in p_SteamId bigint
)
begin
	if exists (select 1 from Players where SteamId = p_SteamId) then
		select
			Id,
			SteamId,
			Message
		from playerwarnings
		where SteamId = p_SteamId;
    else
		select concat('Player does not exist SteamId: ', cast(p_SteamId as char)) as Message;
	end if;
end$$
delimiter ;

delimiter $$
create procedure AddUserWarns(
	in p_SteamId bigint,
    in p_Message varchar(255)
)
begin  
	if exists (select 1 from Players where SteamId = p_SteamId) then
		-- insert the message if the player exists
		insert into playerwarnings (SteamId, Message)
		values (p_SteamId, p_Message);
		
		select * from playerwarnings where SteamId = p_SteamId;
    else
		select concat('Player does not exist SteamId: ', cast(p_SteamId as char)) as Message;
	end if;
end$$
delimiter ;
