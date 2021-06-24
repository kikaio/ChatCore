
USE `test_db`;

CREATE TABLE `Account` (
    `Id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `NickName` VARCHAR(50) NOT NULL UNIQUE, 
    `Pw` varchar(100) NOT NULL,
    `RegistDate` DateTime DEFAULT(UTC_TIMESTAMP()),
    `UpdatedDate` DateTime DEFAULT(UTC_TIMESTAMP()),
    `IsBlocked` BIT NOT NULL DEFAULT(0),
    `IsSignOut` BIT NOT NULL DEFAULT(0)
);


