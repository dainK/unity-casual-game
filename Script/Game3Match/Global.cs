using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameHeaven
{
    namespace Game3Match
    {
        [Serializable]
        public class Global
        {
            public static int col = 7;
            public static int row = 9;
            public static float offset = 0f;
            public static float size = 140f;
            public static float moveDuration = 0.5f;
        }

        public enum MatchType
        {
            None,
            Match3,
            Both,
            Vertical,
            Horizontal,
            //Both4
        }

        public enum State
        {
            None,
            Start,
            Idle,
            CheckEnable,
            Reset,
            Change,
            Revert,
            CurrentCheckMatch,
            CheckMatch,
            RemoveBlock,
            MoveDown,
            TimeOver,
            //Pause,
        }
    }
}