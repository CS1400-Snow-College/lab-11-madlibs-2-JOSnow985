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
            string storyLine = File.ReadAllText(filename);

            // Split into words based on whitespaces and Add to storyWords
            storyWords.Add(storyLine.Split(" ").ToList());
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
                        // If we haven't found the separator to indicate where the key begins
                        if (foundKeyStart == false)
                        {
                            // Is the character the start of the key? If not, append it to the word for the dictionary
                            if (storyWords[storyIndex][wordListIndex][letterIndex] != ':')
                            {
                                buildWordToAdd.Append(storyWords[storyIndex][wordListIndex][letterIndex]);
                            }
                            // If it is the start of the key separator, change bool to true to switch over to key processing
                            else if (storyWords[storyIndex][wordListIndex][letterIndex] == ':')
                            {
                                foundKeyStart = true;
                            }
                        }
                        else if (foundKeyStart == true)
                        {
                            // Check if we're still in the separator
                            if (storyWords[storyIndex][wordListIndex][letterIndex] != ':')
                            {
                                // Check if we're looking at punctuation, if we aren't, put that character into the key
                                if (!".,! ".Contains(storyWords[storyIndex][wordListIndex][letterIndex]))
                                {
                                    buildKeyToAdd.Append(storyWords[storyIndex][wordListIndex][letterIndex]);
                                }
                                // If we are looking at punctuation, split that off to it's own string so it's not in our key
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

        Random rng = new();

        // Action<string> writeOut = Console.Write;

        for (int storyIndex = 0; storyIndex < storyWords.Count; storyIndex++)
        {
            using StreamWriter writeOut = new StreamWriter($"generatedstory{storyIndex + 1}.txt", append: false);
            for (int wordListIndex = 0; wordListIndex < storyWords[storyIndex].Count; wordListIndex++)
            {
                if (wordCategories.TryGetValue(storyWords[storyIndex][wordListIndex], out List<string>? dictionaryList))
                {
                    int randomIndex = rng.Next(0, dictionaryList.Count);
                    storyWords[storyIndex][wordListIndex] = dictionaryList[randomIndex];
                }

                if (storyWords[storyIndex][wordListIndex] == " ")
                {
                    // If it's just a whitespace somehow, we can skip it
                }
                else if (wordListIndex + 1 < storyWords[storyIndex].Count)
                {
                    if ("!,.".Contains(storyWords[storyIndex][wordListIndex + 1]))
                        writeOut.Write(storyWords[storyIndex][wordListIndex].Trim());
                    else
                        writeOut.Write(storyWords[storyIndex][wordListIndex].Trim() + " ");
                }
                else
                    writeOut.Write(storyWords[storyIndex][wordListIndex].Trim() + " ");
            }
        }
    }
}