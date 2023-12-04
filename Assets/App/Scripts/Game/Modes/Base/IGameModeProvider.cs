﻿using PhlegmaticOne.FruitNinja.Shared;

namespace App.Scripts.Game.Modes.Base
{
    public interface IGameModeProvider
    {
        void ApplyGameMode(GameDataBase gameDataBase);
        bool IsCurrentCompleted();
        GameEndStateViewModel BuildGameEndState(PlayersSyncMessage endGameMessage);
        bool IsBreakGame();
    }
}