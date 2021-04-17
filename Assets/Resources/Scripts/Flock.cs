using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Species
{
    Fish,Orca,Megalodon,JellyFish
}
public class Flock 
{
    public enum Diet { Herbivorous,Carnivorous,Omnivorous }
    FlockingAgent fishPrefab;
    public  List<FlockingAgent> agents = new List<FlockingAgent>();
    public  Vector3 FlockCentroid { get; private set; }
    public Vector3 FlockAvgDirection { get; private set; }
    public FlockDataPackage Package { get; private set; }
    public Flock(FlockDataPackage package)
    {
        Package = package;
        SpawnAgents();
    }


    

    public void Refresh()
    {
        RefreshFlockValues();
        RefreshAgents();
    }

  

    void SpawnAgents()
    {
        for (int i = 0; i < Package.numOfAgents; i++)
        {
            float x = Random.Range(-MainFlow.aquariumSize.x / 2, MainFlow.aquariumSize.x / 2);
            float y = Random.Range(-MainFlow.aquariumSize.y / 2, MainFlow.aquariumSize.y / 2);
            float z = Random.Range(-MainFlow.aquariumSize.z / 2, MainFlow.aquariumSize.z / 2);

            FlockingAgent newAgent = GameObject.Instantiate<FlockingAgent>(MainFlow.flockerPrefabs[Package.species], new Vector3(x, y, z),
                Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f)));

            newAgent.Initialize(this,Package.defaultGenome);
            agents.Add(newAgent);
        }

    }
    void RefreshAgents()
    {
        foreach (FlockingAgent agent in agents)
        {
            agent.Refresh();
        }
    }
    void RefreshFlockValues()
    {
        Vector3 centroid = Vector3.zero;
        Vector3 avgDir = Vector3.zero;

        for (int i = 0; i < agents.Count; i++)
        {
            centroid += agents[i].transform.position;
            avgDir += agents[i].transform.forward;
        }

        centroid /= agents.Count;
        FlockCentroid = centroid;
        avgDir = avgDir.normalized;
        FlockAvgDirection = avgDir;
    }

}
[System.Serializable]
public class FlockDataPackage
{
    public int numOfAgents;
    public Species species;
    public Flock.Diet diet;
    public int power;

    public float neighbourhoodRadius;
    public float personalSpaceRadius;
    public float foodPerceptionRange;
    public float mouthSize;
    public Vector2 randomDirChangeTimeRange;
    public Vector2 rotateSpeedRange;
    public Vector2 moveSpeedRange;


    public FlockerGenome defaultGenome;
}
