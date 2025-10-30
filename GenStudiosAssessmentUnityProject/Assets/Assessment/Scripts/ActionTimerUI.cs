using UnityEngine;
using UnityEngine.UI;

public class ActionTimerUI : MonoBehaviour
{
    [SerializeField] Image doneImage;
    [SerializeField] Image toDoImage;
    [SerializeField] Image cleanImage;

    public void RunToDo()
    {
        gameObject.SetActive(true);
        toDoImage.gameObject.SetActive(true);
    }

    public void DoneAction()
    {
        doneImage.gameObject.SetActive(true);
        toDoImage.gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        doneImage.gameObject.SetActive(false);
        toDoImage.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// value should be 0 - 1
    /// </summary>
    public void UpdateToDoFillValue(float value)
    {
        toDoImage.fillAmount = value;
    }
}
