using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMultiplayerManager : MonoBehaviour
{
    public Player spider;
    public MantisPlayer mantis;

    private Gamepad spiderPad;
    private Gamepad mantisPad;

    void Update()
    {
        foreach (var pad in Gamepad.all)
        {
            if (pad.startButton.wasPressedThisFrame)
            {
                if (spiderPad == null)
                {
                    AssignSpider(pad);
                }
                else if (mantisPad == null && pad != spiderPad)
                {
                    AssignMantis(pad);
                }
            }
        }
    }

    void AssignSpider(Gamepad pad)
    {
        spiderPad = pad;

        var input = spider.GetComponent<PlayerInput>();
        input.SwitchCurrentControlScheme("Gamepad", pad);

        spider.control = true;

        Debug.Log("Spider assigned to " + pad.name);
    }

    void AssignMantis(Gamepad pad)
    {
        mantisPad = pad;

        var input = mantis.GetComponent<PlayerInput>();
        input.SwitchCurrentControlScheme("Gamepad", pad);

        mantis.control = true;

        Debug.Log("Mantis assigned to " + pad.name);
    }
}