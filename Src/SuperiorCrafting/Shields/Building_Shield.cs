// Decompiled with JetBrains decompiler
// Type: Enhanced_Defence.Shields.Building_Shield
// Assembly: Enhanced_Defence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A381E51-C852-41B2-B251-E7AC4F04E532
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\Enhanced_Defence.dll

using Enhanced_Defence.ShieldUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Enhanced_Defence.Shields
{
[StaticConstructorOnStartup]
  public class Building_Shield : Building
  {
    public bool flag_direct = true;
    public bool flag_indirect = true;
    public bool flag_fireSupression = true;
    public bool flag_shieldRepairMode = true;
    public bool flag_showVisually = true;
    public bool flag_InterceptDropPod = true;
    private ShieldBlendingParticle[] sparksParticle = new ShieldBlendingParticle[3];
    private static Texture2D UI_DIRECT_ON;
    private static Texture2D UI_DIRECT_OFF;
    private static Texture2D UI_INDIRECT_ON;
    private static Texture2D UI_INDIRECT_OFF;
    private static Texture2D UI_FIRE_ON;
    private static Texture2D UI_FIRE_OFF;
    private static Texture2D UI_INTERCEPT_DROPPOD_ON;
    private static Texture2D UI_INTERCEPT_DROPPOD_OFF;
    private static Texture2D UI_REPAIR_ON;
    private static Texture2D UI_REPAIR_OFF;
    private static Texture2D UI_SHOW_ON;
    private static Texture2D UI_SHOW_OFF;
    public int shieldMaxShieldStrength;
    public int shieldInitialShieldStrength;
    public int shieldShieldRadius;
    public int shieldPowerRequiredCharging;
    public int shieldPowerRequiredSustaining;
    public bool shieldBlockIndirect;
    public bool shieldBlockDirect;
    public bool shieldFireSupression;
    public bool shieldInterceptDropPod;
    public bool shieldStructuralIntegrityMode;
    public int shieldRechargeTickDelay;
    public int shieldRecoverWarmup;
    public float colourRed;
    public float colourGreen;
    public float colourBlue;
    public List<string> SIFBuildings;
    private ShieldField shieldField;
    private CompPowerTrader power;
    
	public override string GetInspectStringLowPriority()
	{
		
		int C =(int) Math.Round((double)this.shieldField.shieldCurrentStrength_base  /  shieldMaxShieldStrength * 100);
		return "Shield charge rate: " + C +"%";
	}
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      
      Building_Shield.UI_DIRECT_OFF = ContentFinder<Texture2D>.Get("UI/DirectOff", true);
      Building_Shield.UI_DIRECT_ON = ContentFinder<Texture2D>.Get("UI/DirectOn", true);
      Building_Shield.UI_INDIRECT_OFF = ContentFinder<Texture2D>.Get("UI/IndirectOff", true);
      Building_Shield.UI_INDIRECT_ON = ContentFinder<Texture2D>.Get("UI/IndirectOn", true);
      Building_Shield.UI_FIRE_OFF = ContentFinder<Texture2D>.Get("UI/FireOff", true);
      Building_Shield.UI_FIRE_ON = ContentFinder<Texture2D>.Get("UI/FireOn", true);
      Building_Shield.UI_INTERCEPT_DROPPOD_OFF = ContentFinder<Texture2D>.Get("UI/FireOff", true);
      Building_Shield.UI_INTERCEPT_DROPPOD_ON = ContentFinder<Texture2D>.Get("UI/FireOn", true);
      Building_Shield.UI_REPAIR_ON = ContentFinder<Texture2D>.Get("UI/RepairON", true);
      Building_Shield.UI_REPAIR_OFF = ContentFinder<Texture2D>.Get("UI/RepairOFF", true);
      Building_Shield.UI_SHOW_ON = ContentFinder<Texture2D>.Get("UI/ShieldShowOn", true);
      Building_Shield.UI_SHOW_OFF = ContentFinder<Texture2D>.Get("UI/ShieldShowOff", true);
      base.SpawnSetup(map, respawningAfterLoad);
      this.power = this.GetComp<CompPowerTrader>();
      if (this.def is ShieldBuildingThingDef)
      {
        this.shieldMaxShieldStrength = ((ShieldBuildingThingDef) this.def).shieldMaxShieldStrength;
        this.shieldInitialShieldStrength = ((ShieldBuildingThingDef) this.def).shieldInitialShieldStrength;
        this.shieldShieldRadius = ((ShieldBuildingThingDef) this.def).shieldShieldRadius;
        this.shieldPowerRequiredCharging = ((ShieldBuildingThingDef) this.def).shieldPowerRequiredCharging;
        this.shieldPowerRequiredSustaining = ((ShieldBuildingThingDef) this.def).shieldPowerRequiredSustaining;
        this.shieldRechargeTickDelay = ((ShieldBuildingThingDef) this.def).shieldRechargeTickDelay;
        this.shieldRecoverWarmup = ((ShieldBuildingThingDef) this.def).shieldRecoverWarmup;
        this.shieldBlockIndirect = ((ShieldBuildingThingDef) this.def).shieldBlockIndirect;
        this.shieldBlockDirect = ((ShieldBuildingThingDef) this.def).shieldBlockDirect;
        this.shieldFireSupression = ((ShieldBuildingThingDef) this.def).shieldFireSupression;
        this.shieldInterceptDropPod = ((ShieldBuildingThingDef) this.def).shieldInterceptDropPod;
        this.shieldStructuralIntegrityMode = ((ShieldBuildingThingDef) this.def).shieldStructuralIntegrityMode;
        this.colourRed = ((ShieldBuildingThingDef) this.def).colourRed;
        this.colourGreen = ((ShieldBuildingThingDef) this.def).colourGreen;
        this.colourBlue = ((ShieldBuildingThingDef) this.def).colourBlue;
        this.SIFBuildings = ((ShieldBuildingThingDef) this.def).SIFBuildings;
      }
      else
        Log.Error("Shield definition not of type \"ShieldBuildingThingDef\"");
      if (this.shieldField != null)
        return;
      this.shieldField = new ShieldField(this, this.Position, this.shieldMaxShieldStrength, this.shieldInitialShieldStrength, this.shieldShieldRadius, this.shieldRechargeTickDelay, this.shieldRecoverWarmup, this.shieldBlockIndirect, this.shieldBlockDirect, this.shieldFireSupression, this.shieldInterceptDropPod, this.shieldStructuralIntegrityMode, this.colourRed, this.colourGreen, this.colourBlue, this.SIFBuildings);
    }

    public override void Tick()
    {
      base.Tick();
      if (this.shieldField != null)
      {
        if (this.power != null)
          this.shieldField.enabled = this.power.PowerOn;
        this.shieldField.ShieldTick(this.flag_direct, this.flag_indirect, this.flag_fireSupression, this.flag_InterceptDropPod, this.flag_shieldRepairMode);
        switch (this.shieldField.status)
        {
          case enumShieldStatus.Disabled:
          case enumShieldStatus.Loading:
            this.power.powerOutputInt = (float) this.shieldPowerRequiredCharging;
            break;
          case enumShieldStatus.Charging:
            this.power.powerOutputInt = (float) this.shieldPowerRequiredCharging;
            break;
          case enumShieldStatus.Sustaining:
            this.power.powerOutputInt = (float) this.shieldPowerRequiredSustaining;
            break;
        }
      }
      else
        this.shieldField = new ShieldField(this, this.Position, this.shieldMaxShieldStrength, this.shieldInitialShieldStrength, this.shieldShieldRadius, this.shieldRechargeTickDelay, this.shieldRecoverWarmup, this.shieldBlockIndirect, this.shieldBlockDirect, this.shieldFireSupression, this.shieldInterceptDropPod, this.shieldStructuralIntegrityMode, this.colourRed, this.colourGreen, this.colourBlue, this.SIFBuildings);
    }

    public void DrawShieldField()
    {
      if (!this.shieldField.isOnline() && this.shieldField.shieldRecoverWarmup - this.shieldField.warmupPower >= 60)
        return;
      this.shieldField.DrawField(Vectors.IntVecToVec(this.Position));
      if (this.sparksParticle[0] == null)
      {
        for (int index = 0; index < this.sparksParticle.Length; ++index)
          this.sparksParticle[index] = new ShieldBlendingParticle(this.DrawPos, (int) Math.Round((double) index / (double) (this.sparksParticle.Length - 1) * 80.0));
      }
      int index1 = 0;
      for (int length = this.sparksParticle.Length; index1 < length; ++index1)
        this.sparksParticle[index1].DrawMe();
    }

    public override void Draw()
    {
      base.Draw();
      if (!this.flag_showVisually)
        return;
      this.DrawShieldField();
    }

    public override void DrawExtraSelectionOverlays()
    {
      GenDraw.DrawRadiusRing(this.Position, (float) this.shieldField.shieldShieldRadius);
    }

    public override string GetInspectString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.shieldField.GetInspectString());
      if (this.power != null)
      {
        string str = this.power.CompInspectStringExtra();
        if (!str.NullOrEmpty())
          stringBuilder.AppendLine(str);
      }
      return stringBuilder.ToString();
    }

    public override void ExposeData()
    {
      base.ExposeData();
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Deep.Look<ShieldField>(ref this.shieldField, "shieldField", new object[0]);
      this.shieldField.position = this.Position;
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.flag_direct, "flag_direct", default(bool), false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.flag_indirect, "flag_indirect", default(bool) , false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.flag_fireSupression, "flag_fireSupression", default(bool) , false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.flag_InterceptDropPod, "flag_InterceptDropPod", default(bool) , false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.flag_shieldRepairMode, "flag_shieldRepairMode", default(bool) , false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.flag_showVisually, "flag_showVisually", default(bool) , false);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
      foreach (Gizmo gizmo in base.GetGizmos())
        yield return gizmo;
      if (this.shieldBlockDirect)
      {
        if (this.flag_direct)
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchDirect());
          act.icon = Building_Shield.UI_DIRECT_ON;
          act.defaultLabel = "Block Direct";
          act.defaultDesc = "On";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
        else
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchDirect());
          act.icon = Building_Shield.UI_DIRECT_OFF;
          act.defaultLabel = "Block Direct";
          act.defaultDesc = "Off";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
      }
      if (this.shieldBlockIndirect)
      {
        if (this.flag_indirect)
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchIndirect());
          act.icon = Building_Shield.UI_INDIRECT_ON;
          act.defaultLabel = "Block Indirect";
          act.defaultDesc = "On";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
        else
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchIndirect());
          act.icon = Building_Shield.UI_INDIRECT_OFF;
          act.defaultLabel = "Block Indirect";
          act.defaultDesc = "Off";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
      }
      if (this.shieldFireSupression)
      {
        if (this.flag_fireSupression)
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchFire());
          act.icon = Building_Shield.UI_INDIRECT_ON;
          act.defaultLabel = "Fire Suppression";
          act.defaultDesc = "On";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
        else
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchFire());
          act.icon = Building_Shield.UI_INDIRECT_OFF;
          act.defaultLabel = "Fire Suppression";
          act.defaultDesc = "Off";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
      }
      if (this.shieldInterceptDropPod)
      {
        if (this.flag_InterceptDropPod)
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchInterceptDropPod());
          act.icon = Building_Shield.UI_INTERCEPT_DROPPOD_ON;
          act.defaultLabel = "Intercept DropPod";
          act.defaultDesc = "On";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
        else
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchInterceptDropPod());
          act.icon = Building_Shield.UI_INTERCEPT_DROPPOD_OFF;
          act.defaultLabel = "Intercept DropPod";
          act.defaultDesc = "Off";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
      }
      if (this.shieldStructuralIntegrityMode)
      {
        if (this.flag_shieldRepairMode)
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchShieldRepairMode());
          act.icon = Building_Shield.UI_REPAIR_ON;
          act.defaultLabel = "Repair Mode";
          act.defaultDesc = "On";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
        else
        {
          Command_Action act = new Command_Action();
          act.action = (Action) (() => this.SwitchShieldRepairMode());
          act.icon = Building_Shield.UI_REPAIR_OFF;
          act.defaultLabel = "Repair Mode";
          act.defaultDesc = "Off";
          act.activateSound = SoundDef.Named("Click");
          yield return (Gizmo) act;
        }
      }
      if (this.flag_showVisually)
      {
        Command_Action act = new Command_Action();
        act.action = (Action) (() => this.SwitchVisual());
        act.icon = Building_Shield.UI_SHOW_ON;
        act.defaultLabel = "Show Visually";
        act.defaultDesc = "Show";
        act.activateSound = SoundDef.Named("Click");
        yield return (Gizmo) act;
      }
      else
      {
        Command_Action act = new Command_Action();
        act.action = (Action) (() => this.SwitchVisual());
        act.icon = Building_Shield.UI_SHOW_OFF;
        act.defaultLabel = "Show Visually";
        act.defaultDesc = "Hide";
        act.activateSound = SoundDef.Named("Click");
        yield return (Gizmo) act;
      }
    }

    private void SwitchDirect()
    {
      this.flag_direct = !this.flag_direct;
    }

    private void SwitchIndirect()
    {
      this.flag_indirect = !this.flag_indirect;
    }

    private void SwitchFire()
    {
      this.flag_fireSupression = !this.flag_fireSupression;
    }

    private void SwitchInterceptDropPod()
    {
      this.flag_InterceptDropPod = !this.flag_InterceptDropPod;
    }

    private void SwitchVisual()
    {
      this.flag_showVisually = !this.flag_showVisually;
    }

    private void SwitchShieldRepairMode()
    {
      this.flag_shieldRepairMode = !this.flag_shieldRepairMode;
    }
  }
}
