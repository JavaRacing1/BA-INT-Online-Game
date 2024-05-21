using System;
using System.Collections.Generic;

using Godot;

using INTOnlineCoop.Script.Util;

namespace INTOnlineCoop.Script.Level
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PlayerPositionGenerator : RefCounted
    {
        private List<(int, int)> _surfacePoints;
        private int _haltonIndex;
        private Image _terrainImage;

        /// <summary>
        /// Initializes the position generator
        /// </summary>
        /// <param name="image">Generated terrain image</param>
        /// <param name="terrainName">Name of the terrain type</param>
        /// <param name="debugMode">true if generated images should be saved</param>
        public void Init(Image image, string terrainName, bool debugMode = false)
        {
            List<(int, int)> surfacePoints = ImageUtils.ComputeSurface(image, airPixelAmount: 4, xAirOffset: 1);
            surfacePoints = RemoveSecludedSurfacePoints(surfacePoints, image.GetWidth(), image.GetHeight());
            surfacePoints = RemoveLowSurfacePoints(surfacePoints, image.GetHeight());

            if (debugMode)
            {
                Image debugImage = (Image)image.Duplicate();
                foreach ((int, int) pixel in surfacePoints)
                {
                    debugImage.SetPixel(pixel.Item1, pixel.Item2, Colors.Yellow);
                }

                _ = debugImage.SavePng($"res://output/{terrainName}/7_filtered_surface.png");
            }

            _surfacePoints = SortSurfacePoints(surfacePoints, image.GetHeight());
            _haltonIndex = 0;
            _terrainImage = image;
        }

        /// <summary>
        /// Calculates the spawn position for the next player
        /// </summary>
        /// <param name="seed">Seed for randomness</param>
        /// <param name="characterHeight">Height of the character</param>
        /// <returns>Unscaled position of the characters center point</returns>
        public (double, double) GetSpawnPosition(double seed, int characterHeight = 4)
        {
            if (_surfacePoints.Count == 0)
            {
                GD.PrintErr("PlayerPositionGenerator not initialized!");
                return (0.0, 0.0);
            }
            double nextRandomPoint = Halton(_haltonIndex++, 2);
            nextRandomPoint = (nextRandomPoint + seed) % 1.0;
            (int, int) surfacePoint = _surfacePoints[(int)Math.Floor(nextRandomPoint * _surfacePoints.Count)];

            bool pixelHasEnoughAir = ImageUtils.HasPixelEnoughAir(_terrainImage, surfacePoint.Item1, surfacePoint.Item2,
                airAmount: 4, xOffset: -1);
            (double, double) spawnPosition = pixelHasEnoughAir
                ? (surfacePoint.Item1, surfacePoint.Item2 - (characterHeight / 2))
                : (surfacePoint.Item1 + 1, surfacePoint.Item2 - (characterHeight / 2));
            return spawnPosition;
        }

        private static List<(int, int)> RemoveSecludedSurfacePoints(List<(int, int)> surfacePoints, int xSize,
            int ySize)
        {
            Bitmap bitmap = new();
            bitmap.Create(new Vector2I(xSize, ySize));
            foreach ((int, int) pixel in surfacePoints)
            {
                bitmap.SetBit(pixel.Item1, pixel.Item2, true);
            }

            List<(int, int)> filteredPixels = new();
            foreach ((int, int) pixel in surfacePoints)
            {
                if (PixelHasSurfaceNeighbors(bitmap, pixel))
                {
                    filteredPixels.Add(pixel);
                }
            }

            return filteredPixels;
        }

        private static List<(int, int)> RemoveLowSurfacePoints(List<(int, int)> surfacePoints, int imageHeight,
            int threshold = 5)
        {
            List<(int, int)> filteredPixels = new();
            foreach ((int, int) pixel in surfacePoints)
            {
                if (pixel.Item2 < imageHeight - threshold)
                {
                    filteredPixels.Add(pixel);
                }
            }

            return filteredPixels;
        }

        private static bool PixelHasSurfaceNeighbors(Bitmap bitmap, (int, int) pixel)
        {
            int x = pixel.Item1;
            int y = pixel.Item2;

            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    int combinedX = x + xOffset;
                    int combinedY = y + yOffset;

                    if ((combinedX == x && combinedY == y) || combinedX < 0 || combinedX >= bitmap.GetSize().X ||
                        combinedY < 0 || combinedY >= bitmap.GetSize().Y)
                    {
                        continue;
                    }

                    if (bitmap.GetBit(combinedX, combinedY))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static List<(int, int)> SortSurfacePoints(List<(int, int)> surfacePoints, int imageHeight)
        {
            //Sort points by distance -> points with a similar height are sorted from left to right
            List<(int, int)> sortedPoints = new();
            (int, int) currentPos = (0, imageHeight - 1);
            while (surfacePoints.Count > 0)
            {
                if (DistanceSquare(currentPos, surfacePoints[^1]) > 25)
                {
                    surfacePoints.Sort((a, b) => (int)(DistanceSquare(currentPos, b) - DistanceSquare(currentPos, a)));
                }

                currentPos = surfacePoints[^1];
                surfacePoints.RemoveAt(surfacePoints.Count - 1);
                sortedPoints.Add(currentPos);
            }

            return sortedPoints;
        }

        private static double Halton(int index, int numBase)
        {
            double result = 0;
            double f = 1f / numBase;
            double i = index;
            while (i > 0)
            {
                result += f * (i % numBase);
                i = Math.Floor(i / numBase);
                f /= numBase;
            }

            return result;
        }

        private static double DistanceSquare((int, int) firstPoint, (int, int) secondPoint)
        {
            return Math.Pow(secondPoint.Item1 - firstPoint.Item1, 2) +
                   Math.Pow(secondPoint.Item2 - firstPoint.Item2, 2);
        }
    }
}