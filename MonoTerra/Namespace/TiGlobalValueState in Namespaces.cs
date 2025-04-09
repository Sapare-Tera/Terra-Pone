using AssetBundles;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PavonisInteractive.TerraInvicta
{
    public class patch_TIGlobalValuesState : TIGlobalValuesState
        {
        public void TriggerNuclearDetonationEffect(bool barrage, TINationState attacker, TIRegionState region, TINationState enemy)
        {
            if (barrage)
            {
                this.nuclearStrikes++;
                foreach (NuclearExchange nuclearExchange in this.currentNuclearExchanges)
                {
                    if (nuclearExchange.attacker == attacker && nuclearExchange.enemyTargeted == enemy && nuclearExchange.target == region)
                    {
                        this.currentNuclearExchanges.Remove(nuclearExchange);
                        break;
                    }
                }
                //this.AddStratosphericAerosols_ppm(0.00777f, true);disabled
                return;
            }
            //this.AddStratosphericAerosols_ppm(7.77E-05f, true);disabled
        }

        public int nuclearStrikes { get; private set; }
        public static patch_TIGlobalValuesState GlobalValues => (patch_TIGlobalValuesState)GameStateManager.GlobalValues();

        public float stratosphericAerosols_ppm { get; private set; }
            private GameTimeManager gameTime;

            public float GlobalCasualties { get; private set; }

            public void AddtoCasualties(float value, bool Combat)
            {
            if (Combat) {
                this.earthAtmosphericN2O_ppm -= (value);
                value = value * 1000000;
                    }
                this.stratosphericAerosols_ppm += value;
            }

        public void ReduceUnity(float value)
        {
            this.earthAtmosphericN2O_ppm -= value;
        }



        public int WorldAlliances(float value, bool Combat)
        {
            int total = 0;
            foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
            {
                foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
              if (!(tifactionState2 == tifactionState))
                    {
                        if (tifactionState.HasNAP(tifactionState2, true))
                        {
                            ++total;
                        }
                    }
            }
                return total/2;////TEST THIS
        }


        public void MonthlyGlobalEnvironmentalChanges()
            {
                float num = (this.earthAtmosphericCO2_ppm * 1f - 100) / -100f;
                this.AddCO2_ppm(+num, GHGSources.NaturalRemoval);
            //float num2 = this.earthAtmosphericCH4_ppm * 1f / 1000f;
            //this.AddCH4_ppm(+num2, GHGSources.NaturalRemoval);
            //float num3 = this.earthAtmosphericN2O_ppm * 1f / 1000f;
            //this.AddN2O_ppm(+num3, GHGSources.NaturalRemoval);
            //float num4 = GameStateManager.AllRegions().Average((TIRegionState x) => x.xenoforming.xenoformingLevel) / 100f;
            //this.AddCO2_ppm(-num4 * 3.45f / 12f, GHGSources.Xenoforming);
            //this.stratosphericAerosols_ppm = Mathf.Max(this.stratosphericAerosols_ppm * 0.935f - 0.001f, 0f);
            this.pastEarthAtmosphericCO2_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericCO2_ppm;
                this.pastEarthAtmosphericCH4_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericCH4_ppm;
                this.pastEarthAtmosphericN2O_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericN2O_ppm;
                float anomaly_C = this.temperatureAnomaly_C;
                if (anomaly_C > 0f && globalSeaLevelAnomaly_cm <= 100)
                {
                    this.AddToSeaLevel_cm(0.05f * anomaly_C);
                }
                 if (anomaly_C < 0f && globalSeaLevelAnomaly_cm >= 0)
            {
                this.RemoveToSeaLevel_cm(0.05f * anomaly_C);
            }
            GameStateManager.AllExtantNations().ToList<TINationState>().ForEach(delegate (TINationState x)
                {
                    x.ProcessMonthlyGHGsFromEconomy();
                });
                GameStateManager.AllExtantNations().ToList<TINationState>().ForEach(delegate (TINationState x)
                {
                    x.MonthlyTemperatureEconomicImpact(anomaly_C, this.earthAtmosphericCO2_ppm);///UPDATED
                });
            }

            public void AddCO2_ppm(float amount, GHGSources source)
            {
                this.earthAtmosphericCO2_ppm += amount;
                this.earthAtmosphericCO2_ppm = Mathf.Min(this.earthAtmosphericCO2_ppm, 100f);
                this.earthAtmosphericCO2_ppm = Mathf.Max(this.earthAtmosphericCO2_ppm, 0);
                Dictionary<GHGSources, double> co2SourcesRecord_ppm = this.CO2SourcesRecord_ppm;
                co2SourcesRecord_ppm[source] += (double)amount;
            }


            public void AddCH4_ppm(float amount, GHGSources source)
            {
                this.earthAtmosphericCH4_ppm += amount;
                this.earthAtmosphericCH4_ppm = Mathf.Min(this.earthAtmosphericCH4_ppm, 100f);
                this.earthAtmosphericCH4_ppm = Mathf.Max(this.earthAtmosphericCH4_ppm, 0f);
                Dictionary<GHGSources, double> ch4SourcesRecord_ppm = this.CH4SourcesRecord_ppm;
                ch4SourcesRecord_ppm[source] += (double)amount;
            }
            public void AddN2O_ppm(float amount, GHGSources source)
            {
                this.earthAtmosphericN2O_ppm += amount;
                this.earthAtmosphericN2O_ppm = Mathf.Min(this.earthAtmosphericN2O_ppm, 100f);
                this.earthAtmosphericN2O_ppm = Mathf.Max(this.earthAtmosphericN2O_ppm, 0f);
                Dictionary<GHGSources, double> n2OSourcesRecord_ppm = this.N2OSourcesRecord_ppm;
                n2OSourcesRecord_ppm[source] += (double)amount;
            }

        public float temperatureAnomalyCO2_C
            {
                get
                {
                    return Mathf.Max(0f, (this.earthAtmosphericCO2_ppm - 66.66f));
                }
            }

            public float temperatureAnomalyCH4_C
            {
                get
                {
                    return Mathf.Max(0f, (this.earthAtmosphericCH4_ppm - 66.66f));
                }
            }

            public float temperatureAnomalyN2O_C
            {
                get
                {
                    return Mathf.Max(0f, (this.earthAtmosphericN2O_ppm - 66.66f));
                }
            }

            public float temperatureAnomaly_C
            {
                get
                {
                return ((this.earthAtmosphericCO2_ppm + this.earthAtmosphericCH4_ppm + this.earthAtmosphericN2O_ppm - 200f)/10); //+ this.temperatureAnomalyStratosphericAerosols_C; Is Deathcounter
                }
            }

            public float earthAtmosphericCO2_ppm { get; private set; }
            public float earthAtmosphericCH4_ppm { get; private set; }

            public float earthAtmosphericN2O_ppm { get; private set; }

            public const float xenoformingFullCoverageCO2AnnualConsumption_ppm = 3.45f;

            public float globalSeaLevelAnomaly_cm { get; private set; }
        public void AddToSeaLevel_cm(float amount)
        {
            this.globalSeaLevelAnomaly_cm += amount;
            globalSeaLevelAnomaly_cm = Mathf.Min(globalSeaLevelAnomaly_cm, 100f);
            if (this.globalSeaLevelAnomaly_cm >= 85 && !this.globalSeaLevelRise1Triggered)
            {
                this.globalSeaLevelRise1Triggered = true;
                GameStateManager.Earth().SetModelResource();
            }
            if (this.globalSeaLevelAnomaly_cm >= 100 && !this.globalSeaLevelRise2Triggered)
            {
                this.globalSeaLevelRise2Triggered = true;
                GameStateManager.Earth().SetModelResource();
                TIFactionState activePlayer = GameControl.control.activePlayer;
                if (activePlayer == null)
                {
                    return;
                }
                activePlayer.UnlockAchievement("seaLevelRise");
            }
        }
        public void RemoveToSeaLevel_cm(float amount)
        {
            this.globalSeaLevelAnomaly_cm += amount;
            globalSeaLevelAnomaly_cm = Mathf.Max(globalSeaLevelAnomaly_cm, 0f);
            if (this.globalSeaLevelAnomaly_cm <= 85f && !this.globalSeaLevelRise1Triggered)
            {
                this.globalSeaLevelRise1Triggered = true;
                GameStateManager.Earth().SetModelResource();
            }
            if (this.globalSeaLevelAnomaly_cm <= 50 && !this.globalSeaLevelRise2Triggered)
            {
                this.globalSeaLevelRise2Triggered = true;
                GameStateManager.Earth().SetModelResource();
            }
        }
        public void AddSpoilsPriorityEnvEffect(TINationState nation, float scaling)
            {
                float num = nation.economyScore / 100f;
                this.AddCO2_ppm(scaling * num * (TemplateManager.global.SpoCO2_ppm + TemplateManager.global.SpoResCO2_ppm * (float)nation.miningRegions), GHGSources.SpoilsPriority);
                this.AddCH4_ppm(scaling * num * (TemplateManager.global.SpoCH4_ppm + TemplateManager.global.SpoResCH4_ppm * (float)nation.miningRegions), GHGSources.SpoilsPriority);
                this.AddN2O_ppm(scaling * num * (TemplateManager.global.SpoN2O_ppm + TemplateManager.global.SpoResN2O_ppm * (float)nation.miningRegions), GHGSources.SpoilsPriority);
            }

            //public void AddMagicPriorityEnvEffect(TINationState nation, float scaling)
            //{
            //    //float num = nation.economyScore / 100f;
            //    this.AddCO2_ppm(scaling * num * (-0.0050f * (float)nation.oilRegions), GHGSources.SpoilsPriority);
            //    this.AddCH4_ppm(scaling * num * (-0.0000f * (float)nation.oilRegions), GHGSources.SpoilsPriority);
            //    this.AddN2O_ppm(scaling * num * (-0.0000f * (float)nation.oilRegions), GHGSources.SpoilsPriority);
            //}
            //public void AddEnviornmentPriorityEnvEffect(TINationState nation)
            //{
            //    this.AddCO2_ppm(nation.WelfareCO2Removed(), GHGSources.EnvironmentPriority);
            //    this.AddCH4_ppm(nation.WelfareCH4Removed(), GHGSources.EnvironmentPriority);
            //    this.AddN2O_ppm(nation.WelfareN2ORemoved(), GHGSources.EnvironmentPriority);
            //}
            //Need to update now that prios changed
        }
        //public static class patch_Enums
        //{
        //   // public static readonly patch_FactionResource[] FactionResources = ((patch_FactionResource[])Enum.GetValues(typeof(patch_FactionResource))).Except(new patch_FactionResource[1]).ToArray<patch_FactionResource>();
        //    //public static readonly patch_PriorityType[] PriorityTypes = (patch_PriorityType[])Enum.GetValues(typeof(patch_PriorityType));

        //    public static readonly TechCategory[] TechCategories2 = (TechCategory[])Enum.GetValues(typeof(TechCategory));
        //}

        public class patch_TISpaceBodyState : TISpaceBodyState
        {
            public override void PostVisualizerCreationInit_7()
            {
                base.PostVisualizerCreationInit_7();

                if (isEarth && controller != null)
                {
                    var GO4 = controller.modelLink;
                    var GO5 = GO4.GetComponentInChildren<StagitMaterialChanger>();
                    var GO6 = GO5.GetComponent<Renderer>();
                    var mats3 = GO6.sharedMaterials;
                    foreach (var mat in mats3)
                    {
                        var names = mat.GetTexturePropertyNames();
                        mat.SetTexture("_MainTex", AssetBundleManager.LoadAsset<Texture2D>($"earthbundle_d.earth/{mat.mainTexture.name}"));
                        mat.SetTexture("_Normals", AssetBundleManager.LoadAsset<Texture2D>($"earthnormalbundle_d/{mat.GetTexture("_Normals").name}"));
                        mat.SetTexture("_SpecGlossMap", AssetBundleManager.LoadAsset<Texture2D>($"earthspecbundle_d/{mat.GetTexture("_SpecGlossMap").name}"));
                    }
                }
            }
        }
    }
