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
        [Route("api/library/signup")]
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
        [Route("api/library/Login")]
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
        [Route("api/library/Showall")]
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
        [Route("api/library/addbook")]
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
        [Route("api/library/showallbook")]
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
        [Route("api/library/bookdetails")]
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
        [Route("api/library/addstudent")]
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
        [Route("api/library/borbook")]
        public HttpResponseMessage borrowbook([FromBody] BorrowBookRequest request)
        {
            var student = db.Students.FirstOrDefault(s => s.aridno == request.aridno);
            var book = db.Books.FirstOrDefault(b => b.isbn == request.isbn);
            if (student == null || book == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid student or book data");
            }
            Borrowbook borrowbook = new Borrowbook
            {
                aridno = request.aridno,
                isbn = request.isbn,
                issuedate = DateTime.Now.Date,
                returndate = request.returnd.Date,
                status = "issued"
            };
            book.totalcopies--;
            db.Borrowbooks.Add(borrowbook);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Book borrowed successfully");
        }
        [HttpPost]
        [Route("api/library/returnbook")]
        public HttpResponseMessage ReturnBook(int borrowid, DateTime actualReturnDate)
        {
            try
            {
                var borrow = db.Borrowbooks
                               .FirstOrDefault(b => b.borrowid == borrowid && b.status == "issued");

                if (borrow == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        "Invalid borrow record or book already returned");
                }

                // set actual return date (from frontend)
                borrow.actualreturndate = actualReturnDate.Date;
                borrow.status = "returned";

                // fine calculation
                if (borrow.actualreturndate > borrow.returndate)
                {
                    int lateDays =
                        (borrow.actualreturndate - borrow.returndate).Days;
                    borrow.fineamount = lateDays * 50;   // example fine per day
                }
                else
                {
                    borrow.fineamount = 0;
                }

                // update book copies
                var book = db.Books.FirstOrDefault(b => b.isbn == borrow.isbn);
                if (book != null)
                {
                    book.totalcopies++;
                }

                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK,
                    "Book returned successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    ex.Message);
            }
        }
        [HttpGet]
        [Route("api/library/getissue")]
        public HttpResponseMessage GetIssuedBooks()
        {
            try
            {
                // Get only books with status "issued"
                var issuedBooks = db.Borrowbooks
                    .Where(b => b.status.ToLower() == "issued")  // Filter by status
                    .Select(b => new  // Select specific fields
                    {
                        BorrowId = b.borrowid,
                        ISBN = b.isbn,
                        aridnoo = b.aridno,
                        IssueDate = b.issuedate,
                        ReturnDate = b.returndate,
                        Status = b.status,
                        // Add other fields as needed
                    })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, issuedBooks);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { error = ex.Message });
            }
        }





    }
}

