using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentController : MonoBehaviour
{

    public Tilemap tileMapWalkable;
    public Tilemap tileMapBlock;

    public Tilemap tileMapExit;

    public GameObject foodPrefab;
    public GameObject medicinePrefab;

    public GameObject enemy01Prefab;
    public GameObject enemy02Prefab;

    public GameObject FirePrefab;

    public GameObject Snow;
    public GameObject Player;

    public TileBase border;
    public TileBase exit;

    private int max_y;
    private int min_y;
    private int max_x;
    private int min_x;


    public GameObject folderEnemies;
    public GameObject folderItems;

    private bool generated = false;

    // Start is called before the first frame update
    void Start()
    {
        table.Add("0_0", true);
        min_x = Mathf.FloorToInt(tileMapWalkable.GetComponent<Renderer>().bounds.center.x - (tileMapWalkable.GetComponent<Renderer>().bounds.size.x/2));
        max_x = Mathf.FloorToInt(tileMapWalkable.GetComponent<Renderer>().bounds.center.x + (tileMapWalkable.GetComponent<Renderer>().bounds.size.x/2));
        max_y = Mathf.FloorToInt(tileMapWalkable.GetComponent<Renderer>().bounds.center.y + (tileMapWalkable.GetComponent<Renderer>().bounds.size.y/2));
        min_y = Mathf.FloorToInt(tileMapWalkable.GetComponent<Renderer>().bounds.center.y - (tileMapWalkable.GetComponent<Renderer>().bounds.size.y/2));

        for (int col = min_x; col < max_x; col++)
        {                            
            tileMapBlock.SetTile(new Vector3Int(col, min_y, 0), border);
            tileMapBlock.SetTile(new Vector3Int(col, max_y -1, 0), border);
            if (col == min_x || col == max_x - 1) {
                for (int lin = min_y; lin < max_y; lin++)
                {
                    tileMapBlock.SetTile(new Vector3Int(col, lin, 0), border);
                }
            }
        }
        Snow.SetActive(Game.environmentIssues);

    }

    void FixedUpdate() {
        if (!generated && Game.inventoryController != null) {
            GenerateEnvironment(Game.inventoryController);
        }
    }
    public void GenerateEnvironment(InventoryController inventoryController) {
        int numberFood =
            Mathf.FloorToInt(
            1.5f * inventoryController.starving +
            0.8f * inventoryController.healthy +
            2f * inventoryController.sick);

        int numberMedicine =
            Mathf.FloorToInt(
            1.2f * inventoryController.starving +
            0.8f * inventoryController.healthy +
            1.5f * inventoryController.sick);

            for (int i = 0; i < numberFood; i++)
            {                
                Instantiate(foodPrefab, CalculateItemPosition(), Quaternion.identity);
            }

            for (int i = 0; i < numberMedicine; i++)
            {
                Instantiate(medicinePrefab, CalculateItemPosition(), Quaternion.identity);
            }

            int q_enemies_1 = 1;
            int q_enemies_2 = 1; 

            if (Game.Level == 1) {
                q_enemies_1 = Game.enemies01_Level01;
                q_enemies_2 = Game.enemies01_Level01;
            } else if (Game.Level == 2) {
                q_enemies_1 = Game.enemies01_Level02;
                q_enemies_2 = Game.enemies01_Level02;
            }else if (Game.Level == 3) {
                q_enemies_1 = Game.enemies01_Level03;
                q_enemies_2 = Game.enemies01_Level03;
            }

            for (int i = 0; i < q_enemies_1; i++)
            {
                GameObject e1 = Instantiate(enemy01Prefab, CalculateItemPosition(), Quaternion.identity);
                e1.GetComponent<EnemyController>().ChangeSide();
                if (Mathf.FloorToInt(e1.transform.position.x + e1.transform.position.y + e1.transform.position.z)%2 == 0) {
                    e1.GetComponent<EnemyController>().player = Player;
                }
            }

            for (int i = 0; i < q_enemies_2; i++)
            {
                GameObject e2 = Instantiate(enemy02Prefab, CalculateItemPosition(), Quaternion.identity);
                e2.GetComponent<EnemyController>().player = Player;
                if (Mathf.FloorToInt(e2.transform.position.x + e2.transform.position.y + e2.transform.position.z)%2 == 0) {
                    e2.GetComponent<EnemyController>().ChangeSide();
                }
            }

            //put randomly an exit
            tileMapExit.SetTile(CalculateExitPosition(), exit);
            
            //fire barrels
            for (int i = 0; i < Game.FireBarrels; i++)
            {
                GameObject fire_barrel = Instantiate(FirePrefab, CalculateItemPosition(), Quaternion.identity);
            }

            generated = true;
    }

    Hashtable table = new Hashtable();
    private Vector3 CalculateItemPosition(int recursive = 0) {
        Vector3Int v3 =  GenerateValue();
        if ((tileMapWalkable.HasTile(v3) && !tileMapBlock.HasTile(v3) && !tileMapExit.HasTile(v3)) || recursive > 10) {
            return new Vector3(v3.x + 0.5f, v3.y + 0.5f, v3.z - 0.5f);
        } else {
            return CalculateItemPosition(recursive++);    
        }
    }
    private Vector3Int CalculateExitPosition(int recursive = 0) {
        Vector3Int v3 =  GenerateValue();
        if ((tileMapWalkable.HasTile(v3) && !tileMapBlock.HasTile(v3) && !tileMapExit.HasTile(v3)) || recursive > 10) {
            return v3;
        } else {
            return CalculateExitPosition(recursive++);    
        }
    }


    private Vector3Int GenerateValue(int recursive = 0) {
        int x = Random.Range(min_x, max_x + 1);
        int y = Random.Range(min_y, max_y + 1);
        int z = 0;
        string code = x + "_"  + y;
        if(table.ContainsKey(code) && recursive++ < 100) {
            Vector3Int v = FindAdjuntPosition(x, y, z);
            if (v == new Vector3Int(x, y, z)) {
                return GenerateValue(recursive);
            } else {
                return v;
            }
        } else {
            if (!table.ContainsKey(code)) {
                table.Add(code, true);
            }
            return new Vector3Int(x, y, z);
        }
    }

    private Vector3Int FindAdjuntPosition(int x,  int y,  int z) {
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                string code = i + "_"  + j;
                if (!table.ContainsKey(code)) {
                    table.Add(code, true);
                    return new Vector3Int(i, j, z);
                }
            }   
        }
        return new Vector3Int(x, y, z);
    }
}
