using System;
using UniRx;
using UnityEngine;

public class RaceDetailsButtonPanel : MonoBehaviour
{
    public void OnCreatePlayer()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("CreatePlayerDialog", true);
        var dialogRef = DialogService.GetInstance().Show(go);
        
        dialogRef
            .AfterClosed()
            .TakeUntilDestroy(this)
            .Subscribe(data =>
            {
                if (data != null)
                    OnPlayerCreated(data);
            });
    }

    private void OnPlayerCreated(object data)
    {
        var form = data as CreatePlayerForm;
        if (form == null)
        {
            throw new UnityException("Expected CreatePlayerForm but was something else.");
        }

        try
        {
            var playerInfo = RaceTimerServices.GetInstance().RaceService.CreateRacePlayer(
                form.RaceId,
                form.Name,
                form.Age,
                form.Email,
                form.TeamName);

            if (playerInfo == null)
                throw new Exception("Failed to create player");
        }
        catch (Exception ex)
        {
            throw new UnityException("Failed to create player", ex);
        }
    }
}
