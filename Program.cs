// Jaden Olvera, CS-1400, Lab 11 madlibs 2
using System.Text;

class Program
{
    static void Main(string[] filenames)
    {
        Dictionary<string, List<string>> wordCategories = [];
        List<List<string>> storyWords = [];
        foreach(string filename in filenames)
        {
            // Load current filename into a string array
            string[] storyLines = File.ReadAllLines(filename);

            // Split into words based on whitespaces and Add to storyWords
            foreach (string line in storyLines)
            {
                storyWords.Add(line.Split(" ").ToList());
            }
        }

        // Processing each story
        for (int storyIndex = 0; storyIndex < storyWords.Count; storyIndex++)
        {
            // Processing each list of words
            for (int wordListIndex = 0; wordListIndex < storyWords[storyIndex].Count; wordListIndex++)
            {
                // If the word we're looking at contains a :, the word and key need to be in the dictionary
                if (storyWords[storyIndex][wordListIndex].Contains(':'))
                {
                    bool foundKeyStart = false;
                    StringBuilder buildWordToAdd = new();
                    StringBuilder buildKeyToAdd = new();
                    for (int letterIndex = 0; letterIndex < storyWords[storyIndex][wordListIndex].Length; letterIndex++)
                    {
                        if (foundKeyStart == false)
                        {
                            if (storyWords[storyIndex][wordListIndex][letterIndex] != ':')
                            {
                                buildWordToAdd.Append(storyWords[storyIndex][wordListIndex][letterIndex]);
                            }
                            else if (storyWords[storyIndex][wordListIndex][letterIndex] == ':')
                            {
                                foundKeyStart = true;
                            }
                        }

                        else if (foundKeyStart == true)
                        {
                            if (storyWords[storyIndex][wordListIndex][letterIndex] != ':')
                            {
                                if (!".,! ".Contains(storyWords[storyIndex][wordListIndex][letterIndex]))
                                {
                                    buildKeyToAdd.Append(storyWords[storyIndex][wordListIndex][letterIndex]);
                                }
                                else if (".,! ".Contains(storyWords[storyIndex][wordListIndex][letterIndex]))
                                {
                                    // buildWordToAdd.Append(storyWords[storyIndex][wordListIndex][letterIndex]);
                                    storyWords[storyIndex].Insert(wordListIndex + 1, $"{storyWords[storyIndex][wordListIndex][letterIndex]}");
                                }
                            }
                        }
                    }
                    string wordToAdd = buildWordToAdd.ToString();
                    string keyToAdd = buildKeyToAdd.ToString();
                    // Check if the dictionary already has this key in it
                    // If it does, check if that key already has that word in it
                    if (wordCategories.ContainsKey(keyToAdd))
                    {
                        if (!wordCategories[keyToAdd].Contains(wordToAdd))
                        {
                            wordCategories[keyToAdd].Add(wordToAdd);
                        }
                    }
                    else
                    {
                        List<string> ListToAdd = [wordToAdd];
                        wordCategories.Add(keyToAdd, ListToAdd);
                    }
                    storyWords[storyIndex][wordListIndex] = keyToAdd;
                }
            }
        }
    }
}