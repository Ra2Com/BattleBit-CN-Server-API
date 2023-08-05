DROP PROCEDURE IF EXISTS GetUser;
DROP PROCEDURE IF EXISTS GetUsers;
DROP PROCEDURE IF EXISTS GetUserPermissions;

DELIMITER $$
CREATE PROCEDURE GetUser(IN p_SteamId BIGINT)
BEGIN
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
END $$
DELIMITER ;