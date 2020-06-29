using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadJSON : MonoBehaviour
{
    public string filename = "JsonChallenge.json";

    public TextMeshProUGUI title;
    public Transform content;

    void Start()
    {
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

                }
            }
        }

        Debug.Log("File Readed");
    }

    string GetString(string line) {
        int start = line.IndexOf("\"");
        int last = line.LastIndexOf("\"");
        return line.Substring(start + 1, last - start - 1);
    }

    string GetKey(string line) {
        string key = line.Substring(0, line.IndexOf(":"));
        return GetString(key);
    }

    string GetValue(string line) {
        string val = line.Substring(line.IndexOf(":") + 1);
        return GetString(val);
    }

    int ParseHeaders(StreamReader reader) {
        // Prepare the GameObject
        var go = new GameObject("Headers");
        go.AddComponent<HorizontalLayoutGroup>();
        go.transform.SetParent(content);

        // Read the headers
        string line = reader.ReadLine();
        int headers = 0;
        while (!line.Contains("]")) {
            if (line.Contains("\"")) {
                string val = GetString(line);
                print(val);
                headers++;

                // Add the header to the row
                var header = new GameObject();
                var textComponent = header.AddComponent<TextMeshProUGUI>();
                textComponent.text = val;
                textComponent.fontStyle = FontStyles.Bold;
                header.transform.SetParent(go.transform);
            }

            line = reader.ReadLine();
        }
        return headers;
    }

    void ParseObject(StreamReader reader) {

    }
}
