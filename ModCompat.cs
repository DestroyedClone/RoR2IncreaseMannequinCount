using InLobbyConfig.Fields;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoR2IncreaseMannequinCount
{
    internal class ModCompat
    {
        public static void Init()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                RiskOfOptionsCompat();
            }
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.InLobbyConfig"))
            {
                InLobbyConfigCompat();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RiskOfOptionsCompat()
        {
            RiskOfOptions.ModSettingsManager.SetModDescription("Increases amount of character displays in lobby.");
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.IntSliderOption(Plugin.cfgMannequinCount, new RiskOfOptions.OptionConfigs.IntSliderConfig()
            {
                min = 0,
                max = 100
            }));

            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.SliderOption(Plugin.cfgXOffset, new RiskOfOptions.OptionConfigs.SliderConfig()
            {
                min = 0,
                max = 6
            }));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void InLobbyConfigCompat()
        {
            var configEntry = new InLobbyConfig.ModConfigEntry();
            configEntry.DisplayName = "More Mannequins";
            configEntry.SectionFields.Add("Config", new List<IConfigField>
            {
                new InLobbyConfig.Fields.FloatConfigField(Plugin.cfgXOffset.Definition.Key, () => Plugin.cfgXOffset.Value, Hook_MannequinOffsetChanged)
            });
            InLobbyConfig.ModConfigCatalog.Add(configEntry);
        }

        private static void Hook_MannequinOffsetChanged(float newValue)
        {
            Plugin.cfgXOffset.Value = newValue;
            Plugin.UpdateMannequinOffsets();
        }
    }
}