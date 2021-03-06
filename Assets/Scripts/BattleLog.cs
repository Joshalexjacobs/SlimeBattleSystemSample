using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class BattleLog : MonoBehaviour
{

    [SerializeField] private Text text;

    [SerializeField] private int maxLines;

    [SerializeField] private List<String> logList = new List<string>(); 

    public void UpdateLog(string text)
    {
        string [] splitText = Regex.Split(text, @"(?=\ )");

        string line = "\n";
        
        foreach (var s in splitText)
        {
            if (line.Length + s.Length + 1 > 23)
            {
                logList.Add(line);

                line = "\n";
                
                line += $"{s.TrimStart()}";
            }
            else {
                line += $"{s}";    
            }
        }

        if (line.Length > 0)
        {
            logList.Add(line);
        }

        foreach (var l in logList)
        {
            this.text.text += l;
        }

        var numberOfLines = this.text.text.Split("\n").Length;
        
        if (numberOfLines > maxLines)
        {
            var pattern = @"(?=\n)";

            var currentLog = Regex.Split(this.text.text, pattern, RegexOptions.Multiline).ToList();

            if (numberOfLines > maxLines)
            {
                var count = numberOfLines - maxLines;

                currentLog.RemoveRange(0, count);
            }

            this.text.text = currentLog.Aggregate((a, b) => a + b);
        }
        
        logList.Clear();
    }

}
