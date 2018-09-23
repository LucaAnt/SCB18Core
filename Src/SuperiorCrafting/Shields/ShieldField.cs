// Decompiled with JetBrains decompiler
// Type: Enhanced_Defence.Shields.ShieldField
// Assembly: Enhanced_Defence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A381E51-C852-41B2-B251-E7AC4F04E532
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\Enhanced_Defence.dll

using Enhanced_Defence.ShieldUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Enhanced_Defence.Shields
{
  [StaticConstructorOnStartup]
   class ShieldField : IExposable
  {
    
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
    public int shieldCurrentStrength_base;
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
          foreach (IntVec3 square in this.squares)
          {
            this.ProtectSquare(square, flag_direct, flag_indirect, true);
            this.supressFire(flag_fireSupression, square);
            this.repairSytem(square, shieldRepairMode);
            if (this.shieldCurrentStrength <= 0)
              break;
          }
        }
        else
        {
          foreach (IntVec3 a in GenRadial.RadialCellsAround(new IntVec3(0, 0, 0), (float) this.shieldShieldRadius, false))
          {
            if (Vectors.VectorSize(a) >= (double) this.shieldShieldRadius - 1.5)
              this.ProtectSquare(a + this.position, flag_direct, flag_indirect, false);
            if (this.shieldCurrentStrength <= 0)
              break;
          }
          this.supressFire(flag_fireSupression);
          this.interceptPods(flag_InterceptDropPod);
        }
      }
      if (this.online && (this.tick % (long) this.shieldRechargeTickDelay == 0L || DebugSettings.unlimitedPower) && this.shieldCurrentStrength < this.shieldMaxShieldStrength)
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
          if (!DebugSettings.unlimitedPower)
            return;
          this.warmupTicksCurrent += 5;
        }
        else
          this.StartupField(this.shieldInitialShieldStrength);
      }
    }

    private void ProtectSquare(IntVec3 square, bool flag_direct, bool flag_indirect, bool IFFcheck)
    {
      if (!square.InBounds(Find.VisibleMap) || (!this.shieldBlockIndirect || !flag_indirect) && (!this.shieldBlockDirect || !flag_direct))
        return;
      List<Thing> T_GRID = square.GetThingList(Find.VisibleMap);
      
      	IEnumerable<Thing> proiettili = T_GRID.Where<Thing>((Func<Thing,bool>)(p =>p.GetType().BaseType == typeof (Projectile)));
      
      if(!proiettili.Any())
      	return;
      
      List<Projectile> proiettili_da_distruggere = new List<Projectile>();
      Thing[] proiettili_array =proiettili.ToArray();
      foreach(Projectile projectile in proiettili_array)
      {
      	bool flag = true;
            if (IFFcheck)
            {
              Thing instanceField = ReflectionHelper.GetInstanceField(typeof (Projectile), (object) projectile, "launcher") as Thing;
              if (instanceField != null && instanceField.Faction != null && (instanceField.Faction.def != null && instanceField.Faction.def == FactionDefOf.PlayerColony))
                flag = false;
            }
            if (projectile.def.projectile.flyOverhead && !this.WillTargetLandInRange(projectile))
              flag = false;
            if (flag)
            {
              Quaternion exactRotation = projectile.ExactRotation;
              Vector3 exactPosition = projectile.ExactPosition;
              exactPosition.y = 0.0f;
              Vector3 vec = Vectors.IntVecToVec(this.position);
              vec.y = 0.0f;
              Quaternion b = Quaternion.LookRotation(exactPosition - vec);
              if ((double) Quaternion.Angle(exactRotation, b) > 90.0 || IFFcheck)
              {
                MoteMaker.ThrowLightningGlow(projectile.ExactPosition,Find.VisibleMap, 0.5f);
                
                SoundDef HitSoundDef = SoundDef.Named(SoundDefOf.EnergyShieldAbsorbDamage.defName);
                HitSoundDef.PlayOneShot(SoundInfo.InMap(projectile,MaintenanceType.None));
                this.ProcessDamage(projectile.def.projectile.damageAmountBase);
                proiettili_da_distruggere.Add(projectile);
                if (!this.isOnline())
                {
                	SoundDef DangerSoundDef = SoundDef.Named(SoundDefOf.EnergyShieldBroken.defName);
                	DangerSoundDef.PlayOneShot(SoundInfo.InMap(projectile,MaintenanceType.None));
                	break;
                }
              }
            }
      }
      
      foreach (Projectile thing in proiettili_da_distruggere)
        thing.Destroy(DestroyMode.Vanish);
    }

    private void interceptPods(bool flag_InterceptDropPod)
    {
    	List<Thing> threats= new List<Thing>();
    	foreach(Thing T in this.shieldBuilding.Map.listerThings.ThingsOfDef(ThingDefOf.ActiveDropPod)) threats.Add(T);
    		
    	foreach(Thing T in this.shieldBuilding.Map.listerThings.ThingsOfDef(ThingDefOf.CrashedPsychicEmanatorShipPart)) threats.Add(T);
    			
    	foreach(Thing T in this.shieldBuilding.Map.listerThings.ThingsOfDef(ThingDefOf.CrashedPoisonShipPart)) threats.Add(T);
    	
		foreach(Thing T in this.shieldBuilding.Map.listerThings.ThingsOfDef(ThingDefOf.MeteoriteIncoming)) threats.Add(T);
    	
    	
      if (!this.shieldInterceptDropPod || !flag_InterceptDropPod || !threats.Any() || (threats == null))
    		return;

      Thing[] InAreaThreats = threats.Where<Thing>((Func<Thing, bool>) (t => t.Position.InHorDistOf(this.position,  this.shieldShieldRadius))).ToArray();

      foreach (Thing T in InAreaThreats)
      {
      	Log.Message("In area incoming stopped objects: "+InAreaThreats.Count());
        if(T is ActiveDropPod)
        {
        	SoundDef explosionInfoSoundDef = SoundDef.Named("Explosion");
        	GenExplosion.DoExplosion(T.Position,Find.VisibleMap,3.0f,DamageDefOf.Bomb,T,10,explosionInfoSoundDef);
        	this.ProcessDamage(100);
        	T.Destroy(DestroyMode.KillFinalize);
        }else if(T is Building_CrashedShipPart){
        	this.ProcessDamage(1000);
        	SoundDef explosionInfoSoundDef = SoundDef.Named("Explosion");
        	GenExplosion.DoExplosion(T.Position,Find.VisibleMap,10.0f,DamageDefOf.Bomb,T,100,explosionInfoSoundDef);
        	T.Destroy(DestroyMode.KillFinalize);
        }else if(T.def.defName.Equals(ThingDefOf.MeteoriteIncoming.defName)){
        	this.ProcessDamage(1000);
        	SoundDef explosionInfoSoundDef = SoundDef.Named("Explosion");
        	GenExplosion.DoExplosion(T.Position,Find.VisibleMap,10.0f,DamageDefOf.Bomb,T,100,explosionInfoSoundDef);
        	T.Destroy(DestroyMode.KillFinalize);
        }
      } 
    }

    private void supressFire(bool flag_fireSupression)
    {
      if (!this.shieldFireSupression || !flag_fireSupression || this.tick % 15L != 0L || this.shieldBuilding.Map.fireWatcher.FireDanger<=0)
        return;
      IEnumerable<IntVec3> ShieldArea = GenRadial.RadialCellsAround(this.position,shieldShieldRadius,true);
      foreach(IntVec3 AreaTile in ShieldArea)
      	supressFire(true, AreaTile);
  
    }

    private void supressFire(bool flag_fireSupression, IntVec3 AreaTile)
    {
    	Thing[] X= AreaTile.GetThingList(this.shieldBuilding.Map).Where<Thing>((Func<Thing,bool>)(f =>f.IsBurning())).ToArray();
      	foreach (Thing fireTile in X)
      	{
      		if(fireTile is Fire)
      		{
      			if (this.shieldCurrentStrength > 2)
      			{
	      			this.ProcessDamage(2);
	          		fireTile.TakeDamage(new DamageInfo(DamageDefOf.Extinguish, 10,-1f, null, null, (ThingDef) null));
	         	}
      		}
      	}
    }

    private void repairSytem(IntVec3 position, bool shieldRepairMode)
    {
      if (!shieldRepairMode)
        return;
      foreach (Thing thing in Find.VisibleMap.thingGrid.ThingsListAt(position))
      {
        if (thing is Building && thing.HitPoints < thing.MaxHitPoints && this.shieldCurrentStrength > 1)
        {
          this.ProcessDamage(1);
          ++thing.HitPoints;
        }
      }
    }

    private void SetupCurrentSquares()
    {
      this.squares.Clear();
      IEnumerable<IntVec3> collection = GenRadial.RadialCellsAround(this.position, (float) this.shieldShieldRadius, true);
      if (this.shieldStructuralIntegrityMode)
      {
        foreach (IntVec3 intVec3 in collection)
        {
          if (Vectors.VectorSize(intVec3) >= (double) this.shieldShieldRadius - 1.5)
          {
            List<Thing> source = Find.VisibleMap.thingGrid.ThingsListAt(intVec3);
            int index1 = 0;
            for (int index2 = source.Count<Thing>(); index1 < index2; ++index1)
            {
              if (source[index1] is Building && this.isBuildingValid(source[index1]))
              {
                this.squares.Add(intVec3);
                index1 = 99999;
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
        if (this.shieldBuilding.SIFBuildings.Contains(currentBuilding.def.defName))
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
        foreach (IntVec3 square in this.squares)
          this.DrawSubField(Vectors.IntVecToVec(square), 0.8f);
      }
      else
        this.DrawSubField(center, (float) this.shieldShieldRadius);
    }

    public void DrawSubField(Vector3 position, float shieldShieldRadius)
    {
      position += new Vector3(0.5f, 0.0f, 0.5f);
      Vector3 s = new Vector3(shieldShieldRadius, 1f, shieldShieldRadius);
      Matrix4x4 matrix = new Matrix4x4();
      matrix.SetTRS(position, Quaternion.identity, s);
      if ((UnityEngine.Object) this.currentMatrialColour == (UnityEngine.Object) null)
        this.currentMatrialColour = SolidColorMaterials.NewSolidColorMaterial(new Color(this.colourRed, this.colourGreen, this.colourBlue, 0.15f), ShaderDatabase.MetaOverlay);
      UnityEngine.Graphics.DrawMesh(Enhanced_Defence.ShieldUtils.Graphics.CircleMesh, matrix, this.currentMatrialColour, 0);
    }

    public string GetInspectString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.isOnline())
        stringBuilder.AppendLine("Shield: " + (object) this.shieldCurrentStrength + "/" + (object) this.shieldMaxShieldStrength);
      else if (this.enabled)
        stringBuilder.AppendLine("Ready in " + (object) Math.Round((double) GenTicks.TicksToSeconds(this.shieldRecoverWarmup - this.warmupTicksCurrent)) + " seconds.");
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
      Scribe_Values.Look<int>(ref this.shieldMaxShieldStrength, "shieldMaxShieldStrength",  0,false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<int>(ref this.shieldInitialShieldStrength, "shieldInitialShieldStrength", 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<int>(ref this.shieldShieldRadius, "shieldShieldRadius", 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<int>(ref this.shieldRechargeTickDelay, "shieldRechargeTickDelay", 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<int>(ref this.shieldRecoverWarmup, "shieldRecoverWarmup", 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.shieldBlockIndirect, "shieldBlockIndirect", true, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.shieldBlockDirect, "shieldBlockDirect", true, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.shieldFireSupression, "shieldFireSupression", true, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.shieldStructuralIntegrityMode, "shieldStructuralIntegrityMode", false, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<int>(ref this.shieldCurrentStrength_base, "shieldCurrentStrength_base", 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<long>(ref this.tick, "tick", 0L, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<bool>(ref this.online, "online", false, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<int>(ref this.warmupTicksCurrent, "warmupTicksCurrent", 0, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<float>(ref this.colourRed, "colourRed", 0.0f, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<float>(ref this.colourGreen, "colourGreen", 0.0f, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.Look<float>(ref this.colourBlue, "colourBlue", 0.0f, false);
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      
      Scribe_References.Look<Building_Shield>(ref this.shieldBuilding, "shieldBuilding");
    }

    private void ProcessDamage(int damage)
    {
      if (this.shieldCurrentStrength <= 0)
        return;
      this.shieldCurrentStrength -= (int) ((double) damage * 1.0);
    }

    public bool WillTargetLandInRange(Projectile projectile)
    {
      return (double) Vector3.Distance(this.position.ToVector3(), this.GetTargetLocationFromProjectile(projectile)) <= (double) this.shieldShieldRadius;
    }

    public Vector3 GetTargetLocationFromProjectile(Projectile projectile)
    {
      return (Vector3) projectile.GetType().GetField("destination", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue((object) projectile);
    }
  }
}
