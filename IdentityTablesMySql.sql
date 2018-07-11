CREATE TABLE `identitylogin` (
  `LoginProvider` VARCHAR(128) NOT NULL,
  `ProviderKey` VARCHAR(128) NOT NULL,
  `UserId` INT NOT NULL,
  `Name` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`ProviderKey`, `UserId`, `LoginProvider`));

CREATE TABLE `identityrole` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`Id`));


CREATE TABLE `identityuser` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Username` VARCHAR(256) NOT NULL,
  `Email` VARCHAR(256) NULL,
  `EmailConfirmed` BIT(1) NOT NULL,
  `PasswordHash` LONGTEXT NULL,
  `SecurityStamp` VARCHAR(38) NULL,
  `PhoneNumber` VARCHAR(50) NULL,
  `PhoneNumberConfirmed` BIT(1) NOT NULL,
  `TwoFactorEnabled` BIT(1) NOT NULL,
  `LockoutEnd` DATETIME NULL,
  `LockoutEnabled` BIT(1) NOT NULL,
  `AccessFailedCount` INT NOT NULL,
  PRIMARY KEY (`Id`));


CREATE TABLE `identityuserclaim` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `ClaimType` VARCHAR(256) NOT NULL,
  `ClaimValue` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`Id`));

CREATE TABLE `identityuserrole` (
  `UserId` INT NOT NULL,
  `RoleId` INT NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`),
  INDEX `FK_IdentityUserRole_IdentityRole_idx` (`RoleId` ASC),
  CONSTRAINT `FK_IdentityUserRole_IdentityRole`
    FOREIGN KEY (`RoleId`)
    REFERENCES `identityrole` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_IdentityUserRole_IdentityUser`
    FOREIGN KEY (`UserId`)
    REFERENCES `identityuser` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

CREATE TABLE `identityroleclaim` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `RoleId` INT NOT NULL,
  `ClaimType` VARCHAR(256) NOT NULL,
  `ClaimValue` VARCHAR(256) NULL,
  PRIMARY KEY (`Id`));
	
ALTER TABLE `identitylogin` 
ADD INDEX `FK_IdentityLogin_IdentityUser_idx` (`UserId` ASC);
ALTER TABLE `identitylogin` 
ADD CONSTRAINT `FK_IdentityLogin_IdentityUser`
  FOREIGN KEY (`UserId`)
  REFERENCES `identityuser` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;
  
ALTER TABLE `identityuserclaim` 
ADD INDEX `FK_IdentityUserClaim_IdentityUser_idx` (`UserId` ASC);
ALTER TABLE `identityuserclaim` 
ADD CONSTRAINT `FK_IdentityUserClaim_IdentityUser`
  FOREIGN KEY (`UserId`)
  REFERENCES `identityuser` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;

ALTER TABLE `identityroleclaim` 
ADD INDEX `FK_IdentityRoleClaim_IdentityRole_idx` (`RoleId` ASC);
ALTER TABLE `identityroleclaim` 
ADD CONSTRAINT `FK_IdentityRoleClaim_IdentityRole`
  FOREIGN KEY (`RoleId`)
  REFERENCES `identityrole` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;