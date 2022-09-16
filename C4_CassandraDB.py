### HOW TO EXECUTE ###
### Step 1: pip install pip install cassandra-driver ###
### Step 2: Provide ###
### python3 C4_CassandraDB.py ###

# This was not worth the 5 marks and i want my time back

# --- IMPORTANT: Change this to the bin folder of your apache-cassandra cqlsh file ---
cqlshFilePath = r"\Users\super\OneDrive\Desktop\apache-cassandra-3.11.13\bin"
suburbsCsv = r"C:\Users\super\OneDrive\Desktop\Uni\FIT3176\assignments\suburbs.csv"
landmarksCsv = r"C:\Users\super\OneDrive\Desktop\Uni\FIT3176\assignments\landmarks.csv"
propertiesCsv = r"C:\Users\super\OneDrive\Desktop\Uni\FIT3176\assignments\properties.csv"

from time import sleep
from cassandra.cluster import Cluster
import subprocess

host='127.0.0.1'
port=9042
cluster = Cluster([host], port=port)
session = cluster.connect("monugov_keyspace") # May Need to create a monugov_keyspace before executing the file

# # # Delete Everything If Exists:
session.execute("DROP TABLE IF EXISTS suburbs")
session.execute("DROP TABLE IF EXISTS landmarks")
session.execute("DROP TABLE IF EXISTS properties")
session.execute("DROP TYPE IF EXISTS history")


# Print all documents method
def printAll(data):
    for x in data:
        print(x)

################
####Task 2.1####
################

currCommand = "CREATE KEYSPACE IF NOT EXISTS monugov_keyspace WITH REPLICATION = {'class': 'SimpleStrategy', 'replication_factor': 1};"
command = f"""cd {cqlshFilePath} & .\cqlsh -e "{currCommand}" """
process = subprocess.run(command, capture_output=True, shell=True)

currCommand = "DESC monugov_keyspace;"
command = f"""cd {cqlshFilePath} & .\cqlsh -e "{currCommand}" """
process = subprocess.run(command, capture_output=True, shell=True)
print(process.stdout.decode())

currCommand = "DESC keyspaces;"
command = f"""cd {cqlshFilePath} & .\cqlsh -e "{currCommand}" """
process = subprocess.run(command, capture_output=True, shell=True)
print(process.stdout.decode())

###############
###Task 2.2####
###############

useKeySpaceCommand = "use monugov_keyspace;"

# Note: Need Two Double Quotes around _id field as bash will interpret one of them
# suburbs.csv
createCommand = """CREATE TABLE suburbs (""_id"" text, councilArea text, suburb text, regionName text, postcode int,propertyCount int, PRIMARY KEY(""_id""));"""
copyCommand = """COPY suburbs(""_id"", councilArea, suburb, regionName, postcode, propertyCount) FROM '{suburbsCsv}' WITH HEADER = TRUE;""".format(suburbsCsv=suburbsCsv)
createAndCopyCommand = useKeySpaceCommand + """{useKeySpaceCommand} {createCommand} {copyCommand}""".format(useKeySpaceCommand=useKeySpaceCommand,createCommand=createCommand,copyCommand=copyCommand)
command = f"""cd {cqlshFilePath} & .\cqlsh -e "{createAndCopyCommand}" """
process = subprocess.run(command, capture_output=True, shell=True)
print(process.stdout.decode())
print(process.stderr.decode())

# landmarks.csv
createCommand = """CREATE TABLE landmarks (""_id"" text, category text, theme text, landmarkName text, lat float, long float, houseNo text, street text, suburb text, postcode int, PRIMARY KEY((""_id""), landmarkName)) WITH CLUSTERING ORDER BY (landMarkName ASC);"""
copyCommand = """COPY landmarks(""_id"", category, theme, landmarkName, lat, long, houseNo, street, suburb, postcode) FROM '{landmarksCsv}' WITH HEADER = TRUE;""".format(landmarksCsv=landmarksCsv)
createAndCopyCommand = useKeySpaceCommand + """{useKeySpaceCommand} {createCommand} {copyCommand}""".format(useKeySpaceCommand=useKeySpaceCommand,createCommand=createCommand,copyCommand=copyCommand)
command = f"""cd {cqlshFilePath} & .\cqlsh -e "{createAndCopyCommand}" """
process = subprocess.run(command, capture_output=True, shell=True)
print(process.stdout.decode())
print(process.stderr.decode())

# properties.csv
createCommand1 = """CREATE TYPE history (sold_by text, date timestamp, price int);"""
createCommand2 = """CREATE TABLE properties (""_id"" text, address text, postcode int, propertyHistory list<frozen<history>>, rooms int, suburb text, type text, PRIMARY KEY(type, ""_id""));"""
copyCommand = """COPY properties(""_id"", address, postcode, propertyHistory, rooms, suburb, type) FROM '{propertiesCsv}' WITH DELIMITER = '|' AND HEADER = TRUE AND DATETIMEFORMAT='%Y-%m-%dT%H:%M:%SZ';""".format(propertiesCsv=propertiesCsv)
createAndCopyCommand = useKeySpaceCommand + """{useKeySpaceCommand} {createCommand1} {createCommand2} {copyCommand}""".format(useKeySpaceCommand=useKeySpaceCommand,createCommand1=createCommand1,createCommand2=createCommand2,copyCommand=copyCommand)
command = f"""cd {cqlshFilePath} & .\cqlsh -e "{createAndCopyCommand}" """
process = subprocess.run(command, capture_output=True, shell=True)
print(process.stdout.decode())
print(process.stderr.decode())

################
####Task 2.3####
################
session.execute("""INSERT INTO landmarks("_id",category,houseno,landmarkname,lat,long,postcode,street,suburb,theme) VALUES('L999','Place Of Assembly','1', 'Gresswell Theatre',-37.712422,145.072617,3085,'Forrest Road','Macleod','Theatre Live')""")
printAll(session.execute("""SELECT * FROM landmarks WHERE "_id"='L999'"""))
session.execute("""INSERT INTO properties ("_id", address,postcode,propertyhistory,rooms,suburb,type) VALUES ('9c31774853db55f334f1b96e6ae2611d','19 Kinlock St',3085, [{sold_by:'Darren',date:'1970-01-01T00:00:00Z',price:1120000}],5,'Macleod','h')""")
printAll(session.execute("""SELECT * FROM properties WHERE type='h' AND "_id"='9c31774853db55f334f1b96e6ae2611d'"""))
session.execute("""UPDATE suburbs SET propertycount=4169 WHERE "_id"='S335'""")
printAll(session.execute("""SELECT * FROM suburbs WHERE "_id"='S335'"""))

# ################
# ####Task 2.4####
# ################

# Part i
session.execute("CREATE INDEX IF NOT EXISTS ON suburbs (suburb)")

# This sleep is needed as the system.log file suggested that
# the CREATE INDEX command runs asynchronously. Hence, we need to wait for
# CREATE INDEX finish creating before a SELECT statement that uses it can
# actually be initiated.
print("Sleep 5...")
sleep(5)

printAll(session.execute("""SELECT * FROM suburbs WHERE suburb='Caulfield'"""))

# Part ii
session.execute("ALTER TABLE suburbs ADD (otherHomeGround boolean, team int)")

# Part iii
session.execute("""UPDATE suburbs USING TTL 300000 SET otherHomeGround=true, team=38 WHERE "_id"='S272'""")

printAll(session.execute("""SELECT "_id",councilarea,otherhomeground,ttl(otherhomeground),postcode,propertycount,regionname,suburb,team,ttl(team) FROM suburbs WHERE suburb='Caulfield'"""))

################
####Task 2.5####
################
session.execute("CREATE INDEX ON monugov_keyspace.properties(propertyhistory)")

# This sleep is needed as the system.log file suggested that
# the CREATE INDEX command runs asynchronously. Hence, we need to wait for
# CREATE INDEX finish creating before a SELECT statement that uses it can
# actually be initiated.
print("Sleep 5...")
sleep(5)

#Part i
printAll(session.execute("SELECT type,COUNT(type) FROM properties GROUP BY type"))

#Part ii
printAll(session.execute("SELECT * FROM landmarks WHERE postcode>3200 ALLOW FILTERING"))

#Part iii
printAll(session.execute("SELECT * FROM properties WHERE propertyhistory CONTAINS {sold_by: 'Frank', date: '1970-01-01T00:00:00Z', price: 591000}"))
