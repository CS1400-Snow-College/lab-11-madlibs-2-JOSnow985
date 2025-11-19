// Jaden Olvera, CS-1400, Lab 11 madlibs 2
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
    }
}