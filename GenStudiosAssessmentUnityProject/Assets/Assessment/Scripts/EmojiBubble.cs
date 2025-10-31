using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmojiBubble : MonoBehaviour
{
    [SerializeField] Image emojiImage;
    [SerializeField] TMP_Text orderText;

    Camera mainCamera;
    // cache 
    WaitForSeconds secondsToDisplayEmoji;

    SO_GameData gameData;

    public void Initialize()
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
            CharacterBase.EMood.None => gameData.orderSprite,
            CharacterBase.EMood.Happy => gameData.happyEmojiSprite,
            CharacterBase.EMood.Angry => gameData.angryEmojiSprite,
            _ => null
        };

        ShowOrder(false);

        // Set the emoji image to the selected emoji
        emojiImage.sprite = targetSprite;

        // Show the emoji bubble
        gameObject.SetActive(true);

        // Start the coroutine to hide the emoji after a duration
        // StartCoroutine(HideEmojiAfterDelay());
    }
    
    // probably can have a better structure
    public void ShowOrder(bool active, string orderDisplayName = null)
    {
        orderText.gameObject.SetActive(active);
        if (!active)
        {
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        emojiImage.sprite = gameData.orderSprite;
        orderText.text = orderDisplayName;
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
            Vector3 directionToCamera = mainCamera.transform.position - emojiImage.transform.position;

            // Set the y component to 0 to prevent vertical flipping (rotation on X and Z axes)
            // directionToCamera.y = 0;

            // If the direction is non-zero, rotate the emoji to face the camera
            if (directionToCamera != Vector3.zero)
            {
                // Create a rotation that faces the camera
                Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);

                // Apply the rotation to the emoji bubble
                emojiImage.transform.rotation = lookRotation * Quaternion.Euler(0, 180, 0);

                emojiImage.transform.position = transform.position + mainCamera.transform.up;
            }
        }
    }
}
