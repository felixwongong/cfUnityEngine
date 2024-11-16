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
            "CF_INVENTORY"
        };

        [MenuItem("Cf Tools/Feature Setting")]
        public static void ShowPanel()
        {
            CfFeatureSettingEditor wnd = GetWindow<CfFeatureSettingEditor>();
            wnd.titleContent = new GUIContent("CfFeatureSettingEditor");
        }

        private List<(FeatureStr feature, BuildTargetType buildTarget)> featureBuildTargets = new();

        public void CreateGUI()
        {
            featureBuildTargets.Clear();

            InitBuildTargetSymbols();

            void InitBuildTargetSymbols()
            {
                featureBuildTargets.Capacity = FEATURES.Length;
                foreach (var feature in FEATURES)
                {
                    featureBuildTargets.Add((feature, BuildTargetType.None));
                }

                void InitBuildTargetSymbols()
                {
                    foreach (var feature in FEATURES)
                    {
                        featureBuildTargets.Add((feature, BuildTargetType.None));
                    }

                    foreach (var buildTarget in (BuildTargetType[])Enum.GetValues(typeof(BuildTargetType)))
                    {
                        if (buildTarget == BuildTargetType.None) continue;

                        PlayerSettings.GetScriptingDefineSymbols(buildTarget.GetNamed(), out var symbols);

                        for (var i = 0; i < featureBuildTargets.Count; i++)
                        {
                            var (feature, buildTargetType) = featureBuildTargets[i];
                            if (symbols.Contains(feature))
                            {
                                featureBuildTargets[i] = (feature, buildTargetType ^ buildTarget);
                            }
                            else
                            {
                                featureBuildTargets[i] = (feature, buildTargetType ^ buildTarget);
                            }
                        }
                    }
                }

                // Each editor window contains a root VisualElement object
                VisualElement root = rootVisualElement;
                var applyButton = new Button(() =>
                {
                    foreach (var buildTarget in (BuildTargetType[])Enum.GetValues(typeof(BuildTargetType)))
                    {
                        if (buildTarget == BuildTargetType.None) continue;
                        var features = featureBuildTargets
                            .Where(x =>
                            {
                                var hasFlag = ((BuildTargetType)x.buildTarget).HasFlag(buildTarget);
                                return hasFlag;
                            })
                            .Select(x => x.feature).ToArray();
                        PlayerSettings.SetScriptingDefineSymbols(buildTarget.GetNamed(), features);
                    }
                });
                applyButton.text = "Apply";
                root.Add(applyButton);

                var settingList = new ListView(featureBuildTargets);
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

                    foreach (var buildTarget in (BuildTargetType[])Enum.GetValues(typeof(BuildTargetType)))
                    {
                        if (buildTarget == BuildTargetType.None) continue;

                        var button = new Button(() =>
                        {
                            var (feature, target) = featureBuildTargets[idx];
                            featureBuildTargets[idx] = (feature, target ^ buildTarget);
                        });
                        button.text = buildTarget.GetNamed().TargetName;
                        toggleGroup.Add(button);
                    }

                    toggleGroup.label = featureBuildTargets[idx].feature;
                    toggleGroup.value = ToggleButtonGroupState.FromEnumFlags(featureBuildTargets[idx].buildTarget);
                };
            }
        }
    }
}