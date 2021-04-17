using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainFlow : MonoBehaviour
{
    public static Dictionary<Species, FlockingAgent> flockerPrefabs = new Dictionary<Species, FlockingAgent>();


    public static Vector3 aquariumSize= new Vector3(50,50,50);
    //public Canvas canvas;
    public Text food;
    public Text fishes;
    public Text jellyFish;
    public Text orca;
    public int startFoodCount;
    public float foodSpawnTime;
    public float foodSinkSpeed;
    public static List<GameObject> foods = new List<GameObject>();
    GameObject foodPrefab;
    float foodSpawnTimer;
    public List<FlockDataPackage> flockPackages;
   // List<Flock> fishes = new List<Flock>();
    public static List<Flock> allFlocks = new List<Flock>();
    
    void Start()
    {
        //canvas = new Canvas();
        LoadResources();
        SpawnFlocks();
        SpawnInitialFood();
        //  ListOfFishes();

       
      
    }
    //void ListOfFishes()
    //{


     
       
    //}
    void Update()
    {
        RefreshFlocks();
        RefreshFood();
        UpdateUi();
        //  ListOfFishes();
        
    }
    void UpdateUi()
    {
        food.text = "Number Of Food: " + MainFlow.foods.Count;
        orca.text = "Number Of orca: " + MainFlow.allFlocks[1].agents.Count;
        jellyFish.text = "Number Of jellyFish: " + MainFlow.allFlocks[2].agents.Count;
        fishes.text = "Number Of Fishes: " + MainFlow.allFlocks[0].agents.Count;
    }
    
    void SpawnFlocks()
    {
        foreach (FlockDataPackage package in flockPackages)
        {
            Flock newFlock = new Flock(package);
            allFlocks.Add(newFlock);
        }
    }
    void RefreshFlocks()
    {
        foreach(Flock flock in allFlocks)
        {
            flock.Refresh();
        }
    }
    //loading food and loading Fishes (all kind of agents )
    void LoadResources()
    {
       
        foodPrefab = Resources.Load<GameObject>("Prefabs/Food");
        flockerPrefabs.Add(Species.Fish,Resources.Load<FlockingAgent>("Prefabs/Fish"));
        flockerPrefabs.Add(Species.Orca, Resources.Load<FlockingAgent>("Prefabs/Orca"));
        flockerPrefabs.Add(Species.JellyFish, Resources.Load<FlockingAgent>("Prefabs/JellyFish"));
        flockerPrefabs.Add(Species.Megalodon, Resources.Load<FlockingAgent>("Prefabs/Megalodon"));
    }

   

   

    void RefreshFood()
    {
        foodSpawnTimer += Time.deltaTime;
        if(foodSpawnTimer >= foodSpawnTime)
        {
            SpawnFood();
            foodSpawnTimer = 0;
        }

        for (int i = 0; i < foods.Count; i++)
        {
            foods[i].transform.position += Vector3.down * foodSinkSpeed * Time.deltaTime;

            if(foods[i].transform.position.y < (-aquariumSize.y / 2))
            {
                GameObject f = foods[i];
                foods.RemoveAt(i);
                Destroy(f);
            }
        }
    }

    void SpawnFood()
    {
        float x = Random.Range(-aquariumSize.x / 2, aquariumSize.x / 2);
        float y = Random.Range(-aquariumSize.y / 2, aquariumSize.y / 2);
        float z = Random.Range(-aquariumSize.z / 2, aquariumSize.z / 2);

        GameObject newFood = Instantiate<GameObject>(foodPrefab, new Vector3(x, y, z), Quaternion.identity);
        foods.Add(newFood);
    }

   

    void SpawnInitialFood()
    {
        for (int i = 0; i < startFoodCount; i++)
        {
            float x = Random.Range(-aquariumSize.x / 2, aquariumSize.x / 2);
            float y = Random.Range(-aquariumSize.y / 2, aquariumSize.y / 2);
            float z = Random.Range(-aquariumSize.z / 2, aquariumSize.z / 2);

            GameObject newFood = Instantiate<GameObject>(foodPrefab, new Vector3(x, y, z), Quaternion.identity);
            foods.Add(newFood);
        }
    }
}
