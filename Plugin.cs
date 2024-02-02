using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using RoR2.SurvivorMannequins;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace RoR2IncreaseMannequinCount
{
    [BepInPlugin("com.DestroyedClone.MoreMannequins", "More Mannequins", "0.0.0")]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.KingEnderBrine.InLobbyConfig", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<int> cfgMannequinCount;
        public static ConfigEntry<float> cfgXOffset;

        public static SurvivorMannequinDioramaController controllerInstance;

        internal static ManualLogSource _logger;
        internal static ConfigFile _config;

        public void Awake()
        {
            _logger = Logger;
            _config = Config;

            cfgMannequinCount = Config.Bind("", "Mannequin Count", 20, "The amount of extra mannequin slots to add on top of the existing amount. Needs to be set outside of lobby.");
            cfgMannequinCount.SettingChanged += CfgMannequinCount_SettingChanged;
            cfgXOffset = Config.Bind("", "X Offset", 3f, "X transform offset for each mannequin.");
            cfgXOffset.SettingChanged += CfgXOffset_SettingChanged;

            On.RoR2.SurvivorMannequins.SurvivorMannequinDioramaController.OnEnable += SurvivorMannequinDioramaController_OnEnable;
            ModCompat.Init();
        }

        private static void CfgMannequinCount_SettingChanged(object sender, EventArgs e)
        {
            if (controllerInstance)
            {
                _logger.LogMessage($"CfgMannequinCount_SettingChanged : Will be activated outside of the lobby.");
                return;
            }
        }

        private static void CfgXOffset_SettingChanged(object sender, EventArgs e)
        {
            UpdateMannequinOffsets();
        }

        public static void UpdateMannequinOffsets(List<SurvivorMannequinSlotController> slots = null)
        {
            if (slots == null)
            {
                slots = new List<SurvivorMannequinSlotController>();
                controllerInstance.GetSlots(slots);
            }
            for (int i = 0; i < slots.Count; i++)
            {
                UnityEngine.Vector3 vector3 = new UnityEngine.Vector3(
                                    (cfgXOffset.Value * i), 0, 0);
                slots[i].transform.localPosition = vector3;
                _logger.LogMessage($"Moving {slots[i].name} {i} to {vector3.x}");
            }
        }

        private static void SurvivorMannequinDioramaController_OnEnable(On.RoR2.SurvivorMannequins.SurvivorMannequinDioramaController.orig_OnEnable orig, RoR2.SurvivorMannequins.SurvivorMannequinDioramaController self)
        {
            orig(self);
            controllerInstance = self;
            List<SurvivorMannequinSlotController> slots = new List<SurvivorMannequinSlotController>();
            self.GetSlots(slots);
            var referenceSlot = slots[0];

            SurvivorMannequinSlotController CopySlot()
            {
                var copy = UnityEngine.Object.Instantiate(referenceSlot.gameObject);
                copy.name = $"ExtraMannequin{slots.Count + 1}";
                copy.transform.SetParent(referenceSlot.transform.parent);
                copy.transform.rotation = referenceSlot.transform.rotation;
                //copy.transform.localPosition = new UnityEngine.Vector3(slots.Count - 1, 0, 0);
                var copyComp = copy.GetComponent<SurvivorMannequinSlotController>();
                slots.Add(copyComp);
                return copyComp;
            }

            for (int i = 0; i < cfgMannequinCount.Value; i++)
            {
                CopySlot();
            }

            UpdateMannequinOffsets(slots);
            self.SetSlots(slots.ToArray());
        }
    }
}