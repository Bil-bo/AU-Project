using System.Collections;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;


// Player in main scene
public class PlayerPropsRoaming : MonoBehaviour

{

    public BattleInfo BattleInfo;

    public float _speed;
    public Vector2 moveValue;

    private CharacterController charcon;
    private bool charIsGrounded;
    public MainGameManager gameManager;

    public GameData gameData;
    public GameObject pause;

    private bool hasWon = false;

    // Start is called before the first frame update
    void Start()
    {
        charcon = GetComponent<CharacterController>();
        pause.SetActive(false);
    }

    // Temporarily stopping the game: Mainly used for making it back to the main menu
    void OnPause()
    {
        Debug.Log("Paused");
        pause.SetActive(true);
        Time.timeScale = 0;

        //Unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Movement
    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();

    }
    
  
    void Update()
    {
        if (charcon)
        {
            handleMovement();
            charIsGrounded = charcon.isGrounded;
        }

    }

    void handleMovement()
    {

        var camera = Camera.main;

        var forward = camera.transform.forward;
        var right = camera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        Vector3 currentMovement = (forward * moveValue.y +right * moveValue.x) * _speed * Time.deltaTime;
        currentMovement.y = charIsGrounded ? 0.0f : -1.0f;
        Physics.SyncTransforms(); //REQUIRED - allows gamecontroller to move player
        charcon.Move(currentMovement);
    }


    private IEnumerator DelayAndLoadMainMenu()
{
    yield return new WaitForSeconds(3f); // Wait for 3 seconds

    // Load the main menu scene
    SceneManager.LoadScene("Menu");
}

    
    // For winning the game
    private void DisplayWinMessage()
    {
        

        if(hasWon){
            TextMeshProUGUI winText = GameObject.FindGameObjectWithTag("WinText")?.GetComponent<TextMeshProUGUI>();

            if(winText != null)
            {
                winText.text = "You won!";
                gameManager.ResetPositions(new Vector3(4, 4, 4));
                PlayerPrefs.SetInt("PickupsCollected", 0);


                StartCoroutine(DelayAndLoadMainMenu());
                             
            }
        }
    }
}