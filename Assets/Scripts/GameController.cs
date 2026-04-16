using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gam : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject LoadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public static event Action OnReset;
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gems.onGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayedDied += GameOverScreen;
        LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);

    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
        if (survivedLevelsCount != 1) survivedText.text += "S"; //i want only just level so yea 
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            //level completedd
            LoadCanvas.SetActive(true);
            Debug.Log("Level Completed");

        }
        
        
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
         LoadCanvas.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 0, 0);

        currentLevelIndex = level;
        progressAmount = 0;
        progressSlider.value = 0;
        if(wantSurvivedIncrease)survivedLevelsCount++;

    }

    // Update is called once per frame
    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1; //basicly what this line is saying is that its an if statement but one line, its saying if our current level index which the current level and its starts with 0 coz its index array thingy so its minus one , and also if we start at level 2 and theres no more levels to load then we going back to level one thats it level 1 is 0 coz array and then else + 1 so that itslvel 1!
        LoadLevel(nextLevelIndex, true);


    }
}
