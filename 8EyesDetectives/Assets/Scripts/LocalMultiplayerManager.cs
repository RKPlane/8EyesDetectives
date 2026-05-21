using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMultiplayerManager : MonoBehaviour
{
    public GameObject spiderPrefab;
    public GameObject mantisPrefab;

    private bool spiderSpawned;
    private bool mantisSpawned;

    private Gamepad spiderPad;
    private Gamepad mantisPad;

    void Update()
    {
        foreach (Gamepad g in Gamepad.all)
        {
            if (!g.startButton.wasPressedThisFrame)
                continue;

            // Si este mando ya está usado, ignorar
            if (g == spiderPad || g == mantisPad)
                continue;

            // Asignar Player 1 (Spider)
            if (!spiderSpawned)
            {
                spiderPad = g;

                PlayerInput.Instantiate(
                    spiderPrefab,
                    controlScheme: "Gamepad",
                    pairWithDevice: g
                );

                spiderSpawned = true;
                Debug.Log($"Spider joined with {g.displayName}");
                continue;
            }

            // Asignar Player 2 (Mantis)
            if (!mantisSpawned)
            {
                mantisPad = g;

                PlayerInput.Instantiate(
                    mantisPrefab,
                    controlScheme: "Gamepad",
                    pairWithDevice: g
                );

                mantisSpawned = true;
                Debug.Log($"Mantis joined with {g.displayName}");
                continue;
            }
        }
    }
}