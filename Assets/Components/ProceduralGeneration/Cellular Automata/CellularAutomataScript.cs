using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;
using VTools.Utility;

namespace Components.ProceduralGeneration.SimpleRoomPlacement
{
    [CreateAssetMenu(menuName = "Procedural Generation Method/FastNoise")]
    public class FastNoise : ProceduralGenerationMethod
    {
        [SerializeField] private FastNoiseLite noise = new();
        [SerializeField] private FastNoiseLite.NoiseType noiseType;
        [Range(0.1f, 1f)]
        [SerializeField] private float frequency;


        protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
        {
            BuildMap();
            for (int i = 0; i < _maxSteps; i++)
            {


                await UniTask.Delay(200);
            }
        }

        public void BuildMap()
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int z = 0; z < Grid.Lenght; z++)
                {
                    if (!Grid.TryGetCellByCoordinates(x, z, out var chosenCell))
                    {
                        continue;
                    }
                    float NoiseAtCoords = noise.GetNoise(x, z);
                    if (NoiseAtCoords >= 0.5f)
                    {
                        AddTileToCell(chosenCell, ROCK_TILE_NAME, false);
                    }
                    else if (NoiseAtCoords >= 0f)
                    {
                        AddTileToCell(chosenCell, GRASS_TILE_NAME, false);
                    }
                    else if (NoiseAtCoords >= -0.5f)
                    {
                        AddTileToCell(chosenCell, SAND_TILE_NAME, false);
                    }
                    else if (NoiseAtCoords >= -1f)
                    {
                        AddTileToCell(chosenCell, WATER_TILE_NAME, false);
                    }
                }
            }
        }
    }
}