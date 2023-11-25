using System.Collections.Generic;
using Godot;

public class SaveHandler
{
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

    public static List<string> getListDeckNames()
    {
        Dictionary<string, List<string>> save = loadSave();
        return new List<string>(save.Keys);
    }

    public static List<string> getCardsFromDeck(string deckName)
    {
        Dictionary<string, List<string>> save = loadSave();
        return save[deckName];
    }

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
