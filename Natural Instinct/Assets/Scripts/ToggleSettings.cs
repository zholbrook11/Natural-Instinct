using UnityEngine;
using UnityEngine.UI;

public class ToggleSetting : MonoBehaviour
{
    public enum Category { music, action, environment, player, animal }
    public Category category;
    public SettingsManager manager;

    void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener((val) =>
        {
            manager.SetCategoryEnabled(category.ToString(), val);
        });
    }
}
