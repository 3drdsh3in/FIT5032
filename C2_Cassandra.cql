/* 
2.1
*/
CREATE KEYSPACE monugov_keyspace WITH REPLICATION = {'class': 'SimpleStrategy', 'replication_factor': 1};

DESC monugov_keyspace;

DESC keyspaces;

/*
2.2
*/
-- suburbs.csv
CREATE TABLE suburbs ("_id" text, councilArea text, suburb text, regionName text, postcode int, propertyCount int, PRIMARY KEY("_id"));
COPY suburbs("_id", councilArea, suburb, regionName, postcode, propertyCount) FROM 'suburbs.csv' WITH HEADER = TRUE;

-- landmarks.csv
CREATE TABLE landmarks ("_id" text, category text, theme text, landmarkName text, lat float, long float, houseNo text, street text, suburb text, postcode int, PRIMARY KEY(("_id"), landmarkName)) WITH CLUSTERING ORDER BY (landMarkName ASC);
COPY landmarks("_id", category, theme, landmarkName, lat, long, houseNo, street, suburb, postcode) FROM 'landmarks.csv' WITH HEADER = TRUE;

-- properties.csv
CREATE TYPE history (sold_by text, date timestamp, price int);
CREATE TABLE properties ("_id" text, address text, postcode int, propertyHistory list<frozen<history>>, rooms int, suburb text, type text, PRIMARY KEY(type, "_id"));
COPY properties("_id", address, postcode, propertyHistory, rooms, suburb, type) FROM 'properties.csv' WITH DELIMITER = '|' AND HEADER = TRUE AND DATETIMEFORMAT='%Y-%m-%dT%H:%M:%SZ';

/*
2.3
*/
-- Inserts
INSERT INTO landmarks("_id",category,houseno,landmarkname,lat,long,postcode,street,suburb,theme) VALUES('L999','Place Of Assembly','1', 'Gresswell Theatre',-37.712422,145.072617,3085,'Forrest Road','Macleod','Theatre Live');
INSERT INTO properties ("_id", address,postcode,propertyhistory,rooms,suburb,type) VALUES ('9c31774853db55f334f1b96e6ae2611d','19 Kinlock St',3085, [{sold_by:'Darren',date:'1970-01-01T00:00:00Z',price:1120000}],5,'Macleod','h');

-- Updates
CREATE INDEX ON suburbs (suburb);
SELECT "_id" FROM suburbs WHERE suburb='Macleod' -- Used to check what the "_id" is for suburb Macleod which is S335.
UPDATE suburbs SET propertycount=4169 WHERE "_id"='S335';

-- See the updated data.
SELECT * FROM landmarks WHERE "_id"='L99';
SELECT * FROM properties WHERE type='h' AND "_id"='9c31774853db55f334f1b96e6ae2611d';
SELECT * FROM suburbs WHERE "_id"='S335';

/*
2.4
*/
-- (i)
CREATE INDEX ON suburbs (suburb);
SELECT * FROM suburbs WHERE suburb='Caulfield';

-- (ii)
ALTER TABLE suburbs
ADD (otherHomeGround boolean, team int);

/*
--- IMPORTANT ---
otherHomeGround=true and team=38 are set in the UPDATE
with the setting of the TTL.
*/

Showing the TTL columns, we can use the query:
SELECT "_id",councilarea, otherhomeground, ttl(otherhomeground), postcode, propertycount, regionname, suburb, team, ttl(team) FROM suburbs WHERE suburb='Caulfield';

-- (iii)
UPDATE suburbs USING TTL 300000
SET otherHomeGround=true, team=38
WHERE "_id"='S272';

/*
2.5
*/
CREATE INDEX ON monugov_keyspace.properties(propertyhistory);

/*
2.6
*/
-- (i)
SELECT type, COUNT(type) FROM properties GROUP BY type;

-- (ii)
SELECT landmarkname, postcode FROM landmarks WHERE postcode>3200 ALLOW FILTERING;

-- (iii)
SELECT propertyhistory, address, suburb, postcode FROM properties WHERE propertyhistory CONTAINS {sold_by: 'Frank', date: '1970-01-01T00:00:00Z', price: 591000};





-------- COPY COMMAND FOR (RE)TESTING (Not For Marking) ----------

-- suburbs.csv
CREATE TABLE suburbs ("_id" text, councilArea text, suburb text, regionName text, postcode int, propertyCount int, PRIMARY KEY("_id"));
COPY suburbs("_id", councilArea, suburb, regionName, postcode, propertyCount) FROM 'C:\Users\super\OneDrive\Desktop\Uni\FIT3176\assignments\suburbs.csv' WITH HEADER = TRUE;

-- landmarks.csv
CREATE TABLE landmarks ("_id" text, category text, theme text, landmarkName text, lat float, long float, houseNo text, street text, suburb text, postcode int, PRIMARY KEY(("_id"), landmarkName)) WITH CLUSTERING ORDER BY (landMarkName ASC);
COPY landmarks("_id", category, theme, landmarkName, lat, long, houseNo, street, suburb, postcode) FROM 'C:\Users\super\OneDrive\Desktop\Uni\FIT3176\assignments\landmarks.csv' WITH HEADER = TRUE;

-- properties.csv
CREATE TYPE history (sold_by text, date timestamp, price int);
CREATE TABLE properties ("_id" text, address text, postcode int, propertyHistory list<frozen<history>>, rooms int, suburb text, type text, PRIMARY KEY(type, "_id"));
COPY properties("_id", address, postcode, propertyHistory, rooms, suburb, type) FROM 'C:\Users\super\OneDrive\Desktop\Uni\FIT3176\assignments\properties.csv' WITH DELIMITER = '|' AND HEADER = TRUE AND DATETIMEFORMAT='%Y-%m-%dT%H:%M:%SZ';
