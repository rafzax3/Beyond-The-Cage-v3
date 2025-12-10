using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image backgroundImage; // Untuk highlight warna seleksi

    public void Setup(Sprite icon, string title, bool isUnlocked)
    {
        iconImage.sprite = icon;
        titleText.text = title;

        // Opsional: Ubah warna text jadi abu-abu jika terkunci
        titleText.color = isUnlocked ? Color.white : Color.gray;
    }

    public void SetSelected(bool isSelected, Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? color : new Color(0, 0, 0, 0.5f); // Warna default transparan
        }
    }
}