using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadJSON : MonoBehaviour
{
    public string filename = "JsonChallenge.json";

    public Text title;
    public Transform content;

    Font m_arialFont;

    void Start()
    {
        // Get the default font
        m_arialFont = (Font)Resources.GetBuiltinResource<Font>("Arial.ttf");
        Load();
    }

    public void Load() {
        // Clear Content
        for (int i = content.transform.childCount - 1; i >= 0; i--) {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        // Load JSON
        string path = Application.streamingAssetsPath + "/" + filename;
        if (File.Exists(path)) {
            Debug.Log("Read file");
            StartCoroutine(ReadFile(path));
        } else {
            Debug.Log("File doesn't exists");
        }
    }

    IEnumerator ReadFile(string path) {
        Debug.Log("Reading...");
        title.text = "Reading...";
        yield return null;

        // Read and Parse JSON
        using (var reader = new StreamReader(path)) {
            while (!reader.EndOfStream) {
                string line = reader.ReadLine().Trim();
                string key = "";

                if (line.Contains(":")) {
                    key = GetKey(line);
                }

                // Title
                if (key.Equals("Title")) {
                    title.text = GetValue(line);
                }

                // Headers
                if (key.Equals("ColumnHeaders")) {
                    ParseHeaders(reader);
                }

                // Data
                if (key.Equals("Data")) {
                    while (!line.Contains("]")) {
                        if (line.Contains("{")) {
                            ParseObject(reader);
                        }
                        line = reader.ReadLine();
                    }
                }
            }
        }

        Debug.Log("File Readed");
    }

    // Get the string between the " "
    string GetString(string line) {
        int start = line.IndexOf("\"");
        int last = line.LastIndexOf("\"");
        return line.Substring(start + 1, last - start - 1);
    }

    // Get the Key of the line
    string GetKey(string line) {
        string key = line.Substring(0, line.IndexOf(":"));
        return GetString(key);
    }

    // Get the Value of the line
    string GetValue(string line) {
        string val = line.Substring(line.IndexOf(":") + 1);
        return GetString(val);
    }

    void ParseHeaders(StreamReader reader) {
        // Read the headers
        int headers = 0;
        string line = reader.ReadLine();
        while (!line.Contains("]")) {
            if (line.Contains("\"")) {
                string val = GetString(line);

                // Add the header to the grid
                var header = new GameObject("Header");
                var textComponent = header.AddComponent<Text>();
                textComponent.text = val;
                textComponent.font = m_arialFont;
                textComponent.fontStyle = FontStyle.Bold;
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.fontSize = 36;
                header.transform.SetParent(content);
                headers++;
            }

            line = reader.ReadLine();
        }

        // Set the cell size of the grid
        var layout = content.GetComponent<GridLayoutGroup>();
        var w = content.GetComponent<RectTransform>().rect.width / headers;
        var h = 70;
        layout.cellSize = new Vector2(w, h);
        layout.constraintCount = headers;
    }

    void ParseObject(StreamReader reader) {
        // Read the Data
        string line = reader.ReadLine();
        while (!line.Contains("}")) {
            if (line.Contains("\"")) {
                string val = GetValue(line);

                // Add the data to the grid
                var col = new GameObject("Data");
                var textComponent = col.AddComponent<Text>();
                textComponent.text = val;
                textComponent.font = m_arialFont;
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.fontSize = 24;
                col.transform.SetParent(content);
            }

            line = reader.ReadLine();
        }
    }
}
