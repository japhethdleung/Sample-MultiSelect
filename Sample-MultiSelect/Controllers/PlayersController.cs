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
    public class PlayersController : Controller
    {
        private appDbContext db = new appDbContext();

        // GET: Players
        public ActionResult Index()
        {
            return View(db.Players.ToList().OrderBy(i => i.Name));
        }

        // GET: Players/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            var teams = db.Teams.ToList();

            /*
             * OPTION 1 : THE LONG WAY
             *
             * Description: Intended to show the process of creating the MultiSelectList.
             * Option 2 is much quicker.
             */

            //Initialize the empty list of SelecListItems
            List<SelectListItem> items = new List<SelectListItem>();

            // Loop over each team in our teams List...
            foreach (var team in teams)
            {
                // ... and instantiate a new SelectListItem for each one...
                var item = new SelectListItem
                {
                    Value = team.TeamId.ToString(),
                    Text = team.Name
                };
                // ... then add to our items List
                items.Add(item);
            };

            // Instantiate our MultiSelect list, adding in our items list and ordering by the Text field (i.e. Team name)
            MultiSelectList teamsList = new MultiSelectList(items.OrderBy(i => i.Text), "Value", "Text");

            /*
             * OPTION 2: THE SHORT WAY
             *
             * Description: Directly instantiates the MultiSelectList without all of the preamble.
             * Uncomment the below line and comment out the above instantiation of teamsList to test Option 2.
             */

            //MultiSelectList teamsList = new MultiSelectList(db.Teams.ToList().OrderBy(i => i.Name), "TeamId", "Name");

            // Instantiate our CreatePlayerViewModel and set the Teams property to our teamslist MultiSelectList...
            CreatePlayerViewModel model = new CreatePlayerViewModel { Teams = teamsList };

            // ...and return it in our View.
            return View(model);
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name, TeamIds")] CreatePlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Instantiate our new player with only the Id and Name properties
                Player player = new Player
                {
                    PlayerId = Guid.NewGuid(),
                    Name = model.Name
                };

                if (model.TeamIds != null)
                {
                    foreach (var id in model.TeamIds)
                    {
                        // Convert the id to a Guid from a string
                        var teamId = Guid.Parse(id);
                        // Retrieve team from database...
                        var team = db.Teams.Find(teamId);
                        // ... and add it to the player's Team collection
                        try
                        {
                            player.Teams.Add(team);
                        }
                        catch (Exception ex)
                        {
                            return View("Error", new HandleErrorInfo(ex, "Players", "Index"));
                        }
                    }
                }
                // Add new Player to db & save changes
                try
                {
                    db.Players.Add(player);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Players", "Index"));
                }

                // If successful, return
                return RedirectToAction("Details", new { id = player.PlayerId });
            }
            else
            {
                ModelState.AddModelError("", "Something failed.");
                return View(model);
            }
        }

        // GET: Players/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Retrieve Player from db and perform null check
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            // Instantiate new instance of EditPlayerViewModel
            EditPlayerViewModel model = new EditPlayerViewModel
            {
                // Can set the player name and Id filds of the ViewModel
                PlayerId = player.PlayerId.ToString(),
                Name = player.Name
            };

            // Retrieve list of player teams from db in order to find the teams that the player belongs to
            var playerTeams = db.Teams.Where(i => i.Players.Any(j => j.PlayerId.Equals(player.PlayerId))).ToList();
            //var playerTeams = db.Teams.Where(t => t.Players.Contains(new Player { PlayerId = player.PlayerId })).ToList();

            // Check that playerTeams is not empty
            if (playerTeams != null)
            {
                // Initialize the array to number of teams in playerTeams
                string[] playerTeamsIds = new string[playerTeams.Count];

                // Then, set the value of platerTeams.Count so the for loop doesn't need to work it out every iteration
                int length = playerTeams.Count;

                // Now loop over each of the playerTeams and store the Id in the playerTeamsId array
                for (int i = 0; i < length; i++)
                {
                    // Note that we employ the ToString() method to convert the Guid to the string
                    playerTeamsIds[i] = playerTeams[i].TeamId.ToString();
                }

                // Instantiate the MultiSelectList, plugging in our playerTeamIds array
                MultiSelectList teamsList = new MultiSelectList(db.Teams.ToList().OrderBy(i => i.Name), "TeamId", "Name", playerTeamsIds);

                // Now add the teamsList to the Teams property of our EditPlayerViewModel (model)
                model.Teams = teamsList;

                // Return the ViewModel
                return View(model);
            }
            else
            {
                // Else instantiate the teamsList without any pre-selected values
                MultiSelectList teamsList = new MultiSelectList(db.Teams.ToList().OrderBy(i => i.Name), "TeamId", "Name");

                // Set the Teams property of the EditPlayerViewModel with the teamsList
                model.Teams = teamsList;

                // Return the ViewModel
                return View(model);
            }
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlayerId, Name, TeamIds")] EditPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve player from db and perform null check
                Player player = db.Players.Find(Guid.Parse(model.PlayerId));
                if (player == null)
                {
                    return HttpNotFound();
                }
                // Edit user name per the viewmodel
                player.Name = model.Name;

                // Check if any teams were selected by the user in the form
                if (model.TeamIds.Count > 0)
                {
                    // First, we will instantiate a list to store each of the teams in the EditPlayerViewModel for later comparison
                    List<Team> viewModelTeams = new List<Team>();

                    // Now, loop over each of the ids in the list of TeamIds
                    foreach (var id in model.TeamIds)
                    {
                        // Retrive the team from the db
                        var team = db.Teams.Find(Guid.Parse(id));

                        if (team != null)
                        {
                            // We will add the team to our tracking list of viewmodelteams and player teams
                            try
                            {
                                player.Teams.Add(team);
                                viewModelTeams.Add(team);
                            }
                            catch (Exception ex)
                            {
                                return View("Error", new HandleErrorInfo(ex, "Players", "Index"));
                            }
                        }
                    }
                    // Now we will create a list of all teams in the db, which we will "Except" from the new player's list
                    var allTeams = db.Teams.ToList();
                    // Now exclude the viewModelTeams from the allTeams list to create a list of teams that we need to delete from the player
                    var teamsToRemove = allTeams.Except(viewModelTeams);
                    // Loop over each of the teams in our teamsToRemove List
                    foreach (var team in teamsToRemove)
                    {
                        try
                        {
                            // Remove that team from the player's Teams list
                            player.Teams.Remove(team);
                        }
                        catch (Exception ex)
                        {
                            // Catch any exceptions and error out
                            return View("Error", new HandleErrorInfo(ex, "Players", "Index"));
                        }
                    }
                }
                // Save the changes to the db
                try
                {
                    db.Entry(player).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View("Error", new HandleErrorInfo(ex, "Players", "Indes"));
                }
                // If successfull redirect to player Details
                return RedirectToAction("Details", new { id = player.PlayerId });
            }
            // Else something failed, return
            return View(model);
        }

        // GET: Players/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
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
