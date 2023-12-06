using System;
using System.Collections.Generic;

namespace GeneticAlgoritm
{
    public class Genome
    {
        public List<int> Path { get; private set; }
        public int PathWeight;
        public Genome(List<int> Path, List<List<int>> Map)
        {
            this.Path = Path;
            this.PathWeight = CalculatePath(Map);
        }

        private int CalculatePath(List<List<int>> map)
        {
            int totalWeight = 0;
            for(int i = 1; i < Path.Count; i++)
            {
                if (map[Path[i - 1]][Path[i]] == 0)
                    return int.MaxValue;
                totalWeight += map[Path[i - 1]][Path[i]];
            }
            return totalWeight;
        }

        public Genome(Genome parent1, Genome parent2, List<List<int>> Map, int MutationPercentage)
        {
            List<int> mask = new List<int>();
            for (int i = 0; i < parent1.Path.Count - 2; i++)
                mask.Add(Random.Shared.Next(0, 2));

            this.Path = new List<int>(Enumerable.Repeat(0,parent1.Path.Count));
            this.Path[0] = parent1.Path[0];
            this.Path[parent1.Path.Count-1] = parent1.Path[parent1.Path.Count-1];

            for(int i = 1; i < parent1.Path.Count-1; i++)
            {
                if (mask[i-1] == 1)
                    this.Path[i] = parent1.Path[i];
            }
            for(int i = 1; i < parent2.Path.Count-1; i++)
            {
                if (mask[i-1] == 0 && !this.Path.Contains(parent2.Path[i]))
                    this.Path[i] = parent2.Path[i];
            }

            for(int i = 1; i < this.Path.Count - 1;i++)
            {
                if (this.Path[i] == 0)
                    this.Path[i] = Enumerable.Range(0,parent1.Path.Count).Where(x => !this.Path.Contains(x)).First();
            }
            if (Random.Shared.Next(0, 100) < MutationPercentage)
                this.Mutate(Map);
            this.PathWeight = CalculatePath(Map);
        }
        public void Mutate(List<List<int>> Map)
        {
            int leftBound = Random.Shared.Next(1, this.Path.Count - 2);
            int rightBound = Random.Shared.Next(leftBound+1, this.Path.Count - 1);

            int shuffleNum = rightBound-leftBound;
            while(shuffleNum-- > 0)
            {
                int firstIdx = Random.Shared.Next(leftBound, rightBound + 1),
                    secondIdx;
                do
                {
                    secondIdx = Random.Shared.Next(leftBound, rightBound + 1);
                }while(secondIdx == firstIdx);

                int tmp = this.Path[firstIdx];
                this.Path[firstIdx] = this.Path[secondIdx];
                this.Path[secondIdx] = tmp;
            }
            this.PathWeight = CalculatePath(Map);
        }
    }
}
