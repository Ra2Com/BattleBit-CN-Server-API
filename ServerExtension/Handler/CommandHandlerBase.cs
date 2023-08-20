﻿using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public abstract class CommandHandlerBase
    {
        public string commandMessage;
        public string helpMessage;
        public string[] Aliases { get; set; } = new string[0];
        public List<Roles> roles { get; set; } = new List<Roles>() { Roles.None };
        public bool isPrivate { get; set; } = false;

        public virtual CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return null;
        }

        public abstract void Execute(MyPlayer player, string cmdMsg);
    }
}