namespace GeneticAlgoritm
{
    internal class Program
    {
        static List<List<int>> Map = new List<List<int>>
        {
            new List<int> { 0,10,25,40,26,12 },
            new List<int> { 10,0,20,30,28,18 },
            new List<int> { 25,20,0,15,19,22 },
            new List<int> { 40,30,15,0,8,21 },
            new List<int> { 26,28,19,8,0,16 },
            new List<int> { 12,18,22,21,16,0 }
        };
        const int PopulationSize = 20;
        const int MutationPercentage = 20;
        static void Main(string[] args)
        {
            List<Genome> Population = CreatePopulation();

            Console.WriteLine("Начальная популяция:");
            Population.Sort((x, y) => x.PathWeight.CompareTo(y.PathWeight));
            Population.ForEach(x =>
            {
                Console.Write(x.Path[0]);
                foreach (int i in x.Path.Skip(1))
                    Console.Write("->{0}", i);
                Console.WriteLine(" = {0}", x.PathWeight);
            });
            int Generation = 0;
            do
            {
                //Ранжирование
                Population.Sort((x, y) => x.PathWeight.CompareTo(y.PathWeight));

                int newChildNum = Random.Shared.Next(1, PopulationSize + 1);
                do
                {
                    //Селекция
                    Genome[] parents = Selection(Population);

                    //Скрещивание
                    Population.Add(new Genome(parents[0], parents[1], Map));

                    //Удаление худшего родителя
                    Population.RemoveAt(PopulationSize - 1);
                } while (newChildNum-- > 0);
                for(int i = 0; i < PopulationSize; i++)
                {
                    if (Random.Shared.Next(0, 100) < MutationPercentage)
                        Population[i].Mutate();
                }
                Console.WriteLine("Среднее значение оценочной функции поколения #{0} = {1}", ++Generation, Population.Average(i => i.PathWeight));
            } while (Math.Abs(Population.Average(i => i.PathWeight) - Population[0].PathWeight) >= 1);

            Population.Sort((x, y) => x.PathWeight.CompareTo(y.PathWeight));
            Console.WriteLine("\nЛучший потомок {0}-го поколения:",Generation);
            Console.Write(Population[0].Path[0]);
            foreach (int i in Population[0].Path.Skip(1))
                Console.Write("->{0}", i);
            Console.WriteLine(" = {0}", Population[0].PathWeight);
            Console.ReadKey(true);
        }

        private static List<Genome> CreatePopulation()
        {
            var templist = new List<Genome>();
            List<int> UnseenCities = new List<int>();
            while (templist.Count < PopulationSize)
            {
                for (int i = 1; i < Map[0].Count; i++)
                    UnseenCities.Add(i);

                List<int> newPath = new List<int>() { 0 };
                while (UnseenCities.Count > 0)
                {
                    int currentCity = Random.Shared.Next(0, UnseenCities.Count);
                    newPath.Add(UnseenCities[currentCity]);
                    UnseenCities.RemoveAt(currentCity);
                }
                newPath.Add(0);
                templist.Add(new Genome(newPath, Map));
            }
            return templist;
        }

        static Genome[] Selection(List<Genome> Population)
        {
            List<int[]> WeightSum = new List<int[]>() { new int[] { 0, Population[0].PathWeight } };
            for(int i = 1; i < Population.Count; i++)
                WeightSum.Add(new int[] { i, WeightSum[i - 1][1] + Population[i].PathWeight });

            Genome? firstParent = null, secondParent = null;

            do
            {
                int min = Random.Shared.Next(0, WeightSum[WeightSum.Count - 1][1] - 1);
                firstParent = Population[WeightSum.Where(i => i[1] > min).Select(i => i[0]).First()];
            } while (firstParent == null);

            do
            {
                int min = Random.Shared.Next(0, WeightSum[WeightSum.Count - 1][1] - 1);
                secondParent = Population[WeightSum.Where(i => i[1] > min).Select(i => i[0]).First()];
            } while (secondParent == null || secondParent == firstParent);

            return new Genome[2] { firstParent, secondParent };
        }
    }
}
