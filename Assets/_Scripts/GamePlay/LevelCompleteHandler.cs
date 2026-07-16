using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteHandler : BaseBehaviour
{
    protected override void Start()
    {
        LevelGoalManager.Instance.OnLevelCompleted +=
        this.HandleLevelComplete;
    }
    private void HandleLevelComplete()
    {
        // BoardInputHandler.Disable();

        LevelGoalManager.Instance.SetLevelState(LevelState.Completing);

        // TapToContinueUI.Show();

        StartCoroutine(
            ConvertMovesToRocketRoutine()
        );
    }

    IEnumerator ConvertMovesToRocketRoutine()
    {
        while (LevelGoalManager.Instance.RemainingMoves > 0)
        {
            LevelGoalManager.Instance.UseMove();

            // Random gem.
            // Transform rocket.
            // Trigger.
            // Wait board stable.

            yield return new WaitForSeconds(0.5f);
        }

        LevelGoalManager.Instance
            .SetLevelState(LevelState.WaitingForContinue);
    }
}