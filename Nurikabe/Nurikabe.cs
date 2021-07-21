using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace Nurikabe
{
    public class Nurikabe
    {
        public List<nurikabePoint> blackSpots = new List<nurikabePoint>();
        public List<Island> islandList = new List<Island>();
        public betterArray nurikabeArr;
        public List<nurikabePoint> nurikabeArrIndx = new List<nurikabePoint>();
        private string strPath;

        public List<nurikabePoint> whiteSpots = new List<nurikabePoint>();

        private readonly int xSize;
        private readonly int ySize;

        public Nurikabe(string strPath)
        {
            //nalozimo datoteko in inicializiramo zadevo
            this.strPath = strPath;
            var text = File.ReadAllLines(strPath);

            var first = true;
            foreach (var str in text)
                if (first)
                {
                    first = false;
                    var words = str.Split(' ');
                    var xy = Array.ConvertAll(words, int.Parse);
                    xSize = xy[0];
                    ySize = xy[1];
                    nurikabeArr = new betterArray(xSize, ySize);
                }
                else
                {
                    if (str == "") continue;
                    var words = str.Split(' ').ToList();
                    words.Remove("");
                    var xy = Array.ConvertAll(words.ToArray(), int.Parse);
                    var p = new nurikabePoint(xy[0], xy[1], xy[2]);
                    if (xy[2] != 1)
                    {
                        var wh = new Island(xy[0], xy[1], xy[2]);
                        islandList.Add(wh);
                    }

                    nurikabeArrIndx.Add(p);
                    nurikabeArr[xy[0], xy[1]] = xy[2];
                }
        }

        public string NarisiMe()
        {
            //todo naredi še za ostale ele tabele
            var outStr = "";
            for (var y = 0; y < ySize + 2; y++)
            {
                for (var x = 0; x < xSize + 2; x++)
                {
                    var t = -100;
                    var c = "";
                    if (x == 0 || x == xSize + 1)
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
                        if (t <= 9 && t > 0)
                            c = "0" + t;
                        else if (t < 0) c = "XX";

                        if (t > 9) c = t.ToString();

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
            var dis = 0;
            var y = a.y - b.y;
            var x = a.x - b.x;
            x = Math.Abs(x);
            y = Math.Abs(y);
            if (x != 0 || y != 0) x--;

            return x + y;
        }

        public void ogradiEnke()
        {
            foreach (var nur in nurikabeArrIndx)
                if (nur.val == 1)
                {
                    if (nur.x - 1 >= 0) nurikabeArr[nur.x - 1, nur.y] = -1;
                    if (nur.x + 1 < nurikabeArr.dim[0]) nurikabeArr[nur.x + 1, nur.y] = -1;

                    if (nur.y + 1 < nurikabeArr.dim[1]) nurikabeArr[nur.x, nur.y + 1] = -1;
                    if (nur.y - 1 >= 0) nurikabeArr[nur.x, nur.y - 1] = -1;
                }
        }

        public bool zrihtajSosede()
        {
            var ch = false;
            for (var i = 0; i < nurikabeArrIndx.Count; i++)
            for (var j = i + 1; j < nurikabeArrIndx.Count; j++)
            {
                var dist = Math.Pow(nurikabeArrIndx[i].x - nurikabeArrIndx[j].x, 2) +
                           Math.Pow(nurikabeArrIndx[i].y - nurikabeArrIndx[j].y, 2);
                var neighbour = dist <= 4;

                if (neighbour)
                {
                    var xDiff = nurikabeArrIndx[i].x - nurikabeArrIndx[j].x;
                    var yDiff = nurikabeArrIndx[i].y - nurikabeArrIndx[j].y;

                    if (xDiff < 0)
                    {
                        if (nurikabeArr[nurikabeArrIndx[i].x + 1, nurikabeArrIndx[i].y] == 0)
                        {
                            nurikabeArr[nurikabeArrIndx[i].x + 1, nurikabeArrIndx[i].y] = -1;
                            ch = true;
                        }
                    }
                    else if (xDiff > 0)
                    {
                        if (nurikabeArr[nurikabeArrIndx[i].x - 1, nurikabeArrIndx[i].y] == 0)
                        {
                            nurikabeArr[nurikabeArrIndx[i].x - 1, nurikabeArrIndx[i].y] = -1;
                            ch = true;
                        }
                    }

                    if (yDiff < 0)
                    {
                        if (nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y + 1] == 0)
                        {
                            nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y + 1] = -1;
                            ch = true;
                        }
                    }
                    else if (yDiff > 0)
                    {
                        if (nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y - 1] == 0)
                        {
                            nurikabeArr[nurikabeArrIndx[i].x, nurikabeArrIndx[i].y - 1] = -1;
                            ch = true;
                        }
                    }
                }
            }

            return ch;
        }

        public void ResiMe()
        {
            var debugg = 0;
            var steviloNull = 0;
            ogradiEnke();
            zrihtajSosede();
            markUnreachables(true);
            steviloNull = prestejNullArr();
            var haveIChanged = true;
            int tmp;
            //var a = desperatePlacings();
           
            while (haveIChanged)
            {
                debugg++;
                var change = true;

                while (change)
                {
                    change = false;
                    change = razsiriOgrajeneBele();
                    if(misc.debug)
                    Console.WriteLine(NarisiMe());
                    change = razsiriOgrajeneCrne() || change;
                    if(misc.debug)
                    Console.WriteLine(NarisiMe());
                }
                if(misc.debug)
                Console.WriteLine(NarisiMe());
                zrihtajOgrajene();
                if(misc.debug)
                Console.WriteLine(NarisiMe());
                zrihtajTisteZEnostavnoMoznostjoZaprtja();
                if(misc.debug)
                Console.WriteLine(NarisiMe());
                zrihtajSosede();
                if(misc.debug)
                Console.WriteLine("Debugg=" + debugg);
                if(misc.debug)
                Console.WriteLine(NarisiMe());

                markUnreachables(false);
                if(misc.debug)
                    Console.WriteLine(NarisiMe());

                tmp = prestejNullArr();
                if(misc.debug)
                Console.WriteLine(NarisiMe());
                if (tmp == steviloNull)
                {
                    razsiriTisteKoJihPacGreRazsirit(true);
                    if(misc.debug)
                    Console.WriteLine("Debugg=" + debugg);
                    if(misc.debug)
                    Console.WriteLine(NarisiMe());
                    razsiriTisteKoJihPacGreRazsirit(false);
                    if(misc.debug)
                        Console.WriteLine(NarisiMe());
                    tryAndCloseIslands();
                    if(misc.debug)
                    Console.WriteLine(NarisiMe());
                }

                tmp = prestejNullArr();
                if (tmp == steviloNull)
                {
                    if (tmp < 20) expandBlacks(true);
                    if(misc.debug)
                    Console.WriteLine(NarisiMe());
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
                if(misc.debug)
                Console.WriteLine(NarisiMe());
            }
            //Console.WriteLine(NarisiMe());


            if (prestejNullArr() == 0)
            {
                return;
            }
            else
            {
                if(misc.debug)
                Console.WriteLine(NarisiMe());
                var a = desperatePlacings();

                foreach (var pt in a)
                    if (pt.val < 0)
                        nurikabeArr[pt.x, pt.y] = -1;
                razsiriTisteKoJihPacGreRazsirit();
                haveIChanged = false;
            }
            
        }

        public int prestejNullArr()
        {
            var ret = 0;
            foreach (var a in nurikabeArr.arrNull) ret += a.Count;
            return ret;
        }


        private bool zrihtajOgrajene()
        {
            var ch = false;

            nurikabeArr.getOgrajeneZCrno();
            foreach (var ograjenNull in nurikabeArr.ograjeniZcrno)
            {
                var stDosegljivih = 0;
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
                    var (dos, pot2) = nurikabeArr.aliLahkoPridemIzAvBVsePoti(isl.Value, ograjenNull);
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
                        for (var i = 0; i < pot[0].Count; i++)
                            if (nurikabeArr[pot[0][i].x, pot[0][i].y] == 0)
                            {
                                nurikabeArr[pot[0][i].x, pot[0][i].y] = islMaxSiz;
                                ch = true;
                            }

                        if (nurikabeArr.beliIsland[islId].amFull) nurikabeArr.ogradiIsland(islId);
                    }
                    else
                    {
                        //pogledamo katere tocke so v vseh enake in tiste polozimo
                        var ct = 0;
                        var PreverjeneTocke = new List<nurikabePoint>();
                        var PotrjeneTocke = new List<nurikabePoint>();


                        foreach (var pt in pot[0])
                        {
                            if (PreverjeneTocke.Contains(pt) || PotrjeneTocke.Contains(pt)) continue;

                            var aSemFuj = false;
                            for (var i = ct; i < pot.Count; i++)
                                if (pot[i].Contains(pt) == false)
                                {
                                    aSemFuj = true;
                                    break;
                                }

                            if (aSemFuj)
                                PreverjeneTocke.Add(pt);
                            else
                                PotrjeneTocke.Add(pt);
                        }

                        ct++;

                        foreach (var toc in PotrjeneTocke)
                        {
                            if (nurikabeArr.beliIsland[islId].Points.Contains(toc)) continue;
                            toc.val = nurikabeArr.beliIsland[islId].maxSize;
                            toc.id = nurikabeArr.beliIsland[islId].id;
                            nurikabeArr.dodajIslanduTutCeSeNeDrzim(islId, toc);
                            ch = true;

                            if (nurikabeArr.beliIsland[islId].amFull) nurikabeArr.ogradiIsland(islId);
                        }
                    }
                }
            }

            return ch;
        }

        //zrihtamo tiste kjer imajo robe z 2 moznostma
        private bool zrihtajTisteZEnostavnoMoznostjoZaprtja()
        {
            var ch = false;
            //grem skozi vse islande
            foreach (var isl in nurikabeArr.beliIsland.Values)
            {
                if (isl.amFull || isl.maxSize - isl.size != 1) continue;
                var nullCount = 0;
                var edgePoint = new nurikabePoint();
                var naso = false;
                //grem skozi vse tocke v islandu in preverim ce obstaja ene z 2 izhodoma
                foreach (var pt in isl.Points)
                {
                    var sosedi = nurikabeArr.getSosede(pt);
                    foreach (var sos in sosedi)
                        if (sos.id == 0)
                            nullCount++;
                    if (nullCount == 2)
                    {
                        edgePoint = pt;
                        naso = true;
                    }
                    else if (nullCount != 0)
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
                        if (s.id == 0)
                            nullSos.Add(s);
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
            var sprememba = false;
            foreach (var isld in nurikabeArr.beliIsland)
            {
                if (isld.Value.amFull) continue;
                evil:
                var (a, b) = nurikabeArr.aliMamEnIzhod(isld.Value);
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
            var sprememba = false;
            foreach (var isld in nurikabeArr.crniIsland.ToList())
            {
                var naso = false;
                var staEnakoVelika = true;

                foreach (var isld2 in nurikabeArr.crniIsland.Values)
                    if (isld.Value.id == isld2.id)
                    {
                        naso = true;
                        if (isld.Value.size != isld2.size)
                        {
                            staEnakoVelika = false;
                            break;
                        }
                    }

                if (staEnakoVelika == false) continue;
                if (naso == false) continue;

                var (a, b) = nurikabeArr.aliMamEnIzhod(isld.Value);
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
            for (var i = 0; i < nurikabeArr.arrNull.Count; i++)
            for (var j = 0; j < nurikabeArr.arrNull[i].Count; j++)
            {
                //todo preveri če se dejansko spreminja sranje v better arr
                var rechable = false;
                foreach (var b in nurikabeArr.beliIsland)
                foreach (var bi in b.Value.Points)
                {
                    var dist = razdaljaMedKvadratoma(bi, nurikabeArr.arrNull[i][j]);
                    var availableDist = b.Value.maxSize - b.Value.size;
                    if (dist < availableDist) rechable = true;
                }

                if (rechable == false)
                {
                    nurikabeArr[nurikabeArr.arrNull[i][j].x, nurikabeArr.arrNull[i][j].y] = -1;
                    sprememba = true;
                }
            }

            return sprememba;
        }

        private bool expandBlacks(bool napredno = false, int limitPos = 1000)
        {
            //gremo skozi vse crne in jih razsirimo
            foreach (var blcIsl in nurikabeArr.crniIsland.ToList())
            {
                if (nurikabeArr.crniIsland.ContainsKey(blcIsl.Key) == false) continue;
                var (semNaselResitev, resitve) =
                    nurikabeArr.getMoznostiRazsirjanjaCrnih(blcIsl.Value.id, true, limitPos);
                if (semNaselResitev)
                {
                    //ce je samo ena logicna pot razsiritve jo uporabimo
                    if (resitve.Count == 1)
                    {
                        foreach (var res in resitve[0]) nurikabeArr[res.x, res.y] = -1;
                    }
                    //ce ne pa uporabimo njihove skupne tocke
                    else
                    {
                        //najdemo vse skupne toc v poteh
                        var skupneToc = new List<nurikabePoint>();

                        foreach (var pt in resitve[0])
                        {
                            var fujTocka = false;
                            for (var j = 0; j < resitve.Count; j++)
                                if (resitve[j].Contains(pt) == false)
                                {
                                    fujTocka = true;
                                    break;
                                }

                            if (fujTocka == false) skupneToc.Add(pt);
                        }

                        foreach (var pt in skupneToc) nurikabeArr[pt.x, pt.y] = -1;
                    }
                }
            }

            return false;
        }

        private bool markUnreachables(bool naive = true)
        {
            var res = false;
            if (naive)
            {
                res = markUnreachables();
            }
            else
            {
                //Označimo nedosegljive kvadratke tak da loopamo skozi vse null kvadratke in vse white islande, ne upoštevamo ovir le razdaljo
                res = false;
                for (var i = 0; i < nurikabeArr.arrNull.Count; i++)
                for (var j = 0; j < nurikabeArr.arrNull[i].Count; j++)
                {
                    var rechable = false;
                    foreach (var b in nurikabeArr.beliIsland)
                    {
                        //(var rr,var nn) = nurikabeArr.aliLahkoPridemIzAvB(b.Value, nurikabeArr.arrNull[i][j]);

                        if (nurikabeArr.arrNull[i][j] == new nurikabePoint(2, 0) && b.Value.maxSize == 33 )
                        {
                            var tt = 0;
                        }

                        var (rr, nn) = nurikabeArr.aliLahkoPridemIzAvBVsePoti(b.Value, nurikabeArr.arrNull[i][j]);
                        if (rr || nn == null)
                        {
                            rechable = true;
                            break;
                        }
                    }

                    if (rechable == false)
                    {
                        nurikabeArr[nurikabeArr.arrNull[i][j].x, nurikabeArr.arrNull[i][j].y] = -1;
                        res = true;
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
            foreach (var nul in nulAr)
                nulls.Add(nul.copy());

            var isld = new List<Island>();
            foreach (var isl in nurikabeArr.beliIsland.Values)
                if (!isl.amFull)
                    isld.Add(isl.copy());
            //debug file
            //StreamWriter outputFile = new StreamWriter(Path.Combine("./", "Debug.txt"));
            // dodamo vse nepolne islande
            var id = 0;
            var root = new ListElement(nulls, nurikabeArr.arr, isld, id);
            id++;
            root.father = null;
            var obdelovan = root;
            var garboCounter = 0;
            while (obdelovan != null && obdelovan.semResitev == false)
            {
                //Console.WriteLine(NarisiMe());
                id++;
                if(misc.debug)
                Console.WriteLine((id - 1).ToString());

                var tmp2 = obdelovan.narisiMe(null);
                if(misc.debug)
                Console.WriteLine(tmp2);
                if(id%10000 == 0)
                    Console.WriteLine("iteracija: "+(id).ToString());

                //outputFile.WriteLine(tmp2);

                garboCounter++;
                if (garboCounter > 10000)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    garboCounter = 0;
                }

                //Console.WriteLine(obdelovan.id);
                //Console.WriteLine("dead: " + obdelovan.deadEnd);
                //obdelovan.narisiMe(null);
                if (obdelovan.posibilities.Count == 0)
                {
                    var tmp = obdelovan.semJazResitev(nurikabeArr);
                    obdelovan.semResitev = tmp;
                    if (tmp) break;

                    obdelovan.deadEnd = true;
                    obdelovan = obdelovan.father;
                }

                //ce sem dead end se ubijem
                if (obdelovan.deadEnd)
                {
                    obdelovan = obdelovan.father;
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
                    }
                    //drugace pa dam froca v obdelavo
                    else
                    {
                        try
                        {
                            obdelovan.razvijFroca(id);
                            obdelovan = obdelovan.children[obdelovan.kolkoOtrokSemObdelal - 1];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                             var tmp3 = obdelovan.narisiMe(null);
                            throw;
                        }

                    }
                }
            }

            //outputFile.Close();
            if (obdelovan == null) return new List<nurikabePoint>();
            if (obdelovan.semResitev)
            {
                var resitev = new List<nurikabePoint>();
                foreach (var a in root.posibilities) resitev.Add(new nurikabePoint(a.x, a.y, obdelovan.arr[a.x, a.y]));

                return resitev;
            }

            ;
            return new List<nurikabePoint>();
        }

        private bool razsiriTisteKoJihPacGreRazsirit()
        {
            //rasirimo tiste ki imajo eno moznost razsiritve
            var chng = false;
            foreach (var isld in nurikabeArr.beliIsland.Values.ToList())
            {
                if (isld.amFull || isld.maxSize > 10) continue;
                var (yes, pot) = nurikabeArr.dobiVseMozneIslande(isld, true);
                //ce je samo ena moznost jo izberemo
                if (yes)
                {
                    foreach (var pt in pot[0].Points)
                        if (isld.Points.Contains(pt) == false)
                        {
                            nurikabeArr[pt.x, pt.y] = pot[0].maxSize;
                            chng = true;
                        }

                    if (isld.amFull) nurikabeArr.ogradiIsland(isld.id);
                }
            }

            return chng;
        }

        private bool razsiriTisteKoJihPacGreRazsirit(bool napredno = false)
        {
            var ret = false;
            if (napredno == false)
            {
                ret = razsiriTisteKoJihPacGreRazsirit();
            }
            else
            {
                var chng = false;
                foreach (var isld in nurikabeArr.beliIsland.Values.ToList())
                {
                    if (isld.amFull || isld.maxSize > 10) continue;

                    if (isld.semPovezan == false) continue;
                    var (yes, pot) = nurikabeArr.dobiVseMozneIslande2(isld);

                    var validated = new List<nurikabePoint>();
                    var unValidated = new List<nurikabePoint>();
                    if (pot.Count == 1)
                    {
                        foreach (var V in pot[0].Points) nurikabeArr[V.x, V.y] = pot[0].maxSize;

                        return true;
                    }
                    //za vsak island preverimo ce ima skupne pointe v drugih islandih

                    for (var i = 0; i < pot[0].Points.Count; i++)
                    {
                        var cont = true;
                        for (var i2 = 1; i2 < pot.Count; i2++)
                            if (pot[i2].Points.Contains(pot[0].Points[i]) == false)
                            {
                                cont = false;
                                break;
                            }

                        if (cont) validated.Add(pot[0].Points[i]);
                    }

                    //ce smo nasli skupne pointe jih dodamo
                    if (validated.Count > 0)
                    {
                        //todo ret vrne true vedno ker vzamemo v postev tocke ki so v ze obstojecih islandih
                        ret = true;
                        foreach (var pt in validated)
                        {
                            if (nurikabeArr.beliIsland[isld.id].Points.Contains(pt)) continue;
                            nurikabeArr.dodajIslanduTutCeSeNeDrzim(isld.id, pt);
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
                //za vse nepovezane isl dobimo vse moznosti povezave in jih povezemo
                if (isl.Value.semPovezan == false)
                {
                    var (tru, listListov) = nurikabeArr.dobiVseMoznostiZdruzevanjaOtoka(isl.Value);
                    if (!tru) return false;
                    var skupni = new List<nurikabePoint>();
                    if (listListov.Count == 1)
                        skupni = listListov[0];
                    else
                        for (var j = 0; j < listListov[0].Count; j++)
                        {
                            var ad = true;
                            for (var i = 0; i < listListov.Count; i++)
                                if (listListov[i].Contains(listListov[0][j]) == false)
                                    ad = false;
                            if (ad) skupni.Add(listListov[0][j]);
                        }

                    if (skupni.Count == listListov[0].Count) nurikabeArr.narediIslCeli(isl.Key);

                    foreach (var sk in skupni) nurikabeArr[sk.x, sk.y] = isl.Value.maxSize;
                }

            return false;
        }


        public void ResiMeGen()
        {
            ogradiEnke();
            zrihtajSosede();
            markUnreachables(false);
            Console.WriteLine(NarisiMe());
            var tempArr = new List<nurikabePoint>();

            foreach (var nlArr in nurikabeArr.arrNull)
            foreach (var nl in nlArr)
                tempArr.Add(nl);

            var gen = new GenetskiAlg(tempArr, nurikabeArr);
            gen.doTheEvolution(true);
        }
    }
}