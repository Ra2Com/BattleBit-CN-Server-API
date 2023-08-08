DROP PROCEDURE IF EXISTS InsertGameServer;
DROP PROCEDURE IF EXISTS UpdateGameServerStatus;
DROP PROCEDURE IF EXISTS GetServerByPort;
DROP PROCEDURE IF EXISTS GetServerByGameServerId;

DELIMITER $$
CREATE PROCEDURE InsertGameServer(
	in p_ServerName varchar(255),
    in p_IPAddress varchar(16),
    in p_Port int,
    in p_Status enum('Online', 'Offline', 'Maintenance')
)
BEGIN
	-- insert new row
	INSERT INTO gameserver (ServerName, IPAddress, Port, Status)
    VALUES (p_ServerName, p_IPAddress, p_Port, p_Status);
    
    -- select the gameserver
    select * from gameserver where ServerName = P_ServerName and IPAddress = p_IPAddress and Port = p_Port;
END $$
DELIMITER ;

delimiter $$
create procedure UpdateGameServerStatus(
	in p_ServerName varchar(255),
	in p_IPAddress varchar(16),
    in p_Port int,
    in p_Status enum('Online', 'Offline', 'Maintenance')
)
begin
	if exists (select 1 from gameserver where Port = p_Port and IPAddress = p_IPAddress) then
		-- update the status of the server
		update gameserver
			set 
				Status = p_Status, 
				ServerName = p_ServerName
		where IPAddress = p_IPAddress and Port = p_Port;
    
		-- select the gameserver
		select * from gameserver where IPAddress = p_IPAddress and Port = p_Port;
	else
		select 'That GameServer does not exist' as Message;
	end if;
end $$
delimiter ;

delimiter $$
create procedure GetServerByPort(in p_Port int)
begin
	if exists (select 1 from gameserver where Port = p_Port) then
		select 
			GameServerId,
			ServerName,
			IPAddress,
			Port,
			Status,
			CreatedAt
			from gameserver 
            where Port = p_port;
	else
		select 'That GameServer does not exist' as Message;
	end if;
end $$
delimiter ;

delimiter $$
create procedure GetServerByGameServerId(in p_GameServerId int)
begin
	if exists (select 1 from gameserver where GameServerId = p_GameServerId) then
		select 
			GameServerId,
			ServerName,
			IPAddress,
			Port,
			Status,
			CreatedAt
		from gameserver 
        where GameServerId = p_GameServerId;
	else
		select 'That GameServer does not exist' as Message;
	end if;
end $$
delimiter ;