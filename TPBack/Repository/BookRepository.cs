using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPBack.Data;
using TPBack.Models;
using TPBack.Repository.IRepository;

namespace TPBack.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext db;

        public BookRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool CheckBookExists(int Id)
        {
            return db.books.Any(bo => bo.Id == Id);
        }

        public bool CheckBookExists(string Title)
        {
            return db.books.Any(bo => bo.Title.ToLower().Equals(Title.ToLower()));
        }

        public bool CreateBook(Book Book)
        {
            db.books.Add(Book);
            return Save();
        }

        public bool DeleteBook(Book Book)
        {
             db.books.Remove(Book);
             return Save();
        }

        public Book GetBook(int Id)
        {
            return db.books.Find(Id);
        }

        public Book GetBook(string Title)
        {
            return db.books.FirstOrDefault(b => b.Title.ToLower().Equals(Title.ToLower()));
        }

        public IEnumerable<Book> GetBooks()
        {
            return db.books.ToList();
        }

        public bool Save()
        {
            return db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateBook(Book Book)
        {
            db.books.Update(Book);
            return Save();
        }
    }
}
