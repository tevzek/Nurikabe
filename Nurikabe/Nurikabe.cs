using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Permissions;

namespace Nurikabe
{
    public class Nurikabe
    {
        private string strPath;
        public List<nurikabePoint> nurikabeArrIndx = new List<nurikabePoint>();
        public List<Island> islandList = new List<Island>();

        public List<nurikabePoint> whiteSpots = new List<nurikabePoint>();
        public List<nurikabePoint> blackSpots = new List<nurikabePoint>();
        public betterArray nurikabeArr;

        private int xSize;
        private int ySize;

        public Nurikabe(string strPath)
        {
            //nalozimo datoteko in inicializiramo zadevo
            this.strPath = strPath;
            var text = System.IO.File.ReadAllLines(strPath);

            bool first = true;
            foreach (var str in text)
            {
                if (first)
                {
                    first = false;
                    string[] words = str.Split(' ');
                    int[] xy = Array.ConvertAll(words, int.Parse);
                    this.xSize = xy[0];
                    this.ySize = xy[1];
                    nurikabeArr = new betterArray(xSize, ySize);
                }
                else
                {
                    if (str == "")
                    {
                        continue;
                    }
                    List<string> words = str.Split(' ').ToList();
                    words.Remove("");
                    int[] xy = Array.ConvertAll(words.ToArray(), int.Parse);
                    nurikabePoint p = new nurikabePoint(xy[0], xy[1], xy[2]);
                    if (xy[2] != 1)
                    {
                        Island wh = new Island(xy[0], xy[1], xy[2]);
                        islandList.Add(wh);
                    }
                    
                    nurikabeArrIndx.Add(p);
                    nurikabeArr[xy[0], xy[1]] = xy[2];
                }

            }

        }

        public String NarisiMe()
        {
            //todo naredi še za ostale ele tabele
            string outStr = "";
            for (int y = 0; y < ySize + 2; y++)
            {
                for (int x = 0; x < xSize + 2; x++)
                {
                    int t = -100;
                    string c = "";
                    if ((x == 0 || x == xSize + 1))
                    {

                        c += "|";
                    }
                    else if (y == 0 || y == ySize + 1)
                    {
                        c = "___";
                    }
                    else if (nurikabeArr[x - 1, y - 1] == 0)
                    {
                        c = "  |";
                    }
                    else
                    {
                        t = nurikabeArr[x - 1, y - 1];
                        if (t<= 9 && t > 0)
                        {
                            c = "0"+t.ToString();
                        }
                        else if(t < 0)
                        {
                            c = "XX";
                        }

                        if (t > 9)
                        {
                            c = t.ToString();
                        }

                        c += "|";
                    }

                    outStr += c;
                }

                outStr += "\n";

            }

            return outStr;
        }

        private int razdaljaMedKvadratoma(nurikabePoint a, nurikabePoint b)
        {
            int dis = 0;
            int y = a.y - b.y;
            int x = a.x - b.x;
            x = Math.Abs(x);
            y = Math.Abs(y);
            if (x != 0 || y != 0)
            {
                x--;
            }
       
            return x + y;
        }
        public void ogradiEnke()
        {
            foreach (var nur in nurikabeArrIndx)
            {
                if (nur.val == 1)
                {
                    if(nur.x - 1 >= 0){
                        nurikabeArr[nur.x - 1, nur.y] = -1;
                    }
                    if(nur.x + 1 < nurikabeArr.dim[0]){
                        nurikabeArr[nur.x + 1, nur.y] = -1;
                    }
                    
                    if(nur.y + 1 < nurikabeArr.dim[1]){
                        nurikabeArr[nur.x, nur.y + 1] = -1;
                    }
                    if(nur.y - 1 >= 0){
                        nurikabeArr[nur.x, nur.y - 1] = -1;
                    }
                }
            }
        }

        public bool zrihtajSosede()
        {
            bool ch = false;
            for (int i = 0; i<nurikabeArrIndx.Count; i++)
            {
                for (int j = i+1; j<nurikabeArrIndx.Count; j++)
                {
                    var dist = Math.Pow((nurikabeArrIndx[i].x - nurikabeArrIndx[j].x),2) + Math.Pow((nurikabeArrIndx[i].y - nurikabeArrIndx[j].y),2);
                    bool neighbour = dist <= 4;

                    if (neighbour)
                    {
                        var xDiff = nurikabeArrIndx[i].x - nurikabeArrIndx[j].x;
                        var yDiff = nurikabeArrIndx[i].y - nurikabeArrIndx[j].y;

                        if (xDiff < 0 )
                        {
                            if (nurikabeArr[nurikabeArrIndx[i].x + 1, nurikabeArrIndx[i].y] == 0)
                            {
                                nurikabeArr[nurikabeArrIndx[i].x + 1, nurikabeArrIndx[i].y] = -1;
                                ch = true;
                            } 
                        }
                        else if (xDiff > 0 )
                        {
                            if ( nurikabeArr[nurikabeArrIndx[i].x - 1, nurikabeArrIndx[i].y] == 0)
                            {
                                nurikabeArr[nurikabeArrIndx[i].x - 1, nurikabeArrIndx[i].y] = -1;
                                ch = true;
                            }

                        }
                        if (yDiff < 0 )
                        {
                            if (nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y+1] == 0)
                            {
                                nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y+1] = -1;
                                ch = true;
                            }

                        }
                        else if (yDiff > 0 )
                        {
                            if (nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y-1] == 0)
                            {
                                nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y-1] = -1;
                                ch = true;
                            }
                        }
                    }
                }                
            }
            return ch;
        }
 
        public void ResiMe()
        {
                int steviloNull = 0;
                ogradiEnke();
                zrihtajSosede();
                markUnreachables(naive:true);
                steviloNull = prestejNullArr();
                bool haveIChanged = true;
                int tmp;
                while (haveIChanged)
                {
                    var change = true;
                    
                    while (change)
                    {
                        change = razsiriOgrajeneBele();
                        //Console.WriteLine(NarisiMe());
                        change = razsiriOgrajeneCrne() || change;
                        //Console.WriteLine(NarisiMe());

                    }
                    //Console.WriteLine(NarisiMe());
                    zrihtajOgrajene();
                    //Console.WriteLine(NarisiMe());
                    zrihtajTisteZEnostavnoMoznostjoZaprtja();
                    //Console.WriteLine(NarisiMe());
                    zrihtajSosede();
                    //Console.WriteLine(NarisiMe());
                    markUnreachables(naive:false);
                    //Console.WriteLine(NarisiMe());
                    
                    tmp = prestejNullArr();
                    //Console.WriteLine(NarisiMe());
                    if(tmp == steviloNull){
                    razsiriTisteKoJihPacGreRazsirit(true);
                    razsiriTisteKoJihPacGreRazsirit(false);
                    tryAndCloseIslands();

                    //Console.WriteLine(NarisiMe());
                    }
                    
                    tmp = prestejNullArr();
                    if (tmp == steviloNull)
                    {
                        if(tmp < 20){
                        expandBlacks(true);
                        }
                        //Console.WriteLine(NarisiMe());
                        
                    }
                
                    tmp = prestejNullArr();
                    if (tmp != steviloNull)
                    {
                        steviloNull = tmp;
                        haveIChanged = true;
                    }
                    else
                    {
                        haveIChanged = false;
                    }
                }
                //Console.WriteLine(NarisiMe());


                if (prestejNullArr() == 0)
                {
                    return;
                }
                else
                {
                    var a = desperatePlacings();
                    foreach (var pt in a)
                    {
                        if (pt.val < 0)
                        {
                            nurikabeArr[pt.x, pt.y] = -1;
                        }
                    }
                    razsiriTisteKoJihPacGreRazsirit();
                }
        }
        public int prestejNullArr()
        {
            int ret = 0;
            foreach (var a in nurikabeArr.arrNull)
            {
                ret += a.Count;

            }
            return ret;
        }
            

        private bool zrihtajOgrajene()
        {
            bool ch = false;

            nurikabeArr.getOgrajeneZCrno();
            foreach (var ograjenNull in nurikabeArr.ograjeniZcrno)
            {
                int stDosegljivih = 0;
                var pot = new List<List<nurikabePoint>>();
                var id = 0;
                var islMaxSiz = -1;
                var islId = -1;
                foreach (var isl in nurikabeArr.beliIsland)
                {
                    if (stDosegljivih > 1) break;
                    if (nurikabeArr.beliIsland[isl.Key].amFull) continue;
                    //Console.Clear();

                    //(var dos, var pot2) = nurikabeArr.aliLahkoPridemIzAvB(isl.Value,ograjenNull,true);
                    (var dos, var pot2) = nurikabeArr.aliLahkoPridemIzAvBVsePoti(isl.Value,ograjenNull);
                    if (dos)
                    {
                        stDosegljivih++;
                        pot = pot2;
                        islMaxSiz = isl.Value.maxSize;
                        islId = isl.Key;
                        ch = true;
                    }
                }

                if (stDosegljivih == 1)
                {
                    if (pot.Count == 1)
                    {
                        for (int i = 0; i < pot[0].Count ; i++)
                        {
                            if (nurikabeArr[pot[0][i].x, pot[0][i].y] == 0)
                            {
                                nurikabeArr[pot[0][i].x, pot[0][i].y] = islMaxSiz;
                                ch = true;
                            }
                        }

                        if (nurikabeArr.beliIsland[islId].amFull)
                        {
                            nurikabeArr.ogradiIsland(islId);
                        }
                    }
                    else
                    {
                        //pogledamo katere tocke so v vseh enake in tiste polozimo
                        int ct = 0;
                        var PreverjeneTocke = new List<nurikabePoint>();
                        var PotrjeneTocke = new List<nurikabePoint>();

  
                            foreach (var pt in pot[0])
                            {
                                if (PreverjeneTocke.Contains(pt) || PotrjeneTocke.Contains(pt))
                                {
                                    continue;
                                }

                                bool aSemFuj = false;
                                for (int i = ct; i < pot.Count; i++)
                                {
                                    if (pot[i].Contains(pt) == false)
                                    {
                                        aSemFuj = true;
                                        break;
                                    }
                                }

                                if (aSemFuj)
                                {
                                    PreverjeneTocke.Add(pt);
                                }
                                else
                                {
                                    PotrjeneTocke.Add(pt);
                                }
                                
                            }
                            ct++;
                        
                        foreach (var toc in PotrjeneTocke )
                        {
                            
                            if (nurikabeArr.beliIsland[islId].Points.Contains(toc)) continue;
                            toc.val = nurikabeArr.beliIsland[islId].maxSize;
                            toc.id = nurikabeArr.beliIsland[islId].id;
                            nurikabeArr.dodajIslanduTutCeSeNeDrzim(islId,toc);
                            ch = true;

                            if (nurikabeArr.beliIsland[islId].amFull)
                            {
                                nurikabeArr.ogradiIsland(islId);
                            }
                        }
                    }
                }
            }

            return ch;
        }

        //zrihtamo tiste kjer imajo robe z 2 moznostma
        private bool zrihtajTisteZEnostavnoMoznostjoZaprtja()
        {
            bool ch = false;
            //grem skozi vse islande
            foreach (var isl in nurikabeArr.beliIsland.Values)
            {
                if(isl.amFull || (isl.maxSize - isl.size) != 1) continue;
                int nullCount = 0;
                nurikabePoint edgePoint = new nurikabePoint();
                bool naso = false;
                //grem skozi vse tocke v islandu in preverim ce obstaja ene z 2 izhodoma
               foreach (var pt in isl.Points)
               {
                   var sosedi = nurikabeArr.getSosede(pt);
                   foreach (var sos in sosedi)
                   {
                       if (sos.id == 0) nullCount++;
                   }
                   if (nullCount == 2)
                   {
                       edgePoint = pt;
                       naso = true;
                   }
                   else if(nullCount != 0)
                   {
                       naso = false;
                       break;
                   }
               }

               if (naso)
               {
                   var nullSos = new List<nurikabePoint>();
                   var sos = nurikabeArr.getSosede(edgePoint);
                   foreach (var s in sos)
                   {
                       if (s.id == 0)
                       {
                           nullSos.Add(s);
                       }
                   }
                   if (nullSos[0].x != nullSos[1].x && nullSos[0].y != nullSos[1].y)
                   {
                       if (nurikabeArr[nullSos[0].x, nullSos[1].y] == 0)
                       {
                           nurikabeArr[nullSos[0].x, nullSos[1].y] = -1;
                           ch = true;
                       }
                       else if (nurikabeArr[nullSos[1].x, nullSos[0].y] == 0)
                       {
                           nurikabeArr[nullSos[1].x, nullSos[0].y] = -1;
                           ch = true;
                       }
                       
                   }
               }
               
            }

            return ch;
        }
        private bool razsiriOgrajeneBele()
        {
            bool sprememba = false;
            foreach (var isld in nurikabeArr.beliIsland)
            {
                if (isld.Value.amFull)
                {
                    continue;
                }
                evil:
                (var a, var b) = nurikabeArr.aliMamEnIzhod(isld.Value);
                if (a)
                {
                    sprememba = true;
                    nurikabeArr[b.x, b.y] = isld.Value.maxSize;

                    if (isld.Value.amFull)
                    {
                        nurikabeArr.ogradiIsland(isld.Value.id);
                        continue;
                    }
                    //Console.WriteLine("dodal: "+b.x+","+b.y);
                    goto evil;
                }
            }
            return sprememba;
        }
        private bool razsiriOgrajeneCrne()
        {
            bool sprememba = false;
            foreach (var isld in nurikabeArr.crniIsland.ToList())
            {

                bool naso = false;
                bool staEnakoVelika = true;

                foreach (var isld2 in nurikabeArr.crniIsland.Values)
                {
                    if (isld.Value.id == isld2.id)
                    {
                        naso = true;
                        if (isld.Value.size != isld2.size)
                        {
                            staEnakoVelika = false;
                            break;
                        }
                    }
                }
                if(staEnakoVelika == false) continue;
                if(naso == false) continue;
                
                (var a, var b) = nurikabeArr.aliMamEnIzhod(isld.Value);
                if (a)
                {
                    sprememba = true;
                    nurikabeArr[b.x, b.y] = -1;
                }
            }
            return sprememba;
        }
        private bool markUnreachables()
        {
            //Označimo nedosegljive kvadratke tak da loopamo skozi vse null kvadratke in vse white islande, ne upoštevamo ovir le razdaljo
            var sprememba = false;
            for (int i = 0; i < nurikabeArr.arrNull.Count; i++)
            {
                for (int j = 0; j < nurikabeArr.arrNull[i].Count; j++)
                {
                    //todo preveri če se dejansko spreminja sranje v better arr
                    bool rechable = false;
                    foreach (var b in nurikabeArr.beliIsland)
                    {
                        foreach (var bi in  b.Value.Points)
                        {
                            int dist = razdaljaMedKvadratoma( bi, nurikabeArr.arrNull[i][j]);
                            int availableDist =  b.Value.maxSize -  b.Value.size;
                            if (dist < availableDist)
                            {
                                rechable = true;
                            }
                        }
                    }
                    if (rechable == false)
                    {
                        nurikabeArr[nurikabeArr.arrNull[i][j].x, nurikabeArr.arrNull[i][j].y] = -1;
                        sprememba = true;
                    }
                }
            }
            return sprememba;
        }
        
        private bool expandBlacks(bool napredno = false, int limitPos = 1000)
        {
            //gremo skozi vse crne in jih razsirimo
            foreach (var blcIsl in nurikabeArr.crniIsland.ToList())
            {
                if(nurikabeArr.crniIsland.ContainsKey(blcIsl.Key) == false) continue;
                (var semNaselResitev, var resitve) = nurikabeArr.getMoznostiRazsirjanjaCrnih(blcIsl.Value.id,vse:true,limitPos);
                if (semNaselResitev)
                {
                    //ce je samo ena logicna pot razsiritve jo uporabimo
                    if (resitve.Count == 1)
                    {
                        foreach (var res in resitve[0])
                        {
                            nurikabeArr[res.x, res.y] = -1;
                        }
                    }
                    //ce ne pa uporabimo njihove skupne tocke
                    else
                    {
                        //najdemo vse skupne toc v poteh
                        var skupneToc = new List<nurikabePoint>();
             
                            foreach (var pt in resitve[0])
                            {
                                bool fujTocka = false;
                                for (int j = 0; j< resitve.Count; j++) 
                                {
                                    if (resitve[j].Contains(pt) == false)
                                    {
                                        fujTocka = true;
                                        break;
                                    }
                                }
                                if (fujTocka == false)
                                {
                                    skupneToc.Add(pt);
                                }

                            }

                            foreach (var pt in skupneToc)
                        {
                            nurikabeArr[pt.x, pt.y] = -1;
                        }
                    }
                }
            }
            return false;
        }
        
        private bool markUnreachables(bool naive = true)
        {
            bool res = false;
            if (naive)
            {
                res = markUnreachables();
            }
            else
            {
                //Označimo nedosegljive kvadratke tak da loopamo skozi vse null kvadratke in vse white islande, ne upoštevamo ovir le razdaljo
                res = false;
                for (int i = 0; i < nurikabeArr.arrNull.Count; i++)
                {
                    for (int j = 0; j < nurikabeArr.arrNull[i].Count; j++)
                    {
                        bool rechable = false;
                        foreach (var b in nurikabeArr.beliIsland)
                        {
                                //(var rr,var nn) = nurikabeArr.aliLahkoPridemIzAvB(b.Value, nurikabeArr.arrNull[i][j]);
                                (var rr,var nn) = nurikabeArr.aliLahkoPridemIzAvBVsePoti(b.Value, nurikabeArr.arrNull[i][j]);
                                if (rr == true)
                                {
                                    rechable = true; 
                                }
                        }
                        if (rechable == false)
                        {
                            nurikabeArr[nurikabeArr.arrNull[i][j].x, nurikabeArr.arrNull[i][j].y] = -1;
                            res = true;
                        }
                    }
                }
                return res;
            }

            return res;
        }

        //zdaj pa bomo iskali v prostoru resitev resitev to je zadnja metoda ki jo kljicemno
        private List<nurikabePoint> desperatePlacings()
        {
           // edine resitve ki so morajo obstajat znotraj varijacij null arr tak da samo z tem sranjem delamo
           // naredili bomo drevo resitev in vsakic ko polozimo pt preverimo ce je unicil situacijo torej ce je
           // nardil 2*2 crno, zagradil bel otok, nardil prevelik otok, dotaknil 2 otoka etc
           
           //ustvarimo root node
           
           //prvo rabimo null arr v 1D
           
           
           var nulls = new List<nurikabePoint>();
           foreach (var nulAr in nurikabeArr.arrNull)
           {
               foreach (var nul in nulAr)
               {
                nulls.Add(nul.copy());   
               }
           }
           
           var isld = new List<Island>();
           foreach (var isl in nurikabeArr.beliIsland.Values)
           {
               if (!isl.amFull)
               {
                   isld.Add(isl.copy());
               }
           }
           // dodamo vse nepolne islande
           
           

           var id = 0;
           ListElement root = new ListElement(nulls, nurikabeArr.arr, isld,id);
           id++;
           root.father = null;
           var obdelovan = root;
           var garboCounter = 0;
           while (obdelovan != null && obdelovan.semResitev == false)
           {
               garboCounter++;
               if (garboCounter > 10000)
               {
                   GC.Collect();
                   GC.WaitForPendingFinalizers();
                   garboCounter = 0;
               }

               //Console.WriteLine(obdelovan.id);
               //Console.WriteLine(obdelovan.deadEnd);
               //obdelovan.narisiMe(null);
               if (obdelovan.posibilities.Count == 0)
               {
                   bool tmp = obdelovan.semJazResitev(nurikabeArr);
                   obdelovan.semResitev = tmp;
                   if (tmp == true)
                   {
                       break;
                   }

                   obdelovan.deadEnd = true;
                   obdelovan = obdelovan.father;
               }
               
               //ce sem dead end se ubijem
               if (obdelovan.deadEnd)
               {
                   obdelovan = obdelovan.father;
                   continue;
               }
               //ce ne posljem v obdelavo froca
               else
               {
                   //ce sem obdelal froca grem v oceta
                   if (obdelovan.kolkoOtrokSemObdelal > 1)
                   {
                       obdelovan.deadEnd = true;
                       obdelovan.children = null;
                       obdelovan = obdelovan.father;
                       continue;
                   }
                   //drugace pa dam froca v obdelavo
                   else
                   {
                       obdelovan.razvijFroca(id);
                       id ++;
                       obdelovan = obdelovan.children[obdelovan.kolkoOtrokSemObdelal-1];
                       continue;
                   }
                   
               }
           }

           if (obdelovan == null) return new List<nurikabePoint>();
           if (obdelovan.semResitev == true)
           {
               var resitev = new List<nurikabePoint>();
               foreach (var a in root.posibilities)
               {
                   resitev.Add(new nurikabePoint(a.x, a.y, obdelovan.arr[a.x, a.y]));
               }

               return resitev;

           };
           return  new List<nurikabePoint>();

        }
        private bool razsiriTisteKoJihPacGreRazsirit()
        {
            //rasirimo tiste ki imajo eno moznost razsiritve
            bool chng = false;
            foreach (var isld in nurikabeArr.beliIsland.Values.ToList())
            {
                if (isld.amFull || isld.maxSize>10)
                {
                    continue;
                }
                (var yes, var pot) = nurikabeArr.dobiVseMozneIslande(isld, true);
                //ce je samo ena moznost jo izberemo
                if (yes)
                {
                    foreach (var pt in pot[0].Points)
                    {
                        if (isld.Points.Contains(pt) == false)
                        {
                            nurikabeArr[pt.x, pt.y] = pot[0].maxSize;
                            chng = true;
                        }
                    }

                    if (isld.amFull)
                    {
                        nurikabeArr.ogradiIsland(isld.id);
                    }
                }
            }
            return chng;
        }

        private bool razsiriTisteKoJihPacGreRazsirit(bool napredno = false)
        {
            bool ret = false;
            if (napredno == false)
            {
                ret = razsiriTisteKoJihPacGreRazsirit();
            }
            else
            {
                bool chng = false;
                foreach (var isld in nurikabeArr.beliIsland.Values.ToList())
                {
                    if (isld.amFull || isld.maxSize > 10)
                    {
                        continue;
                    }

                    if (isld.semPovezan == false)
                    {
                        continue;
                    }
                    (var yes, var pot) = nurikabeArr.dobiVseMozneIslande2(isld);

                    List<nurikabePoint> validated = new List<nurikabePoint>();
                    List<nurikabePoint> unValidated = new List<nurikabePoint>();
                    if (pot.Count == 1)
                    {
                        foreach (var V in pot[0].Points)
                        {
                            nurikabeArr[V.x, V.y] = pot[0].maxSize;
                        }

                        return true;
                    }
                    //za vsak island preverimo ce ima skupne pointe v drugih islandih
                    for (int i = 0; i<pot.Count-1; i++)
                    {
                        foreach (var pt in pot[i].Points)
                        {
                            //dobimo vse mozne rezultate in gremo skozi vse mozne poti

                            if (validated.Contains(pt) || unValidated.Contains(pt))
                            {
                                continue;
                            }
                            

                            var tru = true;
                            for (int j = i; j < pot.Count; j++)
                            {
                                if (pot[j].Points.Contains(pt) == false)
                                {
                                    tru = false;
                                    unValidated.Add(pt);
                                    break;
                                }
                            }

                            if (tru)
                            {
                                validated.Add(pt);
                            }
                        }
                    }
                    //ce smo nasli skupne pointe jih dodamo
                    if (validated.Count > 0)
                    {
                        //todo ret vrne true vedno ker vzamemo v postev tocke ki so v ze obstojecih islandih
                        ret = true;
                        foreach (var pt in validated)
                        {
                            if(nurikabeArr.beliIsland[isld.id].Points.Contains(pt)) continue;
                            nurikabeArr.dodajIslanduTutCeSeNeDrzim(isld.id,pt);
                        }

                    }
                }
            }
            return ret;
        }

        //preveri vse moznosti zaprtja odprtih otokov
        private bool tryAndCloseIslands()
        {

            foreach (var isl in nurikabeArr.beliIsland)
            {
                //za vse nepovezane isl dobimo vse moznosti povezave in jih povezemo
                if (isl.Value.semPovezan == false)
                {
                    (var tru, var listListov) = nurikabeArr.dobiVseMoznostiZdruzevanjaOtoka(isl.Value);
                    if (!tru) return false;
                    var skupni = new  List<nurikabePoint>();
                    if (listListov.Count == 1)
                    {
                        skupni = listListov[0];
                    }
                    else
                    {
                        for (int j = 0; j < listListov[0].Count; j++)
                        {
                            bool ad = true;
                            for (int i = 0; i< listListov.Count; i++)
                            {
                                if (listListov[i].Contains(listListov[0][j]) == false)
                                {
                                    ad = false;
                                }        
                            }
                            if (ad)
                            {
                                skupni.Add(listListov[0][j]);
                            }
                            
                        }
                    }

                    if (skupni.Count == listListov[0].Count)
                    {
                        nurikabeArr.narediIslCeli(isl.Key);
                    }

                    foreach (var sk in skupni)
                    {
                        nurikabeArr[sk.x, sk.y] = isl.Value.maxSize;
                    }

                }
            }

            return false;
        }
        
        
        public void ResiMeGen()
        {
            ogradiEnke();
            zrihtajSosede();
            markUnreachables(naive:false);
            Console.WriteLine(NarisiMe());
            var tempArr = new List<nurikabePoint>();
            
            foreach (var nlArr in nurikabeArr.arrNull)
            {
                foreach (var nl in nlArr)
                {
                    tempArr.Add(nl);
                }
            }
            
            var gen = new GenetskiAlg(tempArr,nurikabeArr);
            gen.doTheEvolution(true);

        }
    }
}