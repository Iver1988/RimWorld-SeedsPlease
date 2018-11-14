﻿using System;
using System.Linq;
using System.Reflection;

using Harmony;
using Verse;

namespace SeedsPlease
{
    [StaticConstructorOnStartup]
    public static class SeedsPleaseMod
    {
        static SeedsPleaseMod()
        {
            HarmonyInstance.Create("rimworld.seedsplease").PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch]
    static class Command_IsPlantAvailable_Patch
    {
        static MethodBase target;

        static bool Prepare()
        {
            Type type;

            var mod = LoadedModManager.RunningMods.FirstOrDefault(m => m.Name == "Dubs Mint Menus");
            if (mod == null)
            {
                type = typeof(Command_SetPlantToGrow);
            }
            else
            {
                type = mod.assemblies.loadedAssemblies
                            .FirstOrDefault(a => a.GetName().Name == "DubsMintMenus")?
                            .GetType("DubsMintMenus.Dialog_FancyDanPlantSetterBob");

                if (type == null) {
                    Log.Warning("SeedsPlease :: Can't patch DubsMintMenu. No Dialog_FancyDanPlantSetterBob");

                    return false;
                }
            }

            target = AccessTools.DeclaredMethod(type, "IsPlantAvailable");

            if (target == null) {
                Log.Warning("SeedsPlease :: Can't patch menus. No IsPlantAvailable");

                return false;
            }

            return true;
        }

        static MethodBase TargetMethod()
        {
            return target;
        }

        static void Postfix(ThingDef plantDef, Map map, ref bool __result)
        {
            if (__result && plantDef?.blueprintDef != null) {
                __result = map.listerThings.ThingsOfDef(plantDef.blueprintDef).Count > 0;
            }
        }
    }

    [HarmonyPatch]
    static class JobDriver_SowAll_CanStart_Patch
    {
        static MethodBase target;

        static bool Prepare() {
            var mod = LoadedModManager.RunningMods.FirstOrDefault(m => m.Name == "Achtung!");
            if (mod == null) {
                return false;
            }

            var type = mod.assemblies.loadedAssemblies
                        .FirstOrDefault(a => a.GetName().Name == "AchtungMod")?
                        .GetType("AchtungMod.JobDriver_SowAll");

            if (type == null) {
                Log.Warning("SeedsPlease :: Can't patch Achtung! No JobDriver_SowAll");

                return false;
            }

            target = AccessTools.DeclaredMethod(type, "CanStart");
            if (target == null) {
                Log.Warning("SeedsPlease :: Can't patch Achtung! No JobDriver_SowAll.CanStart");

                return false;
            }

            return true;
        }

        static MethodBase TargetMethod() {
            return target;
        }

        static bool Prefix()
        {
            return false;
        }
    }
}