using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nurikabe
{
    public class GenetskiAlg
    {
        private List<nurikabePoint> tockeZKaterimiDelam;
        private betterArray arr;
        private int pop;
        private int elita;
        private double moznostMutacije;
        private double moznostKrizanja;
        //[osebek, njegoi pointi]
        private nurikabePoint[][] popArray;
        private int[,] vhodniArrCop;
        private int velikostEnega;
        
        private int cenaCrnegaKvadrata = 1;
        private int cenaNaCrniOtok = 3;
        private int cenaOsamljenihBelih = 2;
        private int cenaNepravilnoVelikih = 4;
        private int cenaStikajocihOtokov = 2;
        private int cenaPrevecbelih = 2;
        private int cenaNepravilnihBelih = 1;

        private int randomFolk = 3;
        public GenetskiAlg(List<nurikabePoint> tockeZKaterimiDelam, betterArray arr, int pop = 100, int elita = 4, int randomFolk = 1, double moznostMutacije = 0.002, double moznostKrizanja = 0.3)
        {
            this.tockeZKaterimiDelam = tockeZKaterimiDelam;
            this.arr = arr;
            this.pop = pop;
            this.elita = elita;
            this.moznostMutacije = moznostMutacije;
            this.moznostKrizanja = moznostKrizanja;
            velikostEnega = tockeZKaterimiDelam.Count;
            vhodniArrCop = arr.arr;
            this.randomFolk = randomFolk;
        }

        private void izrisiEnega(nurikabePoint[] osebek)
        {
            var popArrT = osebek.ToList();
            for (int y = 0; y < arr.dim[1]; y++)
            {
                for (int x = 0; x < arr.dim[0]; x++)
                {
                    if (popArrT.Contains(new nurikabePoint(x,y)))
                    {
                        var tt = popArrT.Where(s => s.x == x && s.y == y).Select(s => s);
                        var temp = tt.ToList()[0];
                        if (temp.val > 0)
                        {
                            Console.Write(" NN");
                        }
                        if (temp.val < 0)
                        {
                            Console.Write(" ++");
                        }
                    }
                    else
                    {
                        if(arr[x,y] > 0)
                            Console.Write(" 0"+arr[x,y]);
                        else Console.Write(" XX");
                    }
                }
                Console.WriteLine();
            }
            fitness(osebek, true);
        }
        
        public void test()
        {
            //generiramo pop
            generirajPop();
            //izrisemo testni ele
            var popArrT = popArray[0].ToList();
            for (int y = 0; y < arr.dim[1]; y++)
            {
                for (int x = 0; x < arr.dim[0]; x++)
                {
                    if (popArrT.Contains(new nurikabePoint(x,y)))
                    {
                        var tt = popArray[0].Where(s => s.x == x && s.y == y).Select(s => s);
                        var temp = tt.ToList()[0];
                        if (temp.val > 0)
                        {
                            Console.Write(" NN");
                        }
                        if (temp.val < 0)
                        {
                            Console.Write(" ++");
                        }
                    }
                    else
                    {
                        if(arr[x,y] > 0)
                        Console.Write(" 0"+arr[x,y]);
                        else Console.Write(" XX");
                    }
                }
                Console.WriteLine();
            }
            fitness(popArray[0]);
            var tt2 = 0;
        }

        public void doTheEvolution(bool debug = false)
        {
            generirajPop();
            int maxfit = int.MaxValue; 
            Random rand = new Random(69);
            while (maxfit > 0)
            {
                double fitOcena = 0;
                //ocenimo pop
                List<Tuple<nurikabePoint[],int>> fitnessList = new List<Tuple<nurikabePoint[],int>>(); 
                for (int pop=0; pop<popArray.Length; pop++)
                {
                    fitOcena += (double)this.fitness(popArray[pop]);
                    fitnessList.Add(Tuple.Create(popArray[pop],this.fitness(popArray[pop])));
                }
                
                fitnessList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
                int najslabsi = fitnessList[fitnessList.Count - 1].Item2;
                if (debug)
                {
                    Console.WriteLine("najboljsi:");
                    izrisiEnega(fitnessList[0].Item1);
                    Console.WriteLine("Ocenapopulacije: "+fitOcena/(double)fitnessList.Count);
                }
                //naredimo ruleto
                double[] moznostPrezivetja = new double[popArray.Length];
                for (int i = 0; i < fitnessList.Count; i++)
                {
                    //moznostPrezivetja[i] =  ((double)najslabsi - (double)fitnessList[i].Item2)/(double)najslabsi;
                    moznostPrezivetja[i] = (double) (fitnessList.Count - i)/(double) fitnessList.Count;
                }
                
                //krizam sranje
                List<nurikabePoint[]> krizani = new List<nurikabePoint[]>();
                bool mamEnegaZaKrizaf = false;
                nurikabePoint[] kogaKrizam = new nurikabePoint[fitnessList[0].Item1.Length];
                double moznost;
                for (int i = 0; i<fitnessList.Count; i++)
                {
                    moznost = rand.NextDouble();
                    if (moznost <= moznostKrizanja)
                    {
                        if (mamEnegaZaKrizaf == false)
                        {
                            kogaKrizam = fitnessList[i].Item1;
                            mamEnegaZaKrizaf = true;
                        }
                        else
                        {
                            var froc = krizaj(kogaKrizam, fitnessList[i].Item1);
                            krizani.Add(froc[0]);
                            krizani.Add(froc[1]);
                            mamEnegaZaKrizaf = false;
                        }
                    }
                }
                //pobijem sranje
                int indx = 0;
                int randVal;
                for (int i = 0; i < fitnessList.Count; i++)
                {
                    if (i < elita)
                    {
                        popArray[i] = fitnessList[i].Item1;
                        continue;
                    }

                    if (i - randomFolk <= 0)
                    {
                        popArray[i] = generirajEnega();
                    }

                    if (rand.NextDouble() >moznostPrezivetja[i])
                    {
                        //preverim ce mam dovolj frocov
                        if (krizani.Count > 0)
                        {
                            randVal = rand.Next(krizani.Count);
                            popArray[i] = krizani[randVal];
                            krizani.RemoveAt(randVal);
                        }
                        else
                        {
                            popArray[i] = generirajEnega();
                        }
                    }
                    else
                    {
                        popArray[i] = fitnessList[i].Item1;
                    }

                }

                double mutacija;
                //zdaj pa se mutiram sranje
                for(int i = elita; i<popArray.Length;i++)
                {
                    for(int j = 0; j<popArray[i].Length;j++)
                    {
                        mutacija = rand.NextDouble();
                        if (mutacija <= this.moznostMutacije)
                        {
                            if (popArray[i][j].val < 0)
                            {
                                popArray[i][j] = new nurikabePoint(val:int.MaxValue,id:int.MaxValue,x:popArray[i][j].x,y:popArray[i][j].y);
                            }
                            else
                            {
                                popArray[i][j] = new nurikabePoint(val:int.MinValue,id:int.MinValue,x:popArray[i][j].x,y:popArray[i][j].y);
                            }
                        }
                    }
                }

                var tt = 0;
            }
        }

        private nurikabePoint[] generirajEnega()
        {
            int kolkoBelihImamo = 0;
            int kolkoBelihRabimo = 0;
            
            Random rad  = new Random(69);
            
            foreach (var belIsl in arr.beliIsland)
            {
                kolkoBelihRabimo += belIsl.Value.maxSize;
                kolkoBelihImamo += belIsl.Value.size;
            }

            double moznostDaJePointBeli = (double) kolkoBelihImamo / (double) kolkoBelihRabimo;

            var en = new nurikabePoint[velikostEnega];
            for (int j = 0; j < velikostEnega; j++)
            {
                nurikabePoint pt;
                double rand = rad.NextDouble();
                if (rand <= moznostDaJePointBeli)
                {
                    pt = new nurikabePoint(val:int.MaxValue,id:int.MaxValue,x:tockeZKaterimiDelam[j].x,y:tockeZKaterimiDelam[j].y);
                    en[j] = pt;
                }
                else
                {
                    pt = new nurikabePoint(val:int.MinValue,id:int.MinValue,x:tockeZKaterimiDelam[j].x,y:tockeZKaterimiDelam[j].y);
                    en[j] = pt;
                }
            }

            return en;
        }
        private void generirajPop()
        {
            popArray = new nurikabePoint[pop][];
            //pametno zgenerirajmo szacetno pop tak da vzamemo v postev kolko crnih & belih se rabimo
            //prvo prestejemo kolko belih imamo in koliko jih se potrebujemo
            int kolkoBelihImamo = 0;
            int kolkoBelihRabimo = 0;
            
            Random rad  = new Random(69);
            
            foreach (var belIsl in arr.beliIsland)
            {
                kolkoBelihRabimo += belIsl.Value.maxSize;
                kolkoBelihImamo += belIsl.Value.size;
            }

            double moznostDaJePointBeli = (double) kolkoBelihImamo / (double) kolkoBelihRabimo;
            //zgenerirajmo pop
            for (int i = 0; i < pop; i++)
            {
                popArray[i] = new nurikabePoint[velikostEnega];
                for (int j = 0; j < velikostEnega; j++)
                {
                    nurikabePoint pt;
                    double rand = rad.NextDouble();
                    if (rand <= moznostDaJePointBeli)
                    {
                        pt = new nurikabePoint(val:int.MaxValue,id:int.MaxValue,x:tockeZKaterimiDelam[j].x,y:tockeZKaterimiDelam[j].y);
                        popArray[i][j] = pt;
                    }
                    else
                    {
                        pt = new nurikabePoint(val:int.MinValue,id:int.MinValue,x:tockeZKaterimiDelam[j].x,y:tockeZKaterimiDelam[j].y);
                        popArray[i][j] = pt;
                    }
                }
            }

        }
        
        //rabimo fitnes funkcijo, problem tu je da smo ze implementirali sranje ki ne omogoca enostavnega fitness procesa
        //zato bomo si from scrach zmislili fitness fun
        
        
        private int fitness(nurikabePoint[] osebek, bool debug = false)
        {
            int cena = 0;
            
            var arrCop = vhodniArrCop;
            foreach (var pt in osebek)
            {
                arrCop[pt.x, pt.y] = pt.val;
            }


            //Prvo preverimo kolko crnih kvadratov smo generirali
            int steviloKvadratkov = 0; 
            for (int i = 0; i<osebek.Length; i++)
            {
                if (osebek[i].val < 0)
                {
                    var k = this.dobiSteviloCrnihKvadratov(ref arrCop, ref osebek[i]);
                    steviloKvadratkov += k;
                }
            }
            
            (int steviloCrnihO, int steviloBelihO, int steviloPrevlkihBelihO, int steviloStikajocihO, int steviloNepravilnihBelih)= dobiSteviloOtokov(arrCop);
            if (steviloCrnihO > 1)
            {
                cena += (steviloCrnihO - 1) * cenaNaCrniOtok;
            }

            if (steviloBelihO > arr.beliIsland.Count)
            {
                cena += (steviloBelihO - arr.beliIsland.Count) * cenaPrevecbelih;
            }

            cena += steviloKvadratkov * cenaCrnegaKvadrata;
            cena += cenaNepravilnoVelikih * steviloStikajocihO + cenaNepravilnoVelikih * steviloPrevlkihBelihO + steviloNepravilnihBelih * cenaNepravilnihBelih;
            if (debug)
            {
                Console.WriteLine("stevilo crnih ot:" + steviloCrnihO + " \nstevilo belih ot: " + steviloBelihO +
                                  " \nstevilo Nepravilnih belih: " + steviloPrevlkihBelihO +
                                  " \nstevilo stikajocih o: " + steviloStikajocihO + " \nstevilo kvadratkov: " +
                                  steviloKvadratkov + "\nstevilo nepravilnih belih: "+steviloNepravilnihBelih+"\ncena: " + cena);
            }

            return cena;
        }

        nurikabePoint getPoint(int x, int y, ref int[,] arrT)
        {
            nurikabePoint np = new nurikabePoint(x,y,val:arrT[x,y]);
            return np;
        }

        nurikabePoint[][] krizaj(nurikabePoint[] a, nurikabePoint[] b)
        {
            var froca = new nurikabePoint[2][];
            Random ran = new Random(69);
            int x1 = ran.Next(arr.dim[0]-2);
            int x2 = ran.Next(arr.dim[0]-x1) +x1;
            int y1 = ran.Next(arr.dim[1]-2);
            int y2 = ran.Next(arr.dim[1]-y1) +y1;
            
            nurikabePoint[] froc1 = new nurikabePoint[a.Length];
            nurikabePoint[] froc2 = new nurikabePoint[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].x >= x1 && a[i].x <= x2 && a[i].y >= y1 && a[i].y <= y2)
                {
                    froc1[i] = a[i];
                }
                else
                {
                    froc2[i] = a[i];
                }
                if (b[i].x >= x1 && b[i].x <= x2 && b[i].y >= y1 && b[i].y <= y2)
                {
                    froc2[i] = b[i];
                }
                else
                {
                    froc1[i] = b[i];
                }
            }

            froca[0] = froc1;
            froca[1] = froc2;
            return froca;

        }
        List<nurikabePoint> dobiSosede(ref int[,] arrT, nurikabePoint n)
        {
            var ret =new List<nurikabePoint>();
            if (n.x-1>-1)
            {
                ret.Add(getPoint(n.x-1,n.y,ref arrT));
            }
            if (n.x+1<arr.dim[0])
            {
                ret.Add(getPoint(n.x+1,n.y,ref arrT));
            }
            if (n.y-1>-1)
            {
                ret.Add(getPoint(n.x,n.y-1,ref arrT));
            }
            if (n.y+1<arr.dim[1])
            {
                ret.Add(getPoint(n.x,n.y+1,ref arrT));
            }

            return ret;
            
        }


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
            for (int y = 0; y<arr.dim[1]; y++)
            {
                for (int x = 0; x<arr.dim[0]; x++)
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

        private int dobiSteviloCrnihKvadratov(ref int[,] arrT, ref nurikabePoint np)
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
            if (np.x + 1 < arr.dim[0] && np.y - 1 > -1)
            {
                if (arrT[np.x + 1, np.y - 1] < 0 && arrT[np.x + 1, np.y] < 0 && arrT[np.x , np.y - 1] < 0)
                {
                    steviloKvadratov++;
                }
            }
            //levi spodnji
            if (np.x - 1 > -1 && np.y + 1 < arr.dim[0])
            {
                if (arrT[np.x - 1, np.y + 1] < 0 && arrT[np.x - 1, np.y] < 0 && arrT[np.x , np.y + 1] < 0)
                {
                    steviloKvadratov++;
                }
            }
            //desnji spodni
            if (np.x + 1 < arr.dim[0] && np.y + 1 < arr.dim[0])
            {
                if (arrT[np.x + 1, np.y + 1] < 0 && arrT[np.x + 1, np.y] < 0 && arrT[np.x , np.y + 1] < 0)
                {
                    steviloKvadratov++;
                }
            }

            return steviloKvadratov;
        }


    }
}