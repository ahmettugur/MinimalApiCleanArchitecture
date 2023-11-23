using MinimalApiCleanArchitecture.Domain.Entities;

namespace MinimalApiCleanArchitecture.Domain.Model;

public class Blog: BaseEntity
{
    private Blog()
    {
        Contributors = new List<Author>();
    }
    public string Name { get; private set; }= null!;
    public string Description { get; private set; }= null!;
    public DateTime CreatedDate { get; private set; }
    public Guid AuthorId { get; private set; }
    public Author? Owner { get; private set; }
    public ICollection<Author>? Contributors { get; private set; }

    public static Blog CreateBlog(string name, string description, Guid authorId)
    {
        return new Blog
        {
            Name = name,
            Description = description,
            AuthorId = authorId,
            CreatedDate = DateTime.Now
        };
    }

    public void UpdateBlog(string name, string description, Guid authorId)
    {
        Name = name;
        Description = description;
        AuthorId = authorId;
    }

    public void AddContributor(Author author)
    {
        Contributors ??= new List<Author>();
        if (!(Contributors.Any(x => x.Id == author.Id)))
            Contributors.Add(author);
    }

    public void RemoveContributor(Author author)
    {
        Contributors?.Remove(author);
    }

    public void SetOwner(Author author)
    {
        Owner = author;
    }
}