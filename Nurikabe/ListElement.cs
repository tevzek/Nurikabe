using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nurikabe
{
    //en element linked lista
    public class ListElement
    {
        public List<nurikabePoint> posibilities;
        public int[,] arr;
        public bool deadEnd;
        public List<Island> activeIslands;
        public int id;
        public ListElement father;
        public ListElement[] children = new ListElement[2];
        public bool semResitev = false;
        public int kolkoOtrokSemObdelal = 0;

        public ListElement(List<nurikabePoint> posibilities, int[,] arr, List<Island> isl, int id, bool deadEnd = false)
        {
            this.posibilities = posibilities;
            this.arr = arr;
            this.deadEnd = deadEnd;
            this.activeIslands = isl;
            this.id = id;

        }

        public (List<nurikabePoint>, List<nurikabePoint>) getChainOfRougeWhitesFromSeed(nurikabePoint np)
        {
            //najdem vse povezane tocke rogu so ++ none rouge so 03 ipd

            var rougeOut = new List<nurikabePoint>();
            var normalOut = new List<nurikabePoint>();
            var closed = new List<nurikabePoint>();
            var opn = dobiSosede(ref arr, posibilities[0]);
            while (opn.Count > 0)
            {
                var obdelujem = opn[0];
                if ((obdelujem.val > 0) == false || closed.Contains(obdelujem))
                {
                    opn.RemoveAt(0);
                    closed.Add(obdelujem);
                    continue;
                }

                if (obdelujem.val > 100)
                {
                    rougeOut.Add(obdelujem);
                }
                else
                {
                    normalOut.Add(obdelujem);
                }

                closed.Add(obdelujem);

                var sosTmp = dobiSosede(ref arr, obdelujem);
                foreach (var s in sosTmp)
                {
                    if (closed.Contains(s) == false && s.val > 0)
                    {
                        opn.Add(s);
                    }
                }

                opn.RemoveAt(0);

            }

            return (rougeOut, normalOut);
        }

        public bool semZadelalCrneOtoke(List<nurikabePoint> sos, ref int[,] arr)
        {
            var jaz = sos[sos.Count - 1];
            //metoda ki prejme crne tocke in preveri ce imajo ti otoki izhod
            var ret = false;
            var tru = true;
            //dobim crne sosede iz sosedov
            List<nurikabePoint> crniSos = new List<nurikabePoint>();
            foreach (var s in sos)
            {
                if (s.val < 0)
                {
                    crniSos.Add(s);
                }
            }
            if (crniSos.Count == 0)
            {
                return false;
            }
            
            List<nurikabePoint> Obdelani = new List<nurikabePoint>();
            List<nurikabePoint> Q = new List<nurikabePoint>();

            var obdelujem = crniSos[0];
            
            while (crniSos.Count > 0)
            {
                bool cont = false;
                var sosedi = dobiSosede(ref arr, obdelujem);
                
                foreach (var s in sosedi)
                {
                    //crne sosede dam v Q
                    if (s.val < 0 && !Obdelani.Contains(s) && !Q.Contains(s))
                    {
                        Q.Add(s);
                    }
                    //ce sem sosed z praznim sosedom nisem zadelan
                    if (s.val == 0 && s != jaz)
                    {
                        Obdelani = new List<nurikabePoint>();
                        Q = new List<nurikabePoint>();
                        
                        crniSos.RemoveAt(0);
                        if (crniSos.Count == 0) return false;
                        obdelujem = crniSos[0];
                        cont = true;
                    }
                }
                if(cont) continue;
                
                //ce nimam vec kaj za obdelat sem zapt
                if (Q.Count == 0)
                {
                    return true;
                }
                Obdelani.Add(obdelujem);
                obdelujem = Q[0];
                Q.RemoveAt(0);
                   
                
            }

            return ret;
        }
    


    public string narisiMe(int[,] arr  )
        {
            string outz = "";
            if (arr == null) arr = this.arr;
            for (int i = 0; i<arr.GetLength(1); i++)
            {
                //Console.WriteLine();
                outz += "\n";
                for (int j = 0; j<arr.GetLength(0); j++)
                {
                    if (arr[j, i] > 0)
                    {
                        if(arr[j,i] != Int32.MaxValue)
                        //Console.Write("0" + arr[j,i]+ " ");
                        outz += "0" + arr[j,i]+ " ";

                        else
                        {
                            //Console.Write("NN ");
                            outz += "NN ";
                        }
                    }
                    else if (arr[j, i] == 0)
                    {
                        //Console.Write("-- ");
                        outz += "-- ";
                    }
                    else if (arr[j, i] < 0)
                    {
                        //Console.Write("++ ");
                        outz += "++ ";
                    }
                    
                }
            }
            outz += "\n\n";
            /*
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            ''' */
            return outz;
        }

        private List<Island> copyIslList(List<Island> inList)
        {
            List<Island> outList = new List<Island>();
            for (int i = 0; i < inList.Count; i++)
            {
                outList.Add(inList[i].copy());
            }
            return outList;
        }
        
        public void razvijFroca(int id)
        {
            
            var cpyEleLsArr = posibilities.ToArray();
            var cpyEleLs = new List<nurikabePoint>(cpyEleLsArr);
            
            //to ne kopira vredu sranja?
            var cpyEleLs2 = copyIslList(activeIslands);
            
            var froc = new ListElement(cpyEleLs,arr.Clone() as int[,],cpyEleLs2,id);
            
            int vrednost = 0;
            if (kolkoOtrokSemObdelal == 0)
            {
                vrednost = -1;
                //preverim za sosede ce so 2*2 crni
                var np = new nurikabePoint(posibilities[0].x, posibilities[0].y);
                var pointi = dobiSteviloCrnihKvadratov(ref arr, np);
                if (pointi != 0)
                {
                    froc.deadEnd = true;
                    if (misc.debug)
                    {
                        Console.WriteLine("2*2 otok");
                    }
                }
                else
                {
                    //preverim ce sem zadelal kakega belega
                    foreach (var isl in activeIslands)
                    {
                        if(isl.amFull) continue;
                        var izhodi = izhodiIsl(isl);
                        if (izhodi.Count == 1 && izhodi.Contains(np))
                        {
                            froc.deadEnd = true;
                            if (misc.debug)
                            {
                                Console.WriteLine("Zadelal belega");
                            }
                        }
                    }
                }
                //preveri ce sem sebe dal na zadelano mesto todo ?
                if (!froc.deadEnd)
                {
                    
                    
                }
                froc.arr = arr.Clone() as int[,];
                froc.arr[np.x, np.y] = vrednost;
                froc.posibilities.RemoveAt(0);
                froc.father = this;
                //narisiMe(froc.arr);
                var tt = 0;
            }
            else if(kolkoOtrokSemObdelal == 1)
            {

                var sos = dobiSosede(ref arr, posibilities[0] );
                int stSosIsl = 0;
                int trenutniIslId = 0;
                int stNull = 0;
                vrednost = int.MaxValue;
                int islId = -1;
                int islPt = -1;
                
                //preverimo ce razsirimo kaki isl 
                foreach (var s in sos)
                {
                    if(froc.deadEnd) break;
                    if (s.val > 0)
                    {
                        foreach (var i in activeIslands)
                        {
                            if (i.Points.Contains(s))
                            {
                                // ce  smo polek polnega isl. je to dead end
                                if (i.amFull)
                                {
                                    froc.deadEnd = true;
                                    if (misc.debug)
                                        Console.WriteLine("Polek polnega islanda");
                                    break;    
                                }
                                //ce ne dodamo islandu zadevo (razn ce stikam 2 isl)
                                else
                                {
                                    //ce stikam 2 islanda
                                    if (stSosIsl == 1)
                                    {
                                        if (islId == i.id)
                                        {
                                            continue;
                                        }
                                        froc.deadEnd = true;
                                        if (misc.debug)
                                        {
                                            Console.WriteLine("Dva islanda sestikata");
                                        }
                                        break;
                                    }
                                    // ce ne
                                    else
                                    {
                                        //ker sem lazy in se mi ne da linq-ja checkupat
                                        for (int i2 = 0; i2 < froc.activeIslands.Count; i2++)
                                        {
                                            //najdemo otok in dodamo point
                                            if (froc.activeIslands[i2].id == i.id)
                                            {
                                                if (froc.activeIslands[i2].Points.Contains(posibilities[0]) == false)
                                                {
                                                    //v bistvu predem dodamo point preverimo če se dotikamo kake verige roge islandov
                                                    (var RougeOnes, var noneRougeOnes) = getChainOfRougeWhitesFromSeed(posibilities[0]);
                                                            
                                                   //ce smo prevelik otok ustvarili se ubijemo
                                                    int newIslSize = noneRougeOnes.Count + RougeOnes.Count +1;
                                                    if (newIslSize > froc.activeIslands[i2].maxSize)
                                                    {
                                                        froc.deadEnd = true;
                                                        if (misc.debug)
                                                        {
                                                            Console.WriteLine("Prevelik otok");
                                                        }
                                                        break;   
                                                    }
                                                    
                                                    //ce rouge otoki povzrocajo da se dotikam z drugim otokom se ubijem
                                                    bool seDotikam = false;
                                                    for (int j = 0; j < noneRougeOnes.Count; j++)
                                                    {
                                                        if (noneRougeOnes[j].val != noneRougeOnes[0].val)
                                                        {
                                                            seDotikam = true;
                                                            break;
                                                        }
                                                    }
                                                    if (seDotikam)
                                                    {
                                                        froc.deadEnd = true;
                                                        if (misc.debug)
                                                        {
                                                            Console.WriteLine("Rouge se dotika drugega otoka");    
                                                        }
                                                        break;   
                                                    }
                                                    else
                                                    {
                                                        foreach (var nr in noneRougeOnes)
                                                        {
                                                            if (froc.activeIslands[i2].Points.Contains(nr) == false)
                                                            {
                                                                if (!froc.activeIslands[i2].amFull)
                                                                {
                                                                    froc.activeIslands[i2].addPoint(nr);    
                                                                }
                                                                else
                                                                {
                                                                    froc.deadEnd = true;
                                                                    break;
                                                                }
                                                                
                                                            }
                                                        }
                                                        foreach (var nr in RougeOnes)
                                                        {
                                                            if (froc.activeIslands[i2].Points.Contains(nr) == false)
                                                            {
                                                                if (!froc.activeIslands[i2].amFull)
                                                                {
                                                                froc.activeIslands[i2].addPoint(nr);
                                                                }
                                                                else
                                                                {
                                                                    froc.deadEnd = true;
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (!froc.deadEnd && !froc.activeIslands[i2].amFull)
                                                        {
                                                            froc.activeIslands[i2].addPoint(posibilities[0]);  
                                                            islId = froc.activeIslands[i2].id;
                                                            islPt = i2;
                                                            //to dela vredi?
                                                            stSosIsl++;
                                                        }
                                                    }
                                                    break;    
                                                }
                                                //ce se dotikam istega isl breakam
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        
                                    }
                                }
                            }
                            if(froc.deadEnd) break;
                        }
                    }

                    else if (s.val == 0) stNull++;
                }
                // preverim ce sem se zapr ki med crne
                if (stNull == 0 && stSosIsl == 0) froc.deadEnd = true;
                // preverim ce sem se zapr ki med crne in nimam izhoda?
                else if (islPt != -1 && froc.activeIslands[islPt].amFull == false) {
                    var izh = izhodiIsl(froc.activeIslands[islPt]);
                    if(izh.Count == 0)froc.deadEnd = true;
                    if (misc.debug && izh.Count == 0)
                    {
                        Console.WriteLine("Zapr sem se med crne");
                    }
                }
                //preverim ce sem zapr kaksen crni otok (samo ce nisem poln)
                if (posibilities.Count > 0)
                {
                    //SOS ARAYU DODAM SEBE!!!
                    sos.Add(posibilities[0]);
                    bool semZadelal = semZadelalCrneOtoke(sos, ref arr);
                    if (semZadelal)
                    {
                        froc.deadEnd = true;
                        if (misc.debug)
                        {
                            Console.WriteLine("Zadelan crni");
                        }
                    }

                }

                froc.arr = arr.Clone() as int[,];
                froc.arr[posibilities[0].x, posibilities[0].y] = vrednost;
                froc.posibilities.RemoveAt(0);
                froc.father = this;
                //narisiMe(froc.arr);
                var tt = 0;
            }

            this.children[kolkoOtrokSemObdelal] = froc;
            kolkoOtrokSemObdelal++;
        }
        // to sem vzel iz gen alg        
        private int dobiSteviloCrnihKvadratov(ref int[,] arrT, nurikabePoint np)
        {
            int steviloKvadratov = 0;
            if (np.val > -1) return -1;
            //levi zgornji
            if (np.x - 1 > -1 && np.y - 1 > -1)
            {
                if (arrT[np.x - 1, np.y - 1] < 0 && arrT[np.x - 1, np.y] < 0 && arrT[np.x , np.y - 1] < 0)
                {
                    steviloKvadratov++;
                }
            }
            //desnji zgornji
            if (np.x + 1 < arr.GetLength(0) && np.y - 1 > -1)
            {
                if (arrT[np.x + 1, np.y - 1] < 0 && arrT[np.x + 1, np.y] < 0 && arrT[np.x , np.y - 1] < 0)
                {
                    steviloKvadratov++;
                }
            }
            //levi spodnji
            if (np.x - 1 > -1 && np.y + 1 < arr.GetLength(1))
            {
                if (arrT[np.x - 1, np.y + 1] < 0 && arrT[np.x - 1, np.y] < 0 && arrT[np.x , np.y + 1] < 0)
                {
                    steviloKvadratov++;
                }
            }
            //desnji spodni
            if (np.x + 1 < arr.GetLength(0) && np.y + 1 < arr.GetLength(1))
            {
                if (arrT[np.x + 1, np.y + 1] < 0 && arrT[np.x + 1, np.y] < 0 && arrT[np.x , np.y + 1] < 0)
                {
                    steviloKvadratov++;
                }
            }

            return steviloKvadratov;
        }
        
        List<nurikabePoint> dobiSosede(ref int[,] arrT, nurikabePoint n)
        {
            var ret =new List<nurikabePoint>();
            if (n.x-1>-1)
            {
                ret.Add(getPoint(n.x-1,n.y,ref arrT));
            }
            if (n.x+1<arr.GetLength(0))
            {
                ret.Add(getPoint(n.x+1,n.y,ref arrT));
            }
            if (n.y-1>-1)
            {
                ret.Add(getPoint(n.x,n.y-1,ref arrT));
            }
            if (n.y+1<arr.GetLength(1))
            {
                ret.Add(getPoint(n.x,n.y+1,ref arrT));
            }

            return ret;
            
        }
        nurikabePoint getPoint(int x, int y, ref int[,] arrT)
        {
            nurikabePoint np = new nurikabePoint(x,y,val:arrT[x,y]);
            return np;
        }
        
        //preverimo ce je to resitev
        private (int crniO, int beliO, int nepravilniB, int stikajociB, int steviloNepravilnihBelih) dobiSteviloOtokov(int[,] arrCop)
        {
            var steviloBelihOt = 0;
            var steviloPrevelikihBelih = 0;
            var steviloStikajocihBelih = 0;
            var steviloCrnihOt = 0;
            var kolicinaPremaloPrevecBelih = 0;
            
            //gremo skozi celi arr in ugotovimo koliko otokov ipd. je
            var open = new List<nurikabePoint>();
            var closed = new List<nurikabePoint>();
            //ustvarimo listo nepredelanih tock
            for (int y = 0; y<arr.GetLength(1); y++)
            {
                for (int x = 0; x<arr.GetLength(0); x++)
                {
                open.Add(new nurikabePoint(x,y,arrCop[x,y]));
                }
            }
            //predelamo vse tocke
            while (open.Count>0)
            {
                var obdelujem = open[0];
                
                //v primeru da je open [0] crn pogledamo kak velik je ta otok
                if (obdelujem.val < 0)
                {
                    //povecamo crne ot
                    steviloCrnihOt++;
                    
                    var openCrni = new List<nurikabePoint>();
                    openCrni.Add(obdelujem);
                    //vzamemo sosede in crne damo v openCrnir
                    var sos = dobiSosede(ref arrCop,obdelujem);
                    foreach (var s in sos)
                    {
                        if (s.val < 0)
                        {
                            openCrni.Add(s);    
                        }
                    }
                    openCrni.Remove(obdelujem);
                    open.Remove(obdelujem);
                    closed.Add(obdelujem);
                    //zdaj pa pogledamo kak velik je ta otok
                    while (openCrni.Count>0)
                    {
                        obdelujem = openCrni[0];
                        var sos2 = dobiSosede(ref arrCop,obdelujem);
                        foreach (var s in sos2)
                        {
                            if (openCrni.Contains(s) == false && closed.Contains(s) == false && s.val<0)
                            {
                                openCrni.Add(s);
                            }
                        }
                        //odstranimo iz open & close
                        open.Remove(obdelujem);
                        closed.Add(obdelujem);
                        openCrni.Remove(obdelujem);
                    }
                    // po moji logiki bi moglo to preverit stevilo posameznih otokov v arr
                }
                //v primeru da je open [0] bel pogledamo kak velik je ta otok in notiramo probleme
                else if (obdelujem.val > 0)
                {
                    //povecamo crne ot
                    steviloBelihOt++;
                    var seDrzimOtokaSploh = true;
                    int velikostOtoka = 1;
                    List<int> najdeneVelikosti = new List<int>();
                    
                    var openBeli = new List<nurikabePoint>();
                    openBeli.Add(obdelujem);
                    //preverimo ce je otok ne dodan beli tile
                    if(obdelujem.val != int.MaxValue) najdeneVelikosti.Add(obdelujem.val);
                    
                    //vzamemo sosede in crne damo v openCrnir
                    var sos = dobiSosede(ref arrCop,obdelujem);
                    foreach (var s in sos)
                    {
                        if (s.val > 0)
                        {
                            openBeli.Add(s);    
                        }
                    }
                    openBeli.Remove(obdelujem);
                    open.Remove(obdelujem);
                    closed.Add(obdelujem);
                    //zdaj pa pogledamo kak velik je ta otok
                    while (openBeli.Count>0)
                    {
                        obdelujem = openBeli[0];
                        var sos2 = dobiSosede(ref arrCop,obdelujem);
                        foreach (var s in sos2)
                        {
                            if (openBeli.Contains(s) == false && closed.Contains(s) == false && s.val>0)
                            {
                                openBeli.Add(s);
                            }
                        }
                        //povecamo velikost otoka
                        velikostOtoka++;
                        //preverimo ce je native otok to
                        if (obdelujem.val != int.MaxValue)
                        {
                            if (najdeneVelikosti.Contains(obdelujem.val) == false)
                            {
                                najdeneVelikosti.Add(obdelujem.val);
                            }   
                        }

                        //odstranimo iz open & close
                        open.Remove(obdelujem);
                        closed.Add(obdelujem);
                        openBeli.Remove(obdelujem);
                    }
                    // po moji logiki bi moglo to preverit stevilo posameznih otokov v arr
                    if (najdeneVelikosti.Count == 0)
                    {
                        steviloPrevelikihBelih++;
                    }
                    else if(najdeneVelikosti.Count == 1)
                    {
                        if (velikostOtoka != najdeneVelikosti[0])
                        {
                            steviloPrevelikihBelih++;
                            var dif = Math.Abs(velikostOtoka - najdeneVelikosti[0]);
                            kolicinaPremaloPrevecBelih += dif;

                        }
                    }
                    else
                    {
                        steviloStikajocihBelih+= najdeneVelikosti.Count;
                        steviloPrevelikihBelih++;
                        var dif = 0;
                        List<int> najdeneRaz = new List<int>();
                        for (int vel = 0; vel< najdeneVelikosti.Count; vel++)
                        {
                            int temp2 = velikostOtoka - najdeneVelikosti[vel];
                            var temp = Math.Abs(temp2);
                            if (najdeneRaz.Contains(temp2) == false)
                            {
                                najdeneRaz.Add(temp2);
                                dif+= temp;
                            }
                        }

                        kolicinaPremaloPrevecBelih += dif;

                    }
                }

            }
            return (steviloCrnihOt, steviloBelihOt, steviloPrevelikihBelih, steviloStikajocihBelih,kolicinaPremaloPrevecBelih);
        }

        public bool semJazResitev(betterArray ba)
        {
            (var steviloCrnih, var steviloBelih, var steviloPrevelikihBelih, var steviloStikajocihBelih,
                var kolicinaPrevecPreveliko) = dobiSteviloOtokov(this.arr);
            if (steviloCrnih != 1) return false;
            if (steviloBelih != ba.beliIsland.Count) return false;
            if (steviloPrevelikihBelih != 0) return false;
            if (steviloStikajocihBelih != 0) return false;
            
                return true;
        }
        
        public List<nurikabePoint> izhodiIsl(Island isl)
        {
            var izh = new List<nurikabePoint>();
            for (var i = 0;  i< isl.Points.Count; i++)
            {
                var sos = dobiSosede(ref arr, isl.Points[i]);
                foreach (var s in sos)
                {
                    if (s.val == 0 && izh.Contains(s) == false && isl.Points.Contains(s) == false)
                    {
                        izh.Add(s);       
                    }
                }
            }
            return izh;
        }

    }
}