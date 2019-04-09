using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sample_MultiSelect.Data;
using Sample_MultiSelect.Data.DbContexts;
using Sample_MultiSelect.Models;

namespace Sample_MultiSelect.Controllers
{
    public class TeamsController : Controller
    {
        private appDbContext db = new appDbContext();

        // GET: Teams
        public ActionResult Index()
        {
            return View(db.Teams.ToList().OrderBy(i => i.Name));
        }

        // GET: Teams/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        public ActionResult Create()
        {
            return View(new CreateTeamViewModel());
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] CreateTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var team = new Team
                {
                    TeamId = Guid.NewGuid(),
                    Name = model.Name
                };

                try
                {
                    db.Teams.Add(team);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Teams", "Index"));
                }

                return RedirectToAction("Index");
            }
            // Something failed, return
            return View(model);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(new EditTeamViewModel { TeamId = team.TeamId, Name = team.Name });
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TeamId,Name")] EditTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                Team team = db.Teams.Find(model.TeamId);
                if (team != null)
                {
                    team.Name = model.Name;
                    try
                    {
                        db.Entry(team).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return View("Error", new HandleErrorInfo(ex, "Teams", "Index"));
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    Exception ex = new Exception("Unable to Retrieve Team");
                    return View("Error", new HandleErrorInfo(ex, "Teams", "Index"));
                }
            }
            // Something failed, return model to view
            return View(model);
        }

        // GET: Teams/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
