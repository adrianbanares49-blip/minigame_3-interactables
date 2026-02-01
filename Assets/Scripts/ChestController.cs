using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.SceneManagement;

public class ChestController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField ] GameObject textContainer;
    [SerializeField] GameObject keyContainer;
    [SerializeField] GameObject boxContainer;
    [SerializeField] GameObject[] chestStages;
    [SerializeField] GameObject letterButton;
    [SerializeField] TextAsset possibleWord;
    [SerializeField] GameObject endPanel;
    [SerializeField] Button continueButton;

    private string word;
    private int incorrectGuesses, correctGuesses;
    private bool gameOver;
    private int chestProgress;

    void Start()
    {
        InitializeGame();
        InitializeButtons();
        continueButton.onClick.AddListener(Continue);
    }

    private void InitializeButtons()
    {
        for(int i = 65; i<=90;i++)
        {
            CreateButton(i);
        }
        endPanel.SetActive(false);
        chestProgress = 0;
    }
    
    private void Continue()

    {
        endPanel.SetActive(false);
        InitializeGame();
    }

    private void DisableAllButton()
    {
        foreach(Button btn in keyContainer.GetComponentsInChildren<Button>())
        {
            btn.interactable = false;
        }
            
    }

    private void InitializeGame()
    {
        //reset to origin state
        incorrectGuesses = 0;
        correctGuesses =0;
        endPanel.SetActive(false);
        continueButton.gameObject.SetActive(false);
        foreach(Button child in keyContainer.GetComponentsInChildren<Button>())
        {
            child.interactable = true;
        }
        foreach(Transform child in textContainer.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
        foreach(GameObject stage in chestStages)
        {
            stage.SetActive(false);
        }

        
        //generate new word
        word = generateWord().ToUpper();
        foreach(char letter in word)
        {
            var temp =Instantiate(boxContainer, textContainer.transform);
        }

        gameOver = false;

    }

    private void CreateButton(int i)
    {
        GameObject temp = Instantiate(letterButton,keyContainer.transform);
        temp.GetComponentInChildren<TextMeshProUGUI>().text=((char)i).ToString();
        temp.GetComponent<Button>().onClick.AddListener(delegate{CheckLetter(((char)i).ToString());});
    }

    private string generateWord()
    {
        string[] wordList = possibleWord.text.Split("\n");
        string line = wordList[Random.Range(0, wordList.Length-1)];
        return line.Substring(0, line.Length-1);
    }


    // correct/inco letter logic
    private void CheckLetter(string inputLetter)
    {
        
        if(gameOver) return; //stop if the game ends

        foreach (Button btn in keyContainer.GetComponentsInChildren<Button>())
        {
            if (btn.GetComponentInChildren<TextMeshProUGUI>().text == inputLetter)
            {
                btn.interactable = false;
                break;
            }        
        }
           
        bool letterInWord = false;
        for(int i=0;i<word.Length;i++)
        {
            if(inputLetter == word[i].ToString()) 
            {
                textContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = inputLetter;
                //correctGuesses++;
                letterInWord = true;
            }
        }

        if(letterInWord)
        {
            chestProgress++;

            int stageIndex = Mathf.Clamp(chestProgress -1, 0, chestStages.Length -1);
            //visibility of each stage
            foreach(GameObject stage in chestStages)
            {
                stage.SetActive(false);
            }
            
            chestStages[stageIndex].SetActive(true);
            correctGuesses++;
        }
            else
            {
                incorrectGuesses++;
            }
        CheckOutCome();
    }
    
    private void CheckOutCome()
    {
        if(correctGuesses == word.Length)
        {
            gameOver = true;
            DisableAllButton();
            endPanel.SetActive(true); //unlocked chest
            continueButton.gameObject.SetActive(true); //show btn


            for(int i=0;i<word.Length;i++)
            {
                textContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.red;
            }
            Invoke("InitializeGame", 3f);
            return;
        } //indicates that u won

        if(incorrectGuesses == chestStages.Length)
        {
            for(int i=0;i<word.Length;i++)
            {
                textContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.red;
                textContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = word[i].ToString();
            }
            Invoke("InitializeGame", 3f); //lose

        }

    }

}
