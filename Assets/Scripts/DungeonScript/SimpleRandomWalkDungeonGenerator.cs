using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{

    [SerializeField]
    private SimpleRandomWalkData randomWalkParameters;



    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPostions = RunRandomWalk();
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTile(floorPostions);
        WallGenerator.CreateWalls(floorPostions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk()
    {
        var currentPostion = startPostion;
        HashSet<Vector2Int> floorPostions = new HashSet<Vector2Int>();
        for (int i = 0; i < randomWalkParameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithims.SimpleRandomWalk(currentPostion, randomWalkParameters.walkLength);
            floorPostions.UnionWith(path);
            if (randomWalkParameters.startRandomlyEachIteration)
                currentPostion = floorPostions.ElementAt(Random.Range(0, floorPostions.Count));
        }
        return floorPostions;
    }

}
