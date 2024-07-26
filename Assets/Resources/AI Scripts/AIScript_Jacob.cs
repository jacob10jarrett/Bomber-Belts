using UnityEngine;
using System.Collections;

public class AIScript_Jacob : MonoBehaviour {

    public CharacterScript mainScript;

    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public int[] beltDirections;
    public float[] buttonLocations;

    void Start () {
        mainScript = GetComponent<CharacterScript>();

        if (mainScript == null)
        {
            print("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        buttonLocations = mainScript.getButtonLocations();
        playerSpeed = mainScript.getPlayerSpeed();
    }

    void Update () {
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        bombSpeeds = mainScript.getBombSpeeds();
        
        float[] bombDistances = mainScript.getBombDistances();
        float playerLocation = mainScript.getCharacterLocation();
        float opponentLocation = mainScript.getOpponentLocation();

        // Find the optimal belt to target
        int targetBelt = FindOptimalBelt(bombDistances, opponentLocation);
        
        // Move to the button closest to player
        MoveToClosestButton(playerLocation, buttonLocations[targetBelt]);

        // Send bomb if ready
        SendBombIfReady(targetBelt);
    }

    // Finds the optimal belt to target based on bomb distances and opponent location
    int FindOptimalBelt(float[] bombDistances, float opponentLocation)
    {
        int targetBelt = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < bombDistances.Length; i++)
        {
            if ((beltDirections[i] == 0 || beltDirections[i] == -1) && buttonCooldowns[i] <= 0)
            {
                float distanceToOpponent = Mathf.Abs(opponentLocation - bombDistances[i]);
                if (distanceToOpponent < minDistance)
                {
                    minDistance = distanceToOpponent;
                    targetBelt = i;
                }
            }
        }

        return targetBelt;
    }

    // Moves the player to the closest button location with a threshold to avoid oscillations
    void MoveToClosestButton(float playerLocation, float targetButtonLocation)
    {
        float moveThreshold = 1.0f;
        if (Mathf.Abs(playerLocation - targetButtonLocation) > moveThreshold)
        {
            if (playerLocation < targetButtonLocation)
            {
                mainScript.moveUp();
            }
            else if (playerLocation > targetButtonLocation)
            {
                mainScript.moveDown();
            }
        }
    }

    // Sends the bomb if the button is ready and the player is close enough
    void SendBombIfReady(int targetBelt)
    {
        float interactionDistance = 5.0f;
        if (buttonCooldowns[targetBelt] <= 0 && Mathf.Abs(mainScript.getCharacterLocation() - buttonLocations[targetBelt]) < interactionDistance)
        {
            mainScript.push();
        }
    }
}
