using DiscordApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DiscordApp
{
    public class DataAccess
    {
        #region Class Variables
        string _ConnectionString = @"Data Source=LAPTOP-285\SQLEXPRESS;Initial Catalog=BootCamp;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        string _ErrorLog = @"C:\Users\alexander.leon\Documents\Visual Studio 2017\Projects\BootCampDiscordApp\DiscordApp\LogFile";
        #endregion

        #region Time Handling
        public void Login(string User, DateTime Time)
        {
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("LogTime", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@Login_Time", Time);
                lCommand.Parameters.AddWithValue("@BootCamper", User);
                try
                {
                    lConnection.Open();
                    lCommand.ExecuteNonQuery();
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
        }

        public void GoToLunch(string User, DateTime Time)
        {
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("GoToLunch", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@ToLunch", Time);
                lCommand.Parameters.AddWithValue("@BootCamper", User);
                try
                {
                    lConnection.Open();
                    lCommand.ExecuteNonQuery();
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
        }

        public void BackFromLunch(string User, DateTime Time)
        {
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("BackFromLunch", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@FromLunch", Time);
                lCommand.Parameters.AddWithValue("@BootCamper", User);
                try
                {
                    lConnection.Open();
                    lCommand.ExecuteNonQuery();
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
        }
        
        public void UpdateLogin(string user, DateTime Time)
        {
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("UpdateLoginTime", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@LoginTime", Time);
                lCommand.Parameters.AddWithValue("@BootCamper", user);
                try
                {
                    lConnection.Open();
                    lCommand.ExecuteNonQuery();
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
        }

        public void Logout(string user, DateTime Time)
        {
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("Logout_Time", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@Logout_Time", Time);
                lCommand.Parameters.AddWithValue("@BootCamper", user);
                try
                {
                    lConnection.Open();
                    lCommand.ExecuteNonQuery();
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
        }

        public List<BootCamper> ViewAllLoggedIn()
        {
            List<BootCamper> Members = new List<BootCamper>();

            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("ViewAllLoggedIn", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                try
                {
                    lConnection.Open();
                    SqlDataReader Reader = lCommand.ExecuteReader();

                    while (Reader.Read())
                    {
                        Members.Add(new BootCamper
                        {
                            BootcamperId = Reader.GetInt32(0),
                            DiscordUserName = Reader.GetString(1),
                            FirstName = Reader.GetString(2),
                            LastName = Reader.GetString(3),
                            TimeLogs = new List<TimeSheet>() {
                                new TimeSheet{
                                    TimeSheetId = Reader.GetInt32(4),
                                    LoginTime= Reader.GetDateTime(5),
                                    LogoutTime= DateTime.MinValue,
                                }
                            }
                        });
                    }
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                catch (Exception Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
            return Members;
        }

        private bool IsDayOfWeek(DayOfWeek iDayOfWeek)
        {
            bool dow = false;
            if (iDayOfWeek != DayOfWeek.Saturday || iDayOfWeek != DayOfWeek.Sunday)
            {
                dow = true;
            }
            else
            {

            }
            return dow;
        }

        public List<TimeSheet> ViewBootCamperTimeSheet(string iUsername, int iCount = 5)
        {
            int weekDaysPassed = 0;
            int totalDaysPassed = 0;
            while (weekDaysPassed < iCount)
            {
                if (IsDayOfWeek(DateTime.Now.AddDays(totalDaysPassed * -1).DayOfWeek))
                {
                    weekDaysPassed++;
                }
                totalDaysPassed++;
            }
            totalDaysPassed = totalDaysPassed * -1;//Days ago.

            List<TimeSheet> TimeSheets = new List<TimeSheet>();

            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("ViewBootCamperTimeSheet", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@Username", iUsername);
                lCommand.Parameters.AddWithValue("@Count", totalDaysPassed);
                try
                {
                    lConnection.Open();
                    SqlDataReader Reader = lCommand.ExecuteReader();

                    while (Reader.Read())
                    {
                        TimeSheets.Add(new TimeSheet
                        {
                            TimeSheetId = Reader.GetInt32(0),
                            LoginTime = Reader.GetDateTime(1),
                            LunchInTime = Reader.IsDBNull(2) ? DateTime.MinValue : Reader.GetDateTime(2),
                            LunchOutTime = Reader.IsDBNull(3) ? DateTime.MinValue : Reader.GetDateTime(3),
                            LogoutTime = Reader.IsDBNull(4) ? DateTime.MinValue : Reader.GetDateTime(4)
                        });
                    }
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                catch (Exception Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
            return TimeSheets;
        }

        #endregion

        #region Admin Actions

        public void AddBootCampers(string UserName, string FirstName, string LastName)
        {
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("AddBootCamper", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@BootCamper", UserName);
                lCommand.Parameters.AddWithValue("@FirstName", FirstName);
                lCommand.Parameters.AddWithValue("@LastName", LastName);
                try
                {
                    lConnection.Open();
                    lCommand.ExecuteNonQuery();
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
        }

        public BootCamper ViewBootCamperByUsername(string iUsername)
        {
            BootCamper oBootcamper = new BootCamper();
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("ViewBootCamperByUsername", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@Username", iUsername);
                try
                {
                    lConnection.Open();
                    SqlDataReader Reader = lCommand.ExecuteReader();

                    while (Reader.Read())
                    {
                        oBootcamper = new BootCamper
                        {
                            BootcamperId = Reader.GetInt32(0),
                            DiscordUserName = Reader.GetString(1),
                            FirstName = Reader.GetString(2),
                            LastName = Reader.GetString(3)
                        };
                    }
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                catch (Exception Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
                return oBootcamper;
            }
        }

        #endregion

        #region Tickets
        public bool CreateTicket(Ticket ticket)
        {
            bool success = false;
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("CREATE_TICKET", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;

                lCommand.Parameters.AddWithValue("@Name", ticket.Name);
                lCommand.Parameters.AddWithValue("@Content", ticket.Content);
                lCommand.Parameters.AddWithValue("@Status", ticket.Status);
                lCommand.Parameters.AddWithValue("@SubmittedBy", ticket.SubmittedBy);
                lCommand.Parameters.AddWithValue("@Date", DateTime.Now);

                try
                {
                    lConnection.Open();

                    //Success is based off of rows affected.
                    success = lCommand.ExecuteNonQuery() > 0;
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
            return success;
        }

        public bool UpdateTicket(Ticket ticket)
        {
            bool success = false;
            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("UPDATE_TICKET", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;
                lCommand.Parameters.AddWithValue("@TicketId", ticket.TicketId);
                lCommand.Parameters.AddWithValue("@Name", ticket.Name);
                lCommand.Parameters.AddWithValue("@Content", ticket.Content);
                lCommand.Parameters.AddWithValue("@Date", ticket.Date);
                lCommand.Parameters.AddWithValue("@Status", ticket.Status);
                try
                {
                    lConnection.Open();
                    success = lCommand.ExecuteNonQuery() > 0;
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
            return success;
        }

        public List<Ticket> ViewTickets(string iStatus)
        {
            List<Ticket> oTickets = new List<Ticket>();

            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("VIEW_ALL_TICKETS", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;

                lCommand.Parameters.AddWithValue("@Status", iStatus);
                try
                {
                    lConnection.Open();
                    SqlDataReader Reader = lCommand.ExecuteReader();

                    while (Reader.Read())
                    {
                        oTickets.Add(new Ticket()
                        {
                            TicketId = Reader.GetInt64(0),
                            Date = Reader.GetDateTime(1),
                            Name = Reader.GetString(2),
                            Content = Reader.GetString(3),
                            SubmittedBy = Reader.GetString(4),
                            Status= Reader.GetString(5),
                        });
                    }
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                catch (Exception Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
            return oTickets;
        }

        public Ticket ViewTicketById(Int64 iTicketId)
        {
            Ticket oTicket = new Ticket();

            using (SqlConnection lConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand lCommand = new SqlCommand("VIEW_TICKET_BY_ID", lConnection);
                lCommand.CommandType = CommandType.StoredProcedure;

                lCommand.Parameters.AddWithValue("@TicketId", iTicketId);
                try
                {
                    lConnection.Open();
                    SqlDataReader Reader = lCommand.ExecuteReader();

                    while (Reader.Read())
                    {
                        oTicket = new Ticket()
                        {
                            TicketId = Reader.GetInt64(0),
                            Date = Reader.GetDateTime(1),
                            Name = Reader.GetString(2),
                            Content = Reader.GetString(3),
                            SubmittedBy = Reader.GetString(4),
                            Status = Reader.GetString(5),
                        };
                    }
                }
                catch (SqlException Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                catch (Exception Exception)
                {
                    StreamWriter lWriter = new StreamWriter(_ErrorLog, true);
                    lWriter.Write("{0} || {1} {2}", DateTime.Now, Exception.Message, Environment.NewLine);
                    lWriter.Close();
                }
                finally
                {
                    lConnection.Close();
                }
            }
            return oTicket;
        }

        #endregion
    }
}
