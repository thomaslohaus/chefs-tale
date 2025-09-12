using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    [SerializeField] private Image barImage;
    [SerializeField] private Color barColor = new Color(1f, 183/255f, 0f, 1f);

    private void Start() {
        SetColor(barColor);
        ResetAndHide();
    }

    public void UpdateProgress(float progress) {
        if (barImage.fillAmount == 0f) {
            Show();
        }

        barImage.fillAmount = progress;
    }

    public void SetColor(Color color) {
        barImage.color = color;
    }

    public void ResetAndHide() {
        barImage.fillAmount = 0f;
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
