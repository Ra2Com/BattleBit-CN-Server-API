CREATE TABLE `MapRotation` (
	`MapRotationId` int auto_increment,
    `MapName` varchar(255) not null,
    `Position` int not null,
    `CreatedAt` timestamp default current_timestamp,
    
    primary key (`MapRotationId`)
);

CREATE TABLE `GameModeRotation` (
	`GameModeRotationId` int auto_increment,
    `GameModeName` varchar(255) not null,
    `Position` int not null,
    `CreatedAt` timestamp default current_timestamp,
	
    primary key(`GameModeRotationId`)
);

CREATE TABLE `motd` (
	`MotdId` int auto_increment,
    `Motd` varchar(255) not null,
    `CreatedAt` datetime default current_timestamp,
    
    primary key (`MotdId`)
);

CREATE TABLE `WeaponBans` (
	`WeaponBanId` int auto_increment,
    `WeaponName` varchar(255),
    `CreatedAt` datetime,
    primary key (`WeaponBanId`)
);

CREATE TABLE `Players` (
    `SteamId` BIGINT NOT NULL,
    `Name` VARCHAR(50) NOT NULL,
	`LastTimePlayed` datetime DEFAULT CURRENT_TIMESTAMP,
    `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
    `Kills` INT DEFAULT '0',
	`Deaths` INT DEFAULT '0',
    `Wins` INT DEFAULT "0",
    `Losses` INT DEFAULT "0",
    `Rank` INT DEFAULT "0",
    `Exp` INT DEFAULT "0",
    `FavouriteWeapon` VARCHAR(50) DEFAULT 'none',
    `LongestKill` DECIMAL(5, 2) DEFAULT 0.00,
    `TotalHeadShots` INT DEFAULT "0",
    `TotalPlayTime` INT DEFAULT "0", -- in minutes
    PRIMARY KEY (`SteamId`)
);

CREATE TABLE `PlayerPermissions` (
    `SteamId` BIGINT NOT NULL DEFAULT '0',
    `IsAdmin` BOOLEAN DEFAULT '0',
    `IsModerator` BOOLEAN DEFAULT '0',
    `IsVip` BOOLEAN DEFAULT '0',
    `IsSpecial` BOOLEAN DEFAULT '0',
    `IsTrollFlagController` BOOLEAN DEFAULT '0',
	`IsPremium` BOOLEAN DEFAULT '0',
    `IsBanned` BOOLEAN default '0',
    PRIMARY KEY (`SteamId`),
    FOREIGN KEY (`SteamId`) REFERENCES `Players` (`SteamId`) ON DELETE CASCADE
);

CREATE TABLE `PlayerWeaponStats` (
    `Id` INT AUTO_INCREMENT,
    `SteamId` BIGINT NOT NULL,
    `WeaponName` VARCHAR(50) NOT NULL,
    `Kills` INT DEFAULT '0',
    `Headshots` INT DEFAULT '0',
    `Accuracy` DECIMAL(5, 2) DEFAULT 0.00,
    `CreatedAt` datetime default current_timestamp,
    
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`SteamId`) REFERENCES `Players` (`SteamId`) ON DELETE CASCADE
);

CREATE TABLE `GameServer` (
	`GameServerId` int auto_increment not null,
    `ServerName` varchar(255) not null,
    `IPAddress` varchar(16) not null,
    `Port` int not null,
    `Status` ENUM('Online', 'Offline', 'Maintenance') not null,
    `CreatedAt` timestamp DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (`GameServerId`)
);

CREATE TABLE `MatchData` (
	`MatchId` INT AUTO_INCREMENT,
    `GameServerId` INT,
    `CreatedAt` datetime default current_timestamp,
    
    `MapRotationId` int default '0',
    `GameModeRotationId` int default '0',
    `MotdId` int default '0',
    `WeaponBanId` int default '0',
    
    `USAPoints` INT DEFAULT '0',
    `USAKills` INT DEFAULT '0',
    `USADeaths` INT DEFAULT '0',
    
    `RUSPoints` INT DEFAULT '0',
    `RUSKills` INT DEFAULT '0',
    `RUSDeaths` INT DEFAULT '0',
    PRIMARY KEY (`MatchId`),
    FOREIGN KEY (`GameServerId`) REFERENCES `GameServer` (`GameServerId`) ON DELETE CASCADE,
	FOREIGN KEY (`MapRotationId`) REFERENCES `MapRotation`(`MapRotationId`),
	FOREIGN KEY (`GameModeRotationId`) REFERENCES `GameModeRotation`(`GameModeRotationId`),
	FOREIGN KEY (`MotdId`) REFERENCES `Motd`(`MotdId`),
	FOREIGN KEY (`WeaponBanId`) REFERENCES `WeaponBans`(`WeaponBanId`)
);

CREATE TABLE `TeamData` (
	`MatchId` INT,
    `TeamId` INT AUTO_INCREMENT,
    `CreatedAt` datetime default current_timestamp,
    `Kills` INT DEFAULT '0',
    `Deaths` INT DEFAULT '0',
    
    PRIMARY KEY (`TeamId`),
    FOREIGN KEY (`MatchId`) REFERENCES `MatchData` (`MatchId`) ON DELETE CASCADE
);

CREATE TABLE `TeamPlayer` (
	`TeamId` INT,
    `PlayerId` BIGINT NOT NULL,
    `CreatedAt` datetime default current_timestamp,
    
    `Kills` INT DEFAULT '0',
    `Deaths` INT DEFAULT '0',
    
    PRIMARY KEY (`TeamId`),
    FOREIGN KEY (`TeamId`) REFERENCES `TeamData` (`TeamId`) ON DELETE CASCADE,
    FOREIGN KEY (`PlayerId`) REFERENCES `Players` (`SteamId`)
);
