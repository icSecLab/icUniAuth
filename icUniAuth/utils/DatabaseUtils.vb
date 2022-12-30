Imports MySql.Data.MySqlClient
Module DatabaseUtils

    Public WithEvents DatabaseConnectionWatchingDog As New System.Timers.Timer With {
        .Interval = 10000,
        .Enabled = False
    }

    Public MySQLDatabaseConnection As MySqlConnection = Nothing

    Public Sub CheckDatabaseConnection() Handles DatabaseConnectionWatchingDog.Elapsed
        If MySQLDatabaseConnection Is Nothing Then
            MySQLDatabaseConnection = New MySqlConnection("server=" & DatabaseServerHost & ";port=" & DatabaseServerPort & ";database=" & DatabaseName & ";uid=" & DatabaseConnectionUsername & ";pwd=" & DatabaseConnectionPassword & ";")
        End If
        If MySQLDatabaseConnection.State = ConnectionState.Closed Or MySQLDatabaseConnection.State = ConnectionState.Broken Then
            MySQLDatabaseConnection.Open()
            InitDatabase()
        End If
    End Sub


    Public Sub InitDatabase()
        If MySQLDatabaseConnection IsNot Nothing AndAlso MySQLDatabaseConnection.State = ConnectionState.Open Then
            Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
            cmd.CommandText = "
CREATE TABLE IF NOT EXISTS  `user` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT ,
  `email` VARCHAR(128) NOT NULL UNIQUE,
  `nick_name` TEXT NOT NULL,
  `pwd_salt` TEXT NOT NULL,
  `pwd_hash` TEXT NOT NULL,
  `status` INT NOT NULL,
  `failed_times` INT UNSIGNED NOT NULL,
  `open_id` VARCHAR(32) NOT NULL UNIQUE,
  `avatar_url` TEXT,
  PRIMARY KEY (`id`));

CREATE TABLE IF NOT EXISTS  `reg_pending` (
  `email` VARCHAR(128) NOT NULL UNIQUE,
  `pwd_salt` TEXT NOT NULL,
  `pwd_hash` TEXT NOT NULL,
  `key` VARCHAR(128) NOT NULL  ,
  PRIMARY KEY (`key`)
  );

CREATE TABLE IF NOT EXISTS  `reset_pending` (
  `user_id` INT UNSIGNED NOT NULL UNIQUE,
  `key` VARCHAR(128) NOT NULL ,
  PRIMARY KEY (`key`),
  FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
  );

CREATE TABLE IF NOT EXISTS  `session` (
  `session_id` VARCHAR(128) NOT NULL,
  `user_id` INT UNSIGNED NOT NULL,
  `ip_addr` TEXT NOT NULL,
  PRIMARY KEY (`session_id`),
  FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
  );

CREATE TABLE IF NOT EXISTS `app` (
  `id` VARCHAR(128) NOT NULL,
  `owner` INT UNSIGNED NOT NULL,
  `redirect_url` TEXT NOT NULL,
  `client_secret` TEXT NOT NULL,
  `name` TEXT NOT NULL,
  PRIMARY KEY (`id`),
  FOREIGN KEY (`owner`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
  );

CREATE TABLE IF NOT EXISTS `auth_log` (
  `user` INT UNSIGNED NOT NULL,
  `app_name` TEXT  NOT NULL,
  `time` DATETIME NOT NULL,
  FOREIGN KEY (`user`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
  );

CREATE TABLE IF NOT EXISTS `auth_info` (
  `user` INT UNSIGNED NOT NULL,
  `app_id` VARCHAR(128) NOT NULL,
  `auto` TINYINT NOT NULL,
  PRIMARY KEY (`user`,`app_id`),
  FOREIGN KEY (`user`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`app_id`) REFERENCES `app` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
  );

CREATE TABLE  IF NOT EXISTS `auth_code` (
  `token` VARCHAR(128) NOT NULL,
  `app_id` VARCHAR(128) NOT NULL,  
  `scope` VARCHAR(128),  
  `user_id` INT UNSIGNED NOT NULL,
  `time` DATETIME NOT NULL,
  PRIMARY KEY (`token`),
  FOREIGN KEY (`app_id`) REFERENCES `app` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
  );

CREATE TABLE   IF NOT EXISTS `access_token` (
  `token` VARCHAR(128) NOT NULL,
  `app_id` VARCHAR(128) NOT NULL,
  `time` DATETIME NOT NULL,
  `scope` VARCHAR(128) NOT NULL,
  `refresh_token` VARCHAR(128) NOT NULL UNIQUE,
  `refresh_time` DATETIME NOT NULL,
  `user_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`token`),
  FOREIGN KEY (`app_id`) REFERENCES `app` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
  );
"
            cmd.ExecuteNonQuery()
        Else
            Throw New Exception("Database connection was broken.")
        End If
    End Sub

End Module
