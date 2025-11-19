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
                                // If we are looking at punctuation, split that off to it's own string so it's not in our key
                                if (".,! ".Contains(storyWords[storyIndex][wordListIndex][letterIndex]))
                                {
                                    // buildWordToAdd.Append(storyWords[storyIndex][wordListIndex][letterIndex]);
                                    storyWords[storyIndex].Insert(wordListIndex + 1, $"{storyWords[storyIndex][wordListIndex][letterIndex]}");
                                }
                                // If we aren't looking at punctuation, only thing to do is add it to the key
                                else
                                    buildKeyToAdd.Append(storyWords[storyIndex][wordListIndex][letterIndex]);
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

        for (int storyIndex = 0; storyIndex < storyWords.Count; storyIndex++)
        {
            using StreamWriter writeOut = new StreamWriter($"generatedstory{storyIndex + 1}.txt", append: false);
            for (int wordListIndex = 0; wordListIndex < storyWords[storyIndex].Count; wordListIndex++)
            {
                string currentWord = "";
                bool articleTweak = false;
                bool wasCapital = false;
                // If we're about to write a, an, or A, An, store that and check the next word for which it should be
                if (storyWords[storyIndex][wordListIndex] == "a" || storyWords[storyIndex][wordListIndex] == "an")
                {
                    articleTweak = true;
                    wordListIndex++;
                    wasCapital = false;
                }
                else if (storyWords[storyIndex][wordListIndex] == "A" || storyWords[storyIndex][wordListIndex] == "An")
                {
                    articleTweak = true;
                    wordListIndex++;
                    wasCapital = true;
                }

                // Check if the word we're looking at matches a key in the dictionary
                // If It does, we want to randomly choose a replacement word and supply that word to be written to the file 
                if (wordCategories.TryGetValue(storyWords[storyIndex][wordListIndex], out List<string>? dictionaryList))
                {
                    int randomIndex = rng.Next(0, dictionaryList.Count);
                    currentWord = $"{dictionaryList[randomIndex].Trim()}";
                }
                else
                    currentWord = $"{storyWords[storyIndex][wordListIndex].Trim()}";

                // If we need to add "A" or "An", do it here
                if (articleTweak == true && wasCapital == false)
                {
                    if ("aeiou".Contains(char.ToLowerInvariant(currentWord[0])))
                    {
                        currentWord = "an " +currentWord;
                    }
                    else
                    {
                        currentWord = "a " + currentWord;
                    }
                }
                else if (articleTweak == true && wasCapital == true)
                {
                    if ("aeiou".Contains(char.ToLowerInvariant(currentWord[0])))
                    {
                        currentWord = "An " + currentWord;
                    }
                    else
                    {
                        currentWord = "A " + currentWord;
                    }
                }

                if (string.IsNullOrEmpty(currentWord) || string.IsNullOrWhiteSpace(currentWord))
                {
                    // If it's just a whitespace, empty, or null somehow, we can skip it
                }
                else if (wordListIndex + 1 < storyWords[storyIndex].Count)
                {
                    if ("!,.".Contains(storyWords[storyIndex][wordListIndex + 1]))
                        writeOut.Write(currentWord);
                    else
                        writeOut.Write(currentWord + " ");
                }
                else
                    writeOut.Write(currentWord + " ");
                // If we have a line ending punctuation present, drop in a new line so it's easier to read as a text file
                if (currentWord.EndsWith('.') || currentWord.EndsWith('!') || currentWord.EndsWith('?'))
                {
                    writeOut.Write("\n");
                }
            }
        }
    }
}