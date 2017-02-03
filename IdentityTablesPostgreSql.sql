-- Table: dbo."IdentityUser"
CREATE TABLE dbo."IdentityUser"
(
  "UserName" character varying(256) NOT NULL,
  "Email" character varying(256) NOT NULL,
  "EmailConfirmed" boolean NOT NULL,
  "PasswordHash" text,
  "SecurityStamp" character varying(38),
  "PhoneNumber" character varying(50),
  "PhoneNumberConfirmed" boolean,
  "TwoFactorEnabled" boolean NOT NULL,
  "LockoutEnd" timestamp without time zone,
  "LockoutEnabled" boolean NOT NULL,
  "AccessFailedCount" integer NOT NULL,
  "Id" serial NOT NULL,
  CONSTRAINT "PK_IdentityUser" PRIMARY KEY ("Id")
)
WITH (
  OIDS=FALSE
);

-- Table: dbo."IdentityRole"

CREATE TABLE dbo."IdentityRole"
(
  "Id" serial NOT NULL,
  "Name" character varying(50) NOT NULL,
  CONSTRAINT "IdentityRole_pkey" PRIMARY KEY ("Id")
)
WITH (
  OIDS=FALSE
);

-- Table: dbo."IdentityLogin"

CREATE TABLE dbo."IdentityLogin"
(
  "LoginProvider" character varying(256) NOT NULL,
  "ProviderKey" character varying(128) NOT NULL,
  "UserId" integer NOT NULL,
  "Name" character varying(256) NOT NULL,
  CONSTRAINT "IdentityLogin_pkey" PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId"),
  CONSTRAINT "IdentityLogin_UserId_fkey" FOREIGN KEY ("UserId")
      REFERENCES dbo."IdentityUser" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);

-- Table: dbo."IdentityUserClaim"

CREATE TABLE dbo."IdentityUserClaim"
(
  "Id" serial NOT NULL,
  "UserId" integer NOT NULL,
  "ClaimType" character varying(256) NOT NULL,
  "ClaimValue" character varying(256),
  CONSTRAINT "IdentityUserClaim_pkey" PRIMARY KEY ("Id"),
  CONSTRAINT "IdentityUserClaim_UserId_fkey" FOREIGN KEY ("UserId")
      REFERENCES dbo."IdentityUser" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);

-- Table: dbo."IdentityUserRole"

CREATE TABLE dbo."IdentityUserRole"
(
  "UserId" integer NOT NULL,
  "RoleId" integer NOT NULL,
  CONSTRAINT "IdentityUserRole_pkey" PRIMARY KEY ("UserId", "RoleId"),
  CONSTRAINT "IdentityUserRole_RoleId_fkey" FOREIGN KEY ("RoleId")
      REFERENCES dbo."IdentityRole" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT "IdentityUserRole_UserId_fkey" FOREIGN KEY ("UserId")
      REFERENCES dbo."IdentityUser" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
