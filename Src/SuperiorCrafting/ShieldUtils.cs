using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Enhanced_Defence.ShieldUtils
{

	public class ShieldUtils
	{
		public static class Graphics
  {
    private static Mesh CircleMesh_cache;

    public static Mesh CircleMesh
    {
      get
      {
        if (Object.op_Inequality((Object) Graphics.CircleMesh_cache, (Object) null))
          return Graphics.CircleMesh_cache;
        double num1 = 1.0;
        List<Vector2> vector2List1 = new List<Vector2>();
        vector2List1.Add(new Vector2(0.0f, 0.0f));
        int num2 = 0;
        while (num2 <= 360)
        {
          double num3 = (double) num2 / 180.0 * Math.PI;
          List<Vector2> vector2List2 = vector2List1;
          Vector2 vector2_1;
          ((Vector2) @vector2_1).\u002Ector(0.0f, 0.0f);
          vector2_1.x = (__Null) (num1 * Math.Cos(num3));
          vector2_1.y = (__Null) (num1 * Math.Sin(num3));
          Vector2 vector2_2 = vector2_1;
          vector2List2.Add(vector2_2);
          num2 += 4;
        }
        Vector3[] vector3Array = new Vector3[vector2List1.Count];
        for (int index = 0; index < vector3Array.Length; ++index)
          vector3Array[index] = new Vector3((float) vector2List1[index].x, 0.0f, (float) vector2List1[index].y);
        int[] numArray = new Triangulator(vector2List1.ToArray()).Triangulate();
        Graphics.CircleMesh_cache = new Mesh();
        Graphics.CircleMesh_cache.set_vertices(vector3Array);
        Graphics.CircleMesh_cache.set_uv(new Vector2[vector2List1.Count]);
        Graphics.CircleMesh_cache.set_triangles(numArray);
        Graphics.CircleMesh_cache.RecalculateNormals();
        Graphics.CircleMesh_cache.RecalculateBounds();
        return Graphics.CircleMesh_cache;
      }
    }
  }
	}
	internal class ReflectionHelper
  	{
	    internal static object GetInstanceField(Type type, object instance, string fieldName)
	    {
	      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	      return type.GetField(fieldName, bindingAttr).GetValue(instance);
	    }
  	}
	internal class Vectors
	  {
	    public static double EuclDist(IntVec3 a, IntVec3 b)
	    {
	      return Math.Sqrt((double) ((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z)));
	    }
	
	    public static double VectorSize(IntVec3 a)
	    {
	      return Math.Sqrt((double) (a.x * a.x + a.y * a.y + a.z * a.z));
	    }
	
	    public static IntVec3 vecFromAngle(float angle1, float angle2, float r)
	    {
	      IntVec3 intVec3;
	      // ISSUE: explicit reference operation
	      ((IntVec3) @intVec3).\u002Ector((int) ((double) r * Math.Sin((double) angle1) * Math.Cos((double) angle2)), (int) ((double) r * Math.Sin((double) angle1) * Math.Sin((double) angle2)), (int) ((double) r * Math.Cos((double) angle1)));
	      return intVec3;
	    }
	
	    public static double vectorAngleA(IntVec3 a)
	    {
	      double num = Vectors.VectorSize(a);
	      return Math.Acos((double) a.z / num);
	    }
	
	    public static IntVec3 randomDirection(float r)
	    {
	      return Vectors.vecFromAngle((float) Random.Range(0, 360), 0.0f, r);
	    }
	
	    public static Vector3 IntVecToVec(IntVec3 from)
	    {
	      return new Vector3((float) from.x, (float) from.y, (float) from.z);
	    }
	
	    public static IntVec3 VecToIntVec(Vector3 from)
	    {
	      return new IntVec3((int) from.x, (int) from.y, (int) from.z);
	    }
	  }
}
