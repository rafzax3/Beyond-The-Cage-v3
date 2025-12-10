using UnityEngine;

[CreateAssetMenu(fileName = "NewAnomaly", menuName = "Game/Anomaly Data")]
public class AnomalyData : ScriptableObject
{
    [Header("Identifikasi")]
    [Tooltip("Harus SAMA PERSIS dengan nama scene anomali di Build Settings")]
    public string sceneNameID;

    [Header("Info Tampilan")]
    public string anomalyTitle;
    [TextArea(3, 10)] public string anomalyDescription;
    public Sprite anomalyImage;

    [Header("Tampilan Terkunci")]
    public Sprite lockedImage; // Gambar gembok
    public string lockedTitle = "???";
    public string lockedDescription = "You have not unlocked it yet.";
}