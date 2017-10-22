using System;
using System.Collections.Generic;

namespace simulationHW
{
    //разобраться со списками

    class MainClass
    {
        static Random random1 = new Random();

        public struct SimulationDay
        {
            public int dayNumber;
            public int beginingInventory;
            public int endingInventory;
            public int demand;
            public int shortage;
        }

        public struct Replication
        {
            public int Number; //порядковый номер из пяти
            public int orderSize;
            public int reorderPoint;
            public List<SimulationDay> sd; //структура
        }

        public static int DemandCount()
        {
            
            int demandRandom = random1.Next(0, 100);
            int demand = 0;

            if ((demandRandom >= 0) && (demandRandom < 11))
                demand = 0;
            else if ((demandRandom > 10) && (demandRandom < 36))
                demand = 1;
            else if ((demandRandom > 35) && (demandRandom < 71))
                demand = 2;
            else if ((demandRandom > 70) && (demandRandom < 92))
                demand = 3;
            else if ((demandRandom > 91) && (demandRandom < 101))
                demand = 4;
            
            //Console.WriteLine(demandRandom);

            return demand;
        }

        public static int LeadTime()
        {
            Random random2 = new Random();
            int leadRandom = random2.Next(0, 100);
            int lead = 0;

            if ((leadRandom >= 0) && (leadRandom < 61))
                lead = 1;
            else if ((leadRandom > 60) && (leadRandom < 91))
                lead = 2;
            else if ((leadRandom > 90) && (leadRandom < 101))
                lead = 3;
            return lead;
        }

        public static double GetStandardDeviation(List<double> doubleList)  
        {  
            double average = 0;
            double sumOfDerivation = 0;  
            foreach (double value in doubleList)  
            {
                average += value;
                sumOfDerivation += (value) * (value);  
            }
            average /= doubleList.Count;
            double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count - 1);
            Console.WriteLine("mean is " + average);
            return Math.Sqrt(sumOfDerivationAverage - (average*average));  
        }  

        public static void Main(string[] args)
        {
            int numberOfReplications = 5;
            int numberOfDays = 25;
            bool orderKey = false;
            bool orderMadeKey = false;
            int orderArrivesDay = 0;
            double endInvSum = 0;
            double shortageSum = 0;
            double maxShortage = 0;

            List<Replication> Rep = new List<Replication>();

            SimulationDay BlankDay;

            List<double> endInvAverage = new List<double>();
            List<double> shrtgAverage = new List<double>();
            List<double> maxShrtgList = new List<double>();

            for (int repNum = 1; repNum <= numberOfReplications; repNum++)
            {
                orderKey = false;
                orderMadeKey = false;
                orderArrivesDay = 0;
                endInvSum = 0;
                shortageSum = 0;
                maxShortage = 0;

                List<SimulationDay> simD = new List<SimulationDay>();

                //нулевой день
                BlankDay.beginingInventory = 0;
                BlankDay.dayNumber = 0;
                BlankDay.demand = 0;
                BlankDay.endingInventory = 9;
                BlankDay.shortage = 0;

                simD.Add(BlankDay);
                //добавили нулевой день

                switch (repNum)
                {
                    case 1:
                        Rep.Add(new Replication { Number = 1, orderSize = 11, reorderPoint = 3, sd = simD });
                        break;
                    case 2:
                        Rep.Add(new Replication { Number = 2, orderSize = 10, reorderPoint = 3, sd = simD });
                        break;
                    case 3:
                        Rep.Add(new Replication { Number = 3, orderSize = 12, reorderPoint = 3, sd = simD });
                        break;
                    case 4:
                        Rep.Add(new Replication { Number = 4, orderSize = 11, reorderPoint = 2, sd = simD });
                        break;
                    case 5:
                        Rep.Add(new Replication { Number = 5, orderSize = 11, reorderPoint = 4, sd = simD });
                        break;
                }

                Console.WriteLine("Replication #" + repNum);
                for (int day = 1; day <= numberOfDays; day++)
                {
                    BlankDay.dayNumber = day; //neobyaz

                    if (orderKey) //если вчера в конце дня решили что нужно делать заказ
                    {
                        orderArrivesDay = day + LeadTime(); //находим когда заказ придет
                        orderKey = false; //больше заказывать не нужно
                        orderMadeKey = true; //мы в ожидании заказа
                    }

                    BlankDay.beginingInventory = Rep[repNum-1].sd[day - 1].endingInventory;

                    if ((orderMadeKey) && (day == orderArrivesDay)) //если сегодня прибывает заказ
                    {
                        BlankDay.beginingInventory += Rep[repNum - 1].orderSize; //заказ пришел, казна пополнилась
                        orderMadeKey = false; //больше заказ не ждем
                    }

                    BlankDay.demand = DemandCount(); //узнаем сегодняшний спрос
                    BlankDay.endingInventory = BlankDay.beginingInventory - BlankDay.demand; //обслужили из казны
                   
                    if (BlankDay.endingInventory < 0) //если ушли в минус
                        BlankDay.shortage = (-1) * BlankDay.endingInventory; //создаем недостаток, по заданию, для статистики
                    else BlankDay.shortage = 0;

                    if (!orderMadeKey) //если не ждем заказ
                        if (BlankDay.endingInventory < Rep[repNum - 1].reorderPoint) //проверяем достигли ли мы точки заказа
                        {
                            orderKey = true; //завтра утром нужно будет заказать
                        }

                    endInvSum += BlankDay.endingInventory;
                    shortageSum += BlankDay.shortage;

                    if (BlankDay.shortage > maxShortage) 
                        maxShortage = BlankDay.shortage;
                    Console.WriteLine(BlankDay.dayNumber+"th day, BI:" + BlankDay.beginingInventory + " EI:" + BlankDay.endingInventory + " Demand:" + BlankDay.demand + " Shortage:" + BlankDay.shortage);
                    simD.Add(BlankDay);
                }

                endInvSum /= numberOfDays; //average ending inventory
                endInvAverage.Add(endInvSum);

                shortageSum /= numberOfDays; //average shortage
                shrtgAverage.Add(shortageSum);

                maxShrtgList.Add(maxShortage);
                //output of this replication
            }
            Console.Write("Ending Inventory ");
            double endInvStDev = GetStandardDeviation(endInvAverage);
            Console.WriteLine("Standard deviation of Ending Inventory is " + endInvStDev);

            Console.Write("Shortage ");
            double shrtgStDev = GetStandardDeviation(shrtgAverage);
            Console.WriteLine("Standard deviation of Shortage is " + shrtgStDev);

            Console.Write("Maximum shortage ");
            double maxShrtgStDev = GetStandardDeviation(maxShrtgList);
            Console.WriteLine("Standard deviation of Max Shortages is " + maxShrtgStDev);
        }
    }
}

