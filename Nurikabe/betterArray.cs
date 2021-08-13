using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Nurikabe
{
    public class betterArray
    {
        //ta klasa da nebomo rabli vedno iskat vseh belih in crnih
        // -1 črni
        //0 beli/undecidet
        public int[,] arr;

        //note idji v listih se ne updejtajo
        public List<List<nurikabePoint>> arrBeli;
        public List<List<nurikabePoint>> arrCrni;
        public List<List<nurikabePoint>> arrNull;

        public Dictionary<int, Island> crniIsland = new Dictionary<int, Island>();
        public Dictionary<int, Island> beliIsland = new Dictionary<int, Island>();

        private int crniIslandId = -1;
        private int beliIslandId = 1;
        public int[] dim;
        
        public List <nurikabePoint> ograjeniZcrno { get; set; }
        
        //neke globalne spremenljivke za a* 
        private List<List<nurikabePoint>> poti = new List<List<nurikabePoint>>();

        public void removeFrom(ref List<List<nurikabePoint>> arr, nurikabePoint pt)
        {
            for (int i = 0; i < arr[pt.y].Count; i++)
            {
                if (arr[pt.y][i] == pt)
                {
                    arr[pt.y].RemoveAt(i);
                    break;
                }
            }
        }

        public betterArray(int x, int y)
        {
            //ustvarim vse primerne arr in liste
            arrBeli = new List<List<nurikabePoint>>();
            arrCrni = new List<List<nurikabePoint>>();
            arrNull = new List<List<nurikabePoint>>();
            
            dim = new[] {x, y};
            
            arr = new int[x,y];
            for (int i = 0; i < y; i++)
            {
             List<nurikabePoint> listek = new List<nurikabePoint>();
             List<nurikabePoint> listek2 = new List<nurikabePoint>();
             List<nurikabePoint> listek3 = new List<nurikabePoint>();


             arrBeli.Add(listek);
             arrCrni.Add(listek2);
             arrNull.Add(listek3);

             //napolnim tavelo z non arr
             for (int j = 0; j < x; j++)
                {
                    arrNull[i].Add(new nurikabePoint(j,i,0,id:0));
                }
            }
        }
        public nurikabePoint getPoint(int x, int y)
        {
             bool naso = false;
             nurikabePoint temp;
             
            for (int i = 0; i < arrBeli[y].Count; i++)
            {
                if (arrBeli[y][i].x == x && arrBeli[y][i].y == y)
                {
                    return arrBeli[y][i];
                }
            }
            for (int i = 0; i < arrCrni[y].Count; i++)
            {
                if (arrCrni[y][i].x == x && arrCrni[y][i].y == y)
                {
                    return arrCrni[y][i];
                }
            }
            for (int i = 0; i < arrNull[y].Count; i++)
            {
                if (arrNull[y][i].x == x && arrNull[y][i].y == y)
                {
                    return arrNull[y][i];
                }
            }
            throw new ArgumentNullException("ne obstaja point ("+x+","+y+")", "napaka pri pointu");
        }
        public void setValNId(int x, int y, int val=Int32.MaxValue, int id=Int32.MaxValue)
        {
            bool naso = false;
            nurikabePoint temp;
             
            for (int i = 0; i < arrBeli[y].Count; i++)
            {
                if (arrBeli[y][i].x == x && arrBeli[y][i].y == y)
                {
                    if (val != Int32.MaxValue)
                    {
                        var tmp = arrBeli[y][i];
                        tmp.val = val;
                        arrBeli[y][i] = tmp;
                    }
                    if (id != Int32.MaxValue)
                    {
                        var tmp = arrBeli[y][i];
                        tmp.id = id;
                        arrBeli[y][i] = tmp;
                    }
                    return;
                }
            }
            for (int i = 0; i < arrCrni[y].Count; i++)
            {
                if (arrCrni[y][i].x == x && arrCrni[y][i].y == y)
                {
                    if (val != Int32.MaxValue)
                    {
                        var tmp = arrCrni[y][i];
                        tmp.val = val;
                        arrCrni[y][i] = tmp;
                    }
                    if (id != Int32.MaxValue)
                    {
                        var tmp = arrCrni[y][i];
                        tmp.id = id;
                        arrCrni[y][i] = tmp;
                    }
                    return;
                }
            }
            for (int i = 0; i < arrNull[y].Count; i++)
            {
                if (arrNull[y][i].x == x && arrNull[y][i].y == y)
                {
                    if (val != Int32.MaxValue)
                    {
                        var tmp = arrNull[y][i];
                        tmp.val = val;
                        arrNull[y][i] = tmp;
                    }
                    if (id != Int32.MaxValue)
                    {
                        var tmp = arrNull[y][i];
                        tmp.id = id;
                        arrNull[y][i] = tmp;
                    }
                    return;
                }
            }
            throw new ArgumentNullException("ne obstaja point ("+x+","+y+")", "napaka pri pointu");
        }
        public void dodajIslanduTutCeSeNeDrzim(int islId, nurikabePoint pt)
        {
            
            bool seDrzim = seDrzimIslanda(pt);
            if (seDrzim)
            {

                this[pt.x, pt.y] = beliIsland[islId].maxSize;
            }
            else
            {
                pt.val = beliIsland[islId].maxSize;
                pt.id = beliIsland[islId].id;
                pt.seDrzimMasterja = false;
                beliIsland[islId].addPoint(pt);
                beliIsland[islId].semPovezan = false;
                this.arr[pt.x, pt.y] = pt.val;
                this.arrNull[pt.y].Remove(pt);
                this.arrBeli[pt.y].Add(pt);
            }

        }
        private bool seDrzimIslanda(nurikabePoint pt)
        {
            var sos = getSosede(pt);
            foreach (var sosed in sos)
            {
                if (sosed.val > 0 && sosed.id > 0 && sosed.seDrzimMasterja)
                    return true;
            }

            return false;
        }
        public int this[int i,int j]
        {
        get
        {
            return arr[i,j];
        }
        set
        {
            arr[i, j] = value;
            if (value == -1)
            {
                //spreminjaj vrednosti za tabelo belih in crnih
                var mamZe = false;
                
                var np = new nurikabePoint(i, j, id:crniIslandId);
                var npWhite = new nurikabePoint(i, j, 0);
                mamZe= arrCrni[j].Any(p => (p.x == i && p.y== j));
                //mamZe= arrCrni[j].Contains(np); rabim boljsi contains

                    
                if (mamZe == false)
                {
                    //preverim idje sosedov, in ce se dotikam kakega soseda idje zdruzim
                    List<int> sosednjiIdji = new List<int>();
                    nurikabePoint npTemp;
                    if (i+1<dim[0])
                    {
                    npTemp = getPoint(i+1, j);
                    sosednjiIdji.Add(npTemp.id);
                    }
                    if (i-1>=0)
                    {
                    npTemp = getPoint(i-1, j);
                    sosednjiIdji.Add(npTemp.id);
                    }
                    if (j+1<dim[1])
                    {
                        npTemp = getPoint(i, j+1);
                        sosednjiIdji.Add(npTemp.id);
                    }

                    if (j-1>=0)
                    {
                    npTemp = getPoint(i, j-1);
                    sosednjiIdji.Add(npTemp.id);
                    }
                    List<int> crniIdji = new List<int>();
                    foreach (var idj in sosednjiIdji)
                    {
                        if (idj < 0)
                        {
                            if (crniIdji.Contains(idj))
                            {
                                continue;
                            }
                            crniIdji.Add(idj);
                        }
                    }
                    //ustvarimo nov otok, povecamo otok ali zdruzimo otoke
                    if (crniIdji.Count == 0)
                    {
                        Island newCrniIsland = new Island(i,j,crniIslandId, id:crniIslandId);
                        crniIsland[crniIslandId] = newCrniIsland;
                        np.id = crniIslandId;
                        np.val = crniIslandId;
                        arrCrni[j].Add(np);
                        crniIslandId--;
                    }
                    else if (crniIdji.Count == 1)
                    {
                        //todo pofiksaj id za island classo
                        var point = new nurikabePoint(i, j, id:crniIdji[0],val:crniIdji[0]);
                        crniIsland[crniIdji[0]].addPoint(point);
                        arrCrni[j].Add(point);
                        removeFrom(ref this.arrNull,point);
                    }
                    else
                    {
                        for (int g = 1; g < crniIdji.Count; g++)
                        {
                            crniIsland[crniIdji[0]] = this.combineIslands(crniIsland[crniIdji[0]],crniIsland[crniIdji[g]]);
                            crniIsland.Remove(crniIdji[g]);
                        }
                        var point = new nurikabePoint(i, j, id:crniIdji[0], val:crniIdji[0]);
                        crniIsland[crniIdji[0]].addPoint(point);
                        arrCrni[j].Add(point);
                    }

                }
                arrNull[j].Remove(new nurikabePoint(i, j));

            }
            
            //copy paste od crnih otokov na bele
            if (value > 0)
            {
                //spreminjaj vrednosti za tabelo belih in crnih
                var mamZe = false;
                
                var np = new nurikabePoint(i, j, value);
                mamZe= arrBeli[j].Any(p => (p.x == i && p.y== j));
                //mamZe= arrCrni[j].Contains(np); rabim boljsi contains
                if (mamZe == false)
                {
                    bool dodalKIslandu = false;
                    foreach (var KeyPair in beliIsland)
                    {
                        if (KeyPair.Value.amFull || (KeyPair.Value.maxSize != value))
                        {
                            continue;
                        }
                        if (KeyPair.Value.seMeDrzi(np))
                        {
                            //ce se drzim islanda dodam temu islandi in popravim svoj id da je enak
                            
                            beliIsland[KeyPair.Key].addPoint(np);
                            np.id = beliIsland[KeyPair.Key].id;
                            arrBeli[j].Add(np);
                            removeFrom(ref arrNull, np );
                            dodalKIslandu = true;
                            //preverit se moramo ce smo povezali kaki isl
                            var sos = getSosede(np);
                            foreach (var s in sos)
                            {
                                if (s.seDrzimMasterja == false)
                                {
                                    s.seDrzimMasterja = true;
                                    //todo spodnji linq ne spremeni sranja
                                    beliIsland[KeyPair.Key].Points.Remove(s);
                                    beliIsland[KeyPair.Key].Points.Add(s);
                                    var debugTemp = 0;
                                    

                                }
                            }
                            break;
                        }
                    }

                    if (!dodalKIslandu)
                    {
                        //ce se ne drzim islanda ustvarim island in prilagodim id
                        var isl = new Island(i,j,val:value,id:beliIslandId);
                        beliIsland[beliIslandId] = isl;
                        np.id = beliIsland[beliIslandId].id;
                        beliIslandId++;
                        arrBeli[j].Add(np);
                        removeFrom(ref arrNull, np );
                    }
                }
                arrNull[j].Remove(new nurikabePoint(i, j));
            }
        }
        }

    //ogradi bel island z crnimi
    public void ogradiIsland(int islandId)
    {
        
        var isl = beliIsland[islandId];
        if (!isl.amFull)
        {
            throw new InvalidOperationException("ne mores ogradit ne polnega otoka");
        }
        foreach (var point in isl.Points)
        {
            //todo preveri ce je setano sranje vredu
            if (point.x+1 < dim[0])
            {
                if ( this[point.x+1,point.y] == 0)
                {
                    this[point.x + 1, point.y] = -1;
                }
            }
            if (point.x-1 > -1)
            {
                if ( this[point.x-1,point.y] == 0)
                {
                    this[point.x - 1, point.y] = -1;
                }
            }
            if (point.y+1 < dim[1])
            {
                if ( this[point.x,point.y+1] == 0)
                {
                    this[point.x, point.y+1] = -1;
                }
            }
            if (point.y-1 > -1)
            {
                if ( this[point.x,point.y-1] == 0)
                {
                    this[point.x, point.y-1] = -1;
                }
            } 
            
        }
    }

    //metoda ki previri ce so pozicije ki morajo biti zapolnjene
    private bool semOgrajen(nurikabePoint orPt)
    {
        if (orPt.val!=0)
        {
            return false;
        }
        if (orPt.y-1 > 0 && orPt.x+1 < this.dim[0])
        {
        if ((this[orPt.x,orPt.y-1]+this[orPt.x+1,orPt.y-1]+this[orPt.x+1,orPt.y]) == -3)
        {
            return true;
        }
        }
        
        if (orPt.y-1 >= 0 && orPt.x-1 >= 0)
        {
        if ((this[orPt.x,orPt.y-1]+this[orPt.x-1,orPt.y-1]+this[orPt.x-1,orPt.y]) == -3)
        {
            return true;
        }
        }
        
        if (orPt.y+1 < this.dim[1] && orPt.x-1 >=0)
        {
        if ((this[orPt.x,orPt.y+1]+this[orPt.x-1,orPt.y+1]+this[orPt.x-1,orPt.y]) == -3)
        {
            return true;
        }
        }
        
        if (orPt.y+1 < this.dim[1] && orPt.x+1 <this.dim[0])
        {
        if ((this[orPt.x+1,orPt.y]+this[orPt.x,orPt.y+1]+this[orPt.x+1,orPt.y+1]) == -3)
        {
            return true;
        }
        }

        return false;
    }
    public void getOgrajeneZCrno()
    {
        this.ograjeniZcrno = new List<nurikabePoint>();
        for (int i = 0; i < this.arrNull.Count; i++)
        {
            for (int i1 = 0; i1 < this.arrNull[i].Count; i1++)
            {
                if (semOgrajen(arrNull[i][i1]))
                {
                    ograjeniZcrno.Add(arrNull[i][i1]);
                }
            }
        }
    }

    public (bool, List<List<nurikabePoint>>) getMoznostiRazsirjanjaCrnih(int blackIslId, bool vse = false, int limitPos = 1000)
    {
        //preverimo ce obstaja enolicna resitev za razsiranje crnih
        bool tru = false;
        if (crniIsland.Count == 1)
        {
            return (false,null);
        }

        var blackIsl = crniIsland[blackIslId];
        var open = new List<nurikabePoint>();
        int steviloNajdenih = 0;
        bool semNasoNovOtok = false;
        List<nurikabePoint> obdelani = new List<nurikabePoint>();

        foreach (var pt in blackIsl.Points)
        {
            //vzamemo sosede vseh in preverimo ce so prazni in jih ze nimamo
            var sos = getSosede(pt);
            foreach (var sosed in sos)
            {
                if (sosed.val == 0)
                {
                    if (open.Contains(sosed))
                    {
                        continue;
                    }
                    var sosCpy = sosed.copy();
                    sosCpy.kogaSeDrzim = new[] {pt.x, pt.y};
                    sosCpy.id = pt.id;
                    sosCpy.val = pt.val;
                    open.Add(sosCpy);
                }
            }
            obdelani.Add(pt);

        }
            List<nurikabePoint> resitve = new List<nurikabePoint>();

            //gremo skozi vse potencialne razsiritvene poti
            while (open.Count > 0)
            {
                if (open.Count > limitPos)
                {
                    return (false,null);
                }
                var obdelujem = open[0];
                var sosObdelanega = getSosede(obdelujem);
                var potencialniNovi = new List<nurikabePoint>();

                var semNasel = false;
                foreach (var novSos in sosObdelanega)
                {
                    if (novSos.id < 0 && novSos.id != obdelujem.id)
                    {
                        //nasli smo novo pot do novega crnega otoka
                        resitve.Add(obdelujem);
                        obdelani.Add(obdelujem);
                        steviloNajdenih++;
                        tru = true;
                        if (steviloNajdenih > 1 && vse == false)
                        {
                            return (false,null);
                        }

                        semNasel = true;
                        open.Remove(obdelujem);
                        break;
                    }
                    else if(novSos.val == 0)
                    {
                        if (obdelani.Contains(novSos))
                        {
                            continue;
                        }
                        if (resitve.Contains(novSos))
                        {
                            continue;
                        }
                        var tempCp = novSos.copy();
                        tempCp.id = obdelujem.id;
                        tempCp.val = obdelujem.val;
                        tempCp.kogaSeDrzim = new[] {obdelujem.x, obdelujem.y};
                        potencialniNovi .Add(novSos);
                    }

                }
                //ce najdem prazno polje ga dodam med potencialne poti
                if (semNasel == false && potencialniNovi.Count > 0)
                {
                    foreach (var poc in potencialniNovi)
                    {
                        open.Add(poc);
                    }
                }

                if (!semNasel)
                {
                    open.RemoveAt(0);
                }
            }
            //pretvorim resitve v moznosti razsiritev v obliki tock
            var ls = new List<List<nurikabePoint>>();
            foreach (var res in resitve)
            {
                var tmpLs = new List<nurikabePoint>();
                var tmp = res;
                while (tmp.kogaSeDrzim[0] != -1 && tmp.kogaSeDrzim[1]!=-1)
                {
                    tmpLs.Add(res);
                    var temp = from obd in obdelani
                        where obd.x == res.kogaSeDrzim[0] && obd.y == res.kogaSeDrzim[1]
                        select obd;
                    var tt2 = 0;
                    tmp = temp.ToList()[0];
                }
                ls.Add(tmpLs);
            }
            var tt = 0;
            return (tru, ls);
    }
    public bool lahkoPlacamSemBlock(nurikabePoint pt, int ignoriraj = 0)
    {
        if (pt.val != 0)
        {
            return false;
        }
        bool ig = true;
        if (ignoriraj == 0)
        {
            ig = false;
        }
        var sos = getSosede(pt);
        foreach (var s in sos)
        {
            if(s.val == 0) continue;
            if (s.id > 0)
            {
                if (ig)
                {
                    if (s.id == ignoriraj)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public (bool,nurikabePoint) aliMamEnIzhod(Island isl)
    {
        //preveri ce ma neki island samo en izhod
        
        
        int c = 0;
        nurikabePoint outNp = new nurikabePoint();
        for (int i = 0; i < isl.Points.Count ; i++)
        {
            var tempPf = isl.Points[i];
            if (tempPf.x+1 < dim[0])
            {
                if (this[tempPf.x+1,tempPf.y]==0)
                {
                    c++;
                    outNp = this.getPoint(tempPf.x + 1, tempPf.y);
                }
            }
            if (tempPf.x-1 > -1)
            {
                if (this[tempPf.x-1,tempPf.y]==0)
                {
                    c++;
                    outNp = this.getPoint(tempPf.x - 1, tempPf.y);
                }
            }
            if (tempPf.y+1 < dim[1])
            {
                if (this[tempPf.x,tempPf.y+1]==0)
                {
                    c++;
                    outNp = this.getPoint(tempPf.x, tempPf.y+1);
                }
            }
            if (tempPf.y-1 > -1 )
            {
                if (this[tempPf.x,tempPf.y-1]==0)
                {
                    c++;
                    outNp = this.getPoint(tempPf.x, tempPf.y-1);
                }
            }
            if (c > 1)
            {
                return (false,outNp);
            }
        }
        if (c == 1)
        {
            return (true,outNp);
        }
        return (false,outNp);
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

    public List<nurikabePoint> getSosede(nurikabePoint n)
    {
        var ret =new List<nurikabePoint>();
        if (n.x-1>-1)
        {
            ret.Add(getPoint(n.x-1,n.y));
        }
        if (n.x+1<dim[0])
        {
            ret.Add(getPoint(n.x+1,n.y));
        }
        if (n.y-1>-1)
        {
            ret.Add(getPoint(n.x,n.y-1));
        }
        if (n.y+1<dim[1])
        {
            ret.Add(getPoint(n.x,n.y+1));
        }

        return ret;
    }
    public List<nurikabePoint> getSosedeOdOtoka(nurikabePoint n, Island isl)
    {
        var ret =new List<nurikabePoint>();
        
        var pt1 = isl.Points.Find(a => (a.x == n.x-1 && a.y == n.y));
        var pt2 = isl.Points.Find(a => (a.x == n.x+1 && a.y == n.y));
        var pt3 = isl.Points.Find(a => (a.x == n.x && a.y == n.y+1));
        var pt4 = isl.Points.Find(a => (a.x == n.x && a.y == n.y-1));
        if (pt1 != null)
        {
            ret.Add(pt1);
        }
        if (pt2 != null)
        {
            ret.Add(pt2);
        }
        if (pt3 != null)
        {
            ret.Add(pt3);
        }
        if (pt4 != null)
        {
            ret.Add(pt4);
        }
            
        return ret;
    }
    public (bool, List<List<nurikabePoint>>) aliLahkoPridemIzAvB(Island A, nurikabePoint B, bool vsePoti = false)
    {

        (var tru, var poti) = doTheA(A, B,vsePoti:vsePoti);
        return (tru, poti);
    }

    public (bool, List<List<nurikabePoint>>) aliLahkoPridemIzAvBVsePoti(Island A, nurikabePoint B)
    {

        (var tru, var poti) = doTheA2(A, B);
        return (tru, poti);
    }

    private ( bool, List<List<nurikabePoint>>) doTheA2(Island isl, nurikabePoint B,  int ubijSeNa = Int32.MaxValue)
    {
        var output = new List<List<nurikabePoint>>();
        
        //ustvarimo open
        var open = new List<List<nurikabePoint>>();
        for (int i = 0; i < isl.Points.Count; i++)
        {
            var openList = new List<nurikabePoint>();
            openList.Add(isl.Points[i]);
            open.Add(openList);
        }

        while (open.Count > 0)
        {
            if (open.Count > 1000)
            {
                return (false, null);
            }
            //predelamo tocke
            //vzamemo en open in zadnjo tocko, ter preverimo ce je ok
            var odprtaTocka = open[0].Last();
            if (odprtaTocka == B)
            {
                output.Add(open[0]);
                open.RemoveAt(0);
                continue;
            }
            
            //vzamemo potencialne mozne razsiritve
            var potencialniNovi = this.getSosede(odprtaTocka);
            for (int i = 0; i < potencialniNovi.Count; i++)
            {
                bool splohDosezem = false;
                //ce je ze ta tocka v tej poti jo ignoriramo
                if (open[0].Contains(potencialniNovi[i]))
                {
                    continue;
                }
                //ce je primerna za razsiritev jo dodamo
                if(lahkoPlacamSemBlock(potencialniNovi[i], open[0].Last().id))
                {
                    //preverimo ce sploh lahko dosezemo cilj
                    //todo se zadeva brejka ko je prevec osamelih tock?
                    int dist = razdaljaMedKvadratoma( open[0].Last(), B);
                    int availableDist =  isl.maxSize -  isl.size - open[0].Count + 1;
                    if (dist<availableDist)
                    {
                        splohDosezem = true;
                    }

                    if (splohDosezem)
                    {
                        //todo se sranje kopira sploh? al se samo ptr premakne
                        var varTmp = open[0].ToArray();
                        var newOpen = new List<nurikabePoint>(varTmp);
                        newOpen.Add(potencialniNovi[i]);
                        open.Add(newOpen);
                    }
                }
            }
            //ko preverim sosede odstranim iz open
            open.RemoveAt(0);
        }

        return (output.Count>0,output);
    }
    
    private ( bool,List<List<nurikabePoint>>) doTheA(  Island isl, nurikabePoint B, bool vsePoti = false)
    {

        var output = new List<List<nurikabePoint>>();
        bool outBool = false;

        if (isl.amFull)
        {
            return (outBool, output);
        }
        
        var odprtePoti = new List<nurikabePoint>();
        var zaprtePoti = new List<nurikabePoint>();
        //vzamem tocke iz islanda in jih dodadm v odprte poti

        var splohDosezem = false;
        foreach (var ptr in isl.Points)
        {
            /*
            if (ptr.seDrzimMasterja == false)
            {
                continue;
            }
            */
            int dist = razdaljaMedKvadratoma( ptr, B);
            int availableDist =  isl.maxSize -  isl.size;
            if (dist<availableDist)
            {
                splohDosezem = true;
            }
            else
            {
                continue;
            }
            var ptCp = ptr.copy();
            ptCp.val = isl.maxSize - isl.size;
            odprtePoti.Add(ptCp);
            //debug
            //Console.WriteLine("dodam root node: "+ptCp.x+","+ptCp.y);
            
        }

        if (!splohDosezem)
        {
            return (outBool, output);
        }
        while (odprtePoti.Count > 0)
        {
            //todo problem je ker nena poti hranijo svojega historyja in zberemo tukaj prvega ki smo ga dali v arr
            var Obdelujem = odprtePoti[0];
            //Console.WriteLine("Obdelujem Node: "+Obdelujem.x+","+Obdelujem.y);

            //ce sem naso pot jo izpisem
            if (Obdelujem == B)
            {
                var Pot = new List<nurikabePoint>();
                //Console.WriteLine("Nasel pot: ");

                var tt = Obdelujem;
                //Console.WriteLine(tt.x+","+tt.y);
                Pot.Add(tt);
                while (tt.kogaSeDrzim[0] != -1 && tt.kogaSeDrzim[1] != -1)
                {
                    //Console.WriteLine(tt.kogaSeDrzim[0]+","+tt.kogaSeDrzim[1]);
                    var temp1 = new nurikabePoint(tt.kogaSeDrzim[0],tt.kogaSeDrzim[1]);
                    Pot.Add(temp1);
                    var temp =  from s in zaprtePoti
                        where s.x == tt.kogaSeDrzim[0] && s.y == tt.kogaSeDrzim[1]
                        select s;
                    tt = temp.ToList()[0];
                } 
                output.Add(Pot);
                odprtePoti.RemoveAt(0);
                outBool = true;
                if(!vsePoti) break;
                continue;

            }
            var potencialniNovi = getSosede(Obdelujem);
            //preverim vse potencialne nove kandidate 
            foreach (var pocNov in potencialniNovi)
            {
                 //preverim ce so sploh prazni
                 if(pocNov.val != 0 && !(pocNov.id == isl.id)) continue;
                 //Console.WriteLine("Potencialni novi node: "+pocNov.x+","+pocNov.y);
                 
                 //preverim ce sem ga ze obdelal
                 //todo preveri ce dela contains zdaj
                 if (!vsePoti)
                 {
                     //Console.WriteLine("Ze obstaja: "+pocNov.x+","+pocNov.y);
                     if (zaprtePoti.Contains(pocNov)) continue;
                     if (odprtePoti.Contains(pocNov)) continue;
                 }
                 
                 //preverim ce se drzim kakih belih
                 bool seDrzimBelih = false;
                 var tempSos = getSosede(pocNov);
                 foreach (var sosPocNov in tempSos)
                 {
                     if (sosPocNov.val > 0 && sosPocNov.id != isl.id)
                     {
                         //ce se drzim ateja je to ok
                         if (sosPocNov.x ==  Obdelujem.x && sosPocNov.y == Obdelujem.y) continue;
                         seDrzimBelih = true;
                         //Console.WriteLine("Node ni primeren: "+sosPocNov.x+","+sosPocNov.y);
                         break;
                     }
                 }
                 if (seDrzimBelih) continue;
                 
                 //naredim kopijo in preverim ce sploh dosezem cilj
                 var copPocNov = pocNov.copy();
                 copPocNov.val = Obdelujem.val - 1;
                 var razMedPocNov = razdaljaMedKvadratoma(copPocNov, B);
                 if (copPocNov.x != B.x && copPocNov.y != B.y )
                 {           
                     razMedPocNov++;
                 }
                 if (razMedPocNov > (copPocNov.val))
                 {
                     //Console.WriteLine("Prevelika razdalja: "+copPocNov.x+","+copPocNov.y);

                     continue;
                 } 
                 //ce grem mimo vse pogoje ga dodam v odprte poti
                 copPocNov.kogaSeDrzim[0] = Obdelujem.x;
                 copPocNov.kogaSeDrzim[1] = Obdelujem.y;
                 odprtePoti.Add(copPocNov);
                 //Console.WriteLine("grem proti: " + copPocNov.x+","+copPocNov.y + " val: "+copPocNov.val);
                 var tt = 0;

            }
            zaprtePoti.Add(odprtePoti[0]);
            //Console.WriteLine("Dodam: "+odprtePoti[0].x+","+odprtePoti[0].y);
            odprtePoti.RemoveAt(0);
        }

        return (outBool, output);
    }

    public Island combineIslands(Island island1, Island island2)
    {
        for (int i = 0; i < island2.size; i++)
        {
            var nurikabePoint = island2.Points[i];
            nurikabePoint.id = island1.Points[0].id;
            nurikabePoint.val = island1.Points[0].val;
            island1.addPoint(nurikabePoint);
            setValNId(nurikabePoint.x,nurikabePoint.y,nurikabePoint.id,nurikabePoint.val);
        }
        return island1;
    }
    
    public void ChangeTheIdsInIsland(Island isl)
    {
        foreach (var pt in isl.Points)
        {
                setValNId(pt.x,pt.y,pt.val,pt.id);
        }
    }

    //ta metoda ustvari vse možne otoke
    public (bool, List<Island>) dobiVseMozneIslande(Island isl, bool mamVecKotPa2 = false)
    {
        List<Island> open = new List<Island>();
        List<Island> moznosti = new List<Island>();
        var izhod = false;
        var temp = isl.copy();
        open.Add(temp);
        int cnt = 0;
        //gremo skozi vse otoke namenjeni predelavi
        while (open.Count>cnt )
        {
            var obdelovan = open[cnt];
            //ce je otok poln ga dodamo med moznosit in povecamo cnt
            if (obdelovan.amFull)
            {
                izhod = true;       
                moznosti.Add(obdelovan);
                cnt++;
                if (mamVecKotPa2 == true && cnt > 1)
                {
                    return (false, moznosti);
                }
                continue;
            }
            //gremo skozi vse njihove tocke in jih probamo razsirit
            for (int i = 0; i < obdelovan.Points.Count; i++)
            {
                var sos = getSosede(obdelovan.Points[i]);
                for (int j = 0; j < sos.Count; j++)
                {
                    if(obdelovan.Points.Contains(sos[j]))
                    {
                        continue;
                    }
                    
                    //preverimo ce je mogoce polozit tocko tu
                    if (lahkoPlacamSemBlock(sos[j], obdelovan.id))
                    {
                        var tmpIsl = obdelovan.copy();
                        var sosCop = sos[j].copy();
                        tmpIsl.addPoint(sosCop);
                        //ce je otok mozen ga dodamo med odprte otoke
                        if (!open.Contains(tmpIsl))
                        {
                            open.Add(tmpIsl);
                        }
                    }
                }
            }
            open.RemoveAt(cnt);
        }
        return (izhod,moznosti);
    }
    
    
    
    public (bool, List<Island>) dobiVseMozneIslande2(Island isl, bool mamVecKotPa2 = false)
    {
        List<Island> open = new List<Island>();
        List<Island> moznosti = new List<Island>();
        var izhod = false;
        var temp = isl.copy();
        open.Add(temp);
        int cnt = 0;
        //gremo skozi vse otoke namenjeni predelavi
        while (open.Count>cnt )
        {
            var obdelovan = open[cnt];
            if(misc.debug)obdelovan.printMe();
            //ce je otok poln ga dodamo med moznosit in povecamo cnt
            if (obdelovan.amFull)
            {
                izhod = true;       
                moznosti.Add(obdelovan);
                cnt++;
                if (mamVecKotPa2 == true && cnt > 1)
                {
                    return (false, moznosti);
                }
                continue;
            }
            //gremo skozi vse njihove tocke in jih probamo razsirit
            for (int i = 0; i < obdelovan.Points.Count; i++)
            {
                var sos = getSosede(obdelovan.Points[i]);
                for (int j = 0; j < sos.Count; j++)
                {
                    if(obdelovan.Points.Contains(sos[j]))
                    {
                        continue;
                    }
                    
                    //preverimo ce je mogoce polozit tocko tu
                    if (lahkoPlacamSemBlock(sos[j], obdelovan.id))
                    {
                        var tmpIsl = obdelovan.copy();
                        var sosCop = sos[j].copy();
                        tmpIsl.addPoint(sosCop);
                        //ce je otok mozen ga dodamo med odprte otoke
                        if (!open.Contains(tmpIsl))
                        {
                            open.Add(tmpIsl);
                        }
                    }
                }
            }
            open.RemoveAt(cnt);
        }
        return (izhod,moznosti);
    }

    public bool semPovezanIsl(Island isl)
    {
        // a) mislim da to steje povezave b) mislim da ne gledamo sosedov znotra isl samo 
        int velikostKiJoRabim = isl.size;
        int velikostKiJoImam = 0;
        
        var open = new List<nurikabePoint>();
        var closed = new List<nurikabePoint>();
        open.Add(isl.Points[0]);
        while (open.Count>0)
        {
            var sosedi = getSosedeOdOtoka( open[0],isl);
            foreach (var sos in sosedi)
            {
                if (sos.id == isl.id)
                {
                    if (open.Contains(sos) || closed.Contains(sos))
                    {
                        continue;
                    }
                    
                    //Console.Write(open[0].x + ","+open[0].y +" - "+sos.x+","+sos.y + " ");
                    open.Add(sos);
                }
            }
            closed.Add(open[0]);
            open.RemoveAt(0);
            velikostKiJoImam++;
        }

        return velikostKiJoImam == velikostKiJoRabim;
    }

    private bool comparePointLists(List<nurikabePoint> a, List<nurikabePoint> b)
    {
        if (a.Count != b.Count) return false;
        
        foreach (var p in b)
        {
            if (a.Contains(p) == false) return false;
        }
    
        return true;
    }

    private bool ContainstAbomination(List<(Island, List<nurikabePoint>)> ls, (Island,List<nurikabePoint>) ele)
    {
        if (ls.Count == 0)
        {
            return false;
        }
        foreach (var abo in ls)
        {
            var tru1 = comparePointLists(abo.Item1.Points,ele.Item1.Points);
            if (tru1 == false) return false;
            var tru2 = comparePointLists(abo.Item2,ele.Item2);
            if (tru2 == false) return false;
        }
        return true;
    }
    
    public List<nurikabePoint> dobiMoznostiRazsirjanjaOtoka(Island a)
    {
        var outp = new List<nurikabePoint>();
        foreach (var pt in a.Points)
        {
            var sosedi = getSosede(pt);
            foreach (var sosed in sosedi)
            {
                //dodamo potencialno sosede
                if (sosed.val == 0 && outp.Contains(sosed) == false && lahkoPlacamSemBlock(sosed, a.id) && a.Points.Contains(sosed) == false)
                {
                    outp.Add(sosed);
                }
            }
        }

        return outp;
    }
    
    
    public (bool, List<List<nurikabePoint>>) dobiVseMoznostiZdruzevanjaOtoka(Island isl)
    {
        if (isl.semPovezan)
        {
            return (false, null);
        }
        //tu pa smesno rata ker se "tocka" ne more samo razsirit k svojim sosedom ampak v vse smeri ki se lahko tudi ostale tocke razsirijo
        // zato bo open island (odprte tocke, pot)
        //digusting code
        var open =new  List<(Island, List<nurikabePoint>)>();
        var closed =new  List<(Island, List<nurikabePoint>)>();
        
        var result = new List<List<nurikabePoint>>();
        
        open.Add((isl.copy(),new List<nurikabePoint>()));
        while (open.Count > 0)
        {
            if (open.Count > 1000)
            {
                return (false, new List<List<nurikabePoint>>());

            }
            var obdelujem = open[0];
            //ce je poln ga odstranim
            if (open[0].Item1.amFull) {

                closed.Add(open[0]);
                open.RemoveAt(0);
                continue;
            }
            
            var moznostiOdprtja = dobiMoznostiRazsirjanjaOtoka(obdelujem.Item1);
            for (int i = 0; i < moznostiOdprtja.Count; i++)
            {
                var cpyEleLsArr = obdelujem.Item2.ToArray();
                var cpyEleLs = new List<nurikabePoint>(cpyEleLsArr);

                var tmpEle = (obdelujem.Item1.copy(),cpyEleLs);
                tmpEle.Item1.addPoint(moznostiOdprtja[i]);
                tmpEle.Item2.Add(moznostiOdprtja[i]);

                if(ContainstAbomination(open,tmpEle))continue;
                if(ContainstAbomination(closed,tmpEle))continue;
                
                if (semPovezanIsl(tmpEle.Item1))
                {
                    result.Add(tmpEle.Item2);
                    closed.Add(tmpEle);
                    semPovezanIsl(tmpEle.Item1);
                    /*Console.Write("naso ");
                    //debug sprintam vrednosti v arr
                    foreach (var tt in tmpEle.Item2)
                    {
                        Console.Write(tt.x+","+tt.y+"  ");
                    }
                    Console.WriteLine(); */
                    continue;
                }
                /*
                Console.Write("dodam abm. z potjo ");
                //debug sprintam vrednosti v arr
                foreach (var tt in tmpEle.Item2)
                {
                    Console.Write(tt.x+","+tt.y+"  ");
                }
                Console.WriteLine();
                */
                open.Add(tmpEle);
                
            }
            open.RemoveAt(0);
        }
        return (result.Count>0, result);
    }

    public void narediIslCeli(int islKey)
    {
        foreach (var pt in beliIsland[islKey].Points)
        {
            pt.seDrzimMasterja = true;
        }

        beliIsland[islKey].semPovezan = true;

    }
    }
}
