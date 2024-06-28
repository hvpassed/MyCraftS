using ProtoBuf;
using ProtoBuf.Serializers;
using Unity.Mathematics;

namespace MyCraftS.Utils
{
    [ProtoContract]
    public class Int3Surrogate
    {
        [ProtoMember(1)]
        public int X { get; set; }
        [ProtoMember(2)]
        public int Y { get; set; }
        [ProtoMember(3)]
        public int Z { get; set; }

        public static implicit operator int3(Int3Surrogate surrogate)
        {
            return new int3(surrogate.X, surrogate.Y, surrogate.Z);
        }

        public static implicit operator Int3Surrogate(int3 value)
        {
            return new Int3Surrogate { X = value.x, Y = value.y, Z = value.z };
        }
    }

    

}   
