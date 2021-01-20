using System;
using System.Reflection;

namespace Nurikabe
{


    public class nurikabePoint :IEquatable<nurikabePoint>
    {
        public int x { get; set; }
        public int y { get; set; }
        public int val { get; set; }
        
        public int id { get; set; }
        //za iskanje v sirino
        public int[] kogaSeDrzim;

        public bool seDrzimMasterja = true;
        public nurikabePoint(int x=-1, int y=-1, int val=Int32.MinValue, int id = 0)
        {
            this.x = x;
            this.y = y;
            this.val = val;
            this.id = id;
            kogaSeDrzim = new[] {-1, -1};

        }
        
        
        public static bool operator ==(nurikabePoint obj1, nurikabePoint obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }
            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(nurikabePoint obj1, nurikabePoint obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(nurikabePoint other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (this.x == other.x) && (this.y == other.y);
        }

        public nurikabePoint copy()
        {
            var retThis = new nurikabePoint(this.x, this.y, this.val, this.id);
            return retThis;
        }
    }
}