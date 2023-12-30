using System.Collections.Generic;
using Godot;

/// <summary>
/// The class SaveHandler is an helper class to handle saving information
/// In this project, the only thing to save is the deck that the user create
/// </summary>
public class SaveHandler
{
    /// <summary>
    /// Save the deck to the local file
    /// </summary>
    /// <param name="cardIds"> List of ids of the different card from the deck </param>
    /// <param name="deckName"> The name of the deck that the user choose </param>
    public static void saveDeck(List<string> cardIds, string deckName)
    {
        var saveFile = new Godot.File();
        
        // Load the file deck.save
        Dictionary<string, List<string>> save = loadSave();
        
        // If the deck already exist, we delete it 
        if (save.ContainsKey(deckName))
            save.Remove(deckName);
        
        // add the deck into the existing dict
        save.Add(deckName, cardIds);
        
        // save the dict in the save file
        saveFile.Open("user://deck.save", Godot.File.ModeFlags.Write);
        saveFile.StoreString(Newtonsoft.Json.JsonConvert.SerializeObject(save));
        saveFile.Close();
    }

    /// <summary>
    /// Remove a deck from the local file
    /// </summary>
    /// <param name="deckName"> The name of the deck to remove </param>
    public static void removeDeck(string deckName)
    {
        var saveFile = new Godot.File();
        
        // Load the file deck.save
        Dictionary<string, List<string>> save = loadSave();
        
        // If the deck already exist, we delete it 
        if (save.ContainsKey(deckName))
            save.Remove(deckName);
        
        // save the dict in the save file
        saveFile.Open("user://deck.save", Godot.File.ModeFlags.Write);
        saveFile.StoreString(Newtonsoft.Json.JsonConvert.SerializeObject(save));
        saveFile.Close();
    }

    /// <summary>
    /// Get the complete list of all the deck that are in the local file, only the name
    /// </summary>
    /// <returns> Names of all the deck saved </returns>
    public static List<string> getListDeckNames()
    {
        Dictionary<string, List<string>> save = loadSave();
        return new List<string>(save.Keys);
    }

    /// <summary>
    /// The all the cards ids from the local file using the deck name
    /// </summary>
    /// <param name="deckName"> the name of deck </param>
    /// <returns> a list with all the cards ids from this deck </returns>
    public static List<string> getCardsFromDeck(string deckName)
    {
        Dictionary<string, List<string>> save = loadSave();
        return save[deckName];
    }

    /// <summary>
    /// Helper function to load the information in the local file
    /// </summary>
    /// <returns> A dictionnary with all the deck name as key and items are the list of ids </returns>
    private static Dictionary<string, List<string>> loadSave()
    {
        var saveFile = new Godot.File();
        Dictionary<string, List<string>> save;
        Error e = saveFile.Open("user://deck.save", Godot.File.ModeFlags.Read);
        if (e != Error.Ok)
        {
            //file doesn't already exist
            save = new Dictionary<string, List<string>>();
        }
        else
        {
            //file already already exist
            string text = saveFile.GetAsText();
            save = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text);
        }
        saveFile.Close();

        return save;
    }
}
