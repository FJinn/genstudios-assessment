using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class EmojiBubble : MonoBehaviour
{
    [SerializeField] Image emojiImage;

    Camera mainCamera;
    // cache 
    WaitForSeconds secondsToDisplayEmoji;

    SO_GameData gameData;

    private void Start()
    {
        // Initially hide the emoji bubble
        gameObject.SetActive(false);

        if (mainCamera == null)
        {
            mainCamera = Camera.main.GetComponent<CinemachineBrain>().OutputCamera;
        }

        gameData = GameManager.Instance.gameData;

        secondsToDisplayEmoji = new WaitForSeconds(gameData.secondsToDisplayEmoji);
    }

    // Call this function to show an emoji
    public void ShowEmoji(CharacterBase.EMood mood)
    {
        Sprite targetSprite = mood switch
        {
            CharacterBase.EMood.None => null,
            CharacterBase.EMood.Happy => gameData.happyEmojiSprite,
            CharacterBase.EMood.Angry => gameData.angryEmojiSprite,
            _ => null
        };

        // Set the emoji image to the selected emoji
        emojiImage.sprite = targetSprite;

        // Show the emoji bubble
        gameObject.SetActive(true);

        // Start the coroutine to hide the emoji after a duration
        // StartCoroutine(HideEmojiAfterDelay());
    }

    // Coroutine to hide the emoji after a set duration
    private IEnumerator HideEmojiAfterDelay()
    {
        yield return secondsToDisplayEmoji;

        // Hide the emoji bubble
        gameObject.SetActive(false);
    }

    // manual control emoji display
    public void HideEmoji()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // Make sure the emoji bubble always faces the camera
        if (gameObject.activeSelf && mainCamera != null)
        {
            // Get the direction from the emoji bubble to the camera
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;

            // Set the y component to 0 to prevent vertical flipping (rotation on X and Z axes)
            directionToCamera.y = 0;

            // If the direction is non-zero, rotate the emoji to face the camera
            if (directionToCamera != Vector3.zero)
            {
                // Create a rotation that faces the camera
                Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);

                // Apply the rotation to the emoji bubble
                transform.rotation = lookRotation;
            }
        }
    }
}
