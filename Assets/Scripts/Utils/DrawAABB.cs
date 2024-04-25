using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Color = System.Drawing.Color;

namespace MyCraftS.Utils
{
    public static class DrawAABB
    {
        public static void draw(Aabb aabb,UnityEngine.Color color,float time = 0f)
        {
            Vector3 leftBB = new Vector3(aabb.Min.x, aabb.Min.y, aabb.Min.z);
            Vector3 rightBB = new Vector3(aabb.Max.x, aabb.Min.y, aabb.Min.z);
            Vector3 leftFB = new Vector3(aabb.Min.x, aabb.Min.y, aabb.Max.z);
            Vector3 rightFB = new Vector3(aabb.Max.x, aabb.Min.y, aabb.Max.z);
            Vector3 leftBT = new Vector3(aabb.Min.x, aabb.Max.y, aabb.Min.z);
            Vector3 rightBT = new Vector3(aabb.Max.x, aabb.Max.y, aabb.Min.z);
            Vector3 leftFT = new Vector3(aabb.Min.x, aabb.Max.y, aabb.Max.z);
            Vector3 rightFT = new Vector3(aabb.Max.x, aabb.Max.y, aabb.Max.z);
            
            Debug.DrawLine(leftBB, rightBB,color,time);
            Debug.DrawLine(leftBB, leftBT, color,time);
            Debug.DrawLine(rightBT, rightBB, color,time);
            Debug.DrawLine(rightBT, leftBT, color,time);
            
            Debug.DrawLine(leftBB, leftFB, color,time);
            Debug.DrawLine(rightBB, rightFB, color,time);
            Debug.DrawLine(leftBT, leftFT, color,time);
            Debug.DrawLine(rightBT, rightFT, color,time);
            
            Debug.DrawLine(leftFB, rightFB, color,time);
            Debug.DrawLine(leftFB, leftFT, color,time);
            Debug.DrawLine(rightFT, rightFB, color,time);
            Debug.DrawLine(rightFT, leftFT, color,time);
        }
        
        public static void MoveAABB(ref Aabb aabb, float3 pos)
        {
            aabb.Max.x += pos.x;
            aabb.Max.y += pos.y;
            aabb.Max.z += pos.z;
            aabb.Min.x += pos.x;
            aabb.Min.y += pos.y;
            aabb.Min.z += pos.z;
        }
    }
}