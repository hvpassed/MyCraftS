 
using MyCraftS.Utils;
using Unity.Entities;

namespace MyCraftS.Initializer
{
    public partial class NoiseInitializeSystem:SystemBase
    {
        protected override void OnCreate()
        {
            PerlinNoise2D.InitPerlin();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }
    }
}