using UnityEngine.UI;
using System.Linq;

public static class Extensions
{
    public static Toggle GetActive(this ToggleGroup aGroup)
    {
        return aGroup.ActiveToggles().FirstOrDefault();
    }
}