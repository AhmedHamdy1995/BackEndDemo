using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPBack.Models;
using TPBack.Models.DTO;
using TPBack.Repository;
using TPBack.Repository.IRepository;


namespace TPBack.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class BookController : ControllerBase
    {

        
        private readonly IBookRepository bookRepo;
        private readonly IMapper mapper;

        public BookController(IBookRepository bookRepo, IMapper mapper)
        {
            this.bookRepo = bookRepo;
            this.mapper = mapper;
        }



        // to get all books
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BookDTO>))]
        [ProducesDefaultResponseType]
        public IActionResult getAllBooks()
        {
            var bookList= bookRepo.GetBooks();
            var bookDTOList = new List<BookDTO>();
            foreach (var item in bookList)
            {
                bookDTOList.Add(mapper.Map<BookDTO>(item));
            }

            return Ok(bookDTOList);
        }




        // to get   a spesific book
        [HttpGet("{bookId:int}" , Name ="getBook")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDTO))]
        [ProducesDefaultResponseType]
        public IActionResult getBook(int bookId)
        {
            var book = bookRepo.GetBook(bookId);
            var bookDTO = mapper.Map<BookDTO>(book);
            return Ok(bookDTO);
        }




        // to add book
        [Authorize(Roles = "User")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult CreateBook([FromBody] BookDTO objDTO)
        {
            if (objDTO == null)
            {
                return BadRequest();
            }

            if (bookRepo.CheckBookExists(objDTO.Title))
            {
                ModelState.AddModelError(string.Empty, $"the book is exist");
                return StatusCode(404, ModelState);
            }

            var obj = mapper.Map<Book>(objDTO);
            if (!bookRepo.CreateBook(obj))
            {
                ModelState.AddModelError(string.Empty, $"something went wrong when adding {obj.Title}");
                return StatusCode(500, ModelState);
            }

            // to create and return the created object 
            return CreatedAtRoute("getBook", new { bookId = obj.Id }, obj);

        }




        //to update book
        [Authorize(Roles = "Admin")]
        [HttpPatch("{bookId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult updateBook(int bookId,[FromBody] BookDTO objDTO)
        {
            if(objDTO == null || bookId != objDTO.Id)
            {
                return BadRequest();
            }

            // to check if the new title exist before
            var objName = bookRepo.GetBook(objDTO.Title);

            if (objName != null)
            {
                if (objName.Id != bookId)
                {
                    ModelState.AddModelError(string.Empty, "Book is Exist");
                    return StatusCode(404, ModelState);
                }
            }

            var obj = mapper.Map<Book>(objDTO);
            var objFromDB = bookRepo.GetBook(obj.Id);
            objFromDB.Title = obj.Title;
            objFromDB.Cover = obj.Cover;

            if (!bookRepo.UpdateBook(objFromDB))
            {
                ModelState.AddModelError(string.Empty, $"something went wrong when Updating {objFromDB.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }




        // to delete book
        [Authorize(Roles = "Admin")]
        [HttpDelete("{bookId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult deleteBook(int bookId)
        {
            if (!bookRepo.CheckBookExists(bookId))
            {
                return NotFound();
            }

            var obj = bookRepo.GetBook(bookId);
            if (!bookRepo.DeleteBook(obj))
            {
                ModelState.AddModelError(string.Empty, $"something went wrong when deleting {obj.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
   
    
    }
}
