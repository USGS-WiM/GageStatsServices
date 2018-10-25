CREATE SCHEMA IF NOT EXISTS gagestats;
CREATE TABLE IF NOT EXISTS gagestats."_EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK__EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE SCHEMA IF NOT EXISTS gagestats;

CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE gagestats."Agencies" (
    "ID" serial NOT NULL,
    "Name" text NOT NULL,
    "Description" text NULL,
    "Code" text NOT NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_Agencies" PRIMARY KEY ("ID")
);

CREATE TABLE gagestats."Citations" (
    "ID" serial NOT NULL,
    "Title" text NOT NULL,
    "Author" text NOT NULL,
    "CitationURL" text NOT NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_Citations" PRIMARY KEY ("ID")
);

CREATE TABLE gagestats."StationTypes" (
    "ID" serial NOT NULL,
    "Name" text NOT NULL,
    "Description" text NULL,
    "Code" text NOT NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_StationTypes" PRIMARY KEY ("ID")
);

CREATE TABLE gagestats."Stations" (
    "ID" serial NOT NULL,
    "Code" text NOT NULL,
    "AgencyID" integer NOT NULL,
    "Name" text NOT NULL,
    "Type" text NOT NULL,
    "IsRegulated" boolean NOT NULL,
    "StationTypeID" integer NOT NULL,
    "Location" geometry NOT NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_Stations" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Stations_Agencies_AgencyID" FOREIGN KEY ("AgencyID") REFERENCES gagestats."Agencies" ("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Stations_StationTypes_StationTypeID" FOREIGN KEY ("StationTypeID") REFERENCES gagestats."StationTypes" ("ID") ON DELETE RESTRICT
);

CREATE TABLE gagestats."Statistics" (
    "ID" serial NOT NULL,
    "StatisticGroupID" integer NOT NULL,
    "RegressionTypeID" integer NOT NULL,
    "StationID" integer NOT NULL,
    "Value" double precision NOT NULL,
    "UnitTypeID" integer NOT NULL,
    "Comments" text NULL,
    "YearsofRecord" integer NOT NULL,
    "CitationID" integer NOT NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_Statistics" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Statistics_Citations_StationID" FOREIGN KEY ("StationID") REFERENCES gagestats."Citations" ("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Statistics_Stations_StationID" FOREIGN KEY ("StationID") REFERENCES gagestats."Stations" ("ID") ON DELETE RESTRICT
);

CREATE TABLE gagestats."StatisticErrors" (
    "ID" serial NOT NULL,
    "UnitTypeID" integer NOT NULL,
    "ErrorTypeID" integer NOT NULL,
    "Value" double precision NOT NULL,
    "StatisticID" integer NULL,
    CONSTRAINT "PK_StatisticErrors" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_StatisticErrors_Statistics_StatisticID" FOREIGN KEY ("StatisticID") REFERENCES gagestats."Statistics" ("ID") ON DELETE RESTRICT
);

CREATE TABLE gagestats."StatisticUnitTypes" (
    "StatisticID" integer NOT NULL,
    "UnitTypeID" text NOT NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_StatisticUnitTypes" PRIMARY KEY ("StatisticID", "UnitTypeID"),
    CONSTRAINT "FK_StatisticUnitTypes_Statistics_StatisticID" FOREIGN KEY ("StatisticID") REFERENCES gagestats."Statistics" ("ID") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Agencies_Code" ON gagestats."Agencies" ("Code");

CREATE INDEX "IX_Stations_AgencyID" ON gagestats."Stations" ("AgencyID");

CREATE UNIQUE INDEX "IX_Stations_Code" ON gagestats."Stations" ("Code");

CREATE INDEX "IX_Stations_StationTypeID" ON gagestats."Stations" ("StationTypeID");

CREATE UNIQUE INDEX "IX_StationTypes_Code" ON gagestats."StationTypes" ("Code");

CREATE INDEX "IX_StatisticErrors_StatisticID" ON gagestats."StatisticErrors" ("StatisticID");

CREATE INDEX "IX_Statistics_StationID" ON gagestats."Statistics" ("StationID");

INSERT INTO gagestats."_EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20181025200232_init', '2.1.3-rtm-32065');

