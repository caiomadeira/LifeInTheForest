using RedLoader;
using Sons.Gameplay;
using Sons.Gui;
using SonsSdk;
using UnityEngine;
using TMPro;
using TheForest.Utils;

namespace LifeInTheForest;

[RegisterTypeInIl2Cpp]
public class LITFKelvinMenu : MonoBehaviour
{
    public string CustomText = "Opcoes...";
    private void Update()
    {
        if (LocalPlayer._instance == null || LocalPlayer._instance._specialActions == null) return;
        var robbyInteration = LocalPlayer._instance._specialActions.GetComponent<PlayerRobbyInteraction>();
        if (robbyInteration == null) return;

        RobbyWorldUi tactiPad = robbyInteration._tactiPad;

        if (tactiPad != null && tactiPad.gameObject.activeSelf)
        {
            DrawCustomOption(tactiPad);
        }
    }

    private void DrawCustomOption(RobbyWorldUi tactiPad)
    {
        Transform textObjTransform = tactiPad._canvas.transform.Find("KelvinCustomOptionText");

        if (textObjTransform == null)
        {
            GameObject gameObject = new GameObject("KelvinCustomOptionText");
            gameObject.transform.SetParent(tactiPad._canvas.transform, false);

            TextMeshProUGUI textMesh = gameObject.AddComponent<TextMeshProUGUI>();

            textMesh.rectTransform.sizeDelta = new Vector2(400f, 200f);
            textMesh.rectTransform.anchoredPosition = new Vector2(0, -150f);

            var originalText = tactiPad._canvas.transform.Find("Panel/AndText");
            if (originalText != null)
            {
                textMesh.font = originalText.GetComponent<TextMeshProUGUI>().font;
            }

            textMesh.fontSize = 28f;
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignmentOptions.Center;
            textObjTransform = gameObject.transform;
        }

        TextMeshProUGUI component = textObjTransform.GetComponent<TextMeshProUGUI>();
        if (component != null)
        {
            component.text = CustomText;
        } else { RLog.Msg("componet kelvin menu is null error");  }

    }
}

