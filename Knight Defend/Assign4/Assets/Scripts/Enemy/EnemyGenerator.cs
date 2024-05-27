using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator instance;

    public GameObject SlimePrefab;
    public GameObject HelmetPrefab;
    public GameObject VikingPrefab;
    public GameObject KingPrefab;
    public GameObject EnemyParent;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateEnemy()
    {
        if(GameManager.instance.curStatus == Status.Game)
        {
            while (true)
            {
                int generatX = Random.Range(0 + GameManager.instance.seaWidth, GameManager.instance.mapSize[0] - GameManager.instance.seaWidth);
                int generatZ = Random.Range(0 + GameManager.instance.seaWidth, GameManager.instance.mapSize[2] - GameManager.instance.seaWidth);
                Vector3 check_Point = new Vector3(generatX, 7, generatZ);
                RaycastHit hit;
                if (Physics.Raycast(check_Point, Vector3.down, out hit, 9))
                {
                    if (hit.collider.tag == "Ground")
                    {
                        if(GameManager.instance.Level == 1)
                        {
                            DefaultSlimeData(check_Point);
                            
                        }else if(GameManager.instance.Level == 2)
                        {
                            DefaultHelmetData(check_Point);
                        }
                        else if (GameManager.instance.Level == 3)
                        {
                            DefaultVikingData(check_Point);
                        }
                        else if (GameManager.instance.Level == 4)
                        {
                            DefaultKingData(check_Point);
                        }
                        else
                        {
                            break;
                        }

                        break;

                    }
                }
            }
        }


    }

    EnemyData DefaultSlimeData(Vector3 check_point)
    {
        GameObject temp = Instantiate(SlimePrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;
        

        EnemyData slimeData = new EnemyData();
        slimeData.MaxHealth = 100;
        slimeData.CurHealth = slimeData.MaxHealth;
        slimeData.Attack = 10;
        slimeData.Exp = 20;

        temp.GetComponent<EnemyController>().enemyData = slimeData;
        return slimeData;
    }

    EnemyData DefaultHelmetData(Vector3 check_point)
    {
        GameObject temp = Instantiate(HelmetPrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;

        EnemyData turtleData = new EnemyData();
        turtleData.MaxHealth = 160;
        turtleData.CurHealth = turtleData.MaxHealth;
        turtleData.Attack = 20;
        turtleData.Exp = 35;

        temp.GetComponent<EnemyController>().enemyData = turtleData;
        return turtleData;
    }
    EnemyData DefaultVikingData(Vector3 check_point)
    {
        GameObject temp = Instantiate(VikingPrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;

        EnemyData turtleData = new EnemyData();
        turtleData.MaxHealth = 200;
        turtleData.CurHealth = turtleData.MaxHealth;
        turtleData.Attack = 20;
        turtleData.Exp = 60;

        temp.GetComponent<EnemyController>().enemyData = turtleData;
        return turtleData;
    }
    EnemyData DefaultKingData(Vector3 check_point)
    {
        GameObject temp = Instantiate(KingPrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;

        EnemyData turtleData = new EnemyData();
        turtleData.MaxHealth = 300;
        turtleData.CurHealth = turtleData.MaxHealth;
        turtleData.Attack = 20;
        turtleData.Exp = 100;

        temp.GetComponent<EnemyController>().enemyData = turtleData;
        return turtleData;
    }
}
