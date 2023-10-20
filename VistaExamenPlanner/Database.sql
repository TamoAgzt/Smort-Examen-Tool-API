﻿CREATE TABLE IF NOT EXISTS `Rol` (`Id` integer PRIMARY KEY AUTO_INCREMENT, Naam varchar(32));
CREATE TABLE IF NOT EXISTS `Klas` ( `Id` integer PRIMARY KEY AUTO_INCREMENT ,`Naam` varchar(64), `Mentor` varchar(64));
CREATE TABLE IF NOT EXISTS `Gebruikers` (`Id` integer PRIMARY KEY AUTO_INCREMENT, `Rol_Id` integer, `Naam` varchar(64), `Achternaam` varchar(64),`Email` varchar(128), `Wachtwoord` varchar(128), FOREIGN KEY (`Rol_Id`) REFERENCES `Rol` (`Id`));
CREATE TABLE IF NOT EXISTS `Student` (`Id` integer PRIMARY KEY AUTO_INCREMENT,`studenten_nummer` integer,`Gebruikers_Id` integer,`klas_Id` integer, FOREIGN KEY (`klas_Id`) REFERENCES `Klas` (`Id`), FOREIGN KEY (`Gebruikers_Id`) REFERENCES `Gebruikers` (`Id`));
CREATE TABLE IF NOT EXISTS`Lokaal` (`Id` integer PRIMARY KEY AUTO_INCREMENT, `Lokaal` varchar(32));
CREATE TABLE IF NOT EXISTS `Toezichthouders` (`Id` integer PRIMARY KEY AUTO_INCREMENT,`Naam` varchar(32), Tussenvoegsel varchar(32), Achternaam varchar(32));
CREATE TABLE IF NOT EXISTS `Examen` (`Id` integer PRIMARY KEY AUTO_INCREMENT, `Naam_Examen` varchar(64),`Vak_Examen` varchar(64), Toezichthouders_Id integer, FOREIGN KEY (`Toezichthouders_Id`) REFERENCES `Toezichthouders` (`Id`));
CREATE TABLE IF NOT EXISTS `AgendaItem` (`Id` integer PRIMARY KEY AUTO_INCREMENT,`Klas_Id` integer,`Examen_Id` integer, Lokaal_Id integer, Tijd_Begin DateTime, Tijd_Einden DateTime, FOREIGN KEY (`Examen_Id`) REFERENCES `Examen` (`Id`),  FOREIGN KEY (`Klas_Id`) REFERENCES `Klas` (`Id`), FOREIGN KEY (`Lokaal_Id`) REFERENCES `Lokaal` (`Id`));
INSERT IGNORE INTO `Rol` (Id, Naam) VALUES (1, "Student");
INSERT IGNORE INTO `Rol` (Id, Naam) VALUES (2, "Docent");
INSERT IGNORE INTO `Rol` (Id, Naam) VALUES (3, "Examen Coordinator");
