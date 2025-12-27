using FinalMap.Models;
using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FinalMap.Controllers
{
    public class LibraryController : ApiController
    {
        lmsEntities1 db = new lmsEntities1();
        [HttpPost]
        public HttpResponseMessage Signup(Student student)
        {
            try
            {
                if (student == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid student data");
                }

                db.Students.Add(student);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Signup successful");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage Login(string aridno, string password)
        {
            if (aridno == null || password == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Please enter in both fields");
            }
            if (aridno == "11" && password == "11")
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Admin Login Successfully");
            }

            var res = db.Students.FirstOrDefault(s => s.aridno == aridno && s.password == password);
            if (res == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "email or password incorrect");

            }
            var obj = new
            {
                name = res.name,
                aridno = res.aridno
            };
            return Request.CreateResponse(HttpStatusCode.OK, obj);

        }
        [HttpGet]
        public HttpResponseMessage Showallstudent()
        {
            var res = db.Students.ToList();
            if (res == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No students Found");
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);


        }
        [HttpPost]
        public HttpResponseMessage AddBook(Book book)
        {
            if (book == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid book data");
            }
            db.Books.Add(book);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Book added successfully");
        }
        [HttpPost]
        public HttpResponseMessage ShowAllbook()
        {
            var res = db.Books.ToList();
            if (res == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No Books Found");
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        [HttpGet]
        public HttpResponseMessage bookdetail(int isbn)
        {
            var res = db.Books.FirstOrDefault(b => b.isbn == isbn);
            if (res == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Book not found");
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        [HttpPost]
        public HttpResponseMessage addstudent(Student student)
        {

            if (student == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid student data");
            }
            db.Students.Add(student);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Student added successfully");

        }
        [HttpPost]
        public HttpResponseMessage borrowbook(string aridno, string isbn, DateTime returnd)
        {
            var student = db.Students.FirstOrDefault(s => s.aridno == aridno);
            var book = db.Books.FirstOrDefault(b => b.isbn.ToString() == isbn);
            if (student == null || book == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid student or book data");
            }
            Borrowbook borrowbook = new Borrowbook
            {
                aridno = aridno,
                isbn = int.Parse(isbn),
                issuedate = DateTime.Now,
                returndate = returnd.Date,
                status = "borrowed"
            };
            db.Borrowbooks.Add(borrowbook);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Book borrowed successfully");
        }




    }
}

