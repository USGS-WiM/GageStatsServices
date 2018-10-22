CREATE SCHEMA IF NOT EXISTS gagestats;
CREATE TABLE IF NOT EXISTS gagestats."_EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK__EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE SCHEMA IF NOT EXISTS gagestats;

CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE gagestats."Agency" (
    "ID" serial NOT NULL,
    "Name" text NOT NULL,
    "Description" text NULL,
    "Code" text NOT NULL,
    CONSTRAINT "PK_Agency" PRIMARY KEY ("ID")
);

CREATE TABLE gagestats."Citation" (
    "ID" serial NOT NULL,
    "Title" text NOT NULL,
    "Author" text NOT NULL,
    "CitationURL" text NOT NULL,
    CONSTRAINT "PK_Citation" PRIMARY KEY ("ID")
);

CREATE TABLE gagestats."StationType" (
    "ID" serial NOT NULL,
    "Name" text NOT NULL,
    "Description" text NULL,
    "Code" text NOT NULL,
    CONSTRAINT "PK_StationType" PRIMARY KEY ("ID")
);

CREATE TABLE gagestats."Station" (
    "ID" serial NOT NULL,
    "Code" text NOT NULL,
    "AgencyID" integer NOT NULL,
    "Name" text NOT NULL,
    "Type" text NOT NULL,
    "IsRegulated" boolean NOT NULL,
    "StationTypeID" integer NOT NULL,
    "Location" geometry NOT NULL,
    CONSTRAINT "PK_Station" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Station_Agency_AgencyID" FOREIGN KEY ("AgencyID") REFERENCES gagestats."Agency" ("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Station_StationType_StationTypeID" FOREIGN KEY ("StationTypeID") REFERENCES gagestats."StationType" ("ID") ON DELETE RESTRICT
);

CREATE TABLE gagestats."Statistic" (
    "ID" serial NOT NULL,
    "StatisticGroupID" integer NOT NULL,
    "RegressionTypeID" integer NOT NULL,
    "StationID" integer NOT NULL,
    "Value" double precision NOT NULL,
    "UnitTypeID" integer NOT NULL,
    "Comments" text NULL,
    "YearsofRecord" integer NOT NULL,
    "CitationID" integer NOT NULL,
    CONSTRAINT "PK_Statistic" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Statistic_Citation_StationID" FOREIGN KEY ("StationID") REFERENCES gagestats."Citation" ("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Statistic_Station_StationID" FOREIGN KEY ("StationID") REFERENCES gagestats."Station" ("ID") ON DELETE RESTRICT
);

CREATE TABLE gagestats."StatisticErrors" (
    "ID" serial NOT NULL,
    "UnitTypeID" integer NOT NULL,
    "ErrorTypeID" integer NOT NULL,
    "Value" double precision NOT NULL,
    "StatisticID" integer NULL,
    CONSTRAINT "PK_StatisticErrors" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_StatisticErrors_Statistic_StatisticID" FOREIGN KEY ("StatisticID") REFERENCES gagestats."Statistic" ("ID") ON DELETE RESTRICT
);

CREATE TABLE gagestats."StatisticUnitTypes" (
    "StatisticID" integer NOT NULL,
    "UnitTypeID" text NOT NULL,
    CONSTRAINT "PK_StatisticUnitTypes" PRIMARY KEY ("StatisticID", "UnitTypeID"),
    CONSTRAINT "FK_StatisticUnitTypes_Statistic_StatisticID" FOREIGN KEY ("StatisticID") REFERENCES gagestats."Statistic" ("ID") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Agency_Code" ON gagestats."Agency" ("Code");

CREATE INDEX "IX_Station_AgencyID" ON gagestats."Station" ("AgencyID");

CREATE UNIQUE INDEX "IX_Station_Code" ON gagestats."Station" ("Code");

CREATE INDEX "IX_Station_StationTypeID" ON gagestats."Station" ("StationTypeID");

CREATE UNIQUE INDEX "IX_StationType_Code" ON gagestats."StationType" ("Code");

CREATE INDEX "IX_Statistic_StationID" ON gagestats."Statistic" ("StationID");

CREATE INDEX "IX_StatisticErrors_StatisticID" ON gagestats."StatisticErrors" ("StatisticID");

INSERT INTO gagestats."_EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20181022152626_init', '2.1.3-rtm-32065');

