using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{

    public Transform mouth;
    float timeToNextRandomChange;
    float timeSinceLastRandomChange;
    private Vector3 randomVector;
    float maxTargetDirMagnitude;

    FlockerGenome genome;
    Flock flock;
    public void Initialize(Flock flock,FlockerGenome genome)
    {
        this.flock = flock;
        this.genome = genome;
        calculateMaxTargetDirMagnitude();
        SelectNewRandomVector();
    }

    public void Refresh()
    {
        RefreshRandomVectorStuff();
        calculateMaxTargetDirMagnitude();
        Vector3 targetDir = CalculateTargetVector();
        MoveAgent(targetDir);
    }

    void calculateMaxTargetDirMagnitude()
    {
        maxTargetDirMagnitude = genome.centroidMult + genome.flockAvgDirMult + genome.randomDirMult + genome.neighbourDirMult + genome.personalSpaceMult + genome.foodAttractionMult;
    }

    Vector3 CalculateTargetVector()
    {
        Vector3 targetDir;
        Vector3 towardsCentroid = (flock.FlockCentroid - transform.position).normalized;
        Vector3 neighbourhoodDir = Vector3.zero;
        Vector3 personalSpaceVector = Vector3.zero;
        Vector3 foodVector = Vector3.zero;

        foreach (FlockingAgent otherAgent in flock.agents)
        {
            if(otherAgent != this)
            {
                if(Vector3.SqrMagnitude(otherAgent.transform.position - transform.position) < Mathf.Pow(flock.Package.neighbourhoodRadius, 2)) //Squared magnitude so radius is squared
                {
                    float distance = Vector3.Distance(otherAgent.transform.position, transform.position);
                    neighbourhoodDir += (otherAgent.transform.forward / Mathf.Clamp(distance, 1, flock.Package.neighbourhoodRadius));

                    if (Vector3.SqrMagnitude(otherAgent.transform.position - transform.position) < Mathf.Pow(flock.Package.personalSpaceRadius, 2))
                    {
                        personalSpaceVector += -(otherAgent.transform.position - transform.position).normalized / Mathf.Clamp(distance, 1f, flock.Package.personalSpaceRadius);
                    }
                }
            }
        }
        if(flock.Package.diet == Flock.Diet.Herbivorous || flock.Package.diet == Flock.Diet.Omnivorous)
        {
            for (int i = 0; i < MainFlow.foods.Count; i++)
            {
                float distanceFromFood = Vector3.Distance(MainFlow.foods[i].transform.position, transform.position);
                
                if (Vector3.SqrMagnitude(MainFlow.foods[i].transform.position - transform.position) < Mathf.Pow(flock.Package.foodPerceptionRange, 2))
                {
                    if (Vector3.SqrMagnitude(mouth.position - MainFlow.foods[i].transform.position) <= Mathf.Pow(flock.Package.mouthSize, 2))
                    {
                        GameObject f = MainFlow.foods[i];
                        MainFlow.foods.RemoveAt(i);
                        Destroy(f);
                    }
                    else
                        foodVector += (MainFlow.foods[i].transform.position - transform.position).normalized / Mathf.Clamp(distanceFromFood, 1f, flock.Package.foodPerceptionRange);
                }
            }
        }
        if (flock.Package.diet == Flock.Diet.Carnivorous)
        {

            for (int i =0;i<MainFlow.allFlocks.Count;i++)
            {
                for(int j =0;j< MainFlow.allFlocks[i].agents.Count;j++)
                {
                    if(flock.Package.power > MainFlow.allFlocks[i].agents[j].flock.Package.power)
                    {
                        float predatorDistanceFromTheprey = Vector3.Distance(mouth.position, MainFlow.allFlocks[i].agents[j].transform.position);

                        if (Vector3.SqrMagnitude(transform.position - MainFlow.allFlocks[i].agents[j].transform.position) < Mathf.Pow(flock.Package.foodPerceptionRange, 2))
                        {
                            if (Vector3.SqrMagnitude(mouth.position - MainFlow.allFlocks[i].agents[j].transform.position) <= Mathf.Pow(flock.Package.mouthSize, 2))
                            {
                                FlockingAgent fakeAgent = MainFlow.allFlocks[i].agents[j];
                                MainFlow.allFlocks[i].agents.RemoveAt(j);
                               // MainFlow.allFlocks[i].agents.Remove(MainFlow.allFlocks[i].agents[j]);
                                Destroy(fakeAgent);
                                //Destroy(MainFlow.allFlocks[i].agents[j]);
                            }
                            else
                            {
                                foodVector += (MainFlow.allFlocks[i].agents[j].transform.position - transform.position).normalized / Mathf.Clamp(predatorDistanceFromTheprey, 1f, flock.Package.foodPerceptionRange);
                            }
                        }
                        
                    }
                }
                
            }
           // Debug.Log("Hello");
            //for (int i = 0; i < MainFlow.allFlocks.Count; i++)
            //{
            //    if(MainFlow.allFlocks[i].Package.power<flock.Package.power)
            //    {
                    
            //    }

            //        //foreach (var s in MainFlow.allFlocks[i].agents)
            //        //{

            //        //if(MainFlow.allFlocks[i].Package.power<flock.Package.power)
            //        //{
            //        //    Debug.Log("Comment Got Heated");
            //        //    float f = Vector3.Distance(s.transform.position, mouth.transform.position);
            //        //    if (f < 1f)
            //        //    {
            //        //        MainFlow.allFlocks[i].agents.Remove(s);
            //        //    }
            //        //    else
            //        //    {
            //        //        foodVector += (s.transform.position - transform.position).normalized / Mathf.Clamp(f, 1f, flock.Package.foodPerceptionRange);
            //        //    }
            //     //   }

                

            //}
        }

        neighbourhoodDir.Normalize();
        personalSpaceVector.Normalize();
        foodVector.Normalize();

        targetDir = (towardsCentroid * genome.centroidMult) 
                    + (flock.FlockAvgDirection * genome.flockAvgDirMult)
                    + (randomVector * genome.randomDirMult)
                    + (neighbourhoodDir * genome.neighbourDirMult)
                    + (personalSpaceVector * genome.personalSpaceMult)
                    + (foodVector * genome.foodAttractionMult);

        return targetDir;
    }

    void MoveAgent(Vector3 targetDir)
    {
        float rotateSpeed = Mathf.Lerp(flock.Package.rotateSpeedRange.x, flock.Package.rotateSpeedRange.y, targetDir.magnitude / maxTargetDirMagnitude);
        float moveSpeed = Mathf.Lerp(flock.Package.moveSpeedRange.x,flock.Package.moveSpeedRange.y, targetDir.magnitude / maxTargetDirMagnitude);

        transform.forward = Vector3.RotateTowards(transform.forward, targetDir, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void SelectNewRandomVector()
    {
        randomVector = new Vector3(Random.Range(-1,1f), Random.Range(-1, 1f), Random.Range(-1, 1f)).normalized;
        timeToNextRandomChange = Random.Range(flock.Package.randomDirChangeTimeRange.x, flock.Package.randomDirChangeTimeRange.y);
        timeSinceLastRandomChange = 0;
    }

    void RefreshRandomVectorStuff()
    {
        timeSinceLastRandomChange += Time.deltaTime;
        if(timeSinceLastRandomChange >= timeToNextRandomChange)
        {
            SelectNewRandomVector();
        }
    }
}
[System.Serializable]
public class FlockerGenome
{
    public float centroidMult;
    public float flockAvgDirMult;
    public float randomDirMult;
    public float neighbourDirMult;
    public float personalSpaceMult;
    public float foodAttractionMult;
}