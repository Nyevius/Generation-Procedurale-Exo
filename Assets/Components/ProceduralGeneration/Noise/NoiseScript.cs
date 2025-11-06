using Cysharp.Threading.Tasks;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;

namespace Components.ProceduralGeneration.SimpleRoomPlacement
{
    [CreateAssetMenu(menuName = "Procedural Generation Method/Noise Generator")]
    public class NoiseGenerator : ProceduralGenerationMethod
    {
        private FastNoiseLite noise = new();

        [Header("Général")]
        [SerializeField] private FastNoiseLite.NoiseType _noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
        [Range(0.01f, 0.1f)]
        [SerializeField] private float _frequency = 0.01f;
        [Range(0, 2)]
        [SerializeField] private float _amplitude = 1f;

        [Header("Fractal")]
        [SerializeField] private FastNoiseLite.FractalType _FractalType = FastNoiseLite.FractalType.None;
        [Range(1, 20)]
        [SerializeField] private int _fractalOctaves = 3;
        [Range(0, 5)]
        [SerializeField] private float _lacunarity = 2f;
        [Range(0, 3)]
        [SerializeField] private float _gain = 0.5f;
        [Range(-20, 20)]
        [SerializeField] private float _wStrength = 0f;
        [Range(-20, 20)]
        [SerializeField] private float _ppStrength = 2f;

        [Header("Cellular")]
        [SerializeField] private FastNoiseLite.CellularDistanceFunction _cellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.EuclideanSq;
        [SerializeField] private FastNoiseLite.CellularReturnType _cellularReturnType = FastNoiseLite.CellularReturnType.Distance;
        [Range(-2, 2)]
        [SerializeField] private float _jitter = 1f;

        protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            noise.SetNoiseType(_noiseType);
            noise.SetFrequency(_frequency);
            noise.SetSeed(RandomService.Seed);
            
            noise.SetFractalType(_FractalType);
            noise.SetFractalOctaves(_fractalOctaves);
            noise.SetFractalLacunarity(_lacunarity);
            noise.SetFractalGain(_gain);
            noise.SetFractalWeightedStrength(_wStrength);
            noise.SetFractalPingPongStrength(_ppStrength);
            
            noise.SetCellularDistanceFunction(_cellularDistanceFunction);
            noise.SetCellularReturnType(_cellularReturnType);
            noise.SetCellularJitter(_jitter);

            BuildMap();

            // Waiting between steps to see the result.
            await UniTask.NextFrame(cancellationToken);
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

                    if (GetNoiseData(noise, x, z) >= 0.5f)
                    {
                        AddTileToCell(chosenCell, ROCK_TILE_NAME, false);
                    }
                    else if (GetNoiseData(noise, x, z) >= 0f)
                    {
                        AddTileToCell(chosenCell, GRASS_TILE_NAME, false);
                    }
                    else if (GetNoiseData(noise, x, z) >= -0.5f)
                    {
                        AddTileToCell(chosenCell, SAND_TILE_NAME, false);
                    }
                    else if (GetNoiseData(noise, x, z) >= -1f)
                    {
                        AddTileToCell(chosenCell, WATER_TILE_NAME, false);
                    }
                }
            }
        }

        public float GetNoiseData(FastNoiseLite noise, int x, int z)
        {
            float NoiseAtCoords = noise.GetNoise(x, z) * _amplitude;
            return Mathf.Clamp(NoiseAtCoords, -1, 1);
        }

        public float Get01NoiseData(FastNoiseLite noise, int x, int z)
        {
            return (GetNoiseData(noise, x, z) + 1f) / 2f;
        }
    }
}