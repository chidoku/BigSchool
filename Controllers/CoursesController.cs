using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        BigSchoolContext context = new BigSchoolContext();
        // GET: Courses

        public ActionResult Index()
        {
            
            var upcommingCourse = context.Courses.Where(p => p.Datetime > DateTime.Now).OrderBy(p => p.Datetime).ToList();
            foreach (Course i in upcommingCourse)
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().
                    GetUserManager<ApplicationUserManager>().FindById(i.LecturerId);
                i.Name = user.Name;
            }
            return View(upcommingCourse);
        }
        public ActionResult Create()
        {
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();

            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Course objCourse)
        {

            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Create", objCourse);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().
                GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;

            context.Courses.Add(objCourse);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                    .FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = context.Courses.Where(c => c.LecturerId == currentUser.Id && c.Datetime > DateTime.Now).ToList();
            foreach(Course i in courses)
            {
                i.LectureName = currentUser.Name;
            }
            return View(courses);
        }

        public ActionResult Edit(int id)
        {
            Course course = context.Courses.SingleOrDefault(x => x.Id == id);
            course.ListCategory = context.Categories.ToList();
            return View(course);           
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Course course)
        {
            try
            {
                context.Entry(course).State = EntityState.Modified;
                context.SaveChanges();               
                return RedirectToAction("ListCategory");
            }
            catch
            {
                return View();
            }

        }
        public ActionResult Delete(int id)
        {
            Course course = context.Courses.SingleOrDefault(x => x.Id == id);
            course.ListCategory = context.Categories.ToList();
            return View(course);

        }
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id, Course course)
        {

                Course dbDelete = context.Courses.SingleOrDefault(p => p.Id == id);
                if (dbDelete != null)
                {
                    context.Courses.Remove(course);
                    context.SaveChanges();
                }
                return RedirectToAction("ListCategory");
            

        }
    }
}