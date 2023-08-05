DROP PROCEDURE IF EXISTS InsertGameServer;
DROP PROCEDURE IF EXISTS UpdateGameServerStatus;
DROP PROCEDURE IF EXISTS GetServerByPort;

DELIMITER $$
CREATE PROCEDURE InsertGameServer(
	in p_ServerName varchar(255),
    in p_IPAddress varchar(16),
    in p_Port int,
    in p_Status enum('Online', 'Offline', 'Maintenance')
)
BEGIN
	-- Delete existing rows if they exist
    delete from gameserver
    where ServerName = p_ServerName and IPAddress = p_IPAddress and Port = p_Port;

	-- insert new row
	INSERT INTO gameserver (ServerName, IPAddress, Port, Status)
    VALUES (p_ServerName, p_IPAddress, p_Port, p_Status);
    
    -- select the gameserver
    select * from gameserver where ServerName = P_ServerName and IPAddress = p_IPAddress and Port = p_Port;
END $$
DELIMITER ;

delimiter $$
create procedure UpdateGameServerStatus(
	in p_IPAddress varchar(16),
    in p_Port int,
    in p_Status enum('Online', 'Offline', 'Maintenance')
)
begin
	-- update the status of the server
	update gameserver
    set Status = p_Status
    where IPAddress = p_IPAddress and Port = p_Port;
    
    -- select the gameserver
    select * from gameserver where IPAddress = p_IPAddress and Port = p_Port;
end $$
delimiter ;

delimiter $$
create procedure GetServerByPort(in p_Port int)
begin
	select 
    GameServerId,
    ServerName,
    IPAddress,
    Port,
    Status,
    CreatedAt
    from gameserver where Port = p_port;
end $$
delimiter ;