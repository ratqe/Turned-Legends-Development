using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{

    [SerializeField]
    protected SimpleRandomWalkData randomWalkParameters;



    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTile(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkData parameters, Vector2Int position)
    {
        var currentPostion = position;
        HashSet<Vector2Int> floorPostions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithims.SimpleRandomWalk(currentPostion, parameters.walkLength);
            floorPostions.UnionWith(path);
            if (parameters.startRandomlyEachIteration)
                currentPostion = floorPostions.ElementAt(Random.Range(0, floorPostions.Count));
        }
        return floorPostions;
    }

}
