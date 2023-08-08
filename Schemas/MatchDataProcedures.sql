drop procedure if exists InsertMatchData;
drop procedure if exists UpdateMatchData;
drop procedure if exists GetMatchDataByMatchId;
drop procedure if exists GetMatchDataByServerId;

delimiter $$
create procedure InsertMatchData(
	in p_GameServerId int
)
begin
	declare v_MatchId int;

	-- create the matchdata
	insert into matchdata(GameServerId)
    values (p_GameServerId);
    
    -- get the current match id
    set v_MatchId = LAST_INSERT_ID();
    
    -- return match id
    select v_MatchId as MatchId;
end $$
delimiter ;

delimiter $$
create procedure UpdateMatchData(
	in p_MatchId int,
    in p_GameServerId int,
    in p_USAPoints int,
    in p_USAKills int,
    in p_USADeaths int,
    in p_RUSPoints int,
    in p_RUSKills int,
    in p_RUSDeaths int
)
begin
	insert into matchdata(
		GameServerId,
        USAPoints,
        USAKills,
        USADeaths,
        RUSPoints,
        RUSKills,
        RUSDeaths
    )
    values (p_MatchId, p_GameServerId, p_USAPoints, p_USAKills, 
		p_USADeaths, p_RUSPoints, p_RUSKills, p_RUSDeaths);
    
    select LAST_INSERT_ID() as MatchId;
end $$
delimiter ;

delimiter $$
create procedure GetMatchDataByMatchId(
	in p_MatchId int
)
begin
	if exists (select 1 from matchdata where MatchId = p_MatchId) then
		select
			MatchId,
			GameServerId,
			CreatedAt,
			USAPoints,
			USAKills,
			USADeaths,
			RUSPoints,
			RUSKills,
			RUSDeaths
		from matchdata
		where MatchId = p_MatchId;
	else
		select concat('Match Data could not be found for MatchId: ', cast(p_MatchId as char)) as Message;
	end if;
end $$
delimiter ;

delimiter $$
create procedure GetMatchDataByServerId(
	in p_GameServerId int
)
begin
	if exists (select 1 from matchdata where GameServerId = p_GameServerId) then
		select
			MatchId,
			GameServerId,
			CreatedAt,
			USAPoints,
			USAKills,
			USADeaths,
			RUSPoints,
			RUSKills,
			RUSDeaths
		from matchdata
		where GameServerId = p_GameServerId;
	else
		select concat('Match Data could not be found for GameServerId: ', cast(p_GameServerId as char)) as Message;
	end if;
end $$
delimiter ;