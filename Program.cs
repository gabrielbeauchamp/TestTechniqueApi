using Microsoft.EntityFrameworkCore;
using TestTechniqueApi.DbContext;
using TestTechniqueApi.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Context>(opt => opt.UseInMemoryDatabase("BookList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();


// Return the list of all the books
app.MapGet("/books", async (Context db) =>
    await db.Books.ToListAsync());

// Return only the books that are available
app.MapGet("/books/available", async (Context db) =>
    await db.Books.Where(t => t.IsAvailable).ToListAsync());

// Return a specific book using it's id
app.MapGet("/book/{id}", async (int id, Context db) =>
    await db.Books.FindAsync(id)
        is Book book
            ? Results.Ok(book)
            : Results.NotFound());

// Take a book from the list and change its availability to taken
app.MapPut("/takebook/{id}", async (int id, Context db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null) return Results.NotFound();

    book.IsAvailable = false;
    await db.SaveChangesAsync();

    return Results.Created($"/books/{book.Id}", book);
});

// Take a book from the list and change its availability to available
app.MapPut("/returnbook/{id}", async (int id, Context db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null) return Results.NotFound();

    book.IsAvailable = true;

    await db.SaveChangesAsync();

    return Results.Created($"/books/{book.Id}", book);
});

// Add a new book to the libary
app.MapPost("/addbook", async (Book book, Context db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();

    return Results.Created($"/books/{book.Id}", book);
});

// Replace the name or the availability to a book
app.MapPut("/updatebook/{id}", async (int id, Book inputbook, Context db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null) return Results.NotFound();

    book.Name = inputbook.Name;
    book.IsAvailable = inputbook.IsAvailable;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Delete an existing book
app.MapDelete("/removebook/{id}", async (int id, Context db) =>
{
    if (await db.Books.FindAsync(id) is Book book)
    {
        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return Results.Ok(book);
    }

    return Results.NotFound();
});

app.Run();