using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameHeaven
{
    namespace Game2048
    {

        public enum GameState
        {
            GenerateLevel,
            SpawningBlocks,
            WaitingInput,
            Moving,
            Win,
            Lose
        }
    }
}