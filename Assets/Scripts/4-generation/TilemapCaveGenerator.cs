using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;



/**
 * This class demonstrates the CaveGenerator on a Tilemap.
 * 
 * By: Erel Segal-Halevi
 * Since: 2020-12
 */

public class TilemapCaveGenerator: MonoBehaviour {

    [SerializeField] Tilemap tilemap = null;
    [SerializeField] GameObject player = null;
    [SerializeField] GameObject gate = null;
    
    [Tooltip("The tile that represents a wall (an impassable block)")]
    [SerializeField] TileBase wallTile = null;

    [Tooltip("The tile that represents a floor (a passable block)")]
    [SerializeField] TileBase floorTile = null;

    [Tooltip("The percent of walls in the initial random map")]
    [Range(0, 1)]
    [SerializeField] float randomFillPercent = 0.5f;

    [Tooltip("Length and height of the grid")]
    [SerializeField] int gridSize = 100;
   
    [Tooltip("How many steps do we want to simulate?")]
    [SerializeField] int simulationSteps = 20;

    [Tooltip("For how long will we pause between each simulation step so we can look at the result?")]
    [SerializeField] float pauseTime = 1f;


    private int x_player = int.MaxValue;
    private int y_player = int.MaxValue;
    [SerializeField] int min, max;
    private int x_gate = int.MinValue;
    private int y_gate = int.MinValue;
    private CaveGenerator caveGenerator;

    void Start()  {
       
        //To get the same random numbers each time we run the script
        Random.InitState(100);
        
        caveGenerator = new CaveGenerator(randomFillPercent, gridSize);
        caveGenerator.RandomizeMap();
        
        //For testing that init is working
        GenerateAndDisplayTexture(caveGenerator.GetMap());
            
        //Start the simulation
        StartCoroutine(SimulateCavePattern());

    }


    //Do the simulation in a coroutine so we can pause and see what's going on
    private IEnumerator SimulateCavePattern()  {
        for (int i = 0; i < simulationSteps; i++)   {
            yield return new WaitForSeconds(pauseTime);

            //Calculate the new values
            caveGenerator.SmoothMap();

            //Generate texture and display it on the plane
            GenerateAndDisplayTexture(caveGenerator.GetMap());
        }
        int p = GetPosPlayer(caveGenerator.GetMap());
        int g = GetPosGate(caveGenerator.GetMap());

        Vector3Int player_pos = new Vector3Int(x_player, y_player, p);
        Vector3Int gate_pos = new Vector3Int(x_gate, y_gate, g);

        player.transform.position = tilemap.GetCellCenterWorld(player_pos);
        gate.transform.position = tilemap.GetCellCenterWorld(gate_pos);
        Debug.Log("Simulation completed!");
    }
    /// <summary>
    /// finds the first floor for the player
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private int GetPosPlayer(int[,] grid)
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (grid[x,y] == 0)
                {
                    x_player = System.Math.Min(x_player, x);
                    y_player = System.Math.Min(y_player, y);
                    return 0;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// finds the last floor for the gate
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private int GetPosGate(int[,] grid)
    {
        for (int y = gridSize-1; y > 0; y--)
        {
            for (int x = gridSize-1; x > 0; x--)
            {
                if (grid[x, y] == 0)
                {
                    x_gate = System.Math.Max(x_gate, x);
                    y_gate = System.Math.Max(y_gate, y);
                    return 0;
                }
            }
        }
        return 0;
    }

    //Generate a black or white texture depending on if the pixel is cave or wall
    //Display the texture on a plane
    private void GenerateAndDisplayTexture(int[,] data) {
        for (int y = 0; y < gridSize; y++) {
            for (int x = 0; x < gridSize; x++) {
                var position = new Vector3Int(x, y, 0);
                var tile = data[x, y] == 1 ? wallTile: floorTile;
                tilemap.SetTile(position, tile);
                
            }
        }
    }
}
