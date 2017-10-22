using System;
using System.Collections.Generic;

namespace simulationHW
{


    class MainClass
    {
        static Random random = new Random();

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
            public int Number; 
            public int orderSize;
            public int reorderPoint;
            public List<SimulationDay> sd; // simulation day
        }

        public static int DemandCount()
        {
            
            int demandRandom = random.Next(1, 100);
            int demand = 0;

            if ((demandRandom > 0) && (demandRandom <= 10))
                demand = 0;
            else if ((demandRandom > 10) && (demandRandom <= 35))
                demand = 1;
            else if ((demandRandom > 35) && (demandRandom <= 70))
                demand = 2;
            else if ((demandRandom > 70) && (demandRandom <= 91))
                demand = 3;
            else if ((demandRandom > 91) && (demandRandom <= 100))
                demand = 4;

            return demand;
        }

        public static int LeadTime()
        {
            
            int leadRandom = random.Next(1, 100);
            int lead = 0;

            if ((leadRandom > 0) && (leadRandom <= 60))
                lead = 1;
            else if ((leadRandom > 60) && (leadRandom <= 90))
                lead = 2;
            else if ((leadRandom > 90) && (leadRandom <= 100))
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

               Rep.Add(new Replication { Number = 1, orderSize = 11, reorderPoint = 3, sd = new List<SimulationDay>() });
               Rep.Add(new Replication { Number = 2, orderSize = 10, reorderPoint = 3, sd = new List<SimulationDay>() });
               Rep.Add(new Replication { Number = 3, orderSize = 12, reorderPoint = 3, sd = new List<SimulationDay>() });
               Rep.Add(new Replication { Number = 4, orderSize = 11, reorderPoint = 2, sd = new List<SimulationDay>() });
               Rep.Add(new Replication { Number = 5, orderSize = 11, reorderPoint = 4, sd = new List<SimulationDay>() });
                   
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

                //List<SimulationDay> simD = new List<SimulationDay>();

                //zero day
                BlankDay.beginingInventory = 0;
                BlankDay.dayNumber = 0;
                BlankDay.demand = 0;
                BlankDay.endingInventory = 9;
                BlankDay.shortage = 0;

                Rep[repNum-1].sd.Add(BlankDay);
                //

                Console.WriteLine("Replication #" + repNum);
                for (int day = 1; day <= numberOfDays; day++)
                {
                    BlankDay.dayNumber = day;

                    if (orderKey) //if yesterday we decided that we need to make an order
                    {
                        orderArrivesDay = day + LeadTime(); 
                        orderKey = false; //we don't need to make an order anymore
                        orderMadeKey = true; //we are waiting for an order
                    }

                    BlankDay.beginingInventory = Rep[repNum-1].sd[day - 1].endingInventory;

                    if ((orderMadeKey) && (day == orderArrivesDay)) //if today is the day the order comes
                    {
                        BlankDay.beginingInventory += Rep[repNum - 1].orderSize; //order arrived
                        orderMadeKey = false; //we are not waiting for an order anymore
                    }

                    BlankDay.demand = DemandCount(); //today's demand
                    BlankDay.endingInventory = BlankDay.beginingInventory - BlankDay.demand; //bye bye demand
                   
                    if (BlankDay.endingInventory < 0) //if we have shortage
                        BlankDay.shortage = (-1) * BlankDay.endingInventory; // "shortage has to be made up"
                    else 
                        BlankDay.shortage = 0;

                    if (!orderMadeKey) //if we are not waiting for an order
                        if (BlankDay.endingInventory < Rep[repNum - 1].reorderPoint) //check whether or not we got to reorder point
                        {
                            orderKey = true; //tomorrow morning we have to make an order
                        }

                    endInvSum += BlankDay.endingInventory; //sum for ending inventory calculation
                    shortageSum += BlankDay.shortage; // the same for shortage

                    if (BlankDay.shortage > maxShortage) 
                        maxShortage = BlankDay.shortage;
                    
                    Console.WriteLine(BlankDay.dayNumber+"th day, BI:" + BlankDay.beginingInventory + " EI:" + BlankDay.endingInventory + " Demand:" + BlankDay.demand + " Shortage:" + BlankDay.shortage);

                    Rep[repNum - 1].sd.Add(BlankDay);
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

