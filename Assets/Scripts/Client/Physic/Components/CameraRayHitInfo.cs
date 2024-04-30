using System;
using LibNoise.Operator;
using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Physic
{
    public enum HitSide
    {
        None,
        NegativeX,
        PositiveX,
        NegativeY,
        PositiveY,
        NegativeZ,
        PositiveZ
    }
    
    public struct CameraRayHitInfo:IComponentData
    {
        public HitSide hitSide;
        public int blockId;
        public float3 blockPosition;
    }


    public static class RayHitHelper
    {

        /// <summary>
        /// positive相等，negative差一
        /// </summary>
        /// <param name="hitPos"></param>
        /// <param name="hitblockPos"></param>
        /// <returns></returns>
        public static HitSide CheckHitSide(float3 hitPos, float3 hitblockPos)
        {
            float3 deltaPos =math.abs( math.floor(hitPos - hitblockPos));

            if (hitPos.x == hitblockPos.x)
            {
                return HitSide.NegativeX;
            }
            else if (hitPos.y == hitblockPos.y)
            {
                return HitSide.NegativeY;
            }
            else if(hitPos.z == hitblockPos.z)
            {
                return HitSide.NegativeZ;
            }

            if (deltaPos.x > 0)
            {
                return HitSide.PositiveX;
            }
            else if (deltaPos.y > 0)
            {
                return HitSide.PositiveY;
            }
            else if(deltaPos.z > 0)
            {
                return HitSide.PositiveZ;
            }


            return HitSide.None;
        }
    }
}