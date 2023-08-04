CREATE TABLE `Player` (
    `SteamId` BIGINT NOT NULL DEFAULT '0',
    `Name` VARCHAR(50) NOT NULL,
    `Kills` INT DEFAULT '0',
	`Deaths` INT DEFAULT '0',
    `Wins` INT DEFAULT "0",
    `Losses` INT DEFAULT "0",
    `Rank` INT DEFAULT "0",
    `TotalScore` INT DEFAULT "0",
    `FavouriteWeapon` VARCHAR(50),
    `LongestKill` DECIMAL(5, 2) DEFAULT 0.00,
    `LastTimePlayed` datetime,
    `TotalHeadShots` INT DEFAULT "0",
    `TotalPlayTime` INT DEFAULT "0",
    PRIMARY KEY (`SteamId`)
);

CREATE TABLE `PlayerPermissions` (
    `SteamId` BIGINT NOT NULL DEFAULT '0',
    `IsAdmin` BOOLEAN NOT NULL DEFAULT 0,
    `IsModerator` BOOLEAN NOT NULL DEFAULT 0,
    `IsVip` BOOLEAN NOT NULL DEFAULT 0,
    `IsSpecial` BOOLEAN NOT NULL DEFAULT 0,
    `IsTrollFlagController` BOOLEAN NOT NULL DEFAULT '0',
	`IsPremium` BOOLEAN NOT NULL DEFAULT '0',
    PRIMARY KEY (`SteamId`),
    FOREIGN KEY (`SteamId`) REFERENCES `Player` (`SteamId`) ON DELETE CASCADE
);

CREATE TABLE `PlayerWeaponStats` (
    `Id` INT AUTO_INCREMENT,
    `SteamId` BIGINT NOT NULL,
    `WeaponName` VARCHAR(50) NOT NULL,
    `Kills` INT DEFAULT 0,
    `Headshots` INT DEFAULT 0,
    `Accuracy` DECIMAL(5, 2) DEFAULT 0.00,
    
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`SteamId`) REFERENCES `Player` (`SteamId`) ON DELETE CASCADE
);

CREATE TABLE `MatchData` (
	`MatchId` INT AUTO_INCREMENT PRIMARY KEY,
    `USAPoints` INT DEFAULT 0,
    `USAKills` INT DEFAULT 0,
    `USADeaths` INT DEFAULT 0,
    `RUSPoints` INT DEFAULT 0,
    `RUSKills` INT DEFAULT 0,
    `USADeaths` INT DEFAULT 0,
    
    PRIMARY KEY (`MatchId`)
);

CREATE TABLE `TeamData` (
	`MatchId` INT,
    `TeamId` INT AUTO_INCREMENT,
    `Kills` INT DEFAULT 0,
    `Deaths` INT DEFAULT 0,
    
    PRIMARY KEY (`TeamId`),
    FOREIGN KEY (`MatchId`) REFERENCES `MatchData` (`MatchId`) ON DELETE CASCADE
);

CREATE TABLE `TeamPlayer` (
	`TeamId` INT,
    `PlayerId` INT,
    `Kills` INT,
    `Deaths` INT,
    
    PRIMARY KEY (`PlayerId`, `TeamId`),
    FOREIGN KEY (`TeamID`) REFERENCES `TeamData` (`TeamId`) ON DELETE CASCADE,
    FOREIGN KEY (`PlayerId`) REFERENCES `Player` (`SteamId`)
);

