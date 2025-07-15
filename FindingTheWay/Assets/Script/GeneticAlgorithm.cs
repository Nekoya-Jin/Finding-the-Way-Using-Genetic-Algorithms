using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GaAgent
{
    public int[] Dna { get; set; }
    public int Fitness { get; set; }
}

public class GeneticAlgorithm
{
    private readonly int _dnaSize;
    private readonly int _geneSize;
    private readonly int _mutationSize;
    private const int GENE_MAX_VALUE = 6;

    public GeneticAlgorithm(int dnaSize, int geneSize, int mutationSize)
    {
        _dnaSize = dnaSize;
        _geneSize = geneSize;
        _mutationSize = mutationSize;
    }

    
    public int[] CreateRandomDna()
    {
        var dna = new int[_dnaSize];
        for (var i = 0; i < dna.Length; i++)
        {
            dna[i] = Random.Range(0, GENE_MAX_VALUE);
        }
        return dna;
    }

    public List<int[]> Evolve(List<GaAgent> currentPopulation, int targetPopulationCount)
    {
        var elites = SelectElites(currentPopulation);
        
        if (elites.Count == 0)//Eliteがない場合Null
        {
            return null;
        }
        
        return CreateNextGenerationDna(elites, targetPopulationCount);
    }
    
    private List<GaAgent> SelectElites(List<GaAgent> population)
    {
        if (population.Count == 0)
        {
            return new List<GaAgent>();
        }

        var averageFitness = population.Average(agent => agent.Fitness);
        return population.Where(agent => agent.Fitness >= averageFitness).ToList();
    }
    
    private List<int[]> CreateNextGenerationDna(List<GaAgent> elites, int targetPopulationCount)
    {
        var nextDnaList = new List<int[]>();
        
        // Eliteは残す
        foreach (var elite in elites)
        {
            nextDnaList.Add(elite.Dna);
        }

        // 他はCrossoverとMutate
        var offspringCount = targetPopulationCount - elites.Count;
        for (var i = 0; i < offspringCount; i++)
        {
            var parent1 = elites[Random.Range(0, elites.Count)];
            var parent2 = elites[Random.Range(0, elites.Count)];

            var childDna = Crossover(parent1.Dna, parent2.Dna);
            Mutate(childDna);

            nextDnaList.Add(childDna);
        }
        return nextDnaList;
    }

    private int[] Crossover(int[] parent1Dna, int[] parent2Dna)
    {
        var childDna = new int[_dnaSize];
        var geneChunkCount = _dnaSize / _geneSize;

        for (var i = 0; i < geneChunkCount; i++)
        {
            var parentSource = (Random.value > 0.5f) ? parent1Dna : parent2Dna;
            for (var n = 0; n < _geneSize; n++)
            {
                var index = i * _geneSize + n;
                childDna[index] = parentSource[index];
            }
        }
        return childDna;
    }

    private void Mutate(int[] dna)
    {
        var mutationCount = Random.Range(0, _mutationSize + 1);
        for (var i = 0; i < mutationCount; i++)
        {
            var mutationIndex = Random.Range(0, dna.Length);
            dna[mutationIndex] = Random.Range(0, GENE_MAX_VALUE);
        }
    }
}