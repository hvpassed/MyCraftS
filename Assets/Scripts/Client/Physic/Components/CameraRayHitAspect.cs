using MyCraftS.UI.DebugUI;
using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MyCraftS.Physic
{
    readonly partial struct CameraRayHitAspect:IAspect
    {
        readonly RefRO<CameraRayHitInfo> hitInfo;
        readonly RefRO<CameraRayHitType> hitType;
        readonly EnabledRefRO<CameraRayIsHit> isHit;


        public int3 GetPlacePos()
        {
            return (int3)math.floor(hitInfo.ValueRO.blockPosition) + RayHitHelper.DeltaPos(hitInfo.ValueRO.hitSide);

        }
    }
}
