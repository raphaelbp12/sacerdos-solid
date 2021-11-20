using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    private static TooltipManager current;

    public Tooltip tooltip;

    private void Awake()
    {
        current = this;
    }

    public static void Show(string content, string header = "")
    {
        if (string.IsNullOrEmpty(content) && string.IsNullOrEmpty(header)) return;
        current.tooltip.SetText(content, header);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
