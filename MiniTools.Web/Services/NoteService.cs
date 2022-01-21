using MiniTools.Web.Models;
using MiniTools.Web.MongoEntities;
using MongoDB.Driver;

namespace MiniTools.Web.Services;

public interface INoteService
{

    void AddTags(string id, string tagList);
    void RemoveTag(string id, string tag);
    void AddNote(string content);
    List<Note> GetNotes();
    Note GetNotes(string id);
    void UpdateNote(string id, string content);
}

public class NoteService : INoteService
{
    readonly ILogger<NoteService> logger;

    readonly IMongoCollection<Note> noteCollection;

    public NoteService(ILogger<NoteService> logger, IMongoCollection<Note> noteCollection)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.noteCollection = noteCollection;
    }

    public void AddNote(string content)
    {
        string title = getTitle(content);

        noteCollection.InsertOne(new Note
        {
            Title = title,
            Content = content
        });
    }

    private string getTitle(string content)
    {
        string title = string.Empty;

        using (StringReader stringReader = new StringReader(content))
        {
            while (true)
            {
                string? line = stringReader.ReadLine();

                if (line == null)
                    break;

                if (string.IsNullOrEmpty(line))
                    continue;

                title = line.Trim().TrimStart('#').Trim();
                break;
            }
        }

        return title;
    }

    public void AddTags(string id, string tagList)
    {
        var list = tagList.Split(",", StringSplitOptions.RemoveEmptyEntries);

        var filter = Builders<Note>.Filter.Eq(r => r.Id, id);

        FieldDefinition<Note> tagsField = "tags";

        var update = Builders<Note>.Update.AddToSetEach(tagsField, list);

        noteCollection.FindOneAndUpdate(filter, update);
    }

    public List<Note> GetNotes()
    {
        return noteCollection.Find(Builders<Note>.Filter.Empty).ToList();
    }

    public Note GetNotes(string id)
    {
        var filter = Builders<Note>.Filter.Eq(r => r.Id, id);

        var result = noteCollection.Find(filter).FirstOrDefault();
        
        return result;
    }

    public void RemoveTag(string id, string tag)
    {

        var filter = Builders<Note>.Filter.Eq(r => r.Id, id);

        FieldDefinition<Note> tagsField = "tags";

        var update = Builders<Note>.Update.Pull(tagsField, tag);

        noteCollection.FindOneAndUpdate(filter, update);
    }

    public void UpdateNote(string id, string content)
    {
        string title = getTitle(content);

        var filter = Builders<Note>.Filter.Eq(r => r.Id, id);
        var update = Builders<Note>.Update
            .Set(r => r.Title, title)
            .Set(r => r.Content, content);

        noteCollection.FindOneAndUpdate(filter, update);
    }
}
