using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Nurikabe
{

        public class Island : IEquatable<Island>
        {
            public nurikabePoint anchor { get; set; }
            public List <nurikabePoint> Points { get; set; }
            public int maxSize;
            public int size;
            public bool amFull;
            public bool semPovezan = true;
            public int id { get; set; }
            //konstruktor za novi island
            public Island(int x, int y, int val=Int32.MaxValue, int id = 0)
            {
                nurikabePoint n = new nurikabePoint(x,y,val:val, id:id);
                anchor = n;
                Points = new List<nurikabePoint>();
                Points.Add(n);
                maxSize = val;
                size = 1;
                amFull = false;
                if (maxSize == size)
                {
                    amFull = true;
                }
                this.id = id;
            }
            
            public Island(nurikabePoint n)
            {
                anchor = n;
                Points = new List<nurikabePoint>();
                Points.Add(n);
                maxSize = n.val;
                size = 1;
                amFull = false;
                if (maxSize == size)
                {
                    amFull = true;
                }
                this.id = n.id;
            }
            //add point to island
            public void addPoint(nurikabePoint p)
            {
                if (amFull)
                {
                    throw new InvalidOperationException("Island is full");
                }

                if (this.Points.Contains(p))
                {
                    return;
                }
                //preverimo ce se dotikam osamele tocke
                p.id = id;
                Points.Add(p);
                size = Points.Count;

                if (size == maxSize)
                {
                    amFull = true;
                }
            }
            public void printMe()
            {
                foreach (var pt in Points)
                {
                    Console.Write("["+pt.x+","+pt.y+"] ");
                }
                Console.Write("\n");

                
            }

            //vzamemo island A in B, vse tocke iz B damo v A in spremenimo Idje
            public void combineIslands(Island island)
            {
                for (int i = 0; i < island.size; i++)
                {
                    var nurikabePoint = island.Points[i];
                    nurikabePoint.id = this.Points[0].id;
                    this.addPoint(nurikabePoint);
                }
            }
            public bool seMeDrzi(nurikabePoint pt)
            {
                foreach (var innerPt in Points)
                {
                    if (pt.x == innerPt.x+1 && pt.y == innerPt.y)
                    {
                        return true;
                    }
                    if (pt.x == innerPt.x-1 && pt.y == innerPt.y)
                    {
                        return true;
                    }
                    if (pt.x == innerPt.x && pt.y == innerPt.y-1)
                    {
                        return true;
                    }
                    if (pt.x == innerPt.x && pt.y == innerPt.y+1)
                    {
                        return true;
                    }
                }

                return false;
            }

            private List<nurikabePoint> copyList(List<nurikabePoint> ls)
            {
                List<nurikabePoint> lst = new List<nurikabePoint>();
                foreach (var p in ls)
                {
                    var tmp = p.copy();
                    lst.Add(tmp);
                }
                return lst;
            }

            public Island copy()
            {
                var isl = new Island(this.anchor.x,this.anchor.y, this.maxSize, this.id);
                isl.size = this.size;
                isl.amFull = this.amFull;
                isl.Points = copyList(this.Points);
                return isl;
            }
            
            public static bool operator ==(Island obj1, Island obj2)
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

            public static bool operator !=(Island obj1, Island obj2)
            {
                return !(obj1 == obj2);
            }

            public bool Equals(Island other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                bool tru = false;

                if (this.size != other.size)
                {
                    return false;
                }

                for (int i = 0; i < this.Points.Count; i++)
                {
                    if (!other.Points.Contains(this.Points[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

        }
    }
