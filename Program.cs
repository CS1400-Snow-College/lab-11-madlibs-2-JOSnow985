// Jaden Olvera, CS-1400, Lab 11 madlibs 2
using System.Diagnostics;
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
                // If the word we're looking at contains a :, the word and key need extracted to the dictionary
                if (storyWords[storyIndex][wordListIndex].Contains(':'))
                    madlibExtractor(storyWords, wordCategories, storyIndex, wordListIndex);
            }
        }
        
        // Dictionary should not be empty
        Debug.Assert(wordCategories.Count != 0);
        // Dictionary should have a key for past-tense-verb from the first story
        Debug.Assert(wordCategories.ContainsKey("past-tense-verb"));
        // But it should not have one that includes the punctuation after it
        Debug.Assert(wordCategories.ContainsKey("plural-noun.") == false);
        // It should include the word jeered in the past-tense-verb list
        Debug.Assert(wordCategories["past-tense-verb"].Contains("jeered"));

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
                currentWord = wordSelector(wordCategories, storyWords[storyIndex][wordListIndex]);

                // If we need to add "A" or "An", do it here
                if (articleTweak == true && wasCapital == false)
                {
                    if ("aeiou".Contains(char.ToLowerInvariant(currentWord[0])))
                    {
                        currentWord = "an " + currentWord;
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

        // Methods
        static void madlibExtractor(List<List<string>> storyList, Dictionary<string, List<string>> dictionary, int listIndex, int stringIndex)
        {
            bool foundKeyStart = false;
            StringBuilder buildWordToAdd = new();
            StringBuilder buildKeyToAdd = new();
            for (int letterIndex = 0; letterIndex < storyList[listIndex][stringIndex].Length; letterIndex++)
            {
                // If we haven't found the separator to indicate where the key begins
                if (foundKeyStart == false)
                {
                    // Is the character the start of the key? If not, append it to the word for the dictionary
                    if (storyList[listIndex][stringIndex][letterIndex] != ':')
                        buildWordToAdd.Append(storyList[listIndex][stringIndex][letterIndex]);
                    // If it is the start of the key separator, change bool to true to switch over to key processing
                    else if (storyList[listIndex][stringIndex][letterIndex] == ':')
                        foundKeyStart = true;
                }
                else if (foundKeyStart == true)
                {
                    // Check if we're still in the separator
                    if (storyList[listIndex][stringIndex][letterIndex] != ':')
                    {
                        // If we are looking at punctuation, split that off to it's own string so it's not in our key
                        if (".,! ".Contains(storyList[listIndex][stringIndex][letterIndex]))
                            storyList[listIndex].Insert(stringIndex + 1, $"{storyList[listIndex][stringIndex][letterIndex]}");
                        // If we aren't looking at punctuation, only thing to do is add it to the key
                        else
                            buildKeyToAdd.Append(storyList[listIndex][stringIndex][letterIndex]);
                    }
                }
            }
            string wordToAdd = buildWordToAdd.ToString();
            string keyToAdd = buildKeyToAdd.ToString();
            // Check if the dictionary already has this key in it
            // If it does, check if that key already has that word in it
            if (dictionary.ContainsKey(keyToAdd))
            {
                if (!dictionary[keyToAdd].Contains(wordToAdd))
                {
                    dictionary[keyToAdd].Add(wordToAdd);
                }
            }
            else
            {
                List<string> ListToAdd = [wordToAdd];
                dictionary.Add(keyToAdd, ListToAdd);
            }
            storyList[listIndex][stringIndex] = keyToAdd;
        }

        static string wordSelector(Dictionary<string, List<string>> dictionary, string stringToCheck)
        {
            Random rng = new();
            if (dictionary.TryGetValue(stringToCheck, out List<string>? dictionaryList))
            {
                int randomIndex = rng.Next(0, dictionaryList.Count);
                return $"{dictionaryList[randomIndex].Trim()}";
            }
            else
                return $"{stringToCheck.Trim()}";
        }
    }
}