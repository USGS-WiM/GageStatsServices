CREATE SCHEMA IF NOT EXISTS gagestats;
CREATE TABLE IF NOT EXISTS gagestats."_EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK__EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE SCHEMA IF NOT EXISTS gagestats;

CREATE EXTENSION IF NOT EXISTS postgis VERSION " ''";

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


                CREATE OR REPLACE FUNCTION "gagestats"."trigger_set_lastmodified"()
                    RETURNS TRIGGER AS $$
                    BEGIN
                      NEW."LastModified" = NOW();
                      RETURN NEW;
                    END;
                    $$ LANGUAGE plpgsql;
                


                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON "gagestats"."Agencies"  FOR EACH ROW EXECUTE PROCEDURE "gagestats"."trigger_set_lastmodified"();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON "gagestats"."Citations"  FOR EACH ROW EXECUTE PROCEDURE "gagestats"."trigger_set_lastmodified"();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON "gagestats"."Stations" FOR EACH ROW EXECUTE PROCEDURE  "gagestats"."trigger_set_lastmodified"();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON "gagestats"."StationTypes"  FOR EACH ROW EXECUTE PROCEDURE "gagestats"."trigger_set_lastmodified"();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON "gagestats"."Statistics" FOR EACH ROW EXECUTE PROCEDURE  "gagestats"."trigger_set_lastmodified"();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON "gagestats"."StatisticUnitTypes"  FOR EACH ROW EXECUTE PROCEDURE "gagestats"."trigger_set_lastmodified"();
                

INSERT INTO gagestats."_EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20181025200232_init', '2.2.4-servicing-10062');

ALTER TABLE gagestats."Statistics" DROP CONSTRAINT "FK_Statistics_Citations_StationID";

DROP TABLE gagestats."StatisticErrors";

DROP TABLE gagestats."StatisticUnitTypes";

ALTER TABLE gagestats."Stations" DROP COLUMN "Type";

ALTER TABLE gagestats."Statistics" RENAME COLUMN "StatisticGroupID" TO "StatisticGroupTypeID";

CREATE EXTENSION IF NOT EXISTS postgis;

ALTER TABLE gagestats."Statistics" ALTER COLUMN "YearsofRecord" TYPE double precision;
ALTER TABLE gagestats."Statistics" ALTER COLUMN "YearsofRecord" DROP NOT NULL;
ALTER TABLE gagestats."Statistics" ALTER COLUMN "YearsofRecord" DROP DEFAULT;

ALTER TABLE gagestats."Statistics" ALTER COLUMN "CitationID" TYPE integer;
ALTER TABLE gagestats."Statistics" ALTER COLUMN "CitationID" DROP NOT NULL;
ALTER TABLE gagestats."Statistics" ALTER COLUMN "CitationID" DROP DEFAULT;

ALTER TABLE gagestats."Statistics" ADD "PredictionIntervalID" integer NULL;

ALTER TABLE gagestats."Stations" ALTER COLUMN "IsRegulated" TYPE boolean;
ALTER TABLE gagestats."Stations" ALTER COLUMN "IsRegulated" DROP NOT NULL;
ALTER TABLE gagestats."Stations" ALTER COLUMN "IsRegulated" DROP DEFAULT;

CREATE TABLE gagestats."Characteristics" (
    "ID" serial NOT NULL,
    "StationID" integer NOT NULL,
    "VariableTypeID" integer NOT NULL,
    "UnitTypeID" integer NOT NULL,
    "CitationID" integer NULL,
    "Value" double precision NOT NULL,
    "Comments" text NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_Characteristics" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Characteristics_Citations_CitationID" FOREIGN KEY ("CitationID") REFERENCES gagestats."Citations" ("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Characteristics_Stations_StationID" FOREIGN KEY ("StationID") REFERENCES gagestats."Stations" ("ID") ON DELETE CASCADE
);

CREATE TABLE gagestats."PredictionInterval" (
    "ID" serial NOT NULL,
    "Variance" double precision NULL,
    "LowerConfidenceInterval" double precision NULL,
    "UpperConfidenceInterval" double precision NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_PredictionInterval" PRIMARY KEY ("ID")
);

CREATE TABLE gagestats."StatisticError" (
    "ID" serial NOT NULL,
    "StatisticID" integer NOT NULL,
    "ErrorTypeID" integer NOT NULL,
    "Value" double precision NOT NULL,
    CONSTRAINT "PK_StatisticError" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_StatisticError_Statistics_StatisticID" FOREIGN KEY ("StatisticID") REFERENCES gagestats."Statistics" ("ID") ON DELETE CASCADE
);

CREATE TABLE gagestats."User" (
    "ID" serial NOT NULL,
    "Username" text NOT NULL,
    "Email" text NOT NULL,
    "FirstName" text NOT NULL,
    "LastName" text NOT NULL,
    "Role" text NOT NULL,
    "PrimaryPhone" text NULL,
    "Password" text NOT NULL,
    "Salt" text NOT NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_User" PRIMARY KEY ("ID")
);

CREATE INDEX "IX_Statistics_CitationID" ON gagestats."Statistics" ("CitationID");

CREATE UNIQUE INDEX "IX_Statistics_PredictionIntervalID" ON gagestats."Statistics" ("PredictionIntervalID");

CREATE INDEX "IX_Characteristics_CitationID" ON gagestats."Characteristics" ("CitationID");

CREATE INDEX "IX_Characteristics_StationID" ON gagestats."Characteristics" ("StationID");

CREATE INDEX "IX_StatisticError_StatisticID" ON gagestats."StatisticError" ("StatisticID");

ALTER TABLE gagestats."Statistics" ADD CONSTRAINT "FK_Statistics_Citations_CitationID" FOREIGN KEY ("CitationID") REFERENCES gagestats."Citations" ("ID") ON DELETE RESTRICT;

ALTER TABLE gagestats."Statistics" ADD CONSTRAINT "FK_Statistics_PredictionInterval_PredictionIntervalID" FOREIGN KEY ("PredictionIntervalID") REFERENCES gagestats."PredictionInterval" ("ID") ON DELETE CASCADE;

INSERT INTO gagestats."_EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190522162439_reconfigureObjects', '2.2.4-servicing-10062');

ALTER TABLE gagestats."Statistics" ADD "IsPreferred" boolean NOT NULL DEFAULT FALSE;

INSERT INTO gagestats."_EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200423200043_StatisticIsPreferred', '2.2.4-servicing-10062');

