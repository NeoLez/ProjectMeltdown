using UnityEngine;

namespace Timers {
    public static class SeedUtils
    {
        public static int TextToSeed(string text) {
            int hash = 23;
        
            foreach (char c in text) {
                hash = hash * 31 + c; 
            }
        
            return hash;
        }

        public static int Combine(int[] ints) {
            int hash = 23;
        
            foreach (char i in ints) {
                hash = hash * 31 + i; 
            }
        
            return hash;
        }

        public static int PositionToSeed(Vector3 position, float resolution = 10) {
            position *= resolution;
            return Combine(new [] {GameManager.seed, (int)(position.x), (int)(position.y), (int)(position.z)});
        }
    }
}