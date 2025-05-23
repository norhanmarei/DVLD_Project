﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_D
{
    public class clsDetainedLicensesData
    {
        public static bool IsLicenseDetained(int LicenseID)
        {
            bool isDetained = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT Found=1 FROM DetainedLicenses WHERE
                             LicenseID=@LicenseID AND IsReleased = 0";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            try
            {
                connection.Open();
                object res = command.ExecuteScalar();
                if (res != null && int.TryParse(res.ToString(), out int r))
                    isDetained = r > 0;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error : {e.Message}");
            }
            finally
            {
                connection.Close();
            }
            return isDetained;
        }
        public static DataTable GetAllDetainedLicenses()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM detainedLicenses_View ORDER BY IsReleased ,DetainID;";
            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    dt.Load(reader);
                reader.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
        public static bool GetDetainedLicenseInfo(int DetainID, ref int LicenseID, ref DateTime DetainDate, ref float FineFees, ref int CreatedByUserID, ref bool IsReleased,
                                        ref DateTime ReleaseDate, ref int ReleasedByUserID, ref int ReleaseApplicationID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM DetainedLicenses WHERE DetainID=@DetainID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DetainID", DetainID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    LicenseID = (int)reader["LicenseID"];
                    DetainDate = (DateTime)reader["DetainDate"];
                    FineFees = Convert.ToSingle(reader["FineFees"]);
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsReleased = (bool)reader["IsReleased"];
                    ReleaseDate = (DateTime)reader["ReleaseDate"];
                    ReleasedByUserID = (int)reader["ReleasedByUserID"];
                    ReleaseApplicationID = (int)reader["ReleaseApplicationID"];
                }
                reader.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }
        public static bool GetDetainedLicenseInfoByLicenseID(int LicenseID, ref int DetainID, ref DateTime DetainDate, ref float FineFees, ref int CreatedByUserID, ref bool IsReleased,
                                        ref DateTime? ReleaseDate, ref int? ReleasedByUserID, ref int? ReleaseApplicationID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM DetainedLicenses WHERE LicenseID=@LicenseID ORDER BY DetainID DESC";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    DetainID = (int)reader["DetainID"];
                    DetainDate = (DateTime)reader["DetainDate"];
                    FineFees = Convert.ToSingle(reader["FineFees"]);
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsReleased = (bool)reader["IsReleased"];
                    ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ReleaseDate"];
                    ReleasedByUserID = reader["ReleasedByUserID"] == DBNull.Value ? (int?)null : (int)reader["ReleasedByUserID"];
                    ReleaseApplicationID = reader["ReleaseApplicationID"] == DBNull.Value ? (int?)null : (int)reader["ReleaseApplicationID"];
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }
        public static int DetainLicense(int LicenseID, DateTime DetainDate, float FineFees, int CreatedByUserID, bool IsReleased, 
                                        DateTime? ReleaseDate, int? ReleasedByUserID, int? ReleaseApplicationID)
        {
            int detainedLicenseID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO DetainedLicenses 
                             (LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased, ReleaseDate, ReleasedByUserID, ReleaseApplicationID)
                             VALUES
                             (@LicenseID, @DetainDate, @FineFees, @CreatedByUserID, @IsReleased, @ReleaseDate, @ReleasedByUserID, @ReleaseApplicationID);
                             SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            command.Parameters.AddWithValue("@DetainDate", DetainDate);
            command.Parameters.AddWithValue("@FineFees", FineFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            command.Parameters.AddWithValue("@IsReleased", IsReleased);
            if(ReleaseDate == null)
                command.Parameters.AddWithValue("@ReleaseDate", DBNull.Value);
            else
                command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
            if(ReleasedByUserID != -1)
                command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
            else
                command.Parameters.AddWithValue("@ReleasedByUserID", DBNull.Value);
            if (ReleaseApplicationID != -1)
                command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
            else
                command.Parameters.AddWithValue("@ReleaseApplicationID", DBNull.Value);
                try
                {
                    connection.Open();
                    object res = command.ExecuteScalar();
                    if (res != null && int.TryParse(res.ToString(), out int id))
                        detainedLicenseID = id;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                }
                finally
                {
                    connection.Close();
                }
            return detainedLicenseID;
        }
        public static bool UpdateDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate, float FineFees, int CreatedByUserID, bool IsReleased,
                                        DateTime? ReleaseDate, int? ReleasedByUserID, int? ReleaseApplicationID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE DetainedLicenses 
                             SET 
                             LicenseID=@LicenseID, DetainDate=@DetainDate, FineFees=@FineFees, CreatedByUserID=@CreatedByUserID, IsReleased=@IsReleased, ReleaseDate=@ReleaseDate, ReleasedByUserID=@ReleasedByUserID, ReleaseApplicationID=@ReleaseApplicationID
                             WHERE DetainID=@DetainID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DetainID", DetainID);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            command.Parameters.AddWithValue("@DetainDate", DetainDate);
            command.Parameters.AddWithValue("@FineFees", FineFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            command.Parameters.AddWithValue("@IsReleased", IsReleased);
            if (ReleaseDate == null)
                command.Parameters.AddWithValue("@ReleaseDate", DBNull.Value);
            else
                command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
            if (ReleasedByUserID != -1)
                command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
            else
                command.Parameters.AddWithValue("@ReleasedByUserID", DBNull.Value);
            if (ReleaseApplicationID != -1)
                command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
            else
                command.Parameters.AddWithValue("@ReleaseApplicationID", DBNull.Value);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            finally
            {
                connection.Close();
            }
            return rowsAffected > 0;
        }
        public static bool DeleteDetainedLicense(int DetainID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"DELETE DetainedLicenses WHERE DetainID=@DetainID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DetainID", DetainID);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            finally
            {
                connection.Close();
            }
            return rowsAffected > 0;
        }
    }
}
