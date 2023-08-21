namespace CommunityServerAPI.BattleBitAPI.Common.Enums
{
    public enum ReasonOfDamage : byte
    {
        // 服务器命令
        Server = 0,
        // 武器
        Weapon = 1,
        // 流血
        Bleeding = 2,
        // 跌落
        Fall = 3,
        // 机翼、桨叶
        HelicopterBlade = 4,
        // 载具boom
        VehicleExplosion = 5,
        // 爆炸物
        Explosion = 6,
        // 载具碾压
        vehicleRunOver = 7,
        // 建筑倒塌
        BuildingCollapsing = 8,
        // 锤子
        SledgeHammer = 9,
        // 树木倒塌
        TreeFall = 10,
        // 协助算做击杀
        CountAsKill = 11,
        // 自杀
        Suicide = 12,
        // 飞机坠毁
        HelicopterCrash = 13,
        // 铁丝网
        BarbedWire = 14,
    }
}
