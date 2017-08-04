﻿using System;
using System.Linq;
using System.Collections.Generic;

using LoonePermissions.Managers;

using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Logging;

namespace LoonePermissions.Providers
{
    public class MySqlProvider : IRocketPermissionsProvider
    {

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            return MySqlManager.AddPlayerToGroup(ulong.Parse(player.Id), groupId);
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            return MySqlManager.RemovePlayerFromGroup(ulong.Parse(player.Id), groupId);
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return MySqlManager.GetGroup(groupId);
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            string groupId = MySqlManager.GetPlayerGroup(ulong.Parse(player.Id));
            return new List<RocketPermissionsGroup> { MySqlManager.GetGroup(groupId) };
        }


        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            string groupId = MySqlManager.GetPlayerGroup(ulong.Parse(player.Id));
            return MySqlManager.GetPermissionsByGroup(groupId, true);
        }


        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            return (from p in GetPermissions(player)
                    where requestedPermissions.Exists((string i) => string.Equals(i, p.Name, StringComparison.OrdinalIgnoreCase))
                    select p).ToList();
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            if (player.IsAdmin)
                return true;

            List<Permission> perms = GetPermissions(player, requestedPermissions);
            List<string> permsAsStrs = new List<string>();

            foreach (Permission perm in perms)
                permsAsStrs.Add(perm.Name);

            bool found = false;

            foreach (string str in requestedPermissions) {
                if (!permsAsStrs.Contains(str)) {
                    Logger.Log(String.Format("Failed Comparison {0}!", str), ConsoleColor.Yellow);
                } else {
                    Logger.Log(String.Format("Successful Comparison {0}!", str), ConsoleColor.Yellow);
                    found = true;
                }
            }

            return found;
        }

        public void Reload()
        {

        }

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            return RocketPermissionsProviderResult.UnspecifiedError;
        }

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            return RocketPermissionsProviderResult.UnspecifiedError;
        }

        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            return RocketPermissionsProviderResult.UnspecifiedError;
        }
    }
}