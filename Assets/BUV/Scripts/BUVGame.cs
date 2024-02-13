﻿using UnityEngine;

    public class BUVGame
    {
        public const float BattleUnit_MIN_SPAWN_TIME = 5.0f;
        public const float BattleUnit_MAX_SPAWN_TIME = 10.0f;

        public const float PLAYER_RESPAWN_TIME = 3.0f;

        public const int PLAYER_MAX_LIVES = 3;

        public const string PLAYER_LIVES = "PlayerLives";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

        public static Color GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return Color.red;
                case 1: return Color.red;
                case 2: return Color.blue;
                case 3: return Color.blue;

            }

            return Color.black;
        }
    }
