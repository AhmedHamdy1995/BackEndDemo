using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPBack.Models;

namespace TPBack.Repository.IRepository
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetBooks();
        Book GetBook(int Id);
        Book GetBook(string Name);
        bool CheckBookExists(int Id);
        bool CheckBookExists(string Name);
        bool CreateBook(Book Book);
        bool UpdateBook(Book Book);
        bool DeleteBook(Book Book);
        bool Save();
    }
}
