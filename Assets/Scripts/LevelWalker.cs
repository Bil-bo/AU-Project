using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelWalker
{

    List<Vector2Int> coordinatesGenerated = new();

    protected class DirectionChance
    {
        public int Chance;
        public Vector2Int ReturnValue;
    }

    // Should Total to 100

    // Consider top right of grid to be [0,0]
    protected DirectionChance UpChance = new DirectionChance() { ReturnValue = new Vector2Int(0, -1) }; // [0,-1]
    protected DirectionChance DownChance = new DirectionChance() { ReturnValue = new Vector2Int(0, 1) }; // [0,1]
    protected DirectionChance LeftChance = new DirectionChance() { ReturnValue = new Vector2Int(-1, 0) }; // [-1,0]
    protected DirectionChance RightChance = new DirectionChance() { ReturnValue = new Vector2Int(1, 0) }; // [1,0]

    protected int ChanceToBreak;



    protected Vector2Int GridSize;
    protected int RoomsLeft;


    public LevelWalker(Vector2Int gridSize, int RoomAmountLeft)
    {
        this.GridSize = gridSize;
        this.RoomsLeft = RoomAmountLeft;
    }


    public virtual List<Vector2Int> Walk(List<Vector2Int> setCoordinates) 
    {
        List<Vector2Int> pointsToReturn = new();
        Vector2Int point = setCoordinates[UnityEngine.Random.Range(0, Mathf.Max(1, setCoordinates.Count))];

        bool walk = true;
        while (walk)
        {
            point += CalculateDirection();

            if (setCoordinates.Contains(point)) { return pointsToReturn; }
            else if (point.x > GridSize.x || point.y > GridSize.y || point.x < 0 || point.y < 0) { return pointsToReturn; }
            else if (pointsToReturn.Count >= RoomsLeft) { return pointsToReturn; }
            else if (UnityEngine.Random.Range(0, 101) <= ChanceToBreak) { return pointsToReturn; }

            else
            {
                pointsToReturn.Add(point);
            }
        }

        return pointsToReturn;
    }

    protected Vector2Int CalculateDirection()
    {
        int randomValue = UnityEngine.Random.Range(0, 101);
        int chanceTotal = 0;


        // Create an array to store chances
        DirectionChance[] chances = { UpChance, DownChance, LeftChance, RightChance};

        // Sort the chances in ascending order
        System.Array.Sort(chances, (a, b ) => a.Chance.CompareTo(b.Chance));

        for (int i = 0; i < chances.Length; i++)
        {
            chanceTotal += chances[i].Chance;  
            if (randomValue <= chanceTotal)
            {
                return chances[i].ReturnValue;
            }
        }

        return new Vector2Int(0, -1);

    }
}


public class BasicWalker: LevelWalker
{

    public BasicWalker(Vector2Int gridSize, int RoomAmountLeft) : base(gridSize, RoomAmountLeft)
    {
        UpChance.Chance = 25;
        DownChance.Chance = 25;
        LeftChance.Chance = 25;
        RightChance.Chance = 25;   

        ChanceToBreak = 50;
    }
}

public class SpecialWalker: LevelWalker
{
    public SpecialWalker(Vector2Int gridSize, int RoomAmountLeft) : base (gridSize, RoomAmountLeft) 
    {
        UpChance.Chance = 25;
        DownChance.Chance = 25;
        LeftChance.Chance = 25;
        RightChance.Chance = 25;

    }

    public override List<Vector2Int> Walk(List<Vector2Int> setCoordinates)
    {
        List<Vector2Int> pointsToReturn = new();
        Vector2Int point = setCoordinates[UnityEngine.Random.Range(0, Mathf.Max(1, setCoordinates.Count))];

        bool walk = true;
        while (walk)
        {
            point += CalculateDirection();

            if (setCoordinates.Contains(point)) { continue;  }
            else if (pointsToReturn.Count >= 1) { return pointsToReturn; }
            else
            {
                pointsToReturn.Add(point);
            }
        }

        return pointsToReturn;
    }
}

public class BossWalker : LevelWalker
{
    private Vector2Int StartRoom;
    private List<DirectionChance> directions;
    

    public BossWalker(Vector2Int gridSize, int RoomAmountLeft, Vector2Int StartRoom) : base(gridSize, RoomAmountLeft)
    {
        this.StartRoom = StartRoom;

        UpChance.Chance = 25;
        DownChance.Chance = 25;
        LeftChance.Chance = 25;
        RightChance.Chance = 25;
        directions = new List<DirectionChance>() { UpChance, DownChance, LeftChance, RightChance };


    }

    public override List<Vector2Int> Walk(List<Vector2Int> setCoordinates)
    {
        List<Vector2Int> pointsToReturn = new();
        Vector2Int point = FindFurthestPoint(setCoordinates);

        bool walk = true;
        while (walk)
        {


            foreach (DirectionChance directionChance in directions)
            {
                Vector2Int cardinal = point + directionChance.ReturnValue;
                if (setCoordinates.Contains(point)) { continue; }
                else if (pointsToReturn.Count >= 1) { return pointsToReturn; }
                else
                {
                    pointsToReturn.Add(point);
                }
            }
            point += CalculateDirection();
        }
        return pointsToReturn;
    }

    private Vector2Int FindFurthestPoint(List<Vector2Int> coords)
    {
        Vector2Int furthest = new Vector2Int(0, 0);
        float MaxDistance = Mathf.NegativeInfinity;
        
        if (coords.Count == 0 || coords == null) { return furthest; }

        foreach(Vector2Int point in coords)
        {
            Debug.Log(point);
            if (point != StartRoom)
            {
                Debug.Log("Furthest from start point: " + MaxDistance);
                float distance = Vector2Int.Distance(point, StartRoom);
                if (distance > MaxDistance) 
                {
                    Debug.Log("Distance from start point: "+distance);
                    furthest = point;
                    MaxDistance = distance; 
                }
            }
        }
        return furthest;

    }
   
}


public class WalkerFactory
{

    public static LevelWalker CreateWalker(Vector2Int gridSize, int RoomAmountLeft)
    {
        int rand = UnityEngine.Random.Range(0,1);

        switch (rand)
        {
            case 0:
                return new BasicWalker(gridSize, RoomAmountLeft);
            default:
                return null;

        }
    }


    public static LevelWalker CreateSpecialWalker()
    {
        return new SpecialWalker(new Vector2Int(0, 0), 1);
    }

    public static LevelWalker CreateBossWalker(Vector2Int StartRoom)
    {
        return new BossWalker(new Vector2Int(0, 0), 1, StartRoom);
    }

}
