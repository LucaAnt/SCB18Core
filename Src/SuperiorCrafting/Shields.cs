/*
 * Created by SharpDevelop.
 * User: pesky
 * Date: 13/01/2018
 * Time: 13:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using Enhanced_Defence.ShieldUtils;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Enhanced_Defence.Shields
{
	 public enum enumShieldStatus
  {
    Disabled,
    Loading,
    Charging,
    Sustaining,
  }
	internal class ShieldBlendingParticle
  {
    private static readonly Material ShieldSparksMat = MaterialPool.MatFrom("Things/ShieldSparks", Object.op_Implicit((Object) MatBases.LightOverlay));
    private float currentAngle = Random.Range(0.0f, 360f);
    private int transitionDirection = 1;
    private int transitionStep = Random.Range(1, 1);
    public const int transitionMax = 80;
    private int transitionStatus;
    private Vector3 drawPosition;

    public int currentStep
    {
      get
      {
        return this.transitionStatus;
      }
    }

    public int currentDir
    {
      get
      {
        return this.transitionDirection;
      }
    }

    public ShieldBlendingParticle(Vector3 pos)
    {
      this.drawPosition = pos;
    }

    public ShieldBlendingParticle(Vector3 pos, int step)
    {
      this.drawPosition = pos;
      this.transitionStatus = Math.Max(Math.Min(80, step), 0);
    }

    public void DrawMe()
    {
      this.DrawMe(this.drawPosition);
    }

    public void DrawMe(Vector3 location)
    {
      this.doTransitionStep();
      Matrix4x4 matrix4x4 = (Matrix4x4) null;
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).SetTRS(Vector3.op_Addition(location, (Vector3) Altitudes.AltIncVect), Quaternion.Euler(0.0f, this.currentAngle, 0.0f), Vector3.get_one());
      Graphics.DrawMesh((Mesh) MeshPool.plane20, matrix4x4, FadedMaterialPool.FadedVersionOf(ShieldBlendingParticle.ShieldSparksMat, (float) (0.200000002980232 + (double) this.transitionStatus / 80.0 * 0.699999988079071)), 0);
    }

    private void doTransitionStep()
    {
      this.transitionStatus += this.transitionStep * this.transitionDirection;
      if (this.transitionStatus >= 80)
      {
        this.transitionDirection = -1;
        this.transitionStatus = 80;
      }
      else
      {
        if (this.transitionStatus > 0)
          return;
        this.currentAngle = Random.Range(0.0f, 360f);
        this.transitionDirection = 1;
        this.transitionStatus = 0;
      }
    }
  }
	public class Building_Shield : Building
  {
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
    public bool flag_direct;
    public bool flag_indirect;
    public bool flag_fireSupression;
    public bool flag_shieldRepairMode;
    public bool flag_showVisually;
    public bool flag_InterceptDropPod;
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
    private ShieldBlendingParticle[] sparksParticle;

    public Building_Shield()
    {
      //base.\u002Ector();

    }
    public virtual void PostMake()
    {
      ((ThingWithComps) this).PostMake();
    }

    public virtual void SpawnSetup()
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
      base.SpawnSetup();
      this.power = (CompPowerTrader) ((ThingWithComps) this).GetComp<CompPowerTrader>();
      if (((Thing) this).def is ShieldBuildingThingDef)
      {
        this.shieldMaxShieldStrength = ((ShieldBuildingThingDef) ((Thing) this).def).shieldMaxShieldStrength;
        this.shieldInitialShieldStrength = ((ShieldBuildingThingDef) ((Thing) this).def).shieldInitialShieldStrength;
        this.shieldShieldRadius = ((ShieldBuildingThingDef) ((Thing) this).def).shieldShieldRadius;
        this.shieldPowerRequiredCharging = ((ShieldBuildingThingDef) ((Thing) this).def).shieldPowerRequiredCharging;
        this.shieldPowerRequiredSustaining = ((ShieldBuildingThingDef) ((Thing) this).def).shieldPowerRequiredSustaining;
        this.shieldRechargeTickDelay = ((ShieldBuildingThingDef) ((Thing) this).def).shieldRechargeTickDelay;
        this.shieldRecoverWarmup = ((ShieldBuildingThingDef) ((Thing) this).def).shieldRecoverWarmup;
        this.shieldBlockIndirect = ((ShieldBuildingThingDef) ((Thing) this).def).shieldBlockIndirect;
        this.shieldBlockDirect = ((ShieldBuildingThingDef) ((Thing) this).def).shieldBlockDirect;
        this.shieldFireSupression = ((ShieldBuildingThingDef) ((Thing) this).def).shieldFireSupression;
        this.shieldInterceptDropPod = ((ShieldBuildingThingDef) ((Thing) this).def).shieldInterceptDropPod;
        this.shieldStructuralIntegrityMode = ((ShieldBuildingThingDef) ((Thing) this).def).shieldStructuralIntegrityMode;
        this.colourRed = ((ShieldBuildingThingDef) ((Thing) this).def).colourRed;
        this.colourGreen = ((ShieldBuildingThingDef) ((Thing) this).def).colourGreen;
        this.colourBlue = ((ShieldBuildingThingDef) ((Thing) this).def).colourBlue;
        this.SIFBuildings = ((ShieldBuildingThingDef) ((Thing) this).def).SIFBuildings;
      }
      else
        Log.Error("Shield definition not of type \"ShieldBuildingThingDef\"");
      if (this.shieldField != null)
        return;
      this.shieldField = new ShieldField(this, ((Thing) this).get_Position(), this.shieldMaxShieldStrength, this.shieldInitialShieldStrength, this.shieldShieldRadius, this.shieldRechargeTickDelay, this.shieldRecoverWarmup, this.shieldBlockIndirect, this.shieldBlockDirect, this.shieldFireSupression, this.shieldInterceptDropPod, this.shieldStructuralIntegrityMode, this.colourRed, this.colourGreen, this.colourBlue, this.SIFBuildings);
    }

    public virtual void Tick()
    {
      ((ThingWithComps) this).Tick();
      if (this.shieldField != null)
      {
        if (this.power != null)
          this.shieldField.enabled = this.power.get_PowerOn();
        this.shieldField.ShieldTick(this.flag_direct, this.flag_indirect, this.flag_fireSupression, this.flag_InterceptDropPod, this.flag_shieldRepairMode);
        switch (this.shieldField.status)
        {
          case enumShieldStatus.Disabled:
          case enumShieldStatus.Loading:
            this.power.powerOutputInt = (__Null) (double) this.shieldPowerRequiredCharging;
            break;
          case enumShieldStatus.Charging:
            this.power.powerOutputInt = (__Null) (double) this.shieldPowerRequiredCharging;
            break;
          case enumShieldStatus.Sustaining:
            this.power.powerOutputInt = (__Null) (double) this.shieldPowerRequiredSustaining;
            break;
        }
      }
      else
        this.shieldField = new ShieldField(this, ((Thing) this).get_Position(), this.shieldMaxShieldStrength, this.shieldInitialShieldStrength, this.shieldShieldRadius, this.shieldRechargeTickDelay, this.shieldRecoverWarmup, this.shieldBlockIndirect, this.shieldBlockDirect, this.shieldFireSupression, this.shieldInterceptDropPod, this.shieldStructuralIntegrityMode, this.colourRed, this.colourGreen, this.colourBlue, this.SIFBuildings);
    }

    public void DrawShieldField()
    {
      if (!this.shieldField.isOnline() && this.shieldField.shieldRecoverWarmup - this.shieldField.warmupPower >= 60)
        return;
      this.shieldField.DrawField(Vectors.IntVecToVec(((Thing) this).get_Position()));
      if (this.sparksParticle[0] == null)
      {
        for (int index = 0; index < this.sparksParticle.Length; ++index)
          this.sparksParticle[index] = new ShieldBlendingParticle(((Thing) this).get_DrawPos(), (int) Math.Round((double) index / (double) (this.sparksParticle.Length - 1) * 80.0));
      }
      int index1 = 0;
      for (int length = this.sparksParticle.Length; index1 < length; ++index1)
        this.sparksParticle[index1].DrawMe();
    }

    public virtual void Draw()
    {
      base.Draw();
      if (!this.flag_showVisually)
        return;
      this.DrawShieldField();
    }

    public virtual void DrawExtraSelectionOverlays()
    {
      GenDraw.DrawRadiusRing(((Thing) this).get_Position(), (float) this.shieldField.shieldShieldRadius);
    }

    public virtual string GetInspectString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.shieldField.GetInspectString());
      if (this.power != null)
      {
        string str = ((ThingComp) this.power).CompInspectStringExtra();
        if (!GenText.NullOrEmpty(str))
          stringBuilder.AppendLine(str);
      }
      return stringBuilder.ToString();
    }

    public virtual void ExposeData()
    {
      ((ThingWithComps) this).ExposeData();
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Deep.LookDeep<ShieldField>((M0&) @this.shieldField, "shieldField", new object[0]);
      this.shieldField.position = ((Thing) this).get_Position();
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.flag_direct, "flag_direct", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.flag_indirect, "flag_indirect", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.flag_fireSupression, "flag_fireSupression", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.flag_InterceptDropPod, "flag_InterceptDropPod", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.flag_shieldRepairMode, "flag_shieldRepairMode", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.flag_showVisually, "flag_showVisually", (M0) 0, false);
    }

    public virtual IEnumerable<Gizmo> GetGizmos()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerable<Gizmo>) new Building_Shield.\u003CGetGizmos\u003Ed__18(-2)
      {
        \u003C\u003E4__this = this
      };
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
	public class ShieldBuildingThingDef : ThingDef
  {
    public int shieldMaxShieldStrength;
    public int shieldInitialShieldStrength;
    public int shieldShieldRadius;
    public int shieldPowerRequiredCharging;
    public int shieldPowerRequiredSustaining;
    public int shieldRechargeTickDelay;
    public int shieldRecoverWarmup;
    public bool shieldBlockIndirect;
    public bool shieldBlockDirect;
    public bool shieldFireSupression;
    public bool shieldInterceptDropPod;
    public bool shieldStructuralIntegrityMode;
    public float colourRed;
    public float colourGreen;
    public float colourBlue;
    public List<string> SIFBuildings;

    public ShieldBuildingThingDef()
    {
      base.\u002Ector();
    }
  }
	internal class ShieldField : IExposable
  {
    private static readonly SoundDef HitSoundDef = SoundDef.Named("Shields_HitShield");
    private List<IntVec3> squares = new List<IntVec3>();
    private bool enabled_internal = true;
    private const int DAMAGE_TO_FIRE = 10;
    private const int DAMAGE_FROM_FIRE = 2;
    private const int FIRE_SUPRESSION_TICK_DELAY = 15;
    private const float powerToDamage = 1f;
    public IntVec3 position;
    public int shieldMaxShieldStrength;
    private int shieldInitialShieldStrength;
    public int shieldShieldRadius;
    public int shieldRechargeTickDelay;
    public int shieldRecoverWarmup;
    public bool shieldBlockIndirect;
    public bool shieldBlockDirect;
    public bool shieldFireSupression;
    public bool shieldInterceptDropPod;
    public bool shieldStructuralIntegrityMode;
    public Building_Shield shieldBuilding;
    public float colourRed;
    public float colourGreen;
    public float colourBlue;
    private int shieldCurrentStrength_base;
    private long tick;
    private bool online;
    private int warmupTicksCurrent;
    private Material currentMatrialColour;

    public int shieldCurrentStrength
    {
      get
      {
        return this.shieldCurrentStrength_base;
      }
      set
      {
        if (value == this.shieldCurrentStrength_base)
          return;
        this.shieldCurrentStrength_base = Math.Max(0, value);
        if (value > 0)
          return;
        this.online = false;
      }
    }

    public int warmupPower
    {
      get
      {
        if (this.status == enumShieldStatus.Loading)
          return this.warmupTicksCurrent;
        return 0;
      }
    }

    public enumShieldStatus status
    {
      get
      {
        if (!this.enabled)
          return enumShieldStatus.Disabled;
        if (!this.online)
          return enumShieldStatus.Loading;
        return this.online && this.shieldCurrentStrength < this.shieldMaxShieldStrength ? enumShieldStatus.Charging : enumShieldStatus.Sustaining;
      }
    }

    public bool enabled
    {
      get
      {
        return this.enabled_internal;
      }
      set
      {
        if (value == this.enabled_internal)
          return;
        this.enabled_internal = value;
        if (value)
        {
          this.warmupTicksCurrent = 0;
        }
        else
        {
          this.online = false;
          this.shieldCurrentStrength = 0;
        }
      }
    }

    public ShieldField(Building_Shield shieldBuilding, IntVec3 pos, int shieldMaxShieldStrength, int shieldInitialShieldStrength, int shieldShieldRadius, int shieldRechargeTickDelay, int shieldRecoverWarmup, bool shieldBlockIndirect, bool shieldBlockDirect, bool shieldFireSupression, bool shieldInterceptDropPod, bool shieldStructuralIntegrityMode, float colourRed, float colourGreen, float colourBlue, List<string> SIFBuildings)
    {
      this.shieldBuilding = shieldBuilding;
      this.position = pos;
      this.shieldMaxShieldStrength = shieldMaxShieldStrength;
      this.shieldInitialShieldStrength = shieldInitialShieldStrength;
      this.shieldShieldRadius = shieldShieldRadius;
      this.shieldRechargeTickDelay = shieldRechargeTickDelay;
      this.shieldRecoverWarmup = shieldRecoverWarmup;
      this.shieldBlockIndirect = shieldBlockIndirect;
      this.shieldBlockDirect = shieldBlockDirect;
      this.shieldFireSupression = shieldFireSupression;
      this.shieldInterceptDropPod = shieldInterceptDropPod;
      this.shieldStructuralIntegrityMode = shieldStructuralIntegrityMode;
      this.colourRed = colourRed;
      this.colourGreen = colourGreen;
      this.colourBlue = colourBlue;
    }

    public ShieldField()
    {
    }

    private void StartupField(int desiredPower)
    {
      this.online = true;
      this.warmupTicksCurrent = 0;
      this.shieldCurrentStrength = desiredPower;
    }

    public void ShieldTick(bool flag_direct, bool flag_indirect, bool flag_fireSupression, bool flag_InterceptDropPod, bool shieldRepairMode)
    {
      if (!this.enabled)
        return;
      ++this.tick;
      if (this.online && this.shieldCurrentStrength > 0)
      {
        if (this.shieldStructuralIntegrityMode)
        {
          this.SetupCurrentSquares();
          using (List<IntVec3>.Enumerator enumerator = this.squares.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVec3 current = enumerator.Current;
              this.ProtectSquare(current, flag_direct, flag_indirect, true);
              this.supressFire(flag_fireSupression, current);
              this.repairSytem(current, shieldRepairMode);
              if (this.shieldCurrentStrength <= 0)
                break;
            }
          }
        }
        else
        {
          using (IEnumerator<IntVec3> enumerator = GenRadial.RadialCellsAround(new IntVec3(0, 0, 0), (float) this.shieldShieldRadius, false).GetEnumerator())
          {
            while (((IEnumerator) enumerator).MoveNext())
            {
              IntVec3 current = enumerator.Current;
              if (Vectors.VectorSize(current) >= (double) this.shieldShieldRadius - 1.5)
                this.ProtectSquare(IntVec3.op_Addition(current, this.position), flag_direct, flag_indirect, false);
              if (this.shieldCurrentStrength <= 0)
                break;
            }
          }
          this.supressFire(flag_fireSupression);
          this.interceptPods(flag_InterceptDropPod);
        }
      }
      if (this.online && (this.tick % (long) this.shieldRechargeTickDelay == 0L || DebugSettings.unlimitedPower != null) && this.shieldCurrentStrength < this.shieldMaxShieldStrength)
      {
        ++this.shieldCurrentStrength;
      }
      else
      {
        if (this.online)
          return;
        if (this.warmupTicksCurrent < this.shieldRecoverWarmup)
        {
          ++this.warmupTicksCurrent;
          if (DebugSettings.unlimitedPower == null)
            return;
          this.warmupTicksCurrent += 5;
        }
        else
          this.StartupField(this.shieldInitialShieldStrength);
      }
    }

    private void ProtectSquare(IntVec3 square, bool flag_direct, bool flag_indirect, bool IFFcheck)
    {
      if (!GenGrid.InBounds(square) || (!this.shieldBlockIndirect || !flag_indirect) && (!this.shieldBlockDirect || !flag_direct))
        return;
      List<Thing> thingList1 = Find.get_ThingGrid().ThingsListAt(square);
      List<Thing> thingList2 = new List<Thing>();
      int index1 = 0;
      for (int index2 = ((IEnumerable<Thing>) thingList1).Count<Thing>(); index1 < index2; ++index1)
      {
        if (thingList1[index1] != null && thingList1[index1] is Projectile)
        {
          Projectile projectile = (Projectile) thingList1[index1];
          if (!((Thing) projectile).get_Destroyed() && (this.shieldBlockIndirect && flag_indirect && ((ProjectileProperties) ((ThingDef) ((Thing) projectile).def).projectile).flyOverhead != null || this.shieldBlockDirect && flag_direct && ((ProjectileProperties) ((ThingDef) ((Thing) projectile).def).projectile).flyOverhead == null))
          {
            bool flag = true;
            if (IFFcheck)
            {
              Thing instanceField = ReflectionHelper.GetInstanceField(typeof (Projectile), (object) projectile, "launcher") as Thing;
              if (instanceField != null && instanceField.get_Faction() != null && (instanceField.get_Faction().def != null && instanceField.get_Faction().def == FactionDefOf.Colony))
                flag = false;
            }
            if (((ProjectileProperties) ((ThingDef) ((Thing) projectile).def).projectile).flyOverhead != null && !this.WillTargetLandInRange(projectile))
              flag = false;
            if (flag)
            {
              Quaternion exactRotation = projectile.get_ExactRotation();
              Vector3 exactPosition = projectile.get_ExactPosition();
              exactPosition.y = (__Null) 0.0;
              Vector3 vec = Vectors.IntVecToVec(this.position);
              vec.y = (__Null) 0.0;
              Quaternion quaternion = Quaternion.LookRotation(Vector3.op_Subtraction(exactPosition, vec));
              if ((double) Quaternion.Angle(exactRotation, quaternion) > 90.0 || IFFcheck)
              {
                MoteThrower.ThrowLightningGlow(projectile.get_ExactPosition(), 0.5f);
                SoundStarter.PlayOneShot(ShieldField.HitSoundDef, SoundInfo.op_Implicit(((Thing) projectile).get_Position()));
                this.ProcessDamage((int) ((ProjectileProperties) ((ThingDef) ((Thing) projectile).def).projectile).damageAmountBase);
                thingList2.Add((Thing) projectile);
                if (!this.isOnline())
                  break;
              }
            }
          }
        }
      }
      using (List<Thing>.Enumerator enumerator = thingList2.GetEnumerator())
      {
        while (enumerator.MoveNext())
          enumerator.Current.Destroy((DestroyMode) 0);
      }
    }

    private void interceptPods(bool flag_InterceptDropPod)
    {
      if (!this.shieldInterceptDropPod || !flag_InterceptDropPod)
        return;
      IEnumerable<Thing> source = (IEnumerable<Thing>) Find.get_ListerThings().ThingsOfDef((ThingDef) ThingDefOf.DropPod);
      if (source == null)
        return;
      using (List<Thing>.Enumerator enumerator = source.Where<Thing>((Func<Thing, bool>) (t =>
      {
        IntVec3 position = t.get_Position();
        // ISSUE: explicit reference operation
        return ((IntVec3) @position).InHorDistOf(this.position, (float) this.shieldShieldRadius);
      })).ToList<Thing>().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          DropPod current = (DropPod) enumerator.Current;
          ((Thing) current).Destroy((DestroyMode) 0);
          BodyPartDamageInfo bodyPartDamageInfo = new BodyPartDamageInfo(new BodyPartHeight?(), new BodyPartDepth?((BodyPartDepth) 2));
          ExplosionInfo explosionInfo = (ExplosionInfo) null;
          explosionInfo.center = (__Null) ((Thing) current).get_Position();
          explosionInfo.radius = (__Null) 1.0;
          explosionInfo.dinfo = (__Null) new DamageInfo((DamageDef) DamageDefOf.Flame, 10, (Thing) current, new BodyPartDamageInfo?(), (ThingDef) null);
          // ISSUE: explicit reference operation
          ((ExplosionInfo) @explosionInfo).DoExplosion();
        }
      }
    }

    private void supressFire(bool flag_fireSupression)
    {
      if (!this.shieldFireSupression || !flag_fireSupression || this.tick % 15L != 0L)
        return;
      IEnumerable<Thing> source1 = (IEnumerable<Thing>) Find.get_ListerThings().ThingsOfDef((ThingDef) ThingDefOf.Fire);
      if (source1 == null)
        return;
      IEnumerable<Thing> source2 = source1.Where<Thing>((Func<Thing, bool>) (t =>
      {
        IntVec3 position = t.get_Position();
        // ISSUE: explicit reference operation
        return ((IntVec3) @position).InHorDistOf(this.position, (float) this.shieldShieldRadius);
      }));
      if (source2 == null)
        return;
      using (List<Thing>.Enumerator enumerator = source2.ToList<Thing>().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Fire current = (Fire) enumerator.Current;
          if (this.shieldCurrentStrength > 2)
          {
            this.ProcessDamage(2);
            ((Thing) current).TakeDamage(new DamageInfo((DamageDef) DamageDefOf.Extinguish, 10, (Thing) this.shieldBuilding, new BodyPartDamageInfo?(), (ThingDef) null));
          }
        }
      }
    }

    private void supressFire(bool flag_fireSupression, IntVec3 position)
    {
      if (!this.shieldFireSupression || !flag_fireSupression || this.tick % 15L != 0L)
        return;
      IEnumerable<Thing> things = Find.get_ThingGrid().ThingsAt(position);
      List<Thing> thingList = new List<Thing>();
      using (IEnumerator<Thing> enumerator = things.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          Thing current = enumerator.Current;
          if (current is Fire)
            thingList.Add(current);
        }
      }
      using (List<Thing>.Enumerator enumerator = ((IEnumerable<Thing>) thingList).ToList<Thing>().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Fire current = (Fire) enumerator.Current;
          if (this.shieldCurrentStrength > 2)
          {
            this.ProcessDamage(2);
            ((Thing) current).TakeDamage(new DamageInfo((DamageDef) DamageDefOf.Extinguish, 10, (Thing) this.shieldBuilding, new BodyPartDamageInfo?(), (ThingDef) null));
          }
        }
      }
    }

    private void repairSytem(IntVec3 position, bool shieldRepairMode)
    {
      if (!shieldRepairMode)
        return;
      using (List<Thing>.Enumerator enumerator = Find.get_ThingGrid().ThingsListAt(position).GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Thing current = enumerator.Current;
          if (current is Building && current.get_HitPoints() < current.get_MaxHitPoints() && this.shieldCurrentStrength > 1)
          {
            this.ProcessDamage(1);
            Thing thing = current;
            thing.set_HitPoints(thing.get_HitPoints() + 1);
          }
        }
      }
    }

    private void SetupCurrentSquares()
    {
      this.squares.Clear();
      IEnumerable<IntVec3> collection = GenRadial.RadialCellsAround(this.position, (float) this.shieldShieldRadius, true);
      if (this.shieldStructuralIntegrityMode)
      {
        using (IEnumerator<IntVec3> enumerator = collection.GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            IntVec3 current = enumerator.Current;
            if (Vectors.VectorSize(current) >= (double) this.shieldShieldRadius - 1.5)
            {
              List<Thing> thingList = Find.get_ThingGrid().ThingsListAt(current);
              int index1 = 0;
              for (int index2 = ((IEnumerable<Thing>) thingList).Count<Thing>(); index1 < index2; ++index1)
              {
                if (thingList[index1] is Building && this.isBuildingValid(thingList[index1]))
                {
                  this.squares.Add(current);
                  index1 = 99999;
                }
              }
            }
          }
        }
      }
      else
        this.squares.AddRange(collection);
    }

    public bool isBuildingValid(Thing currentBuilding)
    {
      if (this.shieldBuilding.SIFBuildings != null)
      {
        if (this.shieldBuilding.SIFBuildings.Contains((string) ((Def) currentBuilding.def).defName))
          return true;
      }
      else
        Log.Error("ShieldField.validBuildings Not set up properly");
      return false;
    }

    public void DrawField(Vector3 center)
    {
      if (this.status != enumShieldStatus.Charging && this.status != enumShieldStatus.Sustaining && (this.status != enumShieldStatus.Loading || this.shieldRecoverWarmup - this.warmupTicksCurrent >= 60))
        return;
      if (this.shieldStructuralIntegrityMode)
      {
        using (List<IntVec3>.Enumerator enumerator = this.squares.GetEnumerator())
        {
          while (enumerator.MoveNext())
            this.DrawSubField(Vectors.IntVecToVec(enumerator.Current), 0.8f);
        }
      }
      else
        this.DrawSubField(center, (float) this.shieldShieldRadius);
    }

    public void DrawSubField(Vector3 position, float shieldShieldRadius)
    {
      position = Vector3.op_Addition(position, new Vector3(0.5f, 0.0f, 0.5f));
      Vector3 vector3;
      // ISSUE: explicit reference operation
      ((Vector3) @vector3).\u002Ector(shieldShieldRadius, 1f, shieldShieldRadius);
      Matrix4x4 matrix4x4 = (Matrix4x4) null;
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).SetTRS(position, Quaternion.get_identity(), vector3);
      if (Object.op_Equality((Object) this.currentMatrialColour, (Object) null))
        this.currentMatrialColour = SolidColorMaterials.NewSolidColorMaterial(new Color(this.colourRed, this.colourGreen, this.colourBlue, 0.15f), (Shader) ShaderDatabase.MetaOverlay);
      Graphics.DrawMesh(Graphics.CircleMesh, matrix4x4, this.currentMatrialColour, 0);
    }

    public string GetInspectString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.isOnline())
        stringBuilder.AppendLine("Shield: " + (object) this.shieldCurrentStrength + "/" + (object) this.shieldMaxShieldStrength);
      else if (this.enabled)
        stringBuilder.AppendLine("Ready in " + (object) Math.Round((double) GenTime.TicksToSeconds(this.shieldRecoverWarmup - this.warmupTicksCurrent)) + " seconds.");
      else
        stringBuilder.AppendLine("Shield disabled!");
      return stringBuilder.ToString();
    }

    public bool isOnline()
    {
      if (this.online)
        return this.shieldCurrentStrength > 0;
      return false;
    }

    public void ExposeData()
    {
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.shieldMaxShieldStrength, "shieldMaxShieldStrength", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.shieldInitialShieldStrength, "shieldInitialShieldStrength", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.shieldShieldRadius, "shieldShieldRadius", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.shieldRechargeTickDelay, "shieldRechargeTickDelay", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.shieldRecoverWarmup, "shieldRecoverWarmup", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.shieldBlockIndirect, "shieldBlockIndirect", (M0) 1, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.shieldBlockDirect, "shieldBlockDirect", (M0) 1, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.shieldFireSupression, "shieldFireSupression", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.shieldStructuralIntegrityMode, "shieldStructuralIntegrityMode", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.shieldCurrentStrength_base, "shieldCurrentStrength_base", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<long>((M0&) @this.tick, "tick", (M0) 0L, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<bool>((M0&) @this.online, "online", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.warmupTicksCurrent, "warmupTicksCurrent", (M0) 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<float>((M0&) @this.colourRed, "colourRed", (M0) 0.0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<float>((M0&) @this.colourGreen, "colourGreen", (M0) 0.0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<float>((M0&) @this.colourBlue, "colourBlue", (M0) 0.0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_References.LookReference<Building_Shield>((M0&) @this.shieldBuilding, "shieldBuilding");
    }

    private void ProcessDamage(int damage)
    {
      if (this.shieldCurrentStrength <= 0)
        return;
      this.shieldCurrentStrength -= (int) ((double) damage * 1.0);
    }

    public bool WillTargetLandInRange(Projectile projectile)
    {
      // ISSUE: explicit reference operation
      return (double) Vector3.Distance(((IntVec3) this.position).ToVector3(), this.GetTargetLocationFromProjectile(projectile)) <= (double) this.shieldShieldRadius;
    }

    public Vector3 GetTargetLocationFromProjectile(Projectile projectile)
    {
      return (Vector3) ((object) projectile).GetType().GetField("destination", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue((object) projectile);
    }
  }
}
	 