using UnityEngine;

public class BossFightTheme : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.PlayMusic("BossTheme");
    }
}
