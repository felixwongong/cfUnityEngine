using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FeatureStr = System.String;

namespace cfUnityEngine.Editor
{
    public class CfFeatureSettingEditor : EditorWindow
    {
        private static readonly string[] FEATURES = new[]
        {
            "CF_ADDRESSABLE",
            "CF_STATISTIC",
            "CF_INVENTORY",
            "CF_REACTIVE",
            "CF_REACTIVE_DEBUG"
        };

        [MenuItem("Cf Tools/Feature Setting")]
        public static void ShowPanel()
        {
            CfFeatureSettingEditor wnd = GetWindow<CfFeatureSettingEditor>();
            wnd.titleContent = new GUIContent("CfFeatureSettingEditor");
        }

        private List<(FeatureStr feature, PlatformType buildTarget)> platformSymbols = new();

        public void CreateGUI()
        {
            platformSymbols.Clear();

            platformSymbols.Capacity = FEATURES.Length;
            foreach (var feature in FEATURES)
            {
                platformSymbols.Add((feature, PlatformType.None));
            }

            foreach (var platformType in (PlatformType[])Enum.GetValues(typeof(PlatformType)))
            {
                if (platformType == PlatformType.None) continue;

                PlayerSettings.GetScriptingDefineSymbols(platformType.GetNamed(), out var definedSymbols);

                for (var i = 0; i < platformSymbols.Count; i++)
                {
                    var (featureSymbol, platformFlag) = platformSymbols[i];
                    if (definedSymbols.Contains(featureSymbol))
                    {
                        platformSymbols[i] = (featureSymbol, platformFlag ^ platformType);
                    }
                }
            }

            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            var applyButton = new Button(() =>
            {
                foreach (var platformType in (PlatformType[])Enum.GetValues(typeof(PlatformType)))
                {
                    if (platformType == PlatformType.None) continue;
                    var features = platformSymbols
                        .Where(x =>
                        {
                            var hasFlag = ((PlatformType)x.buildTarget).HasFlag(platformType);
                            return hasFlag;
                        })
                        .Select(x => x.feature).ToArray();
                    PlayerSettings.SetScriptingDefineSymbols(platformType.GetNamed(), features);
                }
            });
            applyButton.text = "Apply";
            root.Add(applyButton);

            var settingList = new ListView(platformSymbols);
            root.Add(settingList);

            settingList.makeItem = () => new ToggleButtonGroup()
            {
                allowEmptySelection = true,
                isMultipleSelection = true,
            };
            settingList.bindItem = (ve, idx) =>
            {
                var toggleGroup = ve as ToggleButtonGroup;
                if (toggleGroup == null)
                {
                    Debug.LogError("Toggle setting is not a toggle group");
                    return;
                }

                foreach (var platformType in (PlatformType[])Enum.GetValues(typeof(PlatformType)))
                {
                    if (platformType == PlatformType.None) continue;

                    var button = new Button(() =>
                    {
                        var (featureSymbol, platformFlag) = platformSymbols[idx];
                        platformSymbols[idx] = (featureSymbol, platformFlag ^ platformType);
                    });
                    button.text = platformType.GetNamed().TargetName;
                    toggleGroup.Add(button);
                }

                toggleGroup.label = platformSymbols[idx].feature;
                toggleGroup.value = ToggleButtonGroupState.FromEnumFlags(platformSymbols[idx].buildTarget);
            };
        }
    }
}