using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using TaskKiller.Models;

namespace TaskKiller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        // GET: api/Settings
        [HttpGet]
        public Settings Get()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            connectionStringBuilder.DataSource = "./SqliteDB.db";

            string blob = "";

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT settings_blob FROM settings_blobs";

                try
                {
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            blob = reader.GetString(0);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return new Settings { SettingsBlob = blob};
        }

        // POST: api/Settings
        [HttpPost]
        public void Post([FromBody] Settings settingsBlob)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            connectionStringBuilder.DataSource = "./SqliteDB.db";

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                //Create a table (drop if already exists first):

                var createTbCmd = connection.CreateCommand();
                createTbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS settings_blobs (
                                                settings_blob TEXT NOT NULL
                                            )";
                createTbCmd.ExecuteNonQuery();


                using (var transaction = connection.BeginTransaction())
                {
                    var deleteCommand = connection.CreateCommand();

                    deleteCommand.CommandText = "delete from settings_blobs";
                    deleteCommand.ExecuteNonQuery();

                    transaction.Commit();
                }

                //Seed some data:
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO settings_blobs VALUES('" + settingsBlob.SettingsBlob + "')";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }


            }

        }

        // PUT: api/Settings/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
